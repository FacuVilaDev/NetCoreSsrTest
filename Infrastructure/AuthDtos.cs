namespace NetCoreSsrTest.Infrastructure;

public class AuthDtos
{
	public record SignUpRequest(string Email, string Password, string? Role);
	public record LoginRequest(string Email, string Password);
	public record AuthResponse(string Token);
}
