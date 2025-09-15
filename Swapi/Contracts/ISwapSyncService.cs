namespace NetCoreSsrTest.Swapi.Contracts
{
	public interface ISwapiSyncService
	{
		Task<int> SyncFilmsAsync(CancellationToken ct);
	}
}
