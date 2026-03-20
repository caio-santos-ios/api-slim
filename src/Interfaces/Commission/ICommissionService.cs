using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface ICommissionService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Commission?>> CreateAsync(CreateCommissionDTO request);
        Task<ResponseApi<Commission?>> UpdateAsync(UpdateCommissionDTO request);
        Task<ResponseApi<Commission>> DeleteAsync(string id);
    }
}