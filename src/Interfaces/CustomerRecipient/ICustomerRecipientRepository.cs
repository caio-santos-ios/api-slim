using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface ICustomerRecipientRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<CustomerRecipient> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> GetByCPFAggregateAsync(string cpf);
        Task<ResponseApi<CustomerRecipient?>> GetByIdAsync(string id);
        Task<ResponseApi<CustomerRecipient?>> GetByCodeAccessAsync(string codeAccess);
        Task<ResponseApi<CustomerRecipient?>> GetByRapidocIdAsync(string rapidoc);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<CustomerRecipient> pagination);
        Task<ResponseApi<List<dynamic>>> GetManagerContractorIdAggregationAsync(PaginationUtil<CustomerRecipient> pagination);
        Task<ResponseApi<long?>> GetNextCodeAsync();
        Task<ResponseApi<CustomerRecipient?>> GetByCPFAsync(string cpf, string contractorId);
        Task<ResponseApi<CustomerRecipient?>> GetByDocumentAsync(string cpf);
        Task<ResponseApi<CustomerRecipient?>> GetByPhoneAsync(string phone);
        Task<ResponseApi<CustomerRecipient?>> GetByEmailAsync(string email);
        Task<ResponseApi<CustomerRecipient?>> GetByCPFImportAsync(string cpf, string contractorId);
        Task<ResponseApi<List<CustomerRecipient>>> GetPeriodAsync(int month, int year, string contractorId);
        Task<ResponseApi<List<CustomerRecipient>>> GetContractIdAsync(string contractorId);
        Task<ResponseApi<List<CustomerRecipient>>> GetAsync();
        Task<int> GetCountDocumentsAsync(PaginationUtil<CustomerRecipient> pagination);
        Task<ResponseApi<CustomerRecipient?>> CreateAsync(CustomerRecipient address);
        Task<ResponseApi<CustomerRecipient?>> UpdateAsync(CustomerRecipient address);
        Task<ResponseApi<CustomerRecipient>> DeleteAsync(string id);
    }
}