using api_slim.src.Configuration;
using api_slim.src.Handlers;
using api_slim.src.Models;
using MongoDB.Driver;

namespace api_slim.src.Workers;

public class NotificationWorker(IServiceProvider serviceProvider, ILogger<NotificationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("NotificationWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessPendingJobsAsync();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ProcessPendingJobsAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var smClick = scope.ServiceProvider.GetRequiredService<SmClickHandler>();

        var pending = await context.NotificationJobs
            .Find(j => !j.Sent && j.SendDate <= DateTime.UtcNow && (j.Parent == "InPerson" || j.Parent == "Appointment"))
            .ToListAsync();

        foreach (var job in pending)
        {
            try
            {
                var message = BuildMessage(job);
                await smClick.SendTextMessageAsync(job.Phone, message);

                await context.NotificationJobs.UpdateOneAsync(
                    j => j.Id == job.Id,
                    Builders<NotificationJob>.Update.Set(j => j.Sent, true));

                logger.LogInformation("Notification {Type} sent to {Name}", job.Type, job.BeneficiaryName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send notification {Type} to {Name}", job.Type, job.BeneficiaryName);
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }

    private static string BuildMessage(NotificationJob job)
    {
        return job.Type switch
        {
            "Welcome"    => WhatsAppTemplate.Welcome(job.BeneficiaryName),
            "InstalationApp"    => WhatsAppTemplate.AppDownloadInstructions(),
            "Notification" => job.Message,
            "WhatsApp" => job.Message,
            _ => ""
        };
    }
}