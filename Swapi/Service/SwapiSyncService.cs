using Microsoft.EntityFrameworkCore;
using NetCoreSsrTest.Context;
using NetCoreSsrTest.Domain;
using NetCoreSsrTest.Swapi.Contracts;

namespace NetCoreSsrTest.Swapi.Service;

public class SwapiSyncService : ISwapiSyncService
{
	private readonly ISwapiClient _client;
	private readonly AppDbContext _db;
	public SwapiSyncService(ISwapiClient client, AppDbContext db)
	{
		_client = client; _db = db;
	}

    public async Task<int> SyncFilmsAsync(CancellationToken ct)
    {
        var items = await _client.GetFilmsAsync(ct);
        var inserted = 0;

        foreach (var it in items)
        {
            var p = it.Properties;

            var m = await _db.Movies
                .Include(x => x.People).ThenInclude(y => y.Person)
                .Include(x => x.Species).ThenInclude(y => y.Species)
                .Include(x => x.Vehicles).ThenInclude(y => y.Vehicle)
                .Include(x => x.Starships).ThenInclude(y => y.Starship)
                .FirstOrDefaultAsync(x => x.ExternalUid == it.Uid, ct);

            if (m == null)
            {
                m = new Movie { ExternalUid = it.Uid };
                _db.Movies.Add(m);
                inserted++;
            }

            m.Title = p.Title;
            m.Director = p.Director;
            m.Producer = p.Producer;
            m.OpeningCrawl = p.OpeningCrawl;
            m.ExternalUrl = p.Url;
            m.Description = it.Description;
            m.ReleaseDate = DateTime.TryParse(p.ReleaseDate, out var d) ? d : m.ReleaseDate;
            m.Year = m.ReleaseDate?.Year ?? m.Year;

            await _db.SaveChangesAsync(ct);

            await RebuildLinks(
                m,
                p.Characters.Select(UidFromUrl),
                p.Species.Select(UidFromUrl),
                p.Vehicles.Select(UidFromUrl),
                p.Starships.Select(UidFromUrl),
                ct
            );
        }

        await _db.SaveChangesAsync(ct);
        return inserted;
    }

    static string UidFromUrl(string url)
    {
        var s = url.TrimEnd('/');
        var i = s.LastIndexOf('/') + 1;
        return i > 0 && i <= s.Length ? s[i..] : s;
    }

    async Task RebuildLinks(
        Movie m,
        IEnumerable<string> peopleUids,
        IEnumerable<string> speciesUids,
        IEnumerable<string> vehicleUids,
        IEnumerable<string> starshipUids,
        CancellationToken ct)
    {
        await _db.Entry(m).Collection(x => x.People).LoadAsync(ct);
        await _db.Entry(m).Collection(x => x.Species).LoadAsync(ct);
        await _db.Entry(m).Collection(x => x.Vehicles).LoadAsync(ct);
        await _db.Entry(m).Collection(x => x.Starships).LoadAsync(ct);

        _db.MoviePeople.RemoveRange(m.People);
        _db.MovieSpecies.RemoveRange(m.Species);
        _db.MovieVehicles.RemoveRange(m.Vehicles);
        _db.MovieStarships.RemoveRange(m.Starships);

        foreach (var uid in peopleUids)
        {
            var p = await _db.People.FirstOrDefaultAsync(x => x.ExternalUid == uid, ct) ?? new Person { ExternalUid = uid };
            m.People.Add(new MoviePerson { Movie = m, Person = p });
        }
        foreach (var uid in speciesUids)
        {
            var s = await _db.Species.FirstOrDefaultAsync(x => x.ExternalUid == uid, ct) ?? new Species { ExternalUid = uid };
            m.Species.Add(new MovieSpecies { Movie = m, Species = s });
        }
        foreach (var uid in vehicleUids)
        {
            var v = await _db.Vehicles.FirstOrDefaultAsync(x => x.ExternalUid == uid, ct) ?? new Vehicle { ExternalUid = uid };
            m.Vehicles.Add(new MovieVehicle { Movie = m, Vehicle = v });
        }
        foreach (var uid in starshipUids)
        {
            var s = await _db.Starships.FirstOrDefaultAsync(x => x.ExternalUid == uid, ct) ?? new Starship { ExternalUid = uid };
            m.Starships.Add(new MovieStarship { Movie = m, Starship = s });
        }
    }
}
