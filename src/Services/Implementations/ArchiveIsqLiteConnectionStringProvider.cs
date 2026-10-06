namespace PhotoCli.Services.Implementations;

public class ArchiveIsqLiteConnectionStringProvider : ISQLiteConnectionStringProvider
{
	private readonly ArchiveDatabaseOptions _archiveDatabaseOptions;

	public ArchiveIsqLiteConnectionStringProvider(ArchiveDatabaseOptions options)
	{
		_archiveDatabaseOptions = options;
	}

	public string Value => $"Filename={_archiveDatabaseOptions.CustomDatabasePath ?? $"{_archiveDatabaseOptions.Path}/{Constants.ArchiveSQLiteDatabaseFileName}"}";
}
