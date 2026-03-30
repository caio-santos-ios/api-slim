using System.Globalization;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;
using MongoDB.Driver.Linq;
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
        public async Task<ResponseApi<dynamic?>> SyncVitalsAsync()
        {
            try
            {
                ResponseApi<List<CustomerRecipient>> recipients = await customerRecipientRepository.GetAsync();
                if(recipients.Data is not null)
                {
                    foreach (CustomerRecipient recipient in recipients.Data)
                    {
                        if(recipient.Cpf != "086.306.285-70") continue;

                        ResponseApi<List<Vital>> vitals = await vitalRepository.GetBeneficiaryIAllAsync(recipient.Id);

                        if(vitals.Data is not null)
                        {
                            DateTime? lasteDate = null;
                            foreach (Vital vital in vitals.Data)
                            {


                                // sequence
                                if(lasteDate is null)
                                {
                                    lasteDate = vital.CreatedAt.Date;
                                } 
                                else
                                {
                                    var diferenca = vital.CreatedAt.Date - lasteDate;

                                    if (diferenca.HasValue && diferenca.Value.Days == 1)
                                    {
                                        // Sua lógica de sequência
                                        vital.ExtrasPoint = 1;
                                        vital.SequenceCheckIn =+ 1;
                                    }
                                    else
                                    {
                                        // Lógica para quando a sequência é quebrada
                                        vital.SequenceCheckIn = 0;
                                    }

                                    lasteDate = vital.CreatedAt.Date;

                                    await vitalRepository.UpdateAsync(vital);
                                }
                            }
                        }
                    }
                }
                return new(null, 200, "Sincronizado com sucesso");
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
        public async Task<ResponseApi<List<Vital>>> GetByBeneficiaryAllAsync(string beneficiaryId, string startDate, string endDate)
        {
            try
            {
                DateTime? start = null; 
                DateTime? end = null; 

                if(!string.IsNullOrEmpty(startDate) && startDate != "sem") start = DateTime.Parse(startDate);
                if(!string.IsNullOrEmpty(endDate) && endDate != "sem") end = DateTime.Parse(endDate);
                
                ResponseApi<List<Vital>> vitalWeek = await vitalRepository.GetByBeneficiaryIAllAsync(beneficiaryId, start, end);
                List<Vital> list = new();
                ResponseApi<CustomerRecipient?> customer = await customerRecipientRepository.GetByIdAsync(beneficiaryId);
                if(customer.Data is not null)
                {
                    decimal metaAgua = CalcularMetaAgua(customer.Data.Weight);
                    
                    if(vitalWeek.Data is not null && metaAgua > 0)
                    {
                        foreach (var item in vitalWeek.Data)
                        {
                            string dayBr = GetDayWeek(item.CreatedAt.Date.DayOfWeek.ToString()); 
                            string dayNum = item.CreatedAt.Day.ToString();
                            string monthBr = item.CreatedAt.ToString("MMM", new CultureInfo("pt-BR")).Replace(".", "");
                            
                            list.Add(new ()
                            {
                                Id = item.Id,
                                SleepHours = item.SleepHours,
                                WaterAmount = item.WaterAmount,
                                Metric = new() 
                                {
                                    IGS = CalcularIGS(item, customer.Data.Patrology),
                                    IGN = CalcularIGN(item, CalcularMetaAgua(customer.Data.Weight), customer.Data.Patrology),
                                    IES = CalcularIES(item, customer.Data.Patrology),
                                    IPV = CalcularIPV(item, CalcularMetaAgua(customer.Data.Weight), customer.Data.Patrology),
                                    Day = $"{dayBr}, {dayNum} {monthBr}"
                                },
                                ExtrasPoint = item.ExtrasPoint,
                                ChekinIGS = item.ChekinIGS,
                                ChekinIES = item.ChekinIES,
                                ChekinIGN = item.ChekinIGN,
                                CreatedAt = item.CreatedAt
                            });
                        };
                    };
                };

                list = list.OrderByDescending(x => x.CreatedAt).ToList();

                return new(list);
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<Vital?>> GetByBeneficiaryAsync(string beneficiaryId)
        {
            try
            {
                ResponseApi<Vital?> vital = await vitalRepository.GetByBeneficiaryIdAsync(beneficiaryId);
                ResponseApi<CustomerRecipient?> customer = await customerRecipientRepository.GetByIdAsync(beneficiaryId);

                if(vital.Data is not null && customer.Data is not null)
                {
                    vital.Data.Metric = new ()
                    {
                        IGS = CalcularIGS(vital.Data, customer.Data.Patrology),
                        IGN = CalcularIGN(vital.Data, CalcularMetaAgua(customer.Data.Weight), customer.Data.Patrology),
                        IES = CalcularIES(vital.Data, customer.Data.Patrology),
                        IPV = CalcularIPV(vital.Data, CalcularMetaAgua(customer.Data.Weight), customer.Data.Patrology),
                    };
                }

                return new(vital.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<Vital?>> GetByBeneficiaryIdAsync(string beneficiaryId, string period)
        {
            try
            {
                ResponseApi<List<Vital>> vitalWeek = await vitalRepository.GetByBeneficiaryIdWeekAsync(beneficiaryId, period);

                ResponseApi<Vital?> vital = await vitalRepository.GetByBeneficiaryIdAsync(beneficiaryId);

                List<VitalMetric> weekMetrics = new();
                VitalMetric weekMetric = new();
                double IGS = 0;
                double IGN = 0;
                double IES = 0;
                double IPV = 0;

                double dass1 = 0;
                double dass2 = 0;
                double dass3 = 0;
                double dass4 = 0;
                double dass5 = 0;
                double dass6 = 0;
                double dass7 = 0;
                double dass8 = 0;
                double dass9 = 0;
                int qtd = 0;

                ResponseApi<CustomerRecipient?> customer = await customerRecipientRepository.GetByIdAsync(beneficiaryId);
                if(customer.Data is not null)
                {
                    decimal metaAgua = CalcularMetaAgua(customer.Data.Weight);

                    DateTime hoje = DateTime.Today;
                    DateTime dataInicio;
                    int quantidadeIteracoes;

                    switch (period.ToLower())
                    {
                        case "mes":
                            dataInicio = new DateTime(hoje.Year, hoje.Month, 1);
                            quantidadeIteracoes = DateTime.DaysInMonth(hoje.Year, hoje.Month);
                            break;
                        case "ano":
                            dataInicio = new DateTime(hoje.Year, 1, 1);
                            quantidadeIteracoes = 12; 
                            break;
                        case "semana":
                            dataInicio = hoje.AddDays(-(int)hoje.DayOfWeek); 
                            quantidadeIteracoes = 7;
                            break;
                        default:
                            dataInicio = hoje.AddDays(-(int)hoje.DayOfWeek); 
                            quantidadeIteracoes = vitalWeek.Data is null ? 0 : vitalWeek.Data.Count;
                            break;
                    }

                    string patrology = customer.Data is null ? "" : customer.Data.Patrology;

                    if (vitalWeek.Data is not null)
                    {
                        var diasDaSemanaNomes = new[] { "Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb" };
                        var mesesNomes = new[] { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" };
                        
                        if(!new List<string> { "semana", "mes", "ano" }.Contains(period))
                        {
                            var dadosAgrupados = vitalWeek.Data
                            .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month, x.CreatedAt.Day })
                            .OrderBy(g => g.Key.Year)
                            .ThenBy(g => g.Key.Month)
                            .ThenBy(g => g.Key.Day);

                            foreach (var grupo in dadosAgrupados)
                            {
                                var mediaIPV = grupo.Average(x => CalcularIPV(x, metaAgua, patrology));
                                var mediaIGS = grupo.Average(x => CalcularIGS(x, patrology));
                                var mediaIGN = grupo.Average(x => CalcularIGN(x, metaAgua, patrology));
                                var mediaIES = grupo.Average(x => CalcularIES(x, patrology));

                                string labelTudo = $"{grupo.Key.Day:00}/{grupo.Key.Month:00}/{grupo.Key.Year.ToString().Substring(2)}";
                                
                                weekMetrics.Add(new()
                                {
                                    IGS = (int)Math.Round(mediaIGS),
                                    IGN = (int)Math.Round(mediaIGN),
                                    IES = (int)Math.Round(mediaIES),
                                    IPV = (int)Math.Round(mediaIPV),
                                    Day = labelTudo
                                });
                                
                                qtd += 1;
                                IGS += (int)Math.Round(mediaIGS);
                                IGN += (int)Math.Round(mediaIGN);
                                IES += (int)Math.Round(mediaIES);
                                IPV += (int)Math.Round(mediaIPV);

                                dass1 += grupo.Sum(x => x.Dass1); 
                                dass2 += grupo.Sum(x => x.Dass2); 
                                dass3 += grupo.Sum(x => x.Dass3); 
                                dass4 += grupo.Sum(x => x.Dass4); 
                                dass5 += grupo.Sum(x => x.Dass5); 
                                dass6 += grupo.Sum(x => x.Dass6); 
                                dass7 += grupo.Sum(x => x.Dass7); 
                                dass8 += grupo.Sum(x => x.Dass8); 
                                dass9 += grupo.Sum(x => x.Dass9);
                            }
                        }
                        else 
                        {
                            for (int i = 0; i < quantidadeIteracoes; i++)
                            {
                                string labelEixo = "";
                                List<Vital> registrosPeriodo = new List<Vital>();

                                if (period.Equals("ano"))
                                {
                                    int mesAlvo = i + 1;
                                    labelEixo = mesesNomes[i];
                                    registrosPeriodo = vitalWeek.Data.Where(x => x.CreatedAt.Month == mesAlvo && x.CreatedAt.Year == hoje.Year).ToList();
                                }
                                else
                                {
                                    DateTime dataAlvo = dataInicio.AddDays(i);
                                    labelEixo = period.Equals("mes") ? dataAlvo.Day.ToString("00") : diasDaSemanaNomes[(int)dataAlvo.DayOfWeek];
                                    registrosPeriodo = vitalWeek.Data.Where(x => x.CreatedAt.Date == dataAlvo.Date).ToList();
                                }

                                if (registrosPeriodo.Count > 0)
                                {
                                    var mediaIPV = registrosPeriodo.Average(x => CalcularIPV(x, metaAgua, patrology));
                                    var mediaIGS = registrosPeriodo.Average(x => CalcularIGS(x, patrology));
                                    var mediaIGN = registrosPeriodo.Average(x => CalcularIGN(x, metaAgua, patrology));
                                    var mediaIES = registrosPeriodo.Average(x => CalcularIES(x, patrology));
                                    
                                    dass1 += registrosPeriodo.Sum(x => x.Dass1); 
                                    dass2 += registrosPeriodo.Sum(x => x.Dass2); 
                                    dass3 += registrosPeriodo.Sum(x => x.Dass3); 
                                    dass4 += registrosPeriodo.Sum(x => x.Dass4); 
                                    dass5 += registrosPeriodo.Sum(x => x.Dass5); 
                                    dass6 += registrosPeriodo.Sum(x => x.Dass6); 
                                    dass7 += registrosPeriodo.Sum(x => x.Dass7); 
                                    dass8 += registrosPeriodo.Sum(x => x.Dass8); 
                                    dass9 += registrosPeriodo.Sum(x => x.Dass9); 

                                    qtd += 1;

                                    IGS += (int)Math.Round(mediaIGS);
                                    IGN += (int)Math.Round(mediaIGN);
                                    IES += (int)Math.Round(mediaIES);
                                    IPV += (int)Math.Round(mediaIPV);

                                    weekMetrics.Add(new()
                                    {
                                        IGS = (int)Math.Round(mediaIGS),
                                        IGN = (int)Math.Round(mediaIGN),
                                        IES = (int)Math.Round(mediaIES),
                                        IPV = (int)Math.Round(mediaIPV),
                                        Day = labelEixo
                                    });
                                }
                                else
                                {
                                    weekMetrics.Add(new() { Day = labelEixo, IPV = 0, IGS = 0, IGN = 0, IES = 0 });
                                }
                            }
                        }
                    }
                };

                return new(new Vital() {
                    Id = vital.Data is null ? "" : vital.Data.Id,
                    WeekMetric = weekMetrics,
                    ChekinIGS = vital.Data is null ? false : vital.Data.ChekinIGS,
                    ChekinIGN = vital.Data is null ? false : vital.Data.ChekinIGN,
                    ChekinIES = vital.Data is null ? false : vital.Data.ChekinIES,
                    Metric = new () 
                        { 
                            IGS = qtd == 0 ? 0 : Math.Round(IGS / qtd), 
                            IGN = qtd == 0 ? 0 : Math.Round(IGN / qtd), 
                            IES = qtd == 0 ? 0 : Math.Round(IES / qtd), 
                            IPV = qtd == 0 ? 0 : Math.Round(IPV / qtd),
                        },
                    Dass1 = dass1 > 0 ? (int)dass1 / qtd : 0,
                    Dass2 = dass2 > 0 ? (int)dass2 / qtd : 0,
                    Dass3 = dass3 > 0 ? (int)dass3 / qtd : 0,
                    Dass4 = dass4 > 0 ? (int)dass4 / qtd : 0,
                    Dass5 = dass5 > 0 ? (int)dass5 / qtd : 0,
                    Dass6 = dass6 > 0 ? (int)dass6 / qtd : 0,
                    Dass7 = dass7 > 0 ? (int)dass7 / qtd : 0,
                    Dass8 = dass8 > 0 ? (int)dass8 / qtd : 0,
                    Dass9 = dass9 > 0 ? (int)dass9 / qtd : 0,
                    // Pauta 11+12: calcular dias consecutivos com IES < 50
                    IesBaixoConsec = await CalcularIesBaixoConsecAsync(beneficiaryId, customer.Data.Patrology, CalcularMetaAgua(customer.Data.Weight)),
                });
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

                ResponseApi<List<Vital>> vitals = await vitalRepository.GetBeneficiaryIAllAsync(request.BeneficiaryId);

                DateTime date = DateTime.UtcNow.AddDays(-1);
                ResponseApi<Vital?> vitalsLast = await vitalRepository.GetToDateBeneficiaryAsync(request.BeneficiaryId, date);

                if(vitalsLast.Data is not null)
                {
                    vital.ExtrasPoint = 1;

                    ResponseApi<Vital?> lastVital = await vitalRepository.GetToDateBeneficiaryAsync(request.BeneficiaryId, DateTime.UtcNow.AddDays(-1));
                    if(lastVital.Data is not null) 
                    {
                        vital.SequenceCheckIn += lastVital.Data.SequenceCheckIn;
                    }
                    else
                    {
                        vital.SequenceCheckIn = 1;
                    }
                }
                else
                {
                    vital.SequenceCheckIn = 0;
                }

                int pointIGS = vitals.Data is null ? 0 : vitals.Data.Sum(x => x.ChekinIGSPoint);
                int pointIGN = vitals.Data is null ? 0 : vitals.Data.Sum(x => x.ChekinIGNPoint);
                int pointIES = vitals.Data is null ? 0 : vitals.Data.Sum(x => x.ChekinIESPoint);

                int level = CurrentLevel(pointIGS + pointIGN + pointIES);
                vital.Level = level;

                ResponseApi<Vital?> response = await vitalRepository.CreateAsync(vital);

                if(response.Data is null) return new(null, 400, "Falha ao criar salvar.");
                
                return new(response.Data, 201, "Salvo com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        public async Task<ResponseApi<Vital?>> CreateISOAsync(CreateVitalDTO request)
        {
            try
            {
                Vital vital = _mapper.Map<Vital>(request);
                vital.ChekinISO = true;
                vital.ChekinISOPoint = request.ChekinISOPoint;
                vital.ChekinISOQuestion = request.ChekinISOQuestion;
                vital.ChekinISOResponse = request.ChekinISOResponse;

                ResponseApi<Vital?> response = await vitalRepository.CreateAsync(vital);

                if(response.Data is null) return new(null, 400, "Falha ao salvar.");
                
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
                
                ResponseApi<List<Vital>> vitals = await vitalRepository.GetBeneficiaryIAllAsync(request.BeneficiaryId);
                
                DateTime date = DateTime.UtcNow.AddDays(-1);
                ResponseApi<Vital?> vitalsLast = await vitalRepository.GetToDateBeneficiaryAsync(request.BeneficiaryId, date);

                if(vitalsLast.Data is not null)
                {
                    vitalResponse.Data.ExtrasPoint = 1;

                    ResponseApi<Vital?> lastVital = await vitalRepository.GetToDateBeneficiaryAsync(request.BeneficiaryId, DateTime.UtcNow.AddDays(-1));
                    if(lastVital.Data is not null) 
                    {
                        vitalResponse.Data.SequenceCheckIn += lastVital.Data.SequenceCheckIn;
                    }
                    else
                    {
                        vitalResponse.Data.SequenceCheckIn = 1;
                    }
                }
                else
                {
                    vitalResponse.Data.SequenceCheckIn = 0;
                }

                int pointIGS = vitals.Data is null ? 0 : vitals.Data.Sum(x => x.ChekinIGSPoint);
                int pointIGN = vitals.Data is null ? 0 : vitals.Data.Sum(x => x.ChekinIGNPoint);
                int pointIES = vitals.Data is null ? 0 : vitals.Data.Sum(x => x.ChekinIESPoint);

                int level = CurrentLevel(pointIGS + pointIGN + pointIES);
                vitalResponse.Data.Level = level;

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
        public async Task<ResponseApi<Vital?>> UpdateISOAsync(UpdateVitalDTO request)
        {
            try
            {
                ResponseApi<Vital?> vitalResponse = await vitalRepository.GetByIdAsync(request.Id);
                if(vitalResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                vitalResponse.Data.ChekinISO = true;
                vitalResponse.Data.ChekinISOQuestion = request.ChekinISOQuestion;
                
                ResponseApi<Vital?> response = await vitalRepository.UpdateAsync(vitalResponse.Data);
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
        public double CalcularIGS(Vital vital, string patrology)
        {
            if(!vital.ChekinIGS) return 0;

            double scoreDts = vital.SleepHours >= 8 ? 100 : (vital.SleepHours / 8.0) * 100;

            double scoreEs = (vital.SleepQuality / 5.0) * 100;

            double scoreCr = vital.SleepCell.ToLower() == "não" ? 100 : 50;

            double result = (scoreDts * 0.4) + (scoreEs * 0.3) + (scoreCr * 0.3);
            return AplicarDescontoScore(result, GetDescontoScore(patrology, "igs"));
        }

        public double CalcularIGN(Vital vital, decimal metaHidratacao, string patrology)
        {
            if(!vital.ChekinIGN) return 0;

            double scoreP1 = 100; 
            if (!string.IsNullOrEmpty(vital.LastMeal)) {
                TimeSpan.TryParse(vital.LastMeal, out var lastMealTime);
                scoreP1 = lastMealTime.Hours <= 20 ? 100 : 60;
            }

            double scoreP2 = vital.SnackHours == "3" ? 100 : 70;

            double scoreP3 = vital.GlycemicLoad.ToLower() switch {
                "leve" => 100,
                "media" => 50,
                "alta" => 0,
                _ => 50
            };

            double scoreP4 = (double)(vital.WaterAmount / metaHidratacao) * 100;
            if (scoreP4 > 100) scoreP4 = 100;

            double result = (scoreP1 * 0.4) + (scoreP2 * 0.3) + (scoreP3 * 0.2) + (scoreP4 * 0.1);

            return AplicarDescontoScore(result, GetDescontoScore(patrology, "ign"));
        }

        public double CalcularIES(Vital vital, string patrology)
        {
            if(!vital.ChekinIES) return 0;
            int somaDass = vital.Dass1 + vital.Dass2 + vital.Dass3 + 
                            vital.Dass4 + vital.Dass5 + vital.Dass6 + 
                            vital.Dass7 + vital.Dass8 + vital.Dass9;

            double scoreIES = ((27.0 - somaDass) / 27.0) * 100;
            double result = Math.Round(scoreIES, 2);
            return AplicarDescontoScore(result, GetDescontoScore(patrology, "ies"));
        }

        public double CalcularIPV(Vital vital, decimal metaHidratacao, string patrology)
        {
            var igs = CalcularIGS(vital, patrology);
            var ign = CalcularIGN(vital, metaHidratacao, patrology);
            var ies = CalcularIES(vital, patrology);

            double resultado = (igs + ign + ies) / 3;

            return Math.Round(resultado, 2);
        }

        public static decimal CalcularMetaAgua(decimal peso)
        {
            if (peso <= 0) return 0;

            decimal mlTotal = peso * 35;
            
            decimal litros = mlTotal / 1000;

            return litros;
        }

        public static string GetDayWeek(string day)
        {
            if (string.IsNullOrWhiteSpace(day)) return string.Empty;

            return day.ToLower().Trim() switch
            {
                "monday"    => "Segunda-feira",
                "tuesday"   => "Terça-feira",
                "wednesday" => "Quarta-feira",
                "thursday"  => "Quinta-feira",
                "friday"    => "Sexta-feira",
                "saturday"  => "Sábado",
                "sunday"    => "Domingo",
                _           => "Dia inválido" 
            };
        }
        public double AplicarDescontoScore(double score, double porcentagem)
        {
            double valorDesconto = score * (porcentagem / 100.0);
            double resultado = score - valorDesconto;

            return Math.Round(resultado);
        }
        public double GetDescontoScore(string patrology, string indice)
        {
            if(indice == "igs")
            {
                switch(patrology)
                {
                    case "Diabetes": return 0;
                    case "Hipertensão": return 0;
                    case "Ansiedade": return 15;
                    case "Neoplasia": return 15;
                    case "Bipolar": return 15;
                    case "Pós AVC": return 15;
                    case "Outros": return 0;
                    default: return 0;
                }
            }

            if(indice == "ign")
            {
                switch(patrology)
                {
                    case "Diabetes": return 15;
                    case "Hipertensão": return 15;
                    case "Ansiedade": return 0;
                    case "Neoplasia": return 15;
                    case "Bipolar": return 0;
                    case "Pós AVC": return 15;
                    case "Outros": return 0;
                    default: return 0;
                }
            }
            
            if(indice == "ies")
            {
                switch(patrology)
                {
                    case "Diabetes": return 0;
                    case "Hipertensão": return 0;
                    case "Ansiedade": return 15;
                    case "Neoplasia": return 15;
                    case "Bipolar": return 15;
                    case "Pós AVC": return 15;
                    case "Outros": return 0;
                    default: return 0;
                }
            }

            return 0;
        }
        public int CurrentLevel(int points)
        {
            if(points >= 0 && points <= 1000) return 1;
            if(points > 1000 && points <= 2000) return 2;
            return 3;
        }

        // Pauta 11+12: calcula quantos dias consecutivos recentes o IES ficou abaixo de 50
        private async Task<int> CalcularIesBaixoConsecAsync(string beneficiaryId, string patrology, decimal metaAgua)
        {
            // Busca vitais dos últimos 7 dias (suficiente para detectar 2 ou 3 dias consecutivos)
            ResponseApi<List<Vital>> recentes = await vitalRepository.GetRecentByBeneficiaryAsync(beneficiaryId, 7);
            if (recentes.Data is null || recentes.Data.Count == 0) return 0;

            // Agrupa por dia (pode haver mais de 1 check-in/dia), pega IES médio por dia
            var porDia = recentes.Data
                .GroupBy(v => v.CreatedAt.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => new {
                    Date = g.Key,
                    IES  = g.Average(v => CalcularIES(v, patrology))
                })
                .ToList();

            // Conta quantos dias consecutivos (a partir de hoje/ontem) o IES ficou abaixo de 50
            int consecutivos = 0;
            DateTime referencia = DateTime.UtcNow.Date;

            foreach (var dia in porDia)
            {
                // Aceita o dia de hoje ou dias imediatamente anteriores na sequência
                int diff = (referencia - dia.Date).Days;
                if (diff > 1) break; // quebrou a sequência
                if (dia.IES < 50)
                {
                    consecutivos++;
                    referencia = dia.Date;
                }
                else
                {
                    break; // sequência interrompida
                }
            }

            return consecutivos;
        }
        #endregion
    }
}