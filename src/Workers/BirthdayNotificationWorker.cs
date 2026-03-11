using api_slim.src.Configuration;
using api_slim.src.Handlers;
using api_slim.src.Shared.Utils;
using MongoDB.Driver;

namespace api_slim.src.Workers;

public class BirthdayNotificationWorker(IServiceProvider serviceProvider, ILogger<BirthdayNotificationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("BirthdayNotificationWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await WaitUntilNineAMAsync(stoppingToken);
            await ProcessBirthdaysAsync();
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task WaitUntilNineAMAsync(CancellationToken stoppingToken)
    {
        var now = DateTime.Now;
        var nineAM = DateTime.Today.AddHours(9);

        if (now > nineAM) nineAM = nineAM.AddDays(1);

        var delay = nineAM - now;
        logger.LogInformation("BirthdayWorker aguardando até {Time}", nineAM);
        await Task.Delay(delay, stoppingToken);
    }

    private async Task ProcessBirthdaysAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var smClick = scope.ServiceProvider.GetRequiredService<SmClickHandler>();

        var today = DateTime.UtcNow;

        var birthdayRecipients = await context.CustomerRecipients
            .Find(r => r.DateOfBirth.HasValue && r.DateOfBirth.Value.Month == today.Month && r.DateOfBirth.Value.Day == today.Day)
            .ToListAsync();

        logger.LogInformation("BirthdayWorker encontrou {Count} aniversariantes hoje.", birthdayRecipients.Count);

        foreach (var recipient in birthdayRecipients)
        {
            try
            {
                var message = WhatsAppTemplate.HappyBirthday(recipient.Name);
                // await smClick.SendTextMessageAsync(Util.CleanPhone(recipient.Phone), message);
                logger.LogInformation("Mensagem de aniversário enviada para {Name}", recipient.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao enviar aniversário para {Name}", recipient.Name);
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}