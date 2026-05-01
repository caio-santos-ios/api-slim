using api_slim.src.Handlers;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Services
{
    public class SmClickService(IAppointmentNotificationService appointmentNotificationService, SmClickHandler smClickHandler, INotificationRepository notificationRepository, WebPushHandler pushHandler, ICustomerRecipientRepository customerRecipientRepository) : ISmClickService
    {
        private readonly HttpClient client = new();
        private readonly string baseUrl     = Environment.GetEnvironmentVariable("SMCLICK_BASE_URL")      ?? "";
        private readonly string apiToken    = Environment.GetEnvironmentVariable("SMCLICK_API_TOKEN")     ?? "";
        private readonly string instanceKey = Environment.GetEnvironmentVariable("SMCLICK_INSTANCE_KEY")  ?? "";
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(GetAllDTO requestDTO)
        {
            try
            {
                List<Notification> jobs = [
                    new() {
                        Phone = "11982332816",
                        BeneficiaryName = "Caio dos Santos",
                        SendDate = DateTime.UtcNow,
                        Type = "Welcome",
                        // ScheduledAt = DateTime.UtcNow
                    },
                    new() {
                        Phone = "11982332816",
                        BeneficiaryName = "Caio dos Santos",
                        SendDate = DateTime.UtcNow.AddMinutes(1),
                        Type = "InstalationApp",
                        // ScheduledAt = DateTime.UtcNow.AddSeconds(30)
                    }
                ];

                await appointmentNotificationService.CreateNotificationsAsync(jobs, "11982332816");

                // await smClickHandler.SendTextMessageAsync("11982332816", "Teste de mensagem via SMClick API");

                return new([]);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }   
        #endregion
        #region UPDATE
        public async Task<ResponseApi<dynamic?>> SendNotificationAsync(SendNotificationSmClickDTO requestDTO)
        {
            try
            {
                ResponseApi<Notification?> notification = await notificationRepository.GetByIdAsync(requestDTO.NotificationId);
                if(notification.Data is null) return new(null, 400, "Notificação não foi encontrada");
                bool send = false;

                if(notification.Data.Type == "WhatsApp")
                {
                    await smClickHandler.SendTextMessageAsync(Util.CleanPhone(notification.Data.Phone), notification.Data.Message);
                    send = true;
                    notification.Data.SendDate = DateTime.UtcNow;
                    notification.Data.Sent = true;
                }
                
                if(notification.Data.Type == "AppPush")
                {
                    ResponseApi<CustomerRecipient?> recipient = await customerRecipientRepository.GetByIdAsync(requestDTO.BeneficiaryId);

                    if(recipient.Data is not null)
                    {
                        if(recipient.Data.SubNotification != null && recipient.Data.SubNotification.UserId != "") {
                            await pushHandler.SendPushAsync(
                                subDto : recipient.Data.SubNotification!,
                                title  : notification.Data.Title,
                                message: notification.Data.Message,
                                url    : string.IsNullOrEmpty(notification.Data.Link) ? "/aplicativo/home/" : notification.Data.Link,
                                tag    : "notification"
                            );
                            send = true;
                            notification.Data.SendDate = DateTime.UtcNow;
                            notification.Data.Sent = true;
                        }
                        else
                        {
                            return new(null, 400, "Beneficiário precisa permitir receber notificação no dispositivo dele");
                        }
                    }
                }

                if(send)
                {
                    await notificationRepository.UpdateAsync(notification.Data);
                }


                return new(notification.Data, 200, "Nofiticação enviada com sucesso");
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }   
        #endregion
    }
}