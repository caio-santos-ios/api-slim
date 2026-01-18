using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
public interface IAccreditedNetworkRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<AccreditedNetwork> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<AccreditedNetwork?>> GetByIdAsync(string id);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<AccreditedNetwork> pagination);
    Task<ResponseApi<long?>> GetNextCodeAsync();
    Task<int> GetCountDocumentsAsync(PaginationUtil<AccreditedNetwork> pagination);
    Task<ResponseApi<AccreditedNetwork?>> CreateAsync(AccreditedNetwork billing);
    Task<ResponseApi<AccreditedNetwork?>> UpdateAsync(AccreditedNetwork billing);
    Task<ResponseApi<AccreditedNetwork>> DeleteAsync(string id);
}
}