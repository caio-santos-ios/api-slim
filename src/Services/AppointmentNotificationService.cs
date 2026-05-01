using api_slim.src.Configuration;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using MongoDB.Driver;

namespace api_slim.src.Services;

public class AppointmentNotificationService(AppDbContext context) : IAppointmentNotificationService
{
    public async Task ScheduleNotificationsAsync(string phone, string beneficiaryName, DateTime appointmentDate)
    {
        var jobs = new List<Notification>
        {
        //     new() {
        //         Phone = phone,
        //         BeneficiaryName = beneficiaryName,
        //         AppointmentDate = appointmentDate,
        //         Type = "Confirmation",
        //         ScheduledAt = DateTime.UtcNow
        //     },
        //     new() {
        //         Phone = phone,
        //         BeneficiaryName = beneficiaryName,
        //         AppointmentDate = appointmentDate,
        //         Type = "DayReminder",
        //         ScheduledAt = appointmentDate.AddDays(-1)
        //     },
        //     new() {
        //         Phone = phone,
        //         BeneficiaryName = beneficiaryName,
        //         AppointmentDate = appointmentDate,
        //         Type = "MinutesReminder",
        //         ScheduledAt = appointmentDate.AddMinutes(-15)
        //     }
        };

        // await context.Notifications.DeleteManyAsync(
        //     Builders<Notification>.Filter.And(
        //         Builders<Notification>.Filter.Eq(j => j.Phone, phone),
        //         Builders<Notification>.Filter.Eq(j => j.AppointmentDate, appointmentDate),
        //         Builders<Notification>.Filter.Eq(j => j.Sent, false)
        //     ));
        await context.Notifications.InsertManyAsync(jobs);
    }
    public async Task CreateNotificationsAsync(List<Notification> jobs, string phone)
    {
        // await context.Notifications.DeleteManyAsync(
        //     Builders<Notification>.Filter.And(
        //         Builders<Notification>.Filter.Eq(j => j.Phone, phone),
        //         Builders<Notification>.Filter.Eq(j => j.Sent, false)
        //     ));
        await context.Notifications.InsertManyAsync(jobs);
    }

    public async Task CancelNotificationsAsync(string parentId, string parent)
    {
        var filter = Builders<Notification>.Filter.And(
            Builders<Notification>.Filter.Eq(j => j.ParentId, parentId),
            Builders<Notification>.Filter.Eq(j => j.Parent, parent),
            Builders<Notification>.Filter.Eq(j => j.Sent, false)
        );

        await context.Notifications.DeleteManyAsync(filter);
    }
}