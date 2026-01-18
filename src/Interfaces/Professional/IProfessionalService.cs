using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface IProfessionalService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
    Task<ResponseApi<Professional?>> CreateAsync(CreateProfessionalDTO request);
    Task<ResponseApi<Professional?>> UpdateAsync(UpdateProfessionalDTO request);
    Task<ResponseApi<Professional>> DeleteAsync(string id);
}
}