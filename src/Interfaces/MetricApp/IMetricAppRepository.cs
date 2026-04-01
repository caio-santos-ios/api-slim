using api_slim.src.Models;
using api_slim.src.Models.Base;

namespace api_slim.src.Interfaces
{
    public interface IMetricAppRepository
    {
        Task<ResponseApi<MetricApp?>> CreateAsync(MetricApp metricApp);
        Task<ResponseApi<dynamic>> GetSummaryAsync(DateTime startDate, DateTime endDate);
        Task<ResponseApi<List<dynamic>>> GetTopUsersAsync(int limit);
        Task<ResponseApi<List<dynamic>>> GetTopFeaturesAsync(int limit);
        Task<ResponseApi<List<dynamic>>> GetTimelineAsync(int days);
    }
}