using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
   public interface ICustomerContractService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<CustomerContract?>> CreateAsync(CreateCustomerContractDTO request);
    Task<ResponseApi<CustomerContract?>> UpdateAsync(UpdateCustomerContractDTO request);
    Task<ResponseApi<CustomerContract>> DeleteAsync(string id, string userId);
}
}