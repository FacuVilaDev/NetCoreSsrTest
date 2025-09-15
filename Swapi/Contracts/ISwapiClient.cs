using static NetCoreSsrTest.Infrastructure.SwapiDtos;

namespace NetCoreSsrTest.Swapi.Contracts;

public interface ISwapiClient
{
	Task<List<SwapiListItem>> GetFilmsAsync(CancellationToken ct);
	Task<SwapiFilmDetail> GetFilmAsync(string uid, CancellationToken ct);
}
