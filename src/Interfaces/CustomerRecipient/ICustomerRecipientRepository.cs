using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
public interface ICustomerRecipientRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<CustomerRecipient> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<CustomerRecipient?>> GetByIdAsync(string id);
    Task<ResponseApi<CustomerRecipient?>> GetByRapidocIdAsync(string rapidoc);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<CustomerRecipient> pagination);
    Task<ResponseApi<long?>> GetNextCodeAsync();
    Task<ResponseApi<CustomerRecipient?>> GetByCPFAsync(string cpf, string contractorId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<CustomerRecipient> pagination);
    Task<ResponseApi<CustomerRecipient?>> CreateAsync(CustomerRecipient address);
    Task<ResponseApi<CustomerRecipient?>> UpdateAsync(CustomerRecipient address);
    Task<ResponseApi<CustomerRecipient>> DeleteAsync(string id);
}
}