using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface INotificationRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<NotificationJob> pagination);
        Task<ResponseApi<NotificationJob?>> GetByIdAsync(string id);
        Task<ResponseApi<NotificationJob?>> UpdateAsync(NotificationJob notification);
    }
}