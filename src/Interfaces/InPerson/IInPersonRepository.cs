using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface IInPersonRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<InPerson> pagination);
        Task<ResponseApi<List<dynamic>>> GetTotalAsync(PaginationUtil<InPerson> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<InPerson?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<InPerson> pagination);
        Task<ResponseApi<InPerson?>> CreateAsync(InPerson user);
        Task<ResponseApi<InPerson?>> UpdateAsync(InPerson request);
        Task<ResponseApi<InPerson>> DeleteAsync(string id);
    }
}