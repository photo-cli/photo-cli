using PhotoCli.McpTools;

namespace PhotoCli.Tests.UnitTests;

public class ArchiveMcpToolsUnitTests
{
	private readonly Mock<IDbService> _dbServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IProcessLauncher> _processLauncherMock = new(MockBehavior.Strict);
	private readonly McpOptions _mcpOptions = new("/archive");

	private ArchiveMcpTools Sut => new(_dbServiceMock.Object, _processLauncherMock.Object, _mcpOptions);

	#region ListPhotosByAlbumId

	[Fact]
	public async Task ListPhotosByAlbumId_GivenExistingAlbumId_ShouldReturnSerializedPhotos()
	{
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 15), 1),
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2021, 3, 10), 2),
		};
		_dbServiceMock.Setup(s => s.GetAlbumPhotosById(1))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.Successful, photos));

		var result = await Sut.ListPhotosByAlbumId(1);

		var deserialized = JsonSerializer.Deserialize<JsonElement[]>(result);
		deserialized.Should().HaveCount(2);
		deserialized![0].GetProperty("Path").GetString().Should().Be("1.jpg");
		deserialized[1].GetProperty("Path").GetString().Should().Be("2.jpg");
	}

	[Fact]
	public async Task ListPhotosByAlbumId_GivenNonExistingAlbumId_ShouldReturnNotFoundMessage()
	{
		_dbServiceMock.Setup(s => s.GetAlbumPhotosById(99))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.AlbumNotFound, []));

		var result = await Sut.ListPhotosByAlbumId(99);

		result.Should().Be("Album with id 99 not found.");
	}

	[Fact]
	public async Task ListPhotosByAlbumId_GivenInvalidConfiguration_ShouldReturnInvalidConfigurationMessage()
	{
		_dbServiceMock.Setup(s => s.GetAlbumPhotosById(5))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat, []));

		var result = await Sut.ListPhotosByAlbumId(5);

		result.Should().Be("Album with id 5 has invalid configuration.");
	}

	#endregion

	#region ListPhotosByAlbumName

	[Fact]
	public async Task ListPhotosByAlbumName_GivenExistingAlbumName_ShouldReturnSerializedPhotos()
	{
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 15), 1),
		};
		_dbServiceMock.Setup(s => s.GetAlbumPhotosByName("Vacation"))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.Successful, photos));

		var result = await Sut.ListPhotosByAlbumName("Vacation");

		var deserialized = JsonSerializer.Deserialize<JsonElement[]>(result);
		deserialized.Should().HaveCount(1);
		deserialized![0].GetProperty("Path").GetString().Should().Be("1.jpg");
	}

	[Fact]
	public async Task ListPhotosByAlbumName_GivenNonExistingAlbumName_ShouldReturnNotFoundMessage()
	{
		_dbServiceMock.Setup(s => s.GetAlbumPhotosByName("Ghost"))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.AlbumNotFound, []));

		var result = await Sut.ListPhotosByAlbumName("Ghost");

		result.Should().Be("Album with name 'Ghost' not found.");
	}

	[Fact]
	public async Task ListPhotosByAlbumName_GivenInvalidConfiguration_ShouldReturnInvalidConfigurationMessage()
	{
		_dbServiceMock.Setup(s => s.GetAlbumPhotosByName("Broken"))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat, []));

		var result = await Sut.ListPhotosByAlbumName("Broken");

		result.Should().Be("Album with name 'Broken' has invalid configuration.");
	}

	#endregion

	#region ListPhotosByExactDate

	[Fact]
	public async Task ListPhotosByExactDate_GivenYearOnly_ShouldReturnMatchingPhotos()
	{
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 1, 1), 1),
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 15), 2),
		};
		_dbServiceMock.Setup(s => s.GetPhotosByDate(2020, null, null)).ReturnsAsync(photos);

		var result = await Sut.ListPhotosByExactDate(2020);

		var deserialized = JsonSerializer.Deserialize<JsonElement[]>(result);
		deserialized.Should().HaveCount(2);
	}

	[Fact]
	public async Task ListPhotosByExactDate_GivenYearMonthDay_ShouldReturnMatchingPhotos()
	{
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 17), 1),
		};
		_dbServiceMock.Setup(s => s.GetPhotosByDate(2020, 3, 17)).ReturnsAsync(photos);

		var result = await Sut.ListPhotosByExactDate(2020, 3, 17);

		var deserialized = JsonSerializer.Deserialize<JsonElement[]>(result);
		deserialized.Should().HaveCount(1);
		deserialized![0].GetProperty("Path").GetString().Should().Be("1.jpg");
	}

	[Fact]
	public async Task ListPhotosByExactDate_GivenNoMatches_ShouldReturnEmptyArray()
	{
		_dbServiceMock.Setup(s => s.GetPhotosByDate(1999, null, null)).ReturnsAsync([]);

		var result = await Sut.ListPhotosByExactDate(1999);

		var deserialized = JsonSerializer.Deserialize<JsonElement[]>(result);
		deserialized.Should().BeEmpty();
	}

	#endregion

	#region ListPhotosByDateRange

	[Fact]
	public async Task ListPhotosByDateRange_GivenStartAndEndDate_ShouldReturnMatchingPhotos()
	{
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 17), 1),
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 18), 2),
		};
		_dbServiceMock.Setup(s => s.GetPhotosByDateRange(
			It.Is<DateTime?>(d => d!.Value.Date == new DateTime(2020, 3, 17)),
			It.Is<DateTime?>(d => d!.Value.Date == new DateTime(2020, 3, 19))))
			.ReturnsAsync(photos);

		var result = await Sut.ListPhotosByDateRange("2020-03-17", "2020-03-19");

		var deserialized = JsonSerializer.Deserialize<JsonElement[]>(result);
		deserialized.Should().HaveCount(2);
	}

	[Fact]
	public async Task ListPhotosByDateRange_GivenNoDates_ShouldReturnAllPhotos()
	{
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 1, 1), 1),
		};
		_dbServiceMock.Setup(s => s.GetPhotosByDateRange(null, null)).ReturnsAsync(photos);

		var result = await Sut.ListPhotosByDateRange();

		var deserialized = JsonSerializer.Deserialize<JsonElement[]>(result);
		deserialized.Should().HaveCount(1);
	}

	[Fact]
	public async Task ListPhotosByDateRange_GivenInvalidDateFormat_ShouldTreatAsNull()
	{
		_dbServiceMock.Setup(s => s.GetPhotosByDateRange(null, null)).ReturnsAsync([]);

		var result = await Sut.ListPhotosByDateRange("not-a-date", "also-not-a-date");

		var deserialized = JsonSerializer.Deserialize<JsonElement[]>(result);
		deserialized.Should().BeEmpty();
	}

	#endregion

	#region OpenPhotosByAlbumId

	[Fact]
	public async Task OpenPhotosByAlbumId_GivenExistingAlbumId_ShouldOpenPhotos()
	{
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 15), 1),
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2021, 3, 10), 2),
		};
		_dbServiceMock.Setup(s => s.GetAlbumPhotosById(1))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.Successful, photos));
		_processLauncherMock.Setup(s => s.Launch(It.IsAny<List<string>>())).Returns(Task.CompletedTask);

		var result = await Sut.OpenPhotosByAlbumId(1);

		result.Should().Be("Opened 2 photo(s) in the default viewer.");
		_processLauncherMock.Verify(s => s.Launch(It.Is<List<string>>(paths =>
			paths.Count == 2 &&
			paths[0] == Path.Combine("/archive", "1.jpg") &&
			paths[1] == Path.Combine("/archive", "2.jpg"))), Times.Once);
	}

	[Fact]
	public async Task OpenPhotosByAlbumId_GivenNonExistingAlbumId_ShouldReturnNotFoundMessage()
	{
		_dbServiceMock.Setup(s => s.GetAlbumPhotosById(99))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.AlbumNotFound, []));

		var result = await Sut.OpenPhotosByAlbumId(99);

		result.Should().Be("Album with id 99 not found.");
	}

	[Fact]
	public async Task OpenPhotosByAlbumId_GivenInvalidConfiguration_ShouldReturnInvalidConfigurationMessage()
	{
		_dbServiceMock.Setup(s => s.GetAlbumPhotosById(5))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat, []));

		var result = await Sut.OpenPhotosByAlbumId(5);

		result.Should().Be("Album with id 5 has invalid configuration.");
	}

	#endregion

	#region OpenPhotosByAlbumName

	[Fact]
	public async Task OpenPhotosByAlbumName_GivenExistingAlbumName_ShouldOpenPhotos()
	{
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 15), 1),
		};
		_dbServiceMock.Setup(s => s.GetAlbumPhotosByName("Vacation"))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.Successful, photos));
		_processLauncherMock.Setup(s => s.Launch(It.IsAny<List<string>>())).Returns(Task.CompletedTask);

		var result = await Sut.OpenPhotosByAlbumName("Vacation");

		result.Should().Be("Opened 1 photo(s) in the default viewer.");
		_processLauncherMock.Verify(s => s.Launch(It.Is<List<string>>(paths =>
			paths.Count == 1 &&
			paths[0] == Path.Combine("/archive", "1.jpg"))), Times.Once);
	}

	[Fact]
	public async Task OpenPhotosByAlbumName_GivenNonExistingAlbumName_ShouldReturnNotFoundMessage()
	{
		_dbServiceMock.Setup(s => s.GetAlbumPhotosByName("Ghost"))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.AlbumNotFound, []));

		var result = await Sut.OpenPhotosByAlbumName("Ghost");

		result.Should().Be("Album with name 'Ghost' not found.");
	}

	[Fact]
	public async Task OpenPhotosByAlbumName_GivenInvalidConfiguration_ShouldReturnInvalidConfigurationMessage()
	{
		_dbServiceMock.Setup(s => s.GetAlbumPhotosByName("Broken"))
			.ReturnsAsync(new AlbumPhotoResult(AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat, []));

		var result = await Sut.OpenPhotosByAlbumName("Broken");

		result.Should().Be("Album with name 'Broken' has invalid configuration.");
	}

	#endregion

	#region OpenPhotosByExactDate

	[Fact]
	public async Task OpenPhotosByExactDate_GivenYearOnly_ShouldOpenMatchingPhotos()
	{
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 1, 1), 1),
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 15), 2),
		};
		_dbServiceMock.Setup(s => s.GetPhotosByDate(2020, null, null)).ReturnsAsync(photos);
		_processLauncherMock.Setup(s => s.Launch(It.IsAny<List<string>>())).Returns(Task.CompletedTask);

		var result = await Sut.OpenPhotosByExactDate(2020);

		result.Should().Be("Opened 2 photo(s) in the default viewer.");
	}

	[Fact]
	public async Task OpenPhotosByExactDate_GivenNoMatches_ShouldReturnNoPhotosMessage()
	{
		_dbServiceMock.Setup(s => s.GetPhotosByDate(1999, null, null)).ReturnsAsync([]);

		var result = await Sut.OpenPhotosByExactDate(1999);

		result.Should().Be("No photos found to open.");
	}

	#endregion

	#region OpenPhotosByDateRange

	[Fact]
	public async Task OpenPhotosByDateRange_GivenStartAndEndDate_ShouldOpenMatchingPhotos()
	{
		var photos = new List<PhotoEntity>
		{
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 17), 1),
			PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 18), 2),
		};
		_dbServiceMock.Setup(s => s.GetPhotosByDateRange(
			It.Is<DateTime?>(d => d!.Value.Date == new DateTime(2020, 3, 17)),
			It.Is<DateTime?>(d => d!.Value.Date == new DateTime(2020, 3, 19))))
			.ReturnsAsync(photos);
		_processLauncherMock.Setup(s => s.Launch(It.IsAny<List<string>>())).Returns(Task.CompletedTask);

		var result = await Sut.OpenPhotosByDateRange("2020-03-17", "2020-03-19");

		result.Should().Be("Opened 2 photo(s) in the default viewer.");
	}

	[Fact]
	public async Task OpenPhotosByDateRange_GivenNoDates_AndNoPhotos_ShouldReturnNoPhotosMessage()
	{
		_dbServiceMock.Setup(s => s.GetPhotosByDateRange(null, null)).ReturnsAsync([]);

		var result = await Sut.OpenPhotosByDateRange();

		result.Should().Be("No photos found to open.");
	}

	#endregion
}
