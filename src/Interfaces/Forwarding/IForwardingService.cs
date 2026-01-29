using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface IForwardingService
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<List<dynamic>>> GetByBeneficiaryIdAsync(string beneficiaryId);
        Task<ResponseApi<List<dynamic>>> GetSpecialtiesAllAsync();
        Task<ResponseApi<List<dynamic>>> GetSpecialtyAvailabilityAllAsync(string specialtyUuid, string beneficiaryUuid);
        Task<ResponseApi<List<dynamic>>> GetBeneficiaryMedicalReferralsAsync();
        Task<ResponseApi<dynamic?>> CreateAsync(CreateForwardingDTO forwarding);
        Task<ResponseApi<dynamic?>> CancelAsync(CancelForwardingDTO request);
    }
}