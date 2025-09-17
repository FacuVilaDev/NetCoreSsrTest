using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreSsrTest.Context;
using NetCoreSsrTest.Domain;
using static NetCoreSsrTest.Infrastructure.MovieDtos;

namespace NetCoreSsrTest.Controllers;

[ApiController]
[Route("movies")]
public class MoviesController : ControllerBase
{
	private readonly AppDbContext _db;

	public MoviesController(AppDbContext db) { _db = db; }

	[HttpGet]
	public async Task<IEnumerable<Movie>> Get() =>
		await _db.Movies.AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    [Authorize(Policy = "RegularOnly")]
    public async Task<ActionResult<MovieDetailResponse>> GetById(int id)
    {
        var m = await _db.Movies
            .Include(x => x.People).ThenInclude(y => y.Person)
            .Include(x => x.Species).ThenInclude(y => y.Species)
            .Include(x => x.Vehicles).ThenInclude(y => y.Vehicle)
            .Include(x => x.Starships).ThenInclude(y => y.Starship)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (m is null) return NotFound();

        var dto = new MovieDetailResponse(
            m.Id,
            m.Title,
            m.Description,
            m.Director,
            m.ReleaseDate,
            m.People.Select(p => p.Person.ExternalUid),
            m.Species.Select(s => s.Species.ExternalUid),
            m.Vehicles.Select(v => v.Vehicle.ExternalUid),
            m.Starships.Select(s => s.Starship.ExternalUid)
        );
        return dto;
    }


    [HttpPost]
	[Authorize(Policy = "Admin")]
	public async Task<ActionResult<Movie>> Create(MovieRequest rq)
	{
		if (string.IsNullOrWhiteSpace(rq.Title)) return BadRequest();
		var m = new Movie { Title = rq.Title, Description = rq.Description, Year = rq.Year };
		_db.Movies.Add(m);
		await _db.SaveChangesAsync();
		return CreatedAtAction(nameof(GetById), new { id = m.Id }, m);
	}

	[HttpPut("{id:int}")]
	[Authorize(Policy = "Admin")]
	public async Task<IActionResult> Update(int id, MovieRequest rq)
	{
		var m = await _db.Movies.FindAsync(id);
		if (m is null) return NotFound();
		m.Title = rq.Title;
		m.Description = rq.Description;
		m.Year = rq.Year;
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("{id:int}")]
	[Authorize(Policy = "Admin")]
	public async Task<IActionResult> Delete(int id)
	{
		var m = await _db.Movies.FindAsync(id);
		if (m is null) return NotFound();
		_db.Movies.Remove(m);
		await _db.SaveChangesAsync();
		return NoContent();
	}
}
