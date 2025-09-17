using static NetCoreSsrTest.Infrastructure.SwapiDtos;

namespace NetCoreSsrTest.Swapi.Contracts;

public interface ISwapiClient
{
    Task<List<SwapiFilmItem>> GetFilmsAsync(CancellationToken ct);
}