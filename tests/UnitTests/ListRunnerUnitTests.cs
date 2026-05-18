using System.Runtime.InteropServices;
using Spectre.Console;

namespace PhotoCli.Tests.UnitTests;

public class ListRunnerUnitTests
{
	private readonly Mock<IDbService> _dbServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IConsoleWriter> _consoleWriterMock = new(MockBehavior.Loose);
	private readonly Mock<AnsiConsoleExtended> _ansiConsoleExtendedMock = new(new Mock<IAnsiConsole>().Object);
	private readonly Mock<IProcessLauncher> _processLauncherMock = new(MockBehavior.Strict);
	private readonly MockFileSystem _fileSystemMock = new();
	private const string ArchivePath = "archive-folder";
	private const string DatabaseFileName = "photo-cli.sqlite3";

	#region Expected Code Flow

	[Fact]
	public async Task Execute_SummaryValidWorkflow_ShouldExitWithSuccessWithVerifyingAllMockedServices()
	{
		var options = ListOptionsFakes.Summary(ArchivePath);

		_dbServiceMock.Setup(s => s.TotalAlbumCount()).ReturnsAsync(5);
		_dbServiceMock.Setup(s => s.TotalPhotoCount()).ReturnsAsync(100);
		_dbServiceMock.Setup(s => s.TotalReverseGeocodeCacheCount()).ReturnsAsync(10);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.Success);
		_dbServiceMock.Verify(v => v.TotalAlbumCount(), Times.Once);
		_dbServiceMock.Verify(v => v.TotalPhotoCount(), Times.Once);
		_dbServiceMock.Verify(v => v.TotalReverseGeocodeCacheCount(), Times.Once);
		_consoleWriterMock.Verify(v => v.WriteTable(It.IsAny<Table>()), Times.Once);
		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Execute_AlbumsValidWorkflow_ShouldExitWithSuccessWithVerifyingAllMockedServices()
	{
		var options = ListOptionsFakes.Albums(ArchivePath);
		var albums = new List<AlbumEntity>
		{
			AlbumEntityFakes.Sample(1),
			AlbumEntityFakes.Sample(2),
			AlbumEntityFakes.Sample(3),
		};
		_dbServiceMock.Setup(s => s.GetAllAlbums()).ReturnsAsync(albums);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.Success);
		_dbServiceMock.Verify(v => v.GetAllAlbums(), Times.Once);
		_consoleWriterMock.Verify(v => v.WriteTable(It.IsAny<Table>()), Times.Once);
		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Execute_PhotosByAlbumValidWorkflow_ShouldExitWithSuccessWithVerifyingAllMockedServices()
	{
		const int albumId = 1;
		var options = ListOptionsFakes.PhotosByAlbum(albumId, ArchivePath);
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.Sample(1),
			PhotoEntityFakes.Sample(2),
		};
		var albumPhotoResult = new AlbumPhotoResult(AlbumPhotoResultStatus.Successful, photos);

		_dbServiceMock.Setup(s => s.GetAlbumPhotosById(albumId)).ReturnsAsync(albumPhotoResult);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			_processLauncherMock.Setup(s => s.Launch(It.IsAny<IEnumerable<string>>())).Returns(Task.CompletedTask);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.Success);
		_dbServiceMock.Verify(v => v.GetAlbumPhotosById(albumId), Times.Once);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			_processLauncherMock.Verify(v => v.Launch(It.Is<IEnumerable<string>>(paths => paths.Count() == 2)), Times.Once);

		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Execute_PhotosByAlbumNameValidWorkflow_ShouldExitWithSuccessWithVerifyingAllMockedServices()
	{
		const string albumName = "My Album";
		var options = ListOptionsFakes.PhotosByAlbumName(albumName, ArchivePath);
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.Sample(1),
			PhotoEntityFakes.Sample(2),
		};
		var albumPhotoResult = new AlbumPhotoResult(AlbumPhotoResultStatus.Successful, photos);

		_dbServiceMock.Setup(s => s.GetAlbumPhotosByName(albumName)).ReturnsAsync(albumPhotoResult);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			_processLauncherMock.Setup(s => s.Launch(It.IsAny<IEnumerable<string>>())).Returns(Task.CompletedTask);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.Success);
		_dbServiceMock.Verify(v => v.GetAlbumPhotosByName(albumName), Times.Once);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			_processLauncherMock.Verify(v => v.Launch(It.Is<IEnumerable<string>>(paths => paths.Count() == 2)), Times.Once);

		VerifyNoOtherCalls();
	}

	public static TheoryData<int?, byte?, byte?> PhotoByDateYearMonthDays = new()
	{
		{ 2000, null, null },
		{ 2001, 7, null },
		{ 2017, 1, 17 },
	};

	[Theory]
	[MemberData(nameof(PhotoByDateYearMonthDays))]
	public async Task Execute_PhotosByDateValidWorkflow_ShouldExitWithSuccessWithVerifyingAllMockedServices(int? year = null, byte? month = null, byte? day = null)
	{
		var options = ListOptionsFakes.PhotosByDate(archivePath: ArchivePath, year: year, month: month, day: day);
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.Sample(1),
			PhotoEntityFakes.Sample(2),
		};

		_dbServiceMock.Setup(s => s.GetPhotosByDate(year, month, day)).ReturnsAsync(photos);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			_processLauncherMock.Setup(s => s.Launch(It.IsAny<IEnumerable<string>>())).Returns(Task.CompletedTask);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.Success);
		_dbServiceMock.Verify(v => v.GetPhotosByDate(year, month, day), Times.Once);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			_processLauncherMock.Verify(v => v.Launch(It.IsAny<IEnumerable<string>>()), Times.Once);

		VerifyNoOtherCalls();
	}

	#endregion

	#region Breaking Code Flow

	#region No Photo Found

	[Fact]
	public async Task Execute_PhotosByAlbumWithNoPhotosShouldReturnNoPhotoFoundToList()
	{
		const int albumId = 1;
		var options = ListOptionsFakes.PhotosByAlbum(albumId, ArchivePath);
		var albumPhotoResult = new AlbumPhotoResult(AlbumPhotoResultStatus.AlbumNotFound, []);
		_dbServiceMock.Setup(s => s.GetAlbumPhotosById(albumId)).ReturnsAsync(albumPhotoResult);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.AlbumNotFoundById);
		_dbServiceMock.Verify(v => v.GetAlbumPhotosById(albumId), Times.Once);

		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Execute_PhotosByDateWithNoPhotos_ShouldReturnNoPhotoFoundToList()
	{
		const int year = 2020;
		var options = ListOptionsFakes.PhotosByDate(year: year, archivePath: ArchivePath);
		var photos = new List<PhotoEntity>();
		_dbServiceMock.Setup(s => s.GetPhotosByDate(year, null, null)).ReturnsAsync(photos);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.NoPhotoFoundToList);
		_dbServiceMock.Verify(v => v.GetPhotosByDate(year, null, null), Times.Once);
		_consoleWriterMock.Verify(v => v.Write(It.Is<string>(s => s.Contains("No photo found"))), Times.Once);
		VerifyNoOtherCalls();
	}

	#endregion

	#region Invalid Album

	[Fact]
	public async Task Execute_PhotosByAlbumWithAlbumNotFound_ShouldReturnAlbumNotFoundById()
	{
		const int albumId = 1;
		var options = ListOptionsFakes.PhotosByAlbum(albumId, ArchivePath);
		var albumPhotoResult = new AlbumPhotoResult(AlbumPhotoResultStatus.AlbumNotFound, new List<PhotoEntity>());
		_dbServiceMock.Setup(s => s.GetAlbumPhotosById(albumId)).ReturnsAsync(albumPhotoResult);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.AlbumNotFoundById);
		_dbServiceMock.Verify(v => v.GetAlbumPhotosById(albumId), Times.Once);
		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Execute_PhotosByAlbumWithInvalidConfiguration_ShouldReturnExistingAlbumConfigurationNotValid()
	{
		const int albumId = 1;
		var options = ListOptionsFakes.PhotosByAlbum(albumId, ArchivePath);
		var albumPhotoResult = new AlbumPhotoResult(AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat, new List<PhotoEntity>());

		_dbServiceMock.Setup(s => s.GetAlbumPhotosById(albumId)).ReturnsAsync(albumPhotoResult);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.ExistingAlbumConfigurationNotValid);
		_dbServiceMock.Verify(v => v.GetAlbumPhotosById(albumId), Times.Once);
		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Execute_PhotosByAlbumNameWithAlbumNotFound_ShouldReturnAlbumNotFoundByName()
	{
		const string albumName = "My Album";
		var options = ListOptionsFakes.PhotosByAlbumName(albumName, ArchivePath);
		var albumPhotoResult = new AlbumPhotoResult(AlbumPhotoResultStatus.AlbumNotFound, new List<PhotoEntity>());
		_dbServiceMock.Setup(s => s.GetAlbumPhotosByName(albumName)).ReturnsAsync(albumPhotoResult);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.AlbumNotFoundByName);
		_dbServiceMock.Verify(v => v.GetAlbumPhotosByName(albumName), Times.Once);
		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Execute_PhotosByAlbumNameWithInvalidConfiguration_ShouldReturnExistingAlbumConfigurationNotValid()
	{
		const string albumName = "My Album";
		var options = ListOptionsFakes.PhotosByAlbumName(albumName, ArchivePath);
		var albumPhotoResult = new AlbumPhotoResult(AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat, new List<PhotoEntity>());
		_dbServiceMock.Setup(s => s.GetAlbumPhotosByName(albumName)).ReturnsAsync(albumPhotoResult);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.ExistingAlbumConfigurationNotValid);
		_dbServiceMock.Verify(v => v.GetAlbumPhotosByName(albumName), Times.Once);
		VerifyNoOtherCalls();
	}

	#endregion

	#region Archive Database Not Found

	[Fact]
	public async Task Execute_PhotosByDateRangeValidWorkflow_ShouldExitWithSuccessWithVerifyingAllMockedServices()
	{
		var startDate = new DateTime(2020, 1, 1);
		var endDate = new DateTime(2020, 12, 31);
		var options = ListOptionsFakes.PhotosByDateRange(startDate, endDate, ArchivePath);
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.Sample(1),
			PhotoEntityFakes.Sample(2),
		};

		_dbServiceMock.Setup(s => s.GetPhotosByDateRange(startDate, endDate)).ReturnsAsync(photos);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			_processLauncherMock.Setup(s => s.Launch(It.IsAny<IEnumerable<string>>())).Returns(Task.CompletedTask);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.Success);
		_dbServiceMock.Verify(v => v.GetPhotosByDateRange(startDate, endDate), Times.Once);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			_processLauncherMock.Verify(v => v.Launch(It.IsAny<IEnumerable<string>>()), Times.Once);

		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Execute_PhotosByDateRangeWithNoPhotos_ShouldReturnNoPhotoFoundToList()
	{
		var startDate = new DateTime(2020, 1, 1);
		var endDate = new DateTime(2020, 12, 31);
		var options = ListOptionsFakes.PhotosByDateRange(startDate, endDate, ArchivePath);
		_dbServiceMock.Setup(s => s.GetPhotosByDateRange(startDate, endDate)).ReturnsAsync(new List<PhotoEntity>());

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.NoPhotoFoundToList);
		_dbServiceMock.Verify(v => v.GetPhotosByDateRange(startDate, endDate), Times.Once);
		_consoleWriterMock.Verify(v => v.Write(It.Is<string>(s => s.Contains("No photo found"))), Times.Once);
		VerifyNoOtherCalls();
	}

	public static TheoryData<ListOptions> ListOptionsAllDifferentTypes = new()
	{
		ListOptionsFakes.Summary(ArchivePath),
		ListOptionsFakes.Albums(ArchivePath),
		ListOptionsFakes.PhotosByAlbum(1, ArchivePath),
		ListOptionsFakes.PhotosByAlbumName("My Album", ArchivePath),
		ListOptionsFakes.PhotosByDate(year: 2020, archivePath: ArchivePath),
		ListOptionsFakes.PhotosByDateRange(new DateTime(2020, 1, 1), new DateTime(2020, 12, 31), ArchivePath),
	};

	[Theory]
	[MemberData(nameof(ListOptionsAllDifferentTypes))]
	public async Task Execute_WithNoArchiveDatabaseInput_ShouldReturnNoArchiveDatabaseFound(ListOptions options)
	{
		var sut = Initialize(options, false);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.NoArchiveDatabaseFound);
		VerifyNoOtherCalls();
	}

	#endregion

	#endregion

	#region Helper Methods

	private ListRunner Initialize(ListOptions options, bool createDatabase = true)
	{
		_fileSystemMock.AddDirectory(ArchivePath);

		if (createDatabase)
		{
			var databasePath = Path.Combine(ArchivePath, DatabaseFileName);
			_fileSystemMock.AddFile(databasePath, new MockFileData(string.Empty));
		}

		return new ListRunner(options, _dbServiceMock.Object, _fileSystemMock, StatisticsFakes.Empty(), _consoleWriterMock.Object, _ansiConsoleExtendedMock.Object, _processLauncherMock.Object,
			NullLogger<ListRunner>.Instance);
	}

	private void VerifyNoOtherCalls()
	{
		_dbServiceMock.VerifyNoOtherCalls();
		_processLauncherMock.VerifyNoOtherCalls();
		_ansiConsoleExtendedMock.VerifyNoOtherCalls();
	}

	#endregion
}
