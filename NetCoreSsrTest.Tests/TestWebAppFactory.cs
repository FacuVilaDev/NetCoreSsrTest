using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NetCoreSsrTest.Context;
using NetCoreSsrTest.Domain;

namespace NetCoreSsrTest.Tests;

public class TestWebAppFactory : WebApplicationFactory<Program>
{
    private DbConnection _conn = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var toRemove = services.Single(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            services.Remove(toRemove);

            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();

            services.AddDbContext<AppDbContext>(o => o.UseSqlite(_conn));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            if (!db.Users.Any())
            {
                db.Users.Add(new MovieUser { Email = "admin@local", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), Role = "Admin" });
                db.Users.Add(new MovieUser { Email = "regular@local", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Regular123!"), Role = "Regular" });
                db.SaveChanges();
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _conn?.Dispose();
    }
}