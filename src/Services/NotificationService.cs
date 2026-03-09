using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Services
{
    public class NotificationService(INotificationRepository repository) : INotificationService
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<NotificationJob> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> notifications = await repository.GetAllAsync(pagination);
                return new(notifications.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region UPDATE
        public async Task<ResponseApi<dynamic>> UpdateAsync(string id)
        {
            try
            {
                ResponseApi<NotificationJob?> notification = await repository.GetByIdAsync(id);

                if(notification.Data is not null)
                {
                    notification.Data.Sent = false;
                    notification.Data.SendDate = DateTime.Now;

                    await repository.UpdateAsync(notification.Data);
                }

                return new(null, 200, "Notificação reenviada");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}