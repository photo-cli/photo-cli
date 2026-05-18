using Microsoft.EntityFrameworkCore;

namespace PhotoCli.Models;

public class ArchiveDbContext : DbContext
{
	public virtual DbSet<PhotoEntity> Photos => Set<PhotoEntity>();
	public virtual DbSet<AlbumEntity> Albums => Set<AlbumEntity>();
	public virtual DbSet<AlbumHistoryEntity> AlbumHistories => Set<AlbumHistoryEntity>();
	public virtual DbSet<ReverseGeocodeCacheEntity> ReverseGeocodeCache => Set<ReverseGeocodeCacheEntity>();

	public ArchiveDbContext(DbContextOptions<ArchiveDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<PhotoEntity>().HasKey(k => k.Id);
		modelBuilder.Entity<PhotoEntity>().HasIndex(i => i.DateTaken);
		modelBuilder.Entity<PhotoEntity>().HasIndex(i => i.Year);
		modelBuilder.Entity<PhotoEntity>().HasIndex(i => new { i.Year, i.Month });
		modelBuilder.Entity<PhotoEntity>().HasIndex(i => new { i.Year, i.Month, i.Day });
		modelBuilder.Entity<PhotoEntity>().HasIndex(i => i.ReverseGeocodeFormatted);

		modelBuilder.Entity<AlbumEntity>().HasKey(k => k.Id);
		modelBuilder.Entity<AlbumEntity>().HasIndex(i => i.Name);

		modelBuilder.Entity<AlbumHistoryEntity>().HasKey(k => k.Id);

		modelBuilder.Entity<ReverseGeocodeCacheEntity>().HasKey(k => k.Id);
		modelBuilder.Entity<ReverseGeocodeCacheEntity>().HasIndex(i => new { i.Latitude, i.Longitude, i.Provider, i.Precision });

		foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
	}
}
