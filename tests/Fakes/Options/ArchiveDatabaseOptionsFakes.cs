namespace PhotoCli.Tests.Fakes.Options;

public static class ArchiveDatabaseOptionsFakes
{
	private const string OutputFolder = "output-folder";

	public static ArchiveDatabaseOptions Valid()
	{
		return new ArchiveDatabaseOptions(OutputFolder);
	}
}
