using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface IAppointmentTelemedicineRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<AppointmentTelemedicine> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<AppointmentTelemedicine?>> GetByIdAsync(string id);
        Task<ResponseApi<AppointmentTelemedicine?>> GetByAppointmentUuidAsync(string uuid);
        Task<int> GetCountDocumentsAsync(PaginationUtil<AppointmentTelemedicine> pagination);
        Task<ResponseApi<AppointmentTelemedicine?>> CreateAsync(AppointmentTelemedicine user);
        Task<ResponseApi<AppointmentTelemedicine?>> UpdateAsync(AppointmentTelemedicine request);
        Task<ResponseApi<AppointmentTelemedicine>> DeleteAsync(string id);
    }
}