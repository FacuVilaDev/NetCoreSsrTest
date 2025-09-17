namespace NetCoreSsrTest.Domain;

public class Movie
{
	public int Id { get; set; }
	public string ExternalUid { get; set; } = default!;
	public string Title { get; set; } = default!;
	public string? Description { get; set; }
	public int? Year { get; set; }
	public string? Director { get; set; }
	public string? Producer { get; set; }
	public DateTime? ReleaseDate { get; set; }
	public string? OpeningCrawl { get; set; }
	public string? ExternalUrl { get; set; }

    public ICollection<MoviePerson> People { get; set; } = new List<MoviePerson>();
    public ICollection<MovieSpecies> Species { get; set; } = new List<MovieSpecies>();
    public ICollection<MovieVehicle> Vehicles { get; set; } = new List<MovieVehicle>();
    public ICollection<MovieStarship> Starships { get; set; } = new List<MovieStarship>();

}