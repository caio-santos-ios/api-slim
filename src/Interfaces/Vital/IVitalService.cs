using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface IVitalService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<Vital>>> GetByBeneficiaryAllAsync(string beneficiaryId, string startDate, string endDate);
        Task<ResponseApi<Vital?>> GetByBeneficiaryIdAsync(string beneficiaryId, string period);
        Task<ResponseApi<Vital?>> GetByBeneficiaryAsync(string beneficiaryId);
        Task<ResponseApi<Vital?>> CreateAsync(CreateVitalDTO request);
        Task<ResponseApi<Vital?>> UpdateAsync(UpdateVitalDTO request);
        Task<ResponseApi<Vital>> DeleteAsync(string id, string userId);
    }
}