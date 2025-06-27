// File: NeutrinoApiModerationService.cs
using System.Text.Json;

public class NeutrinoApiModerationService : IModerationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public NeutrinoApiModerationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<ModerationResult> ModerateAsync(string message)
    {
        // ... (The full code for this method from our previous step) ...
        // I'm omitting it for brevity, but ensure it's here.
        if (string.IsNullOrEmpty(message))
        {
            return new ModerationResult { IsFlagged = false };
        }
        var client = _httpClientFactory.CreateClient();
        var apiUrl = _configuration["NeutrinoApi:ApiUrl"];
        var requestData = new Dictionary<string, string> { { "content", message } };
        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl) { Content = new FormUrlEncodedContent(requestData) };
        request.Headers.Add("user-id", _configuration["NeutrinoApi:UserId"]);
        request.Headers.Add("api-key", _configuration["NeutrinoApi:ApiKey"]);
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            bool isBad = doc.RootElement.GetProperty("is-bad").GetBoolean();
            return new ModerationResult { IsFlagged = isBad, Reason = "Your message contains language that is not allowed." };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling Neutrino API: {ex.Message}");
            return new ModerationResult { IsFlagged = true, Reason = "Could not validate your message at this time." };
        }
    }
}