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
		var list = await _client.GetFilmsAsync(ct);
		var count = 0;

		foreach (var item in list)
		{
			var exists = await _db.Movies.FirstOrDefaultAsync(x => x.ExternalUid == item.Uid, ct);
			var detail = await _client.GetFilmAsync(item.Uid, ct);
			var p = detail.Properties;

			if (exists == null)
			{
				var m = new Movie
				{
					ExternalUid = detail.Uid,
					Title = p.Title,
					Director = p.Director,
					Producer = p.Producer,
					OpeningCrawl = p.Opening_crawl,
					ExternalUrl = p.Url,
					ReleaseDate = DateTime.TryParse(p.Release_date, out var d) ? d : null,
					Description = detail.Description
				};
				_db.Movies.Add(m);
				count++;
			}
			else
			{
				exists.Title = p.Title;
				exists.Director = p.Director;
				exists.Producer = p.Producer;
				exists.OpeningCrawl = p.Opening_crawl;
				exists.ExternalUrl = p.Url;
				exists.ReleaseDate = DateTime.TryParse(p.Release_date, out var d2) ? d2 : exists.ReleaseDate;
				exists.Description = detail.Description;
			}
		}

		await _db.SaveChangesAsync(ct);
		return count;
	}
}
