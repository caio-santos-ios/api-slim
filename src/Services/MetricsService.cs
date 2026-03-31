using api_slim.src.Interfaces.Metrics;
using api_slim.src.Models.Base;

namespace api_slim.src.Services
{
    public class MetricsService(IMetricsRepository metricsRepository) : IMetricsService
    {
        public async Task<ResponseApi<dynamic>> GetSummaryAsync()
        {
            try
            {
                return await metricsRepository.GetSummaryAsync();
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
                return await metricsRepository.GetTopUsersAsync(limit);
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
                return await metricsRepository.GetTopFeaturesAsync(limit);
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
                return await metricsRepository.GetTimelineAsync(days);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado ao buscar timeline de atividade.");
            }
        }
    }
}
