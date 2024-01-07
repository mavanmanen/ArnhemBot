using System.Text.Json;
using RestSharp;
using RestSharp.Serializers.Json;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface IMementoApiClient
{
    public Task<string?> GetResultsAsync(string uri);
}

public class MementoApiClient : IMementoApiClient
{
    private record MementoPredictionResult(MementoPredictionResultItem[] MementoInfo);
    private record MementoPredictionResultItem(Uri TimegateUri, string ArchiveId);
    
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

    public async Task<string?> GetResultsAsync(string uri)
    {
        var request = new RestRequest($"/prediction/json/{uri}");
        var result = await _client.ExecuteGetAsync<MementoPredictionResult>(request);
        return result.IsSuccessful == false ? null : result.Data?.MementoInfo.FirstOrDefault(m => m.ArchiveId == "archive.is")?.TimegateUri.ToString();
    }
}