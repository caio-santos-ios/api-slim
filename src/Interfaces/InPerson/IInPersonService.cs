using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface IInPersonService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<List<dynamic>>> GetTotalAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<InPerson?>> CreateAsync(CreateInPersonDTO user);
        Task<ResponseApi<InPerson?>> UpdateAsync(UpdateInPersonDTO request);
        Task<ResponseApi<InPerson?>> UpdateStatusAsync(UpdateInPersonStatusDTO request);
        Task<ResponseApi<InPerson>> DeleteAsync(string id);
    }
}