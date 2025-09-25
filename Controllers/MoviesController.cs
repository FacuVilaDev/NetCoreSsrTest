using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreSsrTest.Context;
using NetCoreSsrTest.Domain;
using static NetCoreSsrTest.Infrastructure.MovieDtos;
using System.Text.Json;

namespace NetCoreSsrTest.Controllers;

[ApiController]
[Route("movies")]
public class MoviesController : ControllerBase
{
    private readonly AppDbContext _db;

    public MoviesController(AppDbContext db) { _db = db; }

	/// <summary>Lista todas las películas.</summary>
	/// <response code="200">Listado de películas.</response>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IEnumerable<Movie>> Get() =>
        await _db.Movies.AsNoTracking().ToListAsync();

	/// <summary>Detalle de una película.</summary>
	/// <remarks>Requiere rol <b>Regular</b> o superior.</remarks>
	/// <param name="id">Id de la película.</param>
	/// <response code="200">Detalle encontrado.</response>
	/// <response code="404">No existe.</response>
	[HttpGet("{id:int}")]
	[Authorize(Policy = "RegularOnly")]
	[ProducesResponseType(typeof(MovieDetailResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
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

	/// <summary>Crea una película.</summary>
	/// <remarks>Solo <b>Admin</b>.</remarks>
	/// <response code="201">Creada.</response>
	/// <response code="400">Datos inválidos.</response>
	/// <response code="403">Sin permiso.</response>
	[HttpPost]
	[Authorize(Policy = "Admin")]
	[ProducesResponseType(typeof(Movie), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<ActionResult<Movie>> Create([FromBody] MovieRequest rq)
    {
        if (rq is null || string.IsNullOrWhiteSpace(rq.Title)) return BadRequest();
        var m = new Movie
        {
            ExternalUid = $"local:{Guid.NewGuid()}",
            Title = rq.Title,
            Description = rq.Description,
            Year = rq.Year,
            Director = rq.Director,
            Producer = rq.Producer,
            ReleaseDate = rq.ReleaseDate,
            OpeningCrawl = rq.OpeningCrawl,
            ExternalUrl = rq.ExternalUrl
        };
        _db.Movies.Add(m);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = m.Id }, m);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] MovieRequest rq)
    {
        if (rq is null || string.IsNullOrWhiteSpace(rq.Title)) return BadRequest();
        var m = await _db.Movies.FindAsync(id);
        if (m is null) return NotFound();
        m.Title = rq.Title;
        m.Description = rq.Description;
        m.Year = rq.Year;
        m.Director = rq.Director;
        m.Producer = rq.Producer;
        m.ReleaseDate = rq.ReleaseDate;
        m.OpeningCrawl = rq.OpeningCrawl;
        m.ExternalUrl = rq.ExternalUrl;
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
