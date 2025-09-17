using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCoreSsrTest.Swapi.Contracts;

namespace NetCoreSsrTest.Controllers;

[ApiController]
[Route("admin")]
//[Authorize(Policy = "Admin")]
public class AdminController : ControllerBase
{
	private readonly ISwapiSyncService _sync;
	public AdminController(ISwapiSyncService sync) { _sync = sync; }

	[HttpPost("sync-swapi")]
	public async Task<IActionResult> SyncSwapi(CancellationToken ct)
	{
		var inserted = await _sync.SyncFilmsAsync(ct);
		return Ok(new { inserted });
	}
}