namespace PhotoCli.Services.Implementations;

public class McpArchiveDbContextProvider(ArchiveDbContext context) : IArchiveDbContextProvider
{
	public ArchiveDbContext CreateOrGetInstance() => context;
}
