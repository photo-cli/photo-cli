using Microsoft.EntityFrameworkCore;

namespace PhotoCli.Services.Implementations;

public class ArchiveDbContextProvider : IArchiveDbContextProvider
{
	private ArchiveDbContext? _value;
	private readonly ISQLiteConnectionStringProvider _connectionStringProvider;
	private readonly IFileService _fileService;
	private readonly ArchiveDatabaseOptions _archiveDatabaseOptions;
	private readonly ILogger<ArchiveDbContextProvider> _logger;

	public ArchiveDbContextProvider(ISQLiteConnectionStringProvider connectionStringProvider, IFileService fileService,
		ArchiveDatabaseOptions archiveDatabaseOptions, ILogger<ArchiveDbContextProvider> logger)
	{
		_connectionStringProvider = connectionStringProvider;
		_fileService = fileService;
		_archiveDatabaseOptions = archiveDatabaseOptions;
		_logger = logger;
	}

	public ArchiveDbContext CreateOrGetInstance()
	{
		if (_value != null)
		{
			_logger.LogInformation("Using existing DbContext - {@ContextId}", _value.ContextId);
			return _value;
		}

		if (_archiveDatabaseOptions.CustomDatabasePath.IsPresent())
		{
			_logger.LogDebug("Checking folder for custom database path: {CustomDatabasePath}", _archiveDatabaseOptions.CustomDatabasePath);
			_fileService.CreateParentFolderIfNotExist(_archiveDatabaseOptions.CustomDatabasePath);
		}
		else
		{
			_logger.LogDebug("Checking folder for default output folder: {Path}", _archiveDatabaseOptions.Path);
			_fileService.CreateOutputFolderIfNotExists(_archiveDatabaseOptions.Path);
		}
		var optionsBuilder = new DbContextOptionsBuilder<ArchiveDbContext>();
		var connectionString = _connectionStringProvider.Value;
		optionsBuilder.UseSqlite(connectionString);
		_value = new ArchiveDbContext(optionsBuilder.Options);
		_logger.LogInformation("Creating DbContext with connection string: {ConnectionString}", connectionString);
		_value.Database.Migrate();
		_logger.LogInformation("Created DbContext with connection string: {ConnectionString}", connectionString);
		return _value;
	}
}
