using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NetCoreSsrTest.Context;
using NetCoreSsrTest.Infrastructure;
using NetCoreSsrTest.Swapi.Contracts;
using NetCoreSsrTest.Swapi.Service;
using Xunit;
using static NetCoreSsrTest.Infrastructure.SwapiDtos;

namespace Tests.Unit;

public class SwapiSyncServiceTests
{
    [Fact]
    public async Task Sync_Inserts_And_Is_Idempotent()
    {
        using var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(conn)
            .Options;
        using var db = new AppDbContext(opts);
        db.Database.EnsureCreated();

        var client = new Mock<ISwapiClient>();
        client.Setup(x => x.GetFilmsAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(new List<SwapiFilmItem>
              {
                  new SwapiFilmItem {
                      Uid = "1",
                      Description = "A",
                      Properties = new SwapiFilmProps {
                          Title = "A New Hope",
                          Director = "George Lucas",
                          Producer = "Gary Kurtz",
                          OpeningCrawl = "...",
                          Url = "u",
                          ReleaseDate = "1977-05-25",
                          Characters = new(), Species = new(), Vehicles = new(), Starships = new()
                      }
                  }
              });

        var svc = new SwapiSyncService(client.Object, db);

        var first = await svc.SyncFilmsAsync(CancellationToken.None);
        var second = await svc.SyncFilmsAsync(CancellationToken.None);

        first.Should().Be(1);
        second.Should().Be(0);
        db.Movies.Count().Should().Be(1);
        db.Movies.Single().ExternalUid.Should().Be("1");
    }
}
