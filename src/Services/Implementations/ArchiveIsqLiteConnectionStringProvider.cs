namespace PhotoCli.Services.Implementations;

public class ArchiveIsqLiteConnectionStringProvider : ISQLiteConnectionStringProvider
{
	private readonly ArchiveOptions _options;

	public ArchiveIsqLiteConnectionStringProvider(ArchiveOptions options)
	{
		_options = options;
	}

	public string Value => $"Filename={_options.OutputPath}/{Constants.ArchiveSQLiteDatabaseFileName}";
}
