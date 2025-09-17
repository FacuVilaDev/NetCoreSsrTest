using System.Text.Json.Serialization;

namespace NetCoreSsrTest.Infrastructure;

public class SwapiDtos
{
    public class SwapiFilmsResponse
    {
        [JsonPropertyName("message")] public string Message { get; set; } = default!;
        [JsonPropertyName("result")] public List<SwapiFilmItem> Result { get; set; } = new();
    }

    public class SwapiFilmItem
    {
        [JsonPropertyName("uid")] public string Uid { get; set; } = default!;
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("properties")] public SwapiFilmProps Properties { get; set; } = default!;
    }

    public class SwapiFilmProps
    {
        [JsonPropertyName("title")] public string Title { get; set; } = default!;
        [JsonPropertyName("episode_id")] public int EpisodeId { get; set; }
        [JsonPropertyName("opening_crawl")] public string OpeningCrawl { get; set; } = default!;
        [JsonPropertyName("director")] public string Director { get; set; } = default!;
        [JsonPropertyName("producer")] public string Producer { get; set; } = default!;
        [JsonPropertyName("release_date")] public string ReleaseDate { get; set; } = default!;
        [JsonPropertyName("url")] public string Url { get; set; } = default!;
        [JsonPropertyName("characters")] public List<string> Characters { get; set; } = new();
        [JsonPropertyName("species")] public List<string> Species { get; set; } = new();
        [JsonPropertyName("vehicles")] public List<string> Vehicles { get; set; } = new();
        [JsonPropertyName("starships")] public List<string> Starships { get; set; } = new();
    }
}
