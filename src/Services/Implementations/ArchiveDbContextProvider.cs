using Microsoft.EntityFrameworkCore;

namespace PhotoCli.Services.Implementations;

public class ArchiveDbContextProvider : IArchiveDbContextProvider
{
	private ArchiveDbContext? _value;
	private readonly ISQLiteConnectionStringProvider _connectionStringProvider;
	private readonly ILogger<ArchiveDbContextProvider> _logger;

	public ArchiveDbContextProvider(ISQLiteConnectionStringProvider connectionStringProvider, ILogger<ArchiveDbContextProvider> logger)
	{
		_connectionStringProvider = connectionStringProvider;
		_logger = logger;
	}

	public ArchiveDbContext CreateOrGetInstance()
	{
		if (_value != null)
		{
			_logger.LogDebug("Using existing DbContext - {@ContextId}", _value.ContextId);
			return _value;
		}

		var optionsBuilder = new DbContextOptionsBuilder<ArchiveDbContext>();
		var connectionString = _connectionStringProvider.Value;
		optionsBuilder.UseSqlite(connectionString);
		_value = new ArchiveDbContext(optionsBuilder.Options);
		_logger.LogDebug("Creating DbContext with connection string: {ConnectionString}", connectionString);
		_value.Database.Migrate();
		_logger.LogInformation("Created DbContext with connection string: {ConnectionString}", connectionString);
		return _value;
	}
}
