using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface IGenericTableRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<GenericTable> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<GenericTable?>> GetByIdAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetByTableAggregateAsync(string table);
        Task<ResponseApi<List<dynamic>>> GetByTableAllAggregateAsync(string table);
        Task<ResponseApi<GenericTable?>> GetByCodeAsync(string code, string table, string? id = null);
        Task<int> GetCountDocumentsAsync(PaginationUtil<GenericTable> pagination);
        Task<ResponseApi<GenericTable?>> CreateAsync(GenericTable user);
        Task<ResponseApi<GenericTable?>> UpdateAsync(GenericTable request);
        Task<ResponseApi<GenericTable>> DeleteAsync(string id);
        Task<ResponseApi<GenericTable>> DeleteByTableAsync(string table);
    }
}