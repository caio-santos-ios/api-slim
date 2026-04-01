using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface IMetricAppService
    {
        Task<ResponseApi<MetricApp?>> CreateAsync(CreateMetricAppDTO request);
        Task<ResponseApi<dynamic>> GetSummaryAsync(DateTime startDate, DateTime endDate);
        Task<ResponseApi<List<dynamic>>> GetTopUsersAsync(int limit = 10);
        Task<ResponseApi<List<dynamic>>> GetTopFeaturesAsync(int limit = 10);
        Task<ResponseApi<List<dynamic>>> GetTimelineAsync(int days = 30);
    }
}