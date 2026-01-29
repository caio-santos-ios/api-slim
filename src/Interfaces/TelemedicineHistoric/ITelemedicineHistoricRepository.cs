using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface ITelemedicineHistoricRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<TelemedicineHistoric> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<TelemedicineHistoric> pagination);
        Task<ResponseApi<TelemedicineHistoric?>> GetByParentIdAsync(string parentId, string type);
        Task<ResponseApi<TelemedicineHistoric?>> CreateAsync(TelemedicineHistoric historic);
    }
}