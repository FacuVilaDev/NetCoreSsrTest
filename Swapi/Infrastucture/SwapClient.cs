using NetCoreSsrTest.Swapi.Contracts;
using static NetCoreSsrTest.Infrastructure.SwapiDtos;

namespace NetCoreSsrTest.Swapi.Infrastucture;
public class SwapiClient : ISwapiClient
{
	private readonly HttpClient _http;
	public SwapiClient(HttpClient http) { _http = http; }

	public async Task<List<SwapiListItem>> GetFilmsAsync(CancellationToken ct)
	{
		var items = new List<SwapiListItem>();
		string? url = "films";
		while (url != null)
		{
			var page = await _http.GetFromJsonAsync<SwapiListResponse<SwapiListItem>>(url, ct);
			if (page?.Results != null) items.AddRange(page.Results);
			url = page?.Next;
		}
		return items;
	}

	public async Task<SwapiFilmDetail> GetFilmAsync(string uid, CancellationToken ct)
	{
		var res = await _http.GetFromJsonAsync<SwapiFilmDetailResponse>($"films/{uid}", ct);
		return res!.Result;
	}
}