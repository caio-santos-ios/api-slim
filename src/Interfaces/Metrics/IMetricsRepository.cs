using api_slim.src.Models.Base;

namespace api_slim.src.Interfaces.Metrics
{
    public interface IMetricsRepository
    {
        Task<ResponseApi<dynamic>> GetSummaryAsync();
        Task<ResponseApi<List<dynamic>>> GetTopUsersAsync(int limit);
        Task<ResponseApi<List<dynamic>>> GetTopFeaturesAsync(int limit);
        Task<ResponseApi<List<dynamic>>> GetTimelineAsync(int days);
    }
}
