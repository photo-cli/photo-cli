namespace PhotoCli.Options;

public record ArchiveDatabaseOptions(string Path, string? CustomDatabasePath = null)
{
	public static ArchiveDatabaseOptions FromArchive(ArchiveOptions options, ToolOptionsRaw raw)
	{
		return new ArchiveDatabaseOptions(options.OutputPath.IsPresent() ? options.OutputPath : raw.ArchivePath ?? string.Empty,
			options.CustomDatabasePath ?? raw.ArchiveDatabasePath);
	}

	public static ArchiveDatabaseOptions FromList(ListOptions options, ToolOptionsRaw raw)
	{
		return new ArchiveDatabaseOptions(options.ArchivePath ?? raw.ArchivePath ?? Environment.CurrentDirectory,
			options.CustomDatabasePath ?? raw.ArchiveDatabasePath);
	}

	public static ArchiveDatabaseOptions FromMcp(McpOptions options, ToolOptionsRaw raw)
	{
		return new ArchiveDatabaseOptions(options.ArchivePath ?? raw.ArchivePath ?? Environment.CurrentDirectory,
			options.CustomDatabasePath ?? raw.ArchiveDatabasePath);
	}
}
