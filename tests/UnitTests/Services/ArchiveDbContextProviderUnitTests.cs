namespace PhotoCli.Tests.UnitTests.Services;

public class ArchiveDbContextProviderUnitTests
{
	[Fact]
	public void Value_GivenValidConnectionString_ShouldCreateContext()
	{
		var sqLiteConnectionStringProvider = new Mock<ISQLiteConnectionStringProvider>(MockBehavior.Strict);
		sqLiteConnectionStringProvider.Setup(s => s.Value).Returns(SQLiteConnectionStringFakes.InMemory);
		var sut = new ArchiveDbContextProvider(sqLiteConnectionStringProvider.Object, new Mock<IFileService>().Object, ArchiveDatabaseOptionsFakes.Valid(), NullLogger<ArchiveDbContextProvider>.Instance);
		sut.CreateOrGetInstance().Should().NotBeNull();
	}
}
