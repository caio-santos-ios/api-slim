using api_slim.src.Models;

namespace api_slim.src.Interfaces
{
    public interface IAppointmentNotificationService
    {
        Task ScheduleNotificationsAsync(string phone, string beneficiaryName, DateTime appointmentDate);
        Task CreateNotificationsAsync(List<NotificationJob> jobs, string phone);
        Task CancelNotificationsAsync(string parentId, string parent);
    }
}