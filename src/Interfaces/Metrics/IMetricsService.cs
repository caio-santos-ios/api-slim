using api_slim.src.Models.Base;

namespace api_slim.src.Interfaces.Metrics
{
    public interface IMetricsService
    {
        Task<ResponseApi<dynamic>> GetSummaryAsync();
        Task<ResponseApi<List<dynamic>>> GetTopUsersAsync(int limit = 10);
        Task<ResponseApi<List<dynamic>>> GetTopFeaturesAsync(int limit = 10);
        Task<ResponseApi<List<dynamic>>> GetTimelineAsync(int days = 30);
    }
}
