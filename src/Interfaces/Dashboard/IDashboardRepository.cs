using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs.Dashboard;

namespace api_slim.src.Interfaces
{
    public interface IDashboardRepository
    {
        Task<ResponseApi<dynamic>>             GetFirstCardAsync();
        Task<ResponseApi<DashboardSummaryDto>> GetSummaryAsync();
    }
}
