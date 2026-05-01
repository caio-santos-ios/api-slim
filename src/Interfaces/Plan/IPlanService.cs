using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface IPlanService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Plan?>> CreateAsync(CreatePlanDTO request);
        Task<ResponseApi<Plan?>> UpdateAsync(UpdatePlanDTO request);
        Task<ResponseApi<Plan?>> UpdateImageAsync(UpdatePlanDTO request);
        Task<ResponseApi<Plan>> DeleteAsync(string id);
    }
}