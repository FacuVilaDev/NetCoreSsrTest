namespace NetCoreSsrTest.Domain;

public class MovieUser
{
	public int Id { get; set; }
	public string Email { get; set; } = default!;
	public string PasswordHash { get; set; } = default!;
	public string Role { get; set; } = "Regular";
}
