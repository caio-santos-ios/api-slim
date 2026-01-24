using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface ITelemedicineHistoricService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<TelemedicineHistoric?>> CreateAsync(CreateTelemedicineHistoricDTO request);
    }
}