using api_slim.src.Configuration;
using api_slim.src.Handlers;
using api_slim.src.Models;
using MongoDB.Driver;

namespace api_slim.src.Workers;

public class WebPushWorker(IServiceProvider serviceProvider, ILogger<WebPushWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("WebPushWorker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessPushJobsAsync();
            // Regra específica: Verificar a cada 30 segundos ou o tempo que desejar
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task ProcessPushJobsAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var pushHandler = scope.ServiceProvider.GetRequiredService<WebPushHandler>();

        var recipients = await context.CustomerRecipients
            .Find(c => !c.Deleted && c.Active && c.SubNotification != null && c.Cpf == "086.306.285-70") 
            .ToListAsync();

        foreach (var recipient in recipients)
        {
            try
            {
                logger.LogInformation("Enviando Push para dispositivos de {Name}", recipient.Name);
                
                await pushHandler.SendPushAsync(
                    recipient.SubNotification, 
                    "Pasbem Saúde",
                    "Check-in"
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar Push para {Name}", recipient.Name);
            }
        }
    }
}