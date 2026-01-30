using System.Net.Http.Headers;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;
using Newtonsoft.Json;

namespace api_slim.src.Services
{
    public class VitalService(IVitalRepository vitalRepository, ICustomerRecipientRepository customerRecipientRepository, IMapper _mapper) : IVitalService
    {
        private readonly HttpClient client = new();
        private readonly string uri = Environment.GetEnvironmentVariable("URI_RAPIDOC") ?? "";
        private readonly string clientId = Environment.GetEnvironmentVariable("CLIENT_ID_RAPIDOC") ?? "";
        private readonly string token = Environment.GetEnvironmentVariable("TOKEN_RAPIDOC") ?? "";

        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Vital> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> vitals = await vitalRepository.GetAllAsync(pagination);
                int count = await vitalRepository.GetCountDocumentsAsync(pagination);
                return new(vitals.Data, count, pagination.PageNumber, pagination.PageSize);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> vital = await vitalRepository.GetByIdAggregateAsync(id);
                if(vital.Data is null) return new(null, 404, "Bem Vital não encontrado");
                return new(vital.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<Vital?>> GetByBeneficiaryIdAsync(string beneficiaryId)
        {
            try
            {
                ResponseApi<Vital?> vital = await vitalRepository.GetByBeneficiaryIdAsync(beneficiaryId);
                ResponseApi<List<Vital>> vitalWeek = await vitalRepository.GetByBeneficiaryIdWeekAsync(beneficiaryId);

                ResponseApi<CustomerRecipient?> customer = await customerRecipientRepository.GetByIdAsync(beneficiaryId);
                decimal metaAgua = CalcularMetaAgua(customer.Data.Weight);
                
                if(vital.Data is not null)
                {
                    if(customer.Data is not null)
                    {
                        vital.Data.Metric = new ()
                        {
                            IGS = CalcularIGS(vital.Data),
                            IGN = CalcularIGN(vital.Data, CalcularMetaAgua(customer.Data.Weight)),
                            IES = CalcularIES(vital.Data),
                            IPV = CalcularIPV(vital.Data, CalcularMetaAgua(customer.Data.Weight))
                        };
                    }

                }

                // 2. Lógica da Semana Atual (Gráfico de Barras)
                var diasDaSemanaNomes = new[] { "Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb" };
                
                // Determina o início da semana (Domingo)
                DateTime hoje = DateTime.Today;
                DateTime inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek);

                // Inicializa a lista de métricas da semana para garantir que todos os dias apareçam no gráfico
                vital.Data.WeekMetric = new List<VitalMetric>();

                for (int i = 0; i < 7; i++)
                {
                    DateTime dataDia = inicioSemana.AddDays(i);
                    
                    // Busca se existe registro para este dia específico na lista retornada do banco
                    var registroDia = vitalWeek.Data.FirstOrDefault(x => x.CreatedAt.Date == dataDia.Date);

                    if (registroDia != null)
                    {
                        vital.Data.WeekMetric.Add(new()
                        {
                            IGS = CalcularIGS(registroDia),
                            IGN = CalcularIGN(registroDia, metaAgua),
                            IES = CalcularIES(registroDia),
                            IPV = CalcularIPV(registroDia, metaAgua),
                            Day = diasDaSemanaNomes[i]
                        });
                    }
                    else
                    {
                        // Dia sem registro: envia IPV 0 para o gráfico mostrar a barra vazia
                        vital.Data.WeekMetric.Add(new() { Day = diasDaSemanaNomes[i], IPV = 0 });
                    }
                }

                return new(vital?.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Vital?>> CreateAsync(CreateVitalDTO request)
        {
            try
            {
                Vital vital = _mapper.Map<Vital>(request);
                ResponseApi<Vital?> response = await vitalRepository.CreateAsync(vital);

                if(response.Data is null) return new(null, 400, "Falha ao criar salvar.");
                
                return new(response.Data, 201, "Salvo com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Vital?>> UpdateAsync(UpdateVitalDTO request)
        {
            try
            {
                ResponseApi<Vital?> vitalResponse = await vitalRepository.GetByIdAsync(request.Id);
                if(vitalResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                Vital vital = _mapper.Map<Vital>(request);
                vital.UpdatedAt = DateTime.UtcNow;

                ResponseApi<Vital?> response = await vitalRepository.UpdateAsync(vital);
                if(!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");

                return new(response.Data, 201, "Atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Vital>> DeleteAsync(string id, string userId)
        {
            try
            {
                ResponseApi<Vital> vital = await vitalRepository.DeleteAsync(id);
                if(!vital.IsSuccess || vital.Data is null) return new(null, 400, vital.Message);

                return new(null, 204, "Excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region  FUNCTIONS
        // 1. CÁLCULO DO SCORE IGS (SONO)
        // DTS (40%) + ES (30%) + CR (30%)
        public double CalcularIGS(Vital vital)
        {
            // DTS: Meta de 8h = 100 pontos
            double scoreDts = vital.SleepHours >= 8 ? 100 : (vital.SleepHours / 8.0) * 100;

            // ES (Qualidade): 1 a 5 (escala de emoji/estrela)
            double scoreEs = (vital.SleepQuality / 5.0) * 100;

            // CR (Consistência/Celular): Se SleepCell for "Não" ganha 100
            double scoreCr = vital.SleepCell.ToLower() == "não" ? 100 : 50;

            return (scoreDts * 0.4) + (scoreEs * 0.3) + (scoreCr * 0.3);
        }

        // 2. CÁLCULO DO SCORE IGN (NUTRIÇÃO)
        // Alinhamento (40%) + Consistência (30%) + Carga Glicêmica (20%) + Hidratação (10%)
        public double CalcularIGN(Vital vital, decimal metaHidratacao)
        {
            // Alinhamento (P1): Se comeu até as 20h ganha 100 (Exemplo baseado na LastMeal)
            double scoreP1 = 100; 
            if (!string.IsNullOrEmpty(vital.LastMeal)) {
                TimeSpan.TryParse(vital.LastMeal, out var lastMealTime);
                scoreP1 = lastMealTime.Hours <= 20 ? 100 : 60;
            }

            // Consistência (P2): Baseado no SnackHours (intervalos)
            double scoreP2 = vital.SnackHours == "3" ? 100 : 70;

            // Carga Glic (P3): "Leve" = 100, "Média" = 50, "Alta" = 0
            double scoreP3 = vital.GlycemicLoad.ToLower() switch {
                "leve" => 100,
                "media" => 50,
                "alta" => 0,
                _ => 50
            };

            // Hidratação (P4): (Consumido / Meta) * 100
            double scoreP4 = (double)(vital.WaterAmount / metaHidratacao) * 100;
            if (scoreP4 > 100) scoreP4 = 100;

            return (scoreP1 * 0.4) + (scoreP2 * 0.3) + (scoreP3 * 0.2) + (scoreP4 * 0.1);
        }

        // 3. CÁLCULO DO SCORE IES (MENTAL)
        // Humor (40%) + Estresse (40%) + Descompressão (20%)
        public double CalcularIES(Vital vital)
        {
            // Humor (P1): Estável/Bom = 100
            double scoreP1 = vital.Mood.ToLower() switch {
                "excelente" => 100,
                "bom" => 80,
                "estável" => 70,
                "ruim" => 30,
                _ => 50
            };

            // Estresse (P2): Inverso (Stress 0 = 100 pontos, Stress 10 = 0 pontos)
            double scoreP2 = (double)(10 - vital.Stress) * 10;

            // Descompressão (P3): "Sim" = 100
            double scoreP3 = vital.Decompression.ToLower() == "sim" ? 100 : 0;

            return (scoreP1 * 0.4) + (scoreP2 * 0.4) + (scoreP3 * 0.2);
        }

        // 4. IPV FINAL (Média dos 3)
        public double CalcularIPV(Vital vital, decimal metaHidratacao)
        {
            var igs = CalcularIGS(vital);
            var ign = CalcularIGN(vital, metaHidratacao);
            var ies = CalcularIES(vital);

            return (igs + ign + ies) / 3;
        }

        public static decimal CalcularMetaAgua(decimal peso)
        {
            if (peso <= 0) return 0;

            // Regra: 35ml por kg
            decimal mlTotal = peso * 35;
            
            // Converte para litros
            decimal litros = mlTotal / 1000;

            return litros;
        }
        #endregion
    }
}