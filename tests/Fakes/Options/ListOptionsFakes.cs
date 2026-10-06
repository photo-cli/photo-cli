namespace PhotoCli.Tests.Fakes.Options;

public static class ListOptionsFakes
{
	private const string ArchivePath = "archive-folder";

	public static ListOptions Summary(string? archivePath = null)
	{
		return new ListOptions(
			listType: ListType.Summary,
			archivePath: archivePath ?? ArchivePath
		);
	}

	public static ListOptions SummaryWithCustomDatabasePath(string customDatabasePath)
	{
		return new ListOptions(
			listType: ListType.Summary,
			archivePath: ArchivePath,
			customDatabasePath: customDatabasePath
		);
	}

	public static ListOptions Albums(string? archivePath = null)
	{
		return new ListOptions(
			listType: ListType.Albums,
			archivePath: archivePath ?? ArchivePath
		);
	}

	public static ListOptions PhotosByAlbum(int albumId, string? archivePath = null, bool rawOutput = false)
	{
		return new ListOptions(
			listType: ListType.PhotosByAlbumId,
			archivePath: archivePath ?? ArchivePath,
			albumId: albumId,
			rawOutput: rawOutput
		);
	}

	public static ListOptions PhotosByAlbumName(string albumName, string? archivePath = null, bool rawOutput = false)
	{
		return new ListOptions(
			listType: ListType.PhotosByAlbumName,
			archivePath: archivePath ?? ArchivePath,
			albumName: albumName,
			rawOutput: rawOutput
		);
	}

	public static ListOptions PhotosByDate(int? year = null, byte? month = null, byte? day = null, string? archivePath = null, bool rawOutput = false)
	{
		return new ListOptions(
			listType: ListType.PhotosByExactDate,
			archivePath: archivePath ?? ArchivePath,
			year: year,
			month: month,
			day: day,
			rawOutput: rawOutput
		);
	}

	public static ListOptions PhotosByDateRange(DateTime startDate, DateTime endDate, string? archivePath = null, bool rawOutput = false)
	{
		return new ListOptions(
			listType: ListType.PhotosByDateRange,
			archivePath: archivePath ?? ArchivePath,
			startDate: startDate,
			endDate: endDate,
			rawOutput: rawOutput
		);
	}
}
