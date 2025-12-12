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
			listType: ListType.PhotosByAlbum,
			archivePath: archivePath ?? ArchivePath,
			albumId: albumId,
			rawOutput: rawOutput
		);
	}

	public static ListOptions PhotosByDate(int? year = null, byte? month = null, byte? day = null, string? archivePath = null, bool rawOutput = false)
	{
		return new ListOptions(
			listType: ListType.PhotosByDate,
			archivePath: archivePath ?? ArchivePath,
			year: year,
			month: month,
			day: day,
			rawOutput: rawOutput
		);
	}
}