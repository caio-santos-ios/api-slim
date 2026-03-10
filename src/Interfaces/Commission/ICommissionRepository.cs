using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface ICommissionRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Commission> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Commission?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Commission> pagination);
        Task<ResponseApi<Commission?>> CreateAsync(Commission commission);
        Task<ResponseApi<Commission?>> UpdateAsync(Commission commission);
        Task<ResponseApi<Commission>> DeleteAsync(string id);
    }
}