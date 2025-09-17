using Microsoft.EntityFrameworkCore;
using Microsoft.PowerBI.Api.Models;
using NetCoreSsrTest.Domain;

namespace NetCoreSsrTest.Context;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
	public DbSet<MovieUser> Users => Set<MovieUser>();
	public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<Species> Species => Set<Species>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Starship> Starships => Set<Starship>();
    public DbSet<MoviePerson> MoviePeople => Set<MoviePerson>();
    public DbSet<MovieSpecies> MovieSpecies => Set<MovieSpecies>();
    public DbSet<MovieVehicle> MovieVehicles => Set<MovieVehicle>();
    public DbSet<MovieStarship> MovieStarships => Set<MovieStarship>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<MovieUser>().HasIndex(x => x.Email).IsUnique();
        b.Entity<Movie>().HasIndex(x => x.ExternalUid).IsUnique();

        b.Entity<Person>().HasIndex(x => x.ExternalUid).IsUnique();
        b.Entity<Species>().HasIndex(x => x.ExternalUid).IsUnique();
        b.Entity<Vehicle>().HasIndex(x => x.ExternalUid).IsUnique();
        b.Entity<Starship>().HasIndex(x => x.ExternalUid).IsUnique();

        b.Entity<MoviePerson>().HasKey(x => new { x.MovieId, x.PersonId });
        b.Entity<MoviePerson>().HasOne(x => x.Movie).WithMany(m => m.People).HasForeignKey(x => x.MovieId);
        b.Entity<MoviePerson>().HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);

        b.Entity<MovieSpecies>().HasKey(x => new { x.MovieId, x.SpeciesId });
        b.Entity<MovieSpecies>().HasOne(x => x.Movie).WithMany(m => m.Species).HasForeignKey(x => x.MovieId);
        b.Entity<MovieSpecies>().HasOne(x => x.Species).WithMany().HasForeignKey(x => x.SpeciesId);

        b.Entity<MovieVehicle>().HasKey(x => new { x.MovieId, x.VehicleId });
        b.Entity<MovieVehicle>().HasOne(x => x.Movie).WithMany(m => m.Vehicles).HasForeignKey(x => x.MovieId);
        b.Entity<MovieVehicle>().HasOne(x => x.Vehicle).WithMany().HasForeignKey(x => x.VehicleId);

        b.Entity<MovieStarship>().HasKey(x => new { x.MovieId, x.StarshipId });
        b.Entity<MovieStarship>().HasOne(x => x.Movie).WithMany(m => m.Starships).HasForeignKey(x => x.MovieId);
        b.Entity<MovieStarship>().HasOne(x => x.Starship).WithMany().HasForeignKey(x => x.StarshipId);
    }

}