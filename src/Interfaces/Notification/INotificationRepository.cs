using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface INotificationRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<NotificationJob> pagination);
        Task<ResponseApi<List<dynamic>>> GetListAsync(PaginationUtil<NotificationJob> pagination);
        Task<ResponseApi<NotificationJob?>> GetByIdAsync(string id);
        Task<ResponseApi<List<NotificationJob>>> GetByParentIdAsync(string parentId, string parent);
        Task<ResponseApi<NotificationJob?>> GetByTypeAsync(string cpf, string type);
        Task<int> GetCountDocumentsAsync(PaginationUtil<NotificationJob> pagination);
        Task<ResponseApi<NotificationJob?>> CreateAsync(NotificationJob notification);
        Task<ResponseApi<NotificationJob?>> UpdateAsync(NotificationJob notification);
    }
}