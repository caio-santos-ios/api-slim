using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface IAppointmentService
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<PaginationApi<List<dynamic>>> GetAllV2Async(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSpecialtiesAllAsync();
        Task<ResponseApi<List<dynamic>>> GetSpecialtyAvailabilityAllAsync(string specialtyUuid, string beneficiaryUuid);
        Task<ResponseApi<List<dynamic>>> GetBeneficiaryMedicalReferralsAsync();
        Task<ResponseApi<dynamic?>> CreateAsync(CreateAppointmentDTO forwarding);
        Task<ResponseApi<dynamic?>> CancelAsync(CancelForwardingDTO request);
    }
}