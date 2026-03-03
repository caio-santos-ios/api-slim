using api_slim.src.Handlers;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Services
{
    public class SmClickService(IAppointmentNotificationService appointmentNotificationService) : ISmClickService
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
                List<NotificationJob> jobs = [
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
    }
}