namespace NetCoreSsrTest.Infrastructure;

public class MovieDtos
{
	public record MovieRequest(string Title, string? Description, int? Year);
}
