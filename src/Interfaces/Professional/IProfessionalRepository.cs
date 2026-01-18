using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface IProfessionalRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Professional> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Professional?>> GetByIdAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Professional> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Professional> pagination);
        Task<ResponseApi<Professional?>> CreateAsync(Professional procedure);
        Task<ResponseApi<Professional?>> UpdateAsync(Professional procedure);
        Task<ResponseApi<Professional>> DeleteAsync(string id);
    }
}