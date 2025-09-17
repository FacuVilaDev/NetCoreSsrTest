using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCoreSsrTest.Context;
using NetCoreSsrTest.Swapi.Contracts;
using NetCoreSsrTest.Swapi.Infrastucture;
using NetCoreSsrTest.Swapi.Service;
using System.Security.Claims;
using System.Text;

public partial class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddDbContext<AppDbContext>(o =>
			o.UseSqlite(builder.Configuration.GetConnectionString("Default")));
		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "NetCoreSsrTest", Version = "v1" });
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
		{ new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[]{} }
			});
		});

		var jwt = builder.Configuration.GetSection("Jwt");
		var key = Encoding.UTF8.GetBytes(jwt["Key"]!);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(o =>
			{
				var jwt = builder.Configuration.GetSection("Jwt");
				var key = Encoding.UTF8.GetBytes(jwt["Key"]!);
				o.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = jwt["Issuer"],
					ValidateAudience = true,
					ValidAudience = jwt["Audience"],
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateLifetime = true
				};
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        if (string.IsNullOrEmpty(ctx.Token) && ctx.Request.Cookies.TryGetValue("access_token", out var t))
                            ctx.Token = t;
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization(o =>
		{
			o.AddPolicy("Admin", p => p.RequireClaim(ClaimTypes.Role, "Admin"));
			o.AddPolicy("Regular", p => p.RequireClaim(ClaimTypes.Role, "Regular"));
			o.AddPolicy("RegularOnly", p => p.RequireAssertion(c => c.User.HasClaim(ClaimTypes.Role, "Regular")));
		});

        builder.Services.AddHttpClient<ISwapiClient, SwapiClient>(c =>
        {
            c.BaseAddress = new Uri("https://www.swapi.tech/api/");
            c.Timeout = TimeSpan.FromSeconds(20);
        });

        builder.Services.AddScoped<ISwapiSyncService, SwapiSyncService>();

		var app = builder.Build();

		using (var scope = app.Services.CreateScope())
		{
			var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			db.Database.Migrate();
			if (!db.Users.Any())
			{
				db.Users.Add(new NetCoreSsrTest.Domain.MovieUser { Email = "admin@local", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), Role = "Admin" });
				db.Users.Add(new NetCoreSsrTest.Domain.MovieUser { Email = "regular@local", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Regular123!"), Role = "Regular" });
				db.SaveChanges();
			}
		}

        app.UseSwagger();
        app.UseStaticFiles();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCoreSsrTest v1");
            c.EnablePersistAuthorization();
            c.UseRequestInterceptor(@"(req) => {
				try {
					var m = document.cookie.match(/(?:^|; )swagger_token=([^;]+)/);
					var saved = window.localStorage.getItem('swaggerBearer');
					var bearer = m ? decodeURIComponent(m[1]) : saved;
					if (bearer && !req.headers['Authorization']) { req.headers['Authorization'] = bearer; }
				} catch(e) {}
				return req;
			}");
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}