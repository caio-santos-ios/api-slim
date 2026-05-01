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
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        SmClickHandler smClick = scope.ServiceProvider.GetRequiredService<SmClickHandler>();
        WebPushHandler pushHandler = scope.ServiceProvider.GetRequiredService<WebPushHandler>();
        
        DateTime now = DateTime.UtcNow;
        DateTime startOfDay = now.Date;           
        DateTime endOfDay = startOfDay.AddDays(1); 

        List<Notification> notifications = await context.Notifications
            .Find(j => !j.Sent 
                    && j.SendPreviusDate >= startOfDay 
                    && j.SendPreviusDate < endOfDay
                    && j.SendPreviusDate <= now)        
            .ToListAsync();

        foreach (var job in notifications)
        {
            try
            {
                bool send = false;

                if(job.Type == "WhatsApp") 
                {
                    await smClick.SendTextMessageAsync(job.Phone, job.Message);
                    send = true;
                }
                
                if(job.Type == "AppPush") 
                {
                    CustomerRecipient? recipient = await context.CustomerRecipients.Find(x => !x.Deleted && x.Cpf == job.BeneficiaryCPF).FirstOrDefaultAsync();
                    
                    if(recipient is not null)
                    {
                        if(recipient.SubNotification != null && recipient.SubNotification.UserId != "")
                        {
                            await pushHandler.SendPushAsync(
                                subDto : recipient.SubNotification!,
                                title  : job.Title,
                                message: $"Olá, {recipient.Name.Split(" ")[0]}! Você tem uma nova notificação importante.",
                                url    : "/aplicativo/home/",
                                tag    : "important-notification"
                            );
                            send = true;
                        }
                    }
                }

                if(send)
                {
                    await context.Notifications.UpdateOneAsync(
                        j => j.Id == job.Id,
                        Builders<Notification>.Update
                        .Set(j => j.Sent, true)
                        .Set(j => j.SendDate, DateTime.UtcNow));

                    logger.LogInformation("Notification {Type} sent to {Name}", job.Type, job.BeneficiaryName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send notification {Type} to {Name}", job.Type, job.BeneficiaryName);
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }

    private static string BuildMessage(Notification job)
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