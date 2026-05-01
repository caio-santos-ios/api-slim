using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface INotificationRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Notification> pagination);
        Task<ResponseApi<List<dynamic>>> GetListAsync(PaginationUtil<Notification> pagination);
        Task<ResponseApi<Notification?>> GetByIdAsync(string id);
        Task<ResponseApi<List<Notification>>> GetByParentIdAsync(string parentId, string parent);
        Task<ResponseApi<Notification?>> GetByTypeAsync(string cpf, string type);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Notification> pagination);
        Task<ResponseApi<Notification?>> CreateAsync(Notification notification);
        Task<ResponseApi<Notification?>> UpdateAsync(Notification notification);
    }
}