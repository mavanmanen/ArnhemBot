using System.Text.Json;
using RestSharp;
using RestSharp.Serializers.Json;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public record MementoPredictionResult(MementoPredictionResultItem[] MementoInfo);

public record MementoPredictionResultItem(Uri TimegateUri, string ArchiveId);

public interface IMementoApiClient
{
    public Task<MementoPredictionResult?> GetResultsAsync(string uri);
}

public class MementoApiClient : IMementoApiClient
{
    private readonly RestClient _client;

    public MementoApiClient()
    {
        _client = new RestClient("http://timetravel.mementoweb.org", configureSerialization: s =>
        {
            s.UseSystemTextJson(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });
        });
    }

    public async Task<MementoPredictionResult?> GetResultsAsync(string uri)
    {
        var request = new RestRequest($"/prediction/json/{uri}");
        var result = await _client.ExecuteGetAsync<MementoPredictionResult>(request);
        return result.IsSuccessful == false ? null : result.Data!;
    }
}