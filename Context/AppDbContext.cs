using Microsoft.EntityFrameworkCore;
using NetCoreSsrTest.Domain;

namespace NetCoreSsrTest.Context;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
	public DbSet<MovieUser> Users => Set<MovieUser>();
	public DbSet<Movie> Movies => Set<Movie>();
	protected override void OnModelCreating(ModelBuilder b)
	{
		b.Entity<MovieUser>().HasIndex(x => x.Email).IsUnique();
		b.Entity<Movie>().HasIndex(x => x.ExternalUid).IsUnique();
	}
}