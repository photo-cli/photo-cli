namespace PhotoCli.Tests.UnitTests;

public class ArchiveRunnerUnitTests
{
	private readonly Mock<IPhotoCollectorService> _photoCollectorMock = new(MockBehavior.Strict);
	private readonly Mock<IExifDataAppenderService> _exifDataAppenderMock = new(MockBehavior.Strict);
	private readonly Mock<IDirectoryGrouperService> _directoryGrouperServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IFileNamerService> _fileNamerServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IFileService> _fileServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IReverseGeocodeFetcherService> _reverseGeocodeFetcherMock = new(MockBehavior.Strict);
	private readonly Mock<IDuplicatePhotoRemoveService> _duplicatePhotoRemoveServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IDbService> _dbServiceMock = new(MockBehavior.Strict);
	private readonly MockFileSystem _fileSystemMock = new();
	private const string OutputPath = "output-folder";
	private const string SourceFolderPath = "source-folder";

	#region Expected Code Flow

	public static TheoryData<ArchiveOptions, bool> WithoutReverseGeocodeOptions = new()
	{
		{
			ArchiveOptionsFakes.WithoutReverseGeocode(OutputPath, SourceFolderPath), false
		},
	};

	public static TheoryData<ArchiveOptions, bool> WithReverseGeocodeOptions = new()
	{
		{
			ArchiveOptionsFakes.WithValidReverseGeocodeService(OutputPath, SourceFolderPath), true
		},
	};

	public static TheoryData<ArchiveOptions, bool> WithDryRun = new()
	{
		{
			ArchiveOptionsFakes.WithDryRun(OutputPath, SourceFolderPath), false
		},
	};

	[Theory]
	[MemberData(nameof(WithoutReverseGeocodeOptions))]
	[MemberData(nameof(WithReverseGeocodeOptions))]
	[MemberData(nameof(WithDryRun))]
	public async Task Execute_ValidWorkflow_ShouldExitWithSuccessWithVerifyingAllMockedServices(ArchiveOptions options, bool verifyReverseGeocodeMocks)
	{
		var photos = new List<Photo> { PhotoFakes.WithYear(2000) };
		const string photoPath = "/photo.jpg";
		const string targetRelativeDirectory = "2000/01/01";
		var photoExifDataByFilePath = new Dictionary<string, ExifData?> { { photoPath, ExifDataFakes.Create(DateTimeFakes.WithYear(2000), CoordinateFakes.Valid()) } };

		Dictionary<string, ExifData?>? reverseGeocodedPhotoExifDataByFilePath = null;
		if (verifyReverseGeocodeMocks)
		{
			reverseGeocodedPhotoExifDataByFilePath = new Dictionary<string, ExifData?>();
			foreach (var (filePathKey, exifData) in photoExifDataByFilePath)
				reverseGeocodedPhotoExifDataByFilePath.Add(filePathKey, ExifDataFakes.Create(exifData?.TakenDate, exifData?.Coordinate, ReverseGeocodeFakes.Valid()));
		}

		var groupedPhotoInfosByRelativeDirectory = new Dictionary<string, IReadOnlyCollection<Photo>> { { targetRelativeDirectory, photos } };

		Setup(options, true, true, true,
			photos, groupedPhotoInfosByRelativeDirectory, verifyReverseGeocodeMocks);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.Success);

		Verify(options, true, true, true, photos, verifyReverseGeocodeMocks);

		VerifyNoOtherCalls();
	}

	private void Setup(ArchiveOptions options, bool allPhotosAreValidMockOutValue, bool allPhotosHasPhotoTakenMockOutValue, bool allPhotosHasCoordinateMockOutValue,
		IReadOnlyCollection<Photo> photos, Dictionary<string, IReadOnlyCollection<Photo>> groupedPhotoInfosByRelativeDirectory, bool setupReverseGeocodeFetcher)
	{
		_photoCollectorMock.Setup(s => s.Collect(options.InputPath!, It.IsAny<bool>(), It.IsAny<bool>())).Returns(() => photos);
		_exifDataAppenderMock.Setup(s => s.ExtractExifData(photos, out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenMockOutValue, out allPhotosHasCoordinateMockOutValue)).Returns(() => photos);
		_fileServiceMock.Setup(s => s.CalculateFileHash(photos)).ReturnsAsync(photos);
		if (setupReverseGeocodeFetcher)
		{
			_reverseGeocodeFetcherMock.Setup(s => s.RateLimitWarning());

			_reverseGeocodeFetcherMock.Setup(s => s.Fetch(photos))
				.Returns(() => Task.FromResult(photos));
		}

		_directoryGrouperServiceMock.Setup(s => s.GroupFiles(photos, options.InputPath!,
				FolderProcessType.FlattenAllSubFolders, GroupByFolderType.YearMonthDay, true, true, false))
			.Returns(() => groupedPhotoInfosByRelativeDirectory);

		_duplicatePhotoRemoveServiceMock.Setup(s => s.GroupAndFilterByPhotoHash(photos)).Returns(() => photos);
		_fileNamerServiceMock.Setup(s => s.SetArchiveFileName(photos)).Returns(photos);
		_fileServiceMock.Setup(s => s.CopyIfNotExists(photos, options.OutputPath, options.IsDryRun)).Returns(photos);
		if (!options.IsDryRun)
			_fileServiceMock.Setup(s => s.VerifyFileIntegrity(photos)).ReturnsAsync(true);

		_dbServiceMock.Setup(s => s.Archive(photos, options.IsDryRun)).ReturnsAsync(1);
	}

	private void Verify(ArchiveOptions options, bool allPhotosAreValidMockOutValue, bool allPhotosHasPhotoTakenMockOutValue, bool allPhotosHasCoordinateMockOutValue,
		IReadOnlyList<Photo> photos, bool setupReverseGeocodeFetcher)
	{
		_photoCollectorMock.Verify(s => s.Collect(options.InputPath!, It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
		_exifDataAppenderMock.Verify(s => s.ExtractExifData(photos, out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenMockOutValue, out allPhotosHasCoordinateMockOutValue), Times.Once);

		if (setupReverseGeocodeFetcher)
		{
			_reverseGeocodeFetcherMock.Verify(v => v.RateLimitWarning(), Times.Once);
			_reverseGeocodeFetcherMock.Verify(s => s.Fetch(photos), Times.Once);
		}

		_directoryGrouperServiceMock.Verify(s => s.GroupFiles(photos, options.InputPath!, FolderProcessType.FlattenAllSubFolders, GroupByFolderType.YearMonthDay,
			true, true, false), Times.Once);

		_fileServiceMock.Verify(s => s.CalculateFileHash(photos), Times.Once);
		_duplicatePhotoRemoveServiceMock.Verify(s => s.GroupAndFilterByPhotoHash(photos), Times.Once);
		_fileNamerServiceMock.Verify(s => s.SetArchiveFileName(photos), Times.Once);
		_fileServiceMock.Verify(s => s.CopyIfNotExists(photos, options.OutputPath, options.IsDryRun), Times.Once);
		if (!options.IsDryRun)
			_fileServiceMock.Verify(s => s.VerifyFileIntegrity(photos), Times.Once);

		_dbServiceMock.Verify(v => v.Archive(photos, options.IsDryRun), Times.Once);
	}

	#endregion

	#region Breaking Code Flow

	[Fact]
	public async Task Execute_SourceFolderPathNotExists_ShouldExitsWithInputFolderNotExistsCode()
	{
		var sut = Initialize(ArchiveOptionsFakes.WithPaths(OutputPath, SourceFolderPath), false);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.InputFolderNotExists);
		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Execute_NoPhotoOnSourcePath_ShouldExitWithNoPhotoFoundOnDirectory()
	{
		_photoCollectorMock.Setup(s => s.Collect(SourceFolderPath, It.IsAny<bool>(), It.IsAny<bool>())).Returns(Array.Empty<Photo>);
		var sut = Initialize(ArchiveOptionsFakes.WithPaths(OutputPath, SourceFolderPath));
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.NoPhotoFoundOnDirectory);
		_photoCollectorMock.Verify(v => v.Collect(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
		VerifyNoOtherCalls();
	}

	#region Prevent Process Actions

	[Theory]
	[InlineData(ArchiveNoPhotoTakenDateAction.PreventProcess, true, ArchiveNoCoordinateAction.PreventProcess,true)]
	[InlineData(ArchiveNoPhotoTakenDateAction.PreventProcess, true, ArchiveNoCoordinateAction.PreventProcess,false)]
	[InlineData(ArchiveNoPhotoTakenDateAction.PreventProcess, false, ArchiveNoCoordinateAction.PreventProcess,true)]
	[InlineData(ArchiveNoPhotoTakenDateAction.PreventProcess, false, ArchiveNoCoordinateAction.PreventProcess,false)]
	public async Task When_InvalidFormatAction_PreventProcess_And_AllPhotosAreValid_Is_False_Runner_Should_Exit_With_PhotosWithInvalidFileFormatPreventedProcess(
		ArchiveNoPhotoTakenDateAction noPhotoTakenDateAction, bool allPhotosHasPhotoTaken, ArchiveNoCoordinateAction noCoordinateAction, bool allPhotosHasCoordinate)
	{
		var options = ArchiveOptionsFakes.WithPreventAction(SourceFolderPath, ArchiveInvalidFormatAction.PreventProcess, noPhotoTakenDateAction, noCoordinateAction);
		await CheckPreventActions(false, allPhotosHasPhotoTaken, allPhotosHasCoordinate, options, ExitCode.PhotosWithInvalidFileFormatPreventedProcess);
	}

	[Theory]
	[InlineData(ArchiveInvalidFormatAction.PreventProcess, true, ArchiveNoCoordinateAction.PreventProcess,true)]
	public async Task When_NoPhotoDateTimeTakenAction_PreventProcess_And_AllPhotosHasPhotoTaken_Is_False_Runner_Should_Exit_With_PhotosWithNoDatePreventedProcess(
		ArchiveInvalidFormatAction invalidFormatAction, bool allPhotosAreValid, ArchiveNoCoordinateAction noCoordinateAction, bool allPhotosHasCoordinate)
	{
		var options = ArchiveOptionsFakes.WithPreventAction(SourceFolderPath, invalidFormatAction, ArchiveNoPhotoTakenDateAction.PreventProcess, noCoordinateAction);
		await CheckPreventActions(allPhotosAreValid, false, allPhotosHasCoordinate, options, ExitCode.PhotosWithNoDatePreventedProcess);
	}

	[Theory]
	[InlineData(ArchiveInvalidFormatAction.PreventProcess, true, ArchiveNoPhotoTakenDateAction.PreventProcess,true)]
	public async Task When_NoPhotoCoordinateAction_PreventProcess_And_AllPhotosHasCoordinate_Is_False_Runner_Should_Exit_With_PhotosWithNoCoordinatePreventedProcess(
		ArchiveInvalidFormatAction invalidFormatAction, bool allPhotosAreValid, ArchiveNoPhotoTakenDateAction noPhotoTakenDateAction, bool allPhotosHasPhotoTaken)
	{
		var options = ArchiveOptionsFakes.WithPreventAction(SourceFolderPath, invalidFormatAction, noPhotoTakenDateAction, ArchiveNoCoordinateAction.PreventProcess);
		await CheckPreventActions(allPhotosAreValid, allPhotosHasPhotoTaken, false, options, ExitCode.PhotosWithNoCoordinatePreventedProcess);
	}

	[Theory]
	[InlineData(ArchiveInvalidFormatAction.PreventProcess, true)]
	public async Task When_NoPhotoDateTimeAction_And_NoCoordinateAction_PreventProcess_And_Both_AllPhotosHasPhotoTaken_AllPhotosHasCoordinate_Are_False_Runner_Should_Exit_With_PhotosWithNoCoordinateAndNoDatePreventedProcess(
		ArchiveInvalidFormatAction invalidFormatAction, bool allPhotosAreValid)
	{
		var options = ArchiveOptionsFakes.WithPreventAction(SourceFolderPath, invalidFormatAction, ArchiveNoPhotoTakenDateAction.PreventProcess, ArchiveNoCoordinateAction.PreventProcess);
		await CheckPreventActions(allPhotosAreValid, false, false, options, ExitCode.PhotosWithNoCoordinateAndNoDatePreventedProcess);
	}

	private async Task CheckPreventActions(bool allPhotosAreValidMockOutValue, bool allPhotosHasPhotoTakenOutValue, bool allPhotosHasCoordinateOutValue, ArchiveOptions options, ExitCode expectedExitCode)
	{
		var photos = new[] { PhotoFakes.Valid() };
		_photoCollectorMock.Setup(s => s.Collect(SourceFolderPath, It.IsAny<bool>(), It.IsAny<bool>())).Returns(() => photos);

		_exifDataAppenderMock.Setup(s => s.ExtractExifData(It.IsAny<IReadOnlyList<Photo>>(), out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenOutValue, out allPhotosHasCoordinateOutValue))
			.Returns(() => photos);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(expectedExitCode);
		_photoCollectorMock.Verify(v => v.Collect(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);

		_exifDataAppenderMock.Verify(v => v.ExtractExifData(It.IsAny<IReadOnlyList<Photo>>(), out allPhotosAreValidMockOutValue,
			out allPhotosHasPhotoTakenOutValue, out allPhotosHasCoordinateOutValue), Times.Once);

		VerifyNoOtherCalls();
	}

	#endregion

	#endregion

	#region Shared

	private ArchiveRunner Initialize(ArchiveOptions options, bool createSourcePath = true)
	{
		if (createSourcePath)
			CreateSourcePathDirectory();

		return new ArchiveRunner(NullLogger<ArchiveRunner>.Instance, options, _photoCollectorMock.Object, _exifDataAppenderMock.Object,
			_directoryGrouperServiceMock.Object, _fileNamerServiceMock.Object, _fileServiceMock.Object, _fileSystemMock, new Statistics(), _reverseGeocodeFetcherMock.Object,
			ConsoleWriterFakes.Valid(), _duplicatePhotoRemoveServiceMock.Object, _dbServiceMock.Object);
	}

	private void CreateSourcePathDirectory()
	{
		_fileSystemMock.AddDirectory(SourceFolderPath);
	}

	private void VerifyNoOtherCalls()
	{
		_photoCollectorMock.VerifyNoOtherCalls();
		_exifDataAppenderMock.VerifyNoOtherCalls();
		_reverseGeocodeFetcherMock.VerifyNoOtherCalls();
		_directoryGrouperServiceMock.VerifyNoOtherCalls();
		_fileServiceMock.VerifyNoOtherCalls();
		_duplicatePhotoRemoveServiceMock.VerifyNoOtherCalls();
		_fileNamerServiceMock.VerifyNoOtherCalls();
		_fileServiceMock.VerifyNoOtherCalls();
		_dbServiceMock.VerifyNoOtherCalls();
	}

	#endregion
}
