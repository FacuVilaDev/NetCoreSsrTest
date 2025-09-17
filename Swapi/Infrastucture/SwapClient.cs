using NetCoreSsrTest.Swapi.Contracts;
using static NetCoreSsrTest.Infrastructure.SwapiDtos;

namespace NetCoreSsrTest.Swapi.Infrastucture;

public class SwapiClient : ISwapiClient
{
    private readonly HttpClient _http;
    public SwapiClient(HttpClient http) { _http = http; }
    public async Task<List<SwapiFilmItem>> GetFilmsAsync(CancellationToken ct)
    {
        var res = await _http.GetFromJsonAsync<SwapiFilmsResponse>("films", ct);
        return res?.Result ?? new();
    }
}
