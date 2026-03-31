using api_slim.src.Configuration;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using MongoDB.Driver;

namespace api_slim.src.Services;

public class AppointmentNotificationService(AppDbContext context) : IAppointmentNotificationService
{
    public async Task ScheduleNotificationsAsync(string phone, string beneficiaryName, DateTime appointmentDate)
    {
        var jobs = new List<NotificationJob>
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

        // await context.NotificationJobs.DeleteManyAsync(
        //     Builders<NotificationJob>.Filter.And(
        //         Builders<NotificationJob>.Filter.Eq(j => j.Phone, phone),
        //         Builders<NotificationJob>.Filter.Eq(j => j.AppointmentDate, appointmentDate),
        //         Builders<NotificationJob>.Filter.Eq(j => j.Sent, false)
        //     ));
        await context.NotificationJobs.InsertManyAsync(jobs);
    }
    public async Task CreateNotificationsAsync(List<NotificationJob> jobs, string phone)
    {
        // await context.NotificationJobs.DeleteManyAsync(
        //     Builders<NotificationJob>.Filter.And(
        //         Builders<NotificationJob>.Filter.Eq(j => j.Phone, phone),
        //         Builders<NotificationJob>.Filter.Eq(j => j.Sent, false)
        //     ));
        await context.NotificationJobs.InsertManyAsync(jobs);
    }

    public async Task CancelNotificationsAsync(string parentId, string parent)
    {
        var filter = Builders<NotificationJob>.Filter.And(
            Builders<NotificationJob>.Filter.Eq(j => j.ParentId, parentId),
            Builders<NotificationJob>.Filter.Eq(j => j.Parent, parent),
            Builders<NotificationJob>.Filter.Eq(j => j.Sent, false)
        );

        await context.NotificationJobs.DeleteManyAsync(filter);
    }
}