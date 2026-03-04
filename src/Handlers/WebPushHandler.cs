using WebPush;
using System.Text.Json;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Handlers;

public class WebPushHandler
{
    // Suas chaves VAPID geradas anteriormente
    private readonly string _publicKey = "BKAi4Ae35cMd0JtCRVgIuHq6tjlqaN0Va0AifE1OzuldnKWkoGILA1F5qRr6iYOh6rcKr_3cp14qEFeNmp6olhs";
    private readonly string _privateKey = "FOv1UoSGCo69pQUIak5zpi_PtOKQZ7TKTvUGNnN0wN8";
    private readonly WebPushClient _webPushClient;

    public WebPushHandler()
    {
        _webPushClient = new WebPushClient();
    }

    public async Task SendPushAsync(PushSubscriptionRequest subDto, string title, string message, string url, string tag)
    {
        var subscription = new PushSubscription(
            subDto.Endpoint, 
            subDto.Keys.P256dh, 
            subDto.Keys.Auth
        );

        var vapidDetails = new VapidDetails(
            "mailto:caiodev.fullstack@gmail.com", 
            _publicKey, 
            _privateKey
        );

        var payload = JsonSerializer.Serialize(new 
        { 
            title, 
            body = message,
            url,
            tag,
        });

        try
        {
            await _webPushClient.SendNotificationAsync(subscription, payload, vapidDetails);
        }
        catch (WebPushException ex)
        {
            // Regra importante: Se o status for 410 (Gone) ou 404 (Not Found),
            // o token do navegador expirou ou o usuário desinstalou o PWA.
            if (ex.StatusCode == System.Net.HttpStatusCode.Gone || 
                ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Aqui você pode disparar um evento para remover essa subscrição do Mongo
                // para não tentar enviar para um "defunto" na próxima vez.
                throw new Exception($"SubscriptionExpired: {subDto.Endpoint}");
            }
            
            throw; // Repassa outros erros para o Worker logar
        }
    }
}