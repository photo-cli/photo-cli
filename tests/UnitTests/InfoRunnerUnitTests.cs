namespace PhotoCli.Tests.UnitTests;

public class InfoRunnerUnitTests
{
	private readonly Mock<IPhotoCollectorService> _photoCollectorMock = new(MockBehavior.Strict);
	private readonly Mock<IExifDataAppenderService> _exifDataAppenderMock = new(MockBehavior.Strict);
	private readonly Mock<ICsvService> _csvServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IReverseGeocodeFetcherService> _reverseGeocodeFetcherMock = new(MockBehavior.Strict);
	private readonly MockFileSystem _fileSystemMock = new();
	private const string OutputPath = "output-folder";
	private const string SourceFolderPath = "source-folder";

	#region Expected Code Flow

	public static TheoryData<InfoOptions, bool> WithoutReverseGeocodeInfoOptionsAndCodeFlowOptions = new()
	{
		{ InfoOptionsFakes.WithoutReverseGeocode(OutputPath, SourceFolderPath), false },
	};

	public static TheoryData<InfoOptions, bool> WithReverseGeocodeInfoOptionsAndCodeFlowOptions = new()
	{
		{ InfoOptionsFakes.WithValidReverseGeocodeService(OutputPath, SourceFolderPath), true },
	};

	[Theory]
	[MemberData(nameof(WithoutReverseGeocodeInfoOptionsAndCodeFlowOptions))]
	[MemberData(nameof(WithReverseGeocodeInfoOptionsAndCodeFlowOptions))]
	public async Task Valid_Full_Workflow_Verify_All_Invocations_With_Specific_Parameters_Should_Exit_With_Success(InfoOptions options, bool verifyReverseGeocodeMocks)
	{
		const string photoPath = "/photo.jpg";
		string[] photoPaths = { "/photo.jpg" };
		var photoExifDataByFilePath = new Dictionary<string, ExifData?> { { photoPath, ExifDataFakes.Create(DateTimeFakes.WithYear(2000), CoordinateFakes.Valid()) } };

		Dictionary<string, ExifData?>? reverseGeocodedPhotoExifDataByFilePath = null;
		if (verifyReverseGeocodeMocks)
		{
			reverseGeocodedPhotoExifDataByFilePath = new Dictionary<string, ExifData?>();
			foreach (var (filePathKey, exifData) in photoExifDataByFilePath)
				reverseGeocodedPhotoExifDataByFilePath.Add(filePathKey, ExifDataFakes.Create(exifData?.TakenDate, exifData?.Coordinate, ReverseGeocodeFakes.Valid()));
		}

		SetupValid(options, photoPaths, true, true, true, photoExifDataByFilePath, verifyReverseGeocodeMocks, reverseGeocodedPhotoExifDataByFilePath);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.Success);
		VerifyValid(photoPaths, true, true, true, photoExifDataByFilePath,
			verifyReverseGeocodeMocks, reverseGeocodedPhotoExifDataByFilePath);
	}

	private void SetupValid(InfoOptions options, string[] photoPaths, bool allPhotosAreValidMockOutValue, bool allPhotosHasPhotoTakenMockOutValue, bool allPhotosHasCoordinateMockOutValue,
		Dictionary<string, ExifData?> photoExifDataByFilePath, bool setupReverseGeocodeFetcher, Dictionary<string, ExifData?>? reverseGeocodedPhotoExifDataByFilePath)
	{
		AddSourceDirectory();
		_photoCollectorMock.Setup(s => s.Collect(options.InputPath!, It.IsAny<bool>())).Returns(() => photoPaths);
		_exifDataAppenderMock.Setup(s => s.ExifDataByPath(photoPaths, out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenMockOutValue, out allPhotosHasCoordinateMockOutValue)).Returns(() => photoExifDataByFilePath);

		if (setupReverseGeocodeFetcher)
		{
			_reverseGeocodeFetcherMock.Setup(s => s.Fetch(photoExifDataByFilePath))
				.Returns(() => Task.FromResult(reverseGeocodedPhotoExifDataByFilePath)!);

			_csvServiceMock.Setup(s => s.WriteExifDataToCsvOutput(reverseGeocodedPhotoExifDataByFilePath!, OutputPath)).Returns(Task.CompletedTask);
		}
		else
		{
			_csvServiceMock.Setup(s => s.WriteExifDataToCsvOutput(photoExifDataByFilePath, OutputPath)).Returns(Task.CompletedTask);
		}
	}

	private void VerifyValid(string[] photoPaths, bool allPhotosAreValidMockOutValue, bool allPhotosHasPhotoTakenMockOutValue, bool allPhotosHasCoordinateMockOutValue, Dictionary<string, ExifData?> photoExifDataByFilePath,
		bool verifyReverseGeocodeFetcher, Dictionary<string, ExifData?>? reverseGeocodedPhotoExifDataByFilePath)
	{
		_photoCollectorMock.Verify(v => v.Collect(SourceFolderPath, It.IsAny<bool>()), Times.Once);
		_exifDataAppenderMock.Verify(v => v.ExifDataByPath(photoPaths, out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenMockOutValue, out allPhotosHasCoordinateMockOutValue), Times.Once);

		if (verifyReverseGeocodeFetcher)
		{
			_reverseGeocodeFetcherMock.Verify(v => v.Fetch(photoExifDataByFilePath), Times.Once);
			_csvServiceMock.Verify(s => s.WriteExifDataToCsvOutput(reverseGeocodedPhotoExifDataByFilePath!, OutputPath), Times.Once);
		}
		else
		{
			_csvServiceMock.Verify(s => s.WriteExifDataToCsvOutput(photoExifDataByFilePath, OutputPath), Times.Once);
		}

		VerifyNoOtherCalls();
	}

	#endregion

	#region Breaking Code Flow

	[Fact]
	public async Task When_Using_With_An_Existing_File_On_OutputPath_Should_Exit_With_OutputPathIsExists()
	{
		AddSourceDirectory();
		_fileSystemMock.AddDirectory(OutputPath);
		var reportFilePath = Path.Combine(OutputPath, "report.csv");
		_fileSystemMock.AddFile(reportFilePath, "dummy content");
		var sut = Initialize(new InfoOptions(reportFilePath, SourceFolderPath));
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.OutputPathIsExists);
		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task NoPhoto_Returns_From_PhotoCollector_Should_Exits_With_NoPhotoFound()
	{
		AddSourceDirectory();
		PhotoCollectorSetupEmptyList();
		var sut = Initialize(InfoOptionsFakes.WithPaths(OutputPath, SourceFolderPath));
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.NoPhotoFoundOnDirectory);
		PhotoCollectorVerify();
		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Source_Folder_Path_Not_Exists_Should_Exits_With_InputFolderNotExists_Code()
	{
		var reportFilePath = Path.Combine(OutputPath, "report.csv");
		var sut = Initialize(new InfoOptions(reportFilePath, SourceFolderPath));
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.InputFolderNotExists);
		VerifyNoOtherCalls();
	}

	#region Prevent Process Actions

	[Theory]
	[InlineData(InfoNoPhotoTakenDateAction.PreventProcess, true, InfoNoCoordinateAction.PreventProcess,true)]
	[InlineData(InfoNoPhotoTakenDateAction.PreventProcess, true, InfoNoCoordinateAction.PreventProcess,false)]
	[InlineData(InfoNoPhotoTakenDateAction.PreventProcess, false, InfoNoCoordinateAction.PreventProcess,true)]
	[InlineData(InfoNoPhotoTakenDateAction.PreventProcess, false, InfoNoCoordinateAction.PreventProcess,false)]
	public async Task When_InvalidFormatAction_PreventProcess_And_AllPhotosAreValid_Is_False_Runner_Should_Exit_With_PhotosWithInvalidFileFormatPreventedProcess(
		InfoNoPhotoTakenDateAction noPhotoTakenDateAction, bool allPhotosHasPhotoTaken, InfoNoCoordinateAction noCoordinateAction, bool allPhotosHasCoordinate)
	{
		var options = InfoOptionsFakes.WithPreventAction(SourceFolderPath, InfoInvalidFormatAction.PreventProcess, noPhotoTakenDateAction, noCoordinateAction);
		await CheckPreventActions(false, allPhotosHasPhotoTaken, allPhotosHasCoordinate, options, ExitCode.PhotosWithInvalidFileFormatPreventedProcess);
	}

	[Theory]
	[InlineData(InfoInvalidFormatAction.PreventProcess, true, InfoNoCoordinateAction.PreventProcess,true)]
	public async Task When_NoPhotoDateTimeTakenAction_PreventProcess_And_AllPhotosHasPhotoTaken_Is_False_Runner_Should_Exit_With_PhotosWithNoDatePreventedProcess(
		InfoInvalidFormatAction invalidFormatAction, bool allPhotosAreValid, InfoNoCoordinateAction noCoordinateAction, bool allPhotosHasCoordinate)
	{
		var options = InfoOptionsFakes.WithPreventAction(SourceFolderPath, invalidFormatAction, InfoNoPhotoTakenDateAction.PreventProcess, noCoordinateAction);
		await CheckPreventActions(allPhotosAreValid, false, allPhotosHasCoordinate, options, ExitCode.PhotosWithNoDatePreventedProcess);
	}

	[Theory]
	[InlineData(InfoInvalidFormatAction.PreventProcess, true, InfoNoPhotoTakenDateAction.PreventProcess,true)]
	public async Task When_NoPhotoCoordinateAction_PreventProcess_And_AllPhotosHasCoordinate_Is_False_Runner_Should_Exit_With_PhotosWithNoCoordinatePreventedProcess(
		InfoInvalidFormatAction invalidFormatAction, bool allPhotosAreValid, InfoNoPhotoTakenDateAction noPhotoTakenDateAction, bool allPhotosHasPhotoTaken)
	{
		var options = InfoOptionsFakes.WithPreventAction(SourceFolderPath, invalidFormatAction, noPhotoTakenDateAction, InfoNoCoordinateAction.PreventProcess);
		await CheckPreventActions(allPhotosAreValid, allPhotosHasPhotoTaken, false, options, ExitCode.PhotosWithNoCoordinatePreventedProcess);
	}

	[Theory]
	[InlineData(InfoInvalidFormatAction.PreventProcess, true)]
	public async Task When_NoPhotoDateTimeAction_And_NoCoordinateAction_PreventProcess_And_Both_AllPhotosHasPhotoTaken_AllPhotosHasCoordinate_Are_False_Runner_Should_Exit_With_PhotosWithNoCoordinateAndNoDatePreventedProcess(
		InfoInvalidFormatAction invalidFormatAction, bool allPhotosAreValid)
	{
		var options = InfoOptionsFakes.WithPreventAction(SourceFolderPath, invalidFormatAction, InfoNoPhotoTakenDateAction.PreventProcess, InfoNoCoordinateAction.PreventProcess);
		await CheckPreventActions(allPhotosAreValid, false, false, options, ExitCode.PhotosWithNoCoordinateAndNoDatePreventedProcess);
	}

	private async Task CheckPreventActions(bool allPhotosAreValidOutValue, bool allPhotosHasPhotoTakenOutValue, bool allPhotosHasCoordinateOutValue, InfoOptions options, ExitCode expectedExitCode)
	{
		AddSourceDirectory();
		PhotoCollectorSetupNonEmptyList();
		_exifDataAppenderMock.Setup(s => s.ExifDataByPath(It.IsAny<string[]>(), out allPhotosAreValidOutValue, out allPhotosHasPhotoTakenOutValue, out allPhotosHasCoordinateOutValue))
			.Returns(() => new Dictionary<string, ExifData?> { { "/photo.jpg", ExifDataFakes.Valid() } });
		var sut = Initialize(options);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(expectedExitCode);
		PhotoCollectorVerify();
		_exifDataAppenderMock.Verify(v => v.ExifDataByPath(It.IsAny<string[]>(), out allPhotosAreValidOutValue, out allPhotosHasPhotoTakenOutValue, out allPhotosHasCoordinateOutValue));
		VerifyNoOtherCalls();
	}

	#endregion

	#endregion

	#region Shared

	private InfoRunner Initialize(InfoOptions options)
	{
		return new InfoRunner(NullLogger<InfoRunner>.Instance, options, _photoCollectorMock.Object, _exifDataAppenderMock.Object, _fileSystemMock, _reverseGeocodeFetcherMock.Object,
			_csvServiceMock.Object, new Statistics(), ConsoleWriterFakes.Valid());
	}

	private void PhotoCollectorSetupEmptyList()
	{
		_photoCollectorMock.Setup(s => s.Collect(SourceFolderPath, It.IsAny<bool>())).Returns(Array.Empty<string>);
	}

	private void PhotoCollectorSetupNonEmptyList()
	{
		_photoCollectorMock.Setup(s => s.Collect(SourceFolderPath, It.IsAny<bool>())).Returns(() => new[] { "/photo.jpg" });
	}

	private void PhotoCollectorVerify()
	{
		_photoCollectorMock.Verify(v => v.Collect(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
	}

	private void VerifyNoOtherCalls()
	{
		_photoCollectorMock.VerifyNoOtherCalls();
		_exifDataAppenderMock.VerifyNoOtherCalls();
	}

	private void AddSourceDirectory()
	{
		_fileSystemMock.AddDirectory(SourceFolderPath);
	}

	#endregion
}
