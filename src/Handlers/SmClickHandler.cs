using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Handlers;

public class SmClickHandler(ILogger<SmClickHandler> logger)
{
    private readonly string baseUrl     = Environment.GetEnvironmentVariable("SMCLICK_BASE_URL")      ?? "";
    private readonly string apiToken    = Environment.GetEnvironmentVariable("SMCLICK_API_TOKEN")     ?? "";
    private readonly string instanceKey = Environment.GetEnvironmentVariable("SMCLICK_INSTANCE_KEY")  ?? "";

    public async Task SendTextMessageAsync(string telephone, string message, CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiToken);

        dynamic payload = new
        {
            instance = instanceKey,
            type = "text",
            content = new
            {
                telephone,
                message
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{baseUrl}/instances/messages", content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("SMClick API error. Status: {Status}, Body: {Body}", response.StatusCode, responseBody);
            throw new HttpRequestException($"SMClick API returned {response.StatusCode}: {responseBody}");
        }

        logger.LogInformation("SMClick message sent to {Phone}", telephone);
    }
}