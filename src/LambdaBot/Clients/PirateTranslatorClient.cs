using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LambdaBot.Models;

using Newtonsoft.Json;

namespace LambdaBot.Clients;

public interface IPirateTranslatorClient
{
    Task<TranslateResponse> Translate(string text, CancellationToken cancellationToken = default);
}

internal class PirateTranslatorClient : IPirateTranslatorClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _settings;

    public PirateTranslatorClient(HttpClient httpClient, JsonSerializerSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<TranslateResponse> Translate(string text, CancellationToken cancellationToken = default)
    {
        var request = new TranslateRequest
        {
            Text = text
        };

        var content = new StringContent(JsonConvert.SerializeObject(request, _settings),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        var response = await _httpClient.PostAsync("/translate/pirate.json", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<TranslateResponse>(result, _settings);
    }
}