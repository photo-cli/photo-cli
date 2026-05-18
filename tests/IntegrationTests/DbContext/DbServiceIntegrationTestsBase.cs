namespace PhotoCli.Tests.IntegrationTests.DbContext;

public class DbServiceIntegrationTestsBase
{
	protected const string OutputPath = "output-folder";

	protected static (DbService, ArchiveDbContextProvider) DbServiceSetup()
	{
		var archiveDbContextProvider = new ArchiveDbContextProvider(new InMemorySQLiteConnectionStringProvider(), new Mock<IFileService>().Object, ArchiveDatabaseOptionsFakes.Valid(),
			NullLogger<ArchiveDbContextProvider>.Instance);

		var dbService = new DbService(archiveDbContextProvider, ToolOptions.Default(), StatisticsFakes.Empty(), ConsoleWriterFakes.Valid(), NullLogger<DbService>.Instance);
		return (dbService, archiveDbContextProvider);
	}

	protected static async Task<DbService> DbServiceSetupWithAlbums(AlbumEntity[] existingAlbums)
	{
		var (dbService, archiveDbContextProvider) = DbServiceSetup();
		var archiveDbContext = archiveDbContextProvider.CreateOrGetInstance();
		await archiveDbContext.Albums.AddRangeAsync(existingAlbums);
		await archiveDbContext.SaveChangesAsync();
		return dbService;
	}

	protected static string OutputFilePathWithJpg(string fileName)
	{
		return MockFileSystemHelper.Combine(true, OutputPath, $"{fileName}.jpg");
	}

	protected static string FilePathWithJpg(string fileName)
	{
		return $"{fileName}.jpg";
	}

	protected static async Task<AlbumEntity> AddAlbum(AlbumEntity albumEntity, ArchiveDbContextProvider archiveDbContextProvider)
	{
		var archiveDbContext = archiveDbContextProvider.CreateOrGetInstance();
		await archiveDbContext.Albums.AddAsync(albumEntity);
		await archiveDbContext.SaveChangesAsync();
		return albumEntity;
	}
}
