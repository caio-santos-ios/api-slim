using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
   public interface IContactService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Contact?>> CreateAsync(CreateContactDTO request);
    Task<ResponseApi<Contact?>> UpdateAsync(UpdateContactDTO request);
    Task<ResponseApi<Contact>> DeleteAsync(string id, string userId);
}
}