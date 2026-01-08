using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface IPlanRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Plan> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Plan?>> GetByIdAsync(string id);
        Task<ResponseApi<long?>> GetNextCodeAsync();
        Task<int> GetCountDocumentsAsync(PaginationUtil<Plan> pagination);
        Task<ResponseApi<Plan?>> CreateAsync(Plan plan);
        Task<ResponseApi<Plan?>> UpdateAsync(Plan plan);
        Task<ResponseApi<Plan>> DeleteAsync(string id);
    }
}