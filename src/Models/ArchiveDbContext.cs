using Microsoft.EntityFrameworkCore;

namespace PhotoCli.Models;

public class ArchiveDbContext : DbContext
{
	public virtual DbSet<PhotoEntity> Photos => Set<PhotoEntity>();
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
	}
}
