namespace PhotoCli.Tests.UnitTests.Services;

public class ArchiveDbContextDesignTimeUnitTest
{
	[Fact]
	public void CreateDbContext_Initialize_ShouldNotBeNull()
	{
		var archiveDbContextDesignTime = new ArchiveDbContextDesignTime();
		archiveDbContextDesignTime.CreateDbContext(null!);
		archiveDbContextDesignTime.Should().NotBeNull();
	}
}
