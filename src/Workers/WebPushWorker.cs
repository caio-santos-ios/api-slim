using api_slim.src.Configuration;
using api_slim.src.Handlers;
using api_slim.src.Models;
using MongoDB.Driver;

namespace api_slim.src.Workers;

public class WebPushWorker(IServiceProvider serviceProvider, ILogger<WebPushWorker> logger) : BackgroundService
{
    private const string CPF_TESTE = "086.306.285-70";

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
                    && c.SubNotification != null && c.SubNotification.UserId != "")
            .ToListAsync();

        Customer? customer = await context.Customers.Find(x => !x.Deleted).FirstOrDefaultAsync();

        var current     = TimeOnly.FromDateTime(DateTime.UtcNow);
        var today      = DateTime.UtcNow.Date;

        foreach (var recipient in recipients)
        {
            try
            {
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
                                url    : "/aplicativo/home/check-in/",
                                tag    : "checkin-igs"
                            );

                            recipient.IGSNotification = DateTime.UtcNow;
                            await context.CustomerRecipients.ReplaceOneAsync(c => c.Id == recipient.Id, recipient);

                            await context.Notifications.InsertOneAsync(new ()
                            {
                                BeneficiaryCPF = recipient.Cpf,
                                BeneficiaryId = recipient.Id,
                                BeneficiaryName = recipient.Name,
                                Title = "Check-in do Sono",
                                Message = "Olá! Como foi sua noite? Reserve um minutinho para registrar seu sono de hoje",
                                Origin = "Bem Vital",
                                Parent = "IGN",
                                ParentId = recipient.Id,
                                Phone = recipient.Whatsapp,
                                Read = false,
                                Sent = true,
                                SendDate = DateTime.UtcNow,
                                Type = "AppPush",
                                Link = "/aplicativo/home/check-in/"
                            });
                        }
                    }
                    continue; 
                }

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
                            subDto : recipient.SubNotification!,
                            title  : "🌙 Check-in da Noite — Nutrição e Saúde Mental",
                            message: $"Boa noite, {recipient.Name.Split(" ")[0]}! Registre sua hidratação, alimentação e estado emocional de hoje.",
                            url    : "/aplicativo/home/check-in/",
                            tag    : "checkin-ign"
                        );

                        recipient.IGNNotification = DateTime.UtcNow;
                        recipient.IESNotification = DateTime.UtcNow;

                        await context.CustomerRecipients.ReplaceOneAsync(c => c.Id == recipient.Id, recipient);

                        await context.Notifications.InsertManyAsync([
                            new () 
                            {
                                BeneficiaryCPF = recipient.Cpf,
                                BeneficiaryId = recipient.Id,
                                BeneficiaryName = recipient.Name,
                                Title = "Check-in da Nutrição",
                                Message = "Hora de registrar suas refeições de hoje!",
                                Origin = "Bem Vital",
                                Parent = "IGN",
                                ParentId = recipient.Id,
                                Phone = recipient.Whatsapp,
                                Read = false,
                                Sent = true,
                                SendDate = DateTime.UtcNow,
                                Type = "AppPush",
                                Link = "/aplicativo/home/check-in/"
                            },
                            new () 
                            {
                                BeneficiaryCPF = recipient.Cpf,
                                BeneficiaryId = recipient.Id,
                                BeneficiaryName = recipient.Name,
                                Title = "Check-in do Saúde Mental",
                                Message = "Como você está se sentindo agora? Faça seu check-in.",
                                Origin = "Bem Vital",
                                Parent = "IGN",
                                ParentId = recipient.Id,
                                Phone = recipient.Whatsapp,
                                Read = false,
                                Sent = true,
                                SendDate = DateTime.UtcNow,
                                Type = "AppPush",
                                Link = "/aplicativo/home/check-in/"
                            }
                        ]);
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