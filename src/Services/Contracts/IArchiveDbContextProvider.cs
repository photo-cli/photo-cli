namespace PhotoCli.Services.Contracts;

public interface IArchiveDbContextProvider
{
	ArchiveDbContext CreateOrGetInstance();
}
