using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PhotoCli.Models;

public class ArchiveDbContextDesignTime : IDesignTimeDbContextFactory<ArchiveDbContext>
{
	public ArchiveDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ArchiveDbContext>();
		optionsBuilder.UseSqlite();
		return new ArchiveDbContext(optionsBuilder.Options);
	}
}
