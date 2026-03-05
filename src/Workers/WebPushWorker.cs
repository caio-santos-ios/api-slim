using api_slim.src.Configuration;
using api_slim.src.Handlers;
using api_slim.src.Shared.Utils;
using MongoDB.Driver;

namespace api_slim.src.Workers;

public class WebPushWorker(IServiceProvider serviceProvider, ILogger<WebPushWorker> logger) : BackgroundService
{
    private const string CPF_TESTE = "086.306.285-70";

    // Horários em UTC (Brasil = UTC-3)
    // IGS  → manhã  07:30 BRT = 10:30 UTC
    // IGN  → noite  18:00 BRT = 21:00 UTC
    // IES  → noite  18:00 BRT = 21:00 UTC (junto com IGN)
    private static readonly TimeOnly IGS_INICIO = new(10, 00); // 07:30 BRT
    private static readonly TimeOnly IGS_FIM    = new(21, 00); // 18:00 BRT (limite antes da noite)
    private static readonly TimeOnly IGN_INICIO = new(21, 00); // 18:00 BRT
    private static readonly TimeOnly IGN_FIM    = new(23, 59); // fim do dia UTC

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("WebPushWorker iniciado.");

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

        var recipients = await context.CustomerRecipients
            .Find(c => !c.Deleted
                    && c.Active
                    // && c.Cpf == CPF_TESTE
                    && c.SubNotification != null)
            .ToListAsync();

        var current     = TimeOnly.FromDateTime(DateTime.UtcNow);
        var today      = DateTime.UtcNow.Date;

        foreach (var recipient in recipients)
        {
            try
            {
                // ── IGS (manhã) ──────────────────────────────────────────────
                // Janela: 07:30–18:00 BRT (10:30–21:00 UTC)
                // Condição: vital do dia sem sleepHours preenchido
                if (current >= IGS_INICIO && current < IGS_FIM)
                {
                    var IGSToday = await context.Vitals
                        .Find(v => v.BeneficiaryId == recipient.Id
                                && v.CreatedAt.Date.Date == today.Date)
                        .FirstOrDefaultAsync();


                    if(recipient.SubNotification != null)
                    {
                        if (IGSToday is null && recipient.IGSNotification.Date != DateTime.UtcNow.Date)
                        {
                            logger.LogInformation("Enviando IGS (manhã) para {Name}", recipient.Name);

                            await pushHandler.SendPushAsync(
                                subDto : recipient.SubNotification!,
                                title  : "☀️ Check-in da Manhã",
                                message: $"Bom dia, {recipient.Name.Split(" ")[0]}! Registre seu sono e comece o dia bem.",
                                url    : "/aplicativo/home/igs",
                                tag    : "checkin-igs"
                            );

                            recipient.IGSNotification = DateTime.UtcNow;
                            await context.CustomerRecipients.ReplaceOneAsync(c => c.Id == recipient.Id, recipient);
                        }
                    }
                    continue; 
                }

                // ── IGN + IES (noite) ────────────────────────────────────────
                // Janela: 18:00–23:59 BRT (21:00–23:59 UTC)
                if (current >= IGN_INICIO && current <= IGN_FIM)
                {
                    var vitalToday = await context.Vitals
                        .Find(v => v.BeneficiaryId == recipient.Id
                                && v.CreatedAt.Date == today.Date && !v.ChekinIGN || !v.ChekinIES)
                        .FirstOrDefaultAsync();

                    if(recipient.IGNNotification.Date != DateTime.UtcNow.Date && recipient.IESNotification.Date != DateTime.UtcNow.Date) 
                    {
                        logger.LogInformation("Enviando IGN (noite) para {Name}", recipient.Name);

                        await pushHandler.SendPushAsync(
                            subDto : recipient.SubNotification,
                            title  : "🌙 Check-in da Noite — Nutrição e Saúde Mental",
                            message: $"Boa noite, {recipient.Name.Split(" ")[0]}! Registre sua hidratação, alimentação e estado emocional de hoje.",
                            url    : "/aplicativo/home",
                            tag    : "checkin-ign"
                        );

                        recipient.IGNNotification = DateTime.UtcNow;
                        recipient.IESNotification = DateTime.UtcNow;

                        await context.CustomerRecipients.ReplaceOneAsync(c => c.Id == recipient.Id, recipient);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar Push para {Name}: {Message}", recipient.Name, ex.Message);

                if (ex.Message.Contains("SubscriptionExpired"))
                {
                    var update = Builders<api_slim.src.Models.CustomerRecipient>.Update
                        .Set(c => c.SubNotification, null);
                    await context.CustomerRecipients.UpdateOneAsync(c => c.Id == recipient.Id, update);
                    logger.LogInformation("Subscription removida para {Name}", recipient.Name);
                }
            }
        }
    }
}