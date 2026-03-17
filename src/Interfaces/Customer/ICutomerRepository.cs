using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
public interface ICustomerRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Customer> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Customer?>> GetByIdAsync(string id);
    Task<ResponseApi<Customer?>> GetByEmailAsync(string email);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Customer> pagination);
    Task<ResponseApi<Customer?>> CreateAsync(Customer address);
    Task<ResponseApi<Customer?>> UpdateAsync(Customer address);
    Task<ResponseApi<Customer>> DeleteAsync(string id);
}
}