using api_slim.src.Configuration;
using api_slim.src.Handlers;
using api_slim.src.Models;
using MongoDB.Driver;

namespace api_slim.src.Workers;

public class AppPushWorker(IServiceProvider serviceProvider, ILogger<AppPushWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("AppPushWorker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessPushJobsAsync();
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessPushJobsAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var context     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var pushHandler = scope.ServiceProvider.GetRequiredService<WebPushHandler>();

        List<NotificationJob> notifications = await context.NotificationJobs
            .Find(n => 
                n.Type == "AppPush" && 
                !n.Sent && 
                n.SendDate <= DateTime.UtcNow
            ).ToListAsync();


        foreach (NotificationJob notification in notifications)
        {
            try
            {
                CustomerRecipient recipients = await context.CustomerRecipients
                .Find(c => c.Cpf == "086.306.285-70" && !c.Deleted
                        && c.Active
                        && c.SubNotification != null && c.SubNotification.UserId != "")
                .FirstOrDefaultAsync();

                if(recipients is not null)
                {
                    logger.LogInformation("Enviando Push para {Name}", recipients.Name);

                    await pushHandler.SendPushAsync(
                        subDto : recipients.SubNotification!,
                        title  : notification.Title,
                        message: $"Olá, {recipients.Name.Split(" ")[0]}! Você tem uma nova notificação importante.",
                        url    : "/aplicativo/home/",
                        tag    : "important-notification"
                    );

                    var update = Builders<NotificationJob>.Update.Set(j => j.Sent, true);
                    await context.NotificationJobs.UpdateOneAsync(j => j.Id == notification.Id, update);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar Push para {Name}: {Message}", notification.BeneficiaryName, ex.Message);

                // if (ex.Message.Contains("SubscriptionExpired"))
                // {
                //     var update = Builders<api_slim.src.Models.CustomerRecipient>.Update
                //         .Set(c => c.SubNotification, null);
                //     await context.CustomerRecipients.UpdateOneAsync(c => c.Id == recipient.Id, update);
                //     logger.LogInformation("Subscription removida para {Name}", recipient.Name);
                // }
            }
        }
    }
}