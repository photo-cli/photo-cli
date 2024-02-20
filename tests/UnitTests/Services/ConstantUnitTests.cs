namespace PhotoCli.Tests.UnitTests.Services;

public class ConstantUnitTests
{
	[Fact]
	public void AppSettingsFileName_CriticalConstantValue_ShouldMatchNotChanged()
	{
		Constants.AppSettingsFileName.Should().Be("appsettings.json");
	}

	[Fact]
	public void VerifyFileHashFileName_CriticalConstantValue_ShouldMatchNotChanged()
	{
		Constants.VerifyFileHashFileName.Should().Be("sha1.lst");
	}

	[Fact]
	public void ArchiveSQLiteDatabaseFileName_CriticalConstantValue_ShouldMatchNotChanged()
	{
		Constants.ArchiveSQLiteDatabaseFileName.Should().Be("photo-cli.sqlite3");
	}
}
