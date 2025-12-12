namespace PhotoCli.Tests.UnitTests.Services;

public class ArchiveSqLiteConnectionStringProviderUnitTests
{
	public static TheoryData<ArchiveDatabaseOptions, string> ArchiveOptionsWithExpectedConnectionStrings = new()
	{
		{
			new ArchiveDatabaseOptions("single-folder"),
			"Filename=single-folder/photo-cli.sqlite3"
		},
		{
			new ArchiveDatabaseOptions("multi/level/folder"),
			"Filename=multi/level/folder/photo-cli.sqlite3"
		},
	};

	[Theory]
	[MemberData(nameof(ArchiveOptionsWithExpectedConnectionStrings))]
	public void Value_GivenArchiveOptions_ShouldMatchWithExpected(ArchiveDatabaseOptions archiveDatabaseOptions, string expectedConnectionString)
	{
		var sut = new ArchiveIsqLiteConnectionStringProvider(archiveDatabaseOptions);
		sut.Value.Should().Be(expectedConnectionString);
	}
}
