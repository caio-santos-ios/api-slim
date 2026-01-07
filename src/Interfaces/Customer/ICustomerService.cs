using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
   public interface ICustomerService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Customer?>> CreateAsync(CreateCustomerDTO request);
    Task<ResponseApi<Customer?>> UpdateAsync(UpdateCustomerDTO request);
    Task<ResponseApi<Customer>> DeleteAsync(string id, string userId);
}
}