namespace NetCoreSsrTest.Infrastructure;

public class MovieDtos
{
    public record MovieRequest(string Title, string? Description, int? Year, string? Director, string? Producer, DateTime? ReleaseDate, string? OpeningCrawl, string? ExternalUrl);
    public record MovieDetailResponse(
        int Id,
        string Title,
        string? Description,
        string? Director,
        DateTime? ReleaseDate,
        IEnumerable<string> People,
        IEnumerable<string> Species,
        IEnumerable<string> Vehicles,
        IEnumerable<string> Starships
    );
}
