namespace PhotoCli.Tests.UnitTests.Services;

public class ArchiveSqLiteConnectionStringProviderUnitTests
{
	public static TheoryData<ArchiveOptions, string> ArchiveOptionsWithExpectedConnectionStrings = new()
	{
		{
			new ArchiveOptions("single-folder"),
			"Filename=single-folder/photo-cli.sqlite3"
		},
		{
			new ArchiveOptions("multi/level/folder"),
			"Filename=multi/level/folder/photo-cli.sqlite3"
		},
	};

	[Theory]
	[MemberData(nameof(ArchiveOptionsWithExpectedConnectionStrings))]
	public void Value_GivenArchiveOptions_ShouldMatchWithExpected(ArchiveOptions archiveOptions, string expectedConnectionString)
	{
		var sut = new ArchiveIsqLiteConnectionStringProvider(archiveOptions);
		sut.Value.Should().Be(expectedConnectionString);
	}
}
