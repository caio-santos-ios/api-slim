using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
   public interface IAccreditedNetworkService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
    Task<ResponseApi<AccreditedNetwork?>> CreateAsync(CreateAccreditedNetworkDTO request);
    Task<ResponseApi<AccreditedNetwork?>> UpdateAsync(UpdateAccreditedNetworkDTO request);
    Task<ResponseApi<AccreditedNetwork?>> UpdateStatusAsync(UpdateAccreditedNetworkDTO request);
    Task<ResponseApi<AccreditedNetwork>> DeleteAsync(string id, string userId);
}
}