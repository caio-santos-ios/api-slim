using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface ICustomerRecipientService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> GetAtendimentoAsync(string id);
        Task<ResponseApi<dynamic?>> GetByCPFAggregateAsync(string cpf);
        Task<ResponseApi<dynamic?>> GetByRapidocIdAsync(string rapidocId);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<CustomerRecipient?>> CreateAsync(CreateCustomerRecipientDTO request);
        Task<ResponseApi<CustomerRecipient?>> EmailAsync(CreateCustomerRecipientDTO request);
        Task<ResponseApi<CustomerRecipient?>> UpdateAsync(UpdateCustomerRecipientDTO request);
        Task<ResponseApi<CustomerRecipient?>> UpdateProfileAsync(UpdateCustomerRecipientDTO request);
        Task<ResponseApi<CustomerRecipient?>> UpdateDassAsync(UpdateDassCustomerRecipientDTO request);
        Task<ResponseApi<CustomerRecipient?>> UpdateProfilePhotoAsync(UpdatePhotoCustomerRecipientDTO request);
        Task<ResponseApi<CustomerRecipient?>> UpdateStatusAsync(UpdateCustomerRecipientDTO request);
        Task<ResponseApi<CustomerRecipient>> DeleteAsync(string id, string userId);
    }
}