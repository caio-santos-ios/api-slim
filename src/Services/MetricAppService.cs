using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    public class MetricAppService(IMetricAppRepository metricAppRepository, IMapper _mapper) : IMetricAppService
    {
        #region READ
        public async Task<ResponseApi<dynamic>> GetSummaryAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await metricAppRepository.GetSummaryAsync(startDate, endDate);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado ao buscar o resumo de métricas.");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetTopUsersAsync(int limit = 10)
        {
            try
            {
                return await metricAppRepository.GetTopUsersAsync(limit);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado ao buscar usuários mais ativos.");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetTopFeaturesAsync(int limit = 10)
        {
            try
            {
                return await metricAppRepository.GetTopFeaturesAsync(limit);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado ao buscar funcionalidades mais usadas.");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetTimelineAsync(int days = 30)
        {
            try
            {
                return await metricAppRepository.GetTimelineAsync(days);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado ao buscar timeline de atividade.");
            }
        }
        
        #endregion
        #region CREATE
        public async Task<ResponseApi<MetricApp?>> CreateAsync(CreateMetricAppDTO request)
        {
            try
            {
                MetricApp metricApp = _mapper.Map<MetricApp>(request);
                metricApp.CreatedAt = DateTime.UtcNow;
                
                ResponseApi<MetricApp?> response = await metricAppRepository.CreateAsync(metricApp);

                return new(null, 201, "Metrica criado com sucesso.");
            }
            catch(Exception ex)
            {                
                System.Console.WriteLine(ex.Message);
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
    }
}