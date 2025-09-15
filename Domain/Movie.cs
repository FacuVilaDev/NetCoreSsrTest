namespace NetCoreSsrTest.Domain;

public class Movie
{
	public int Id { get; set; }
	public string Title { get; set; } = default!;
	public string? Description { get; set; }
	public int? Year { get; set; }
}
