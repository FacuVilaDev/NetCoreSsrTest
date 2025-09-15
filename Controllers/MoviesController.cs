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
	public async Task<ActionResult<Movie>> GetById(int id)
	{
		var m = await _db.Movies.FindAsync(id);
		if (m is null) return NotFound();
		return m;
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
