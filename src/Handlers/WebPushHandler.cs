using WebPush;
using System.Text.Json;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Handlers;

public class WebPushHandler
{
    private readonly string _publicKey  = "BKAi4Ae35cMd0JtCRVgIuHq6tjlqaN0Va0AifE1OzuldnKWkoGILA1F5qRr6iYOh6rcKr_3cp14qEFeNmp6olhs";
    private readonly string _privateKey = "FOv1UoSGCo69pQUIak5zpi_PtOKQZ7TKTvUGNnN0wN8";
    private readonly WebPushClient _webPushClient;

    public WebPushHandler()
    {
        // Timeout explícito — sem isso o VPS trava silenciosamente ao chamar o FCM
        var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        _webPushClient = new WebPushClient(httpClient);
    }

    public async Task SendPushAsync(
        PushSubscriptionRequest subDto,
        string title,
        string message,
        string url   = "/aplicativo/home",
        string tag   = "pasbem",
        string image = "")
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

        // var payload = JsonSerializer.Serialize(new { title, body = message, url, tag, image });
        var options = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        };

        var payload = JsonSerializer.Serialize(new { 
            title, 
            body = message, 
            url, 
            tag, 
            image 
        }, options);

        try
        {
            await _webPushClient.SendNotificationAsync(subscription, payload, vapidDetails);
        }
        catch (TaskCanceledException)
        {
            throw new Exception($"PushTimeout: FCM não respondeu em 15s. Endpoint: {subDto.Endpoint}");
        }
        catch (WebPushException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Gone ||
                ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception($"SubscriptionExpired: {subDto.Endpoint}");
            }
            throw;
        }
    }
}