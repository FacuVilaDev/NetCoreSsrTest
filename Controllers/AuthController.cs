using Docker.DotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetCoreSsrTest.Context;
using NetCoreSsrTest.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static NetCoreSsrTest.Infrastructure.AuthDtos;

namespace NetCoreSsrTest.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
	private readonly AppDbContext _db;
	private readonly IConfiguration _cfg;

	public AuthController(AppDbContext db, IConfiguration cfg)
	{
		_db = db;
		_cfg = cfg;
	}

	[HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpRequest rq)
	{
		var exists = await _db.Users.AnyAsync(u => u.Email == rq.Email);
		if (exists) return Conflict();
		var role = string.IsNullOrWhiteSpace(rq.Role) ? "Regular" : rq.Role!;
		var user = new MovieUser { Email = rq.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(rq.Password), Role = role };
		_db.Users.Add(user);
		await _db.SaveChangesAsync();
		return Created("", new { user.Id, user.Email, user.Role });
	}

	[HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<Infrastructure.AuthDtos.AuthResponse>> Login(Infrastructure.AuthDtos.LoginRequest rq)
	{
		var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == rq.Email);
		if (user is null) return Unauthorized();
		if (!BCrypt.Net.BCrypt.Verify(rq.Password, user.PasswordHash)) return Unauthorized();
		return new Infrastructure.AuthDtos.AuthResponse(GenerateToken(user));
	}

	private string GenerateToken(MovieUser user)
	{
		var jwt = _cfg.GetSection("Jwt");
		var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)), SecurityAlgorithms.HmacSha256);
		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Role, user.Role)
		};
		var token = new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims, expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresMinutes"] ?? "60")), signingCredentials: creds);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
