namespace PhotoCli.Tests.UnitTests;

public class CopyRunnerUnitTests
{
	private readonly Mock<IPhotoCollectorService> _photoCollectorMock = new(MockBehavior.Strict);
	private readonly Mock<IExifDataAppenderService> _exifDataAppenderMock = new(MockBehavior.Strict);
	private readonly Mock<IDirectoryGrouperService> _directoryGrouperMock = new(MockBehavior.Strict);
	private readonly Mock<IFileNamerService> _fileNamerMock = new(MockBehavior.Strict);
	private readonly Mock<IFileService> _fileServiceMock = new(MockBehavior.Strict);
	private readonly Mock<ICsvService> _csvServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IExifOrganizerService> _organizeByNoPhotoTakenActionMock = new(MockBehavior.Strict);
	private readonly Mock<IFolderRenamerService> _organizeDirectoriesByFolderProcessTypeMock = new(MockBehavior.Strict);
	private readonly Mock<IReverseGeocodeFetcherService> _reverseGeocodeFetcherMock = new(MockBehavior.Strict);
	private readonly MockFileSystem _fileSystemMock = new();
	private const string OutputPath = "output-folder";
	private const string SourceFolderPath = "source-folder";

	#region Expected Code Flow

	public static TheoryData<CopyOptions, bool, bool> WithoutFolderAppendTypeCopyOptionsAndCodeFlowOptions = new()
	{
		{ CopyOptionsFakes.WithoutFolderAppendType(OutputPath, SourceFolderPath), false, false },
	};

	public static TheoryData<CopyOptions, bool, bool> WithFolderAppendTypeCopyOptionsAndCodeFlowOptions = new()
	{
		{ CopyOptionsFakes.WithFolderAppendType(OutputPath, SourceFolderPath), true, false },
	};

	public static TheoryData<CopyOptions, bool, bool> WithReverseGeocodeCopyOptionsAndCodeFlowOptions = new()
	{
		{ CopyOptionsFakes.WithReverseGeocode(OutputPath, SourceFolderPath), false, true },
	};

	public static TheoryData<CopyOptions, bool, bool> WithDryRun = new()
	{
		{ CopyOptionsFakes.WithDryRun(OutputPath, SourceFolderPath), false, false },
	};


	[Theory]
	[MemberData(nameof(WithoutFolderAppendTypeCopyOptionsAndCodeFlowOptions))]
	[MemberData(nameof(WithFolderAppendTypeCopyOptionsAndCodeFlowOptions))]
	[MemberData(nameof(WithReverseGeocodeCopyOptionsAndCodeFlowOptions))]
	[MemberData(nameof(WithDryRun))]
	public async Task Valid_Full_Workflow_Verify_All_Invocations_With_Specific_Parameters_Should_Exit_With_Success(CopyOptions copyOptions, bool verifyFolderAppendTypeMocks,
		bool verifyReverseGeocodeMocks)
	{
		var photoInfos = new List<Photo> { PhotoFakes.WithYear(2000) };
		var targetRelativeDirectory = string.Empty;
		const string photoPath = "/photo.jpg";
		string[] photoPaths = { "/photo.jpg" };
		var photoExifDataByFilePath = new Dictionary<string, ExifData> { { photoPath, ExifDataFakes.Create(DateTimeFakes.WithYear(2000), CoordinateFakes.Valid()) } };

		Dictionary<string, ExifData>? reverseGeocodedPhotoExifDataByFilePath = null;
		if (verifyReverseGeocodeMocks)
		{
			reverseGeocodedPhotoExifDataByFilePath = new Dictionary<string, ExifData>();
			foreach (var (filePathKey, exifData) in photoExifDataByFilePath)
				reverseGeocodedPhotoExifDataByFilePath.Add(filePathKey, ExifDataFakes.Create(exifData.TakenDate, exifData.Coordinate, ReverseGeocodeFakes.Valid()));
		}

		var groupedPhotoInfosByRelativeDirectory = new Dictionary<string, List<Photo>> { { targetRelativeDirectory, photoInfos } };
		SetupValid(copyOptions, photoPaths, true, true, photoExifDataByFilePath, photoInfos, targetRelativeDirectory, groupedPhotoInfosByRelativeDirectory,
			verifyFolderAppendTypeMocks, verifyReverseGeocodeMocks, reverseGeocodedPhotoExifDataByFilePath);

		var sut = Initialize(copyOptions);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.Success);
		VerifyValid(copyOptions, photoPaths, true, true, photoExifDataByFilePath, photoInfos, targetRelativeDirectory, groupedPhotoInfosByRelativeDirectory,
			verifyFolderAppendTypeMocks, verifyReverseGeocodeMocks, reverseGeocodedPhotoExifDataByFilePath);
	}

	private void SetupValid(CopyOptions copyOptions, string[] photoPaths, bool allPhotosHasPhotoTakenMockOutValue, bool allPhotosHasCoordinateMockOutValue,
		Dictionary<string, ExifData> photoExifDataByFilePath, IReadOnlyCollection<Photo> photoInfos, string targetRelativeDirectory,
		Dictionary<string, List<Photo>> groupedPhotoInfosByRelativeDirectory, bool setupOrganizeDirectoriesByFolderProcessType, bool setupReverseGeocodeFetcher,
		Dictionary<string, ExifData>? reverseGeocodedPhotoExifDataByFilePath)
	{
		_photoCollectorMock.Setup(s => s.Collect(copyOptions.InputPath!, It.IsAny<bool>())).Returns(() => photoPaths);
		_exifDataAppenderMock.Setup(s => s.ExifDataByPath(photoPaths, out allPhotosHasPhotoTakenMockOutValue, out allPhotosHasCoordinateMockOutValue)).Returns(() => photoExifDataByFilePath);

		if (setupReverseGeocodeFetcher)
		{
			_reverseGeocodeFetcherMock.Setup(s => s.Fetch(photoExifDataByFilePath))
				.Returns(() => Task.FromResult(reverseGeocodedPhotoExifDataByFilePath)!);

			_directoryGrouperMock.Setup(s => s.GroupFiles(reverseGeocodedPhotoExifDataByFilePath!, copyOptions.InputPath!,
					It.IsAny<FolderProcessType>(), It.IsAny<GroupByFolderType?>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Returns(() => groupedPhotoInfosByRelativeDirectory);

			_reverseGeocodeFetcherMock.Setup(s => s.RateLimitWarning());
		}
		else
		{
			_directoryGrouperMock.Setup(s => s.GroupFiles(photoExifDataByFilePath, copyOptions.InputPath!,
					It.IsAny<FolderProcessType>(), It.IsAny<GroupByFolderType?>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Returns(() => groupedPhotoInfosByRelativeDirectory);
		}

		_organizeByNoPhotoTakenActionMock.Setup(s => s.FilterAndSortByNoActionTypes(photoInfos, It.IsAny<CopyNoPhotoTakenDateAction>(), It.IsAny<CopyNoCoordinateAction>()))
			.Returns(() => (photoInfos, new List<Photo>()));

		_fileNamerMock.Setup(s => s.SetFileName(photoInfos, It.IsAny<NamingStyle>(), It.IsAny<NumberNamingTextStyle>()));

		if (setupOrganizeDirectoriesByFolderProcessType)
		{
			_organizeDirectoriesByFolderProcessTypeMock.Setup(s =>
				s.RenameByFolderAppendType(photoInfos, It.IsAny<FolderAppendType>(), It.IsAny<FolderAppendLocationType>(), targetRelativeDirectory));
		}

		_fileServiceMock.Setup(s => s.Copy(photoInfos, OutputPath, It.IsAny<bool>()));
		_csvServiceMock.Setup(s => s.Report(It.IsAny<Dictionary<string, IReadOnlyCollection<Photo>>>(), OutputPath, It.IsAny<bool>())).Returns(Task.CompletedTask);
	}

	private void VerifyValid(CopyOptions copyOptions, string[] photoPaths, bool allPhotosHasPhotoTakenMockOutValue, bool allPhotosHasCoordinateMockOutValue,
		Dictionary<string, ExifData> photoExifDataByFilePath, IReadOnlyCollection<Photo> photoInfos, string targetRelativeDirectory,
		Dictionary<string, List<Photo>> groupedPhotoInfosByRelativeDirectory, bool verifyOrganizeDirectoriesByFolderProcessType, bool verifyReverseGeocodeFetcher,
		Dictionary<string, ExifData>? reverseGeocodedPhotoExifDataByFilePath)
	{
		_photoCollectorMock.Verify(v => v.Collect(SourceFolderPath, It.IsAny<bool>()), Times.Once);
		_exifDataAppenderMock.Verify(v => v.ExifDataByPath(photoPaths, out allPhotosHasPhotoTakenMockOutValue, out allPhotosHasCoordinateMockOutValue), Times.Once);

		if (verifyReverseGeocodeFetcher)
		{
			_reverseGeocodeFetcherMock.Verify(v => v.Fetch(photoExifDataByFilePath), Times.Once);

			_directoryGrouperMock.Verify(v => v.GroupFiles(reverseGeocodedPhotoExifDataByFilePath!, copyOptions.InputPath!,
				It.IsAny<FolderProcessType>(), It.IsAny<GroupByFolderType?>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);

			_reverseGeocodeFetcherMock.Verify(v => v.RateLimitWarning(), Times.Once);
		}
		else
		{
			_directoryGrouperMock.Verify(v => v.GroupFiles(photoExifDataByFilePath, copyOptions.InputPath!,
				It.IsAny<FolderProcessType>(), It.IsAny<GroupByFolderType?>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
		}

		_organizeByNoPhotoTakenActionMock.Verify(v => v.FilterAndSortByNoActionTypes(photoInfos, It.IsAny<CopyNoPhotoTakenDateAction>(), It.IsAny<CopyNoCoordinateAction>()), Times.Once);
		_fileNamerMock.Verify(v => v.SetFileName(photoInfos, It.IsAny<NamingStyle>(), It.IsAny<NumberNamingTextStyle>()), Times.Once);

		if (verifyOrganizeDirectoriesByFolderProcessType)
		{
			_organizeDirectoriesByFolderProcessTypeMock.Verify(v =>
				v.RenameByFolderAppendType(photoInfos, It.IsAny<FolderAppendType>(), It.IsAny<FolderAppendLocationType>(), targetRelativeDirectory), Times.Once);
		}

		_fileServiceMock.Verify(v => v.Copy(photoInfos, OutputPath, It.IsAny<bool>()), Times.Once);
		_csvServiceMock.Verify(s => s.Report(It.IsAny<Dictionary<string, IReadOnlyCollection<Photo>>>(), OutputPath, It.IsAny<bool>()), Times.Once);
		VerifyNoOtherCalls();
	}

	#endregion

	#region Breaking Code Flow

	[Fact]
	public async Task NoPhoto_Returns_From_PhotoCollector_Should_Exits_With_NoPhotoFound()
	{
		PhotoCollectorSetupEmptyList();
		var sut = Initialize(CopyOptionsFakes.WithPaths(OutputPath, SourceFolderPath));
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.NoPhotoFoundOnDirectory);
		PhotoCollectorVerify();
		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Source_Folder_Path_Not_Exists_Should_Exits_With_InputFolderNotExists_Code()
	{
		var sut = Initialize(CopyOptionsFakes.WithPaths(OutputPath, SourceFolderPath), false);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.InputFolderNotExists);
		VerifyNoOtherCalls();
	}

	[Theory]
	[InlineData(true, true)]
	[InlineData(true, false)]
	[InlineData(false, true)]
	public async Task OutputFolderPath_Is_Not_Empty_Should_Halt_Process_And_Exit_With_OutputFolderIsNotEmpty_ExitCode(bool createDummyFile, bool createDummyFolder)
	{
		_fileSystemMock.AddDirectory(OutputPath);
		if (createDummyFile)
			_fileSystemMock.AddFile(Path.Combine(OutputPath, "file-test.txt"), "dummy content");
		if (createDummyFolder)
			_fileSystemMock.AddDirectory(Path.Combine(OutputPath, "directory-test"));
		var sut = Initialize(CopyOptionsFakes.WithPaths(OutputPath, SourceFolderPath));
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.OutputFolderIsNotEmpty);
		VerifyNoOtherCalls();
	}

	[Fact]
	public async Task When_Using_DryRun_With_An_Existing_File_On_OutputPath_Should_Exit_With_OutputPathIsExists()
	{
		_fileSystemMock.AddFile("photo-cli-dry-run.csv", "dummy content");
		var sut = Initialize(CopyOptionsFakes.Create(sourcePhotosFolderPathOptional: SourceFolderPath, isDryRunOptional: true));
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.OutputPathIsExists);
		VerifyNoOtherCalls();
	}

	#region NoExifData Prevent Actions

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task When_NoPhotoDateTimeTakenAction_PreventProcess_Out_Parameter_Of_AllPhotosHasPhotoTaken_As_False_Should_Exit_With_PhotosWithNoDatePreventedProcess(
		bool allPhotosHasCoordinateOutValue)
	{
		var options = CopyOptionsFakes.WithNoExifDataAction(SourceFolderPath, CopyNoPhotoTakenDateAction.PreventProcess, CopyNoCoordinateAction.DontCopyToOutput);
		await NoExifDataPreventActions(false, allPhotosHasCoordinateOutValue, options, ExitCode.PhotosWithNoDatePreventedProcess);
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task When_NoPhotoCoordinateAction_PreventProcess_Out_Parameter_Of_AllPhotosHasCoordinate_As_False_Should_Exit_With_PhotosWithNoCoordinatePreventedProcess(
		bool allPhotosHasPhotoTakenOutValue)
	{
		var options = CopyOptionsFakes.WithNoExifDataAction(SourceFolderPath, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.PreventProcess);
		await NoExifDataPreventActions(allPhotosHasPhotoTakenOutValue, false, options, ExitCode.PhotosWithNoCoordinatePreventedProcess);
	}

	[Fact]
	public async Task When_NoPhotoDateTimeAction_And_NoCoordinateAction_PreventProcess_Out_Parameter_Of_False_Should_Exit_With_PhotosWithNoCoordinateAndNoDatePreventedProcess()
	{
		var options = CopyOptionsFakes.WithNoExifDataAction(SourceFolderPath, CopyNoPhotoTakenDateAction.PreventProcess, CopyNoCoordinateAction.PreventProcess);
		await NoExifDataPreventActions(false, false, options, ExitCode.PhotosWithNoCoordinateAndNoDatePreventedProcess);
	}

	private async Task NoExifDataPreventActions(bool allPhotosHasPhotoTakenOutValue, bool allPhotosHasCoordinateOutValue, CopyOptions copyOptions, ExitCode expectedExitCode)
	{
		PhotoCollectorSetupNonEmptyList();
		_exifDataAppenderMock.Setup(s => s.ExifDataByPath(It.IsAny<string[]>(), out allPhotosHasPhotoTakenOutValue, out allPhotosHasCoordinateOutValue))
			.Returns(() => new Dictionary<string, ExifData> { { "/photo.jpg", ExifDataFakes.NoData() } });
		var sut = Initialize(copyOptions);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(expectedExitCode);
		PhotoCollectorVerify();
		_exifDataAppenderMock.Verify(v => v.ExifDataByPath(It.IsAny<string[]>(), out allPhotosHasPhotoTakenOutValue, out allPhotosHasCoordinateOutValue));
		VerifyNoOtherCalls();
	}

	#endregion

	#endregion

	#region File System

	[Fact]
	public async Task Not_Existed_Folder_Should_Be_Created()
	{
		var directoryBefore = _fileSystemMock.DirectoryInfo.FromDirectoryName(OutputPath);
		directoryBefore.Exists.Should().Be(false);
		PhotoCollectorSetupEmptyList();
		var sut = Initialize(CopyOptionsFakes.WithPaths(OutputPath, SourceFolderPath));
		await sut.Execute();
		var directoryAfter = _fileSystemMock.DirectoryInfo.FromDirectoryName(OutputPath);
		directoryAfter.Exists.Should().Be(true);
	}

	#endregion

	#region Shared

	private CopyRunner Initialize(CopyOptions options, bool createSourcePath = true)
	{
		if (createSourcePath)
			CreateSourcePathDirectory();

		return new CopyRunner(NullLogger<CopyRunner>.Instance, options, _photoCollectorMock.Object, _exifDataAppenderMock.Object, _directoryGrouperMock.Object, _fileNamerMock.Object,
			_fileServiceMock.Object, _fileSystemMock, _organizeByNoPhotoTakenActionMock.Object, _organizeDirectoriesByFolderProcessTypeMock.Object, _reverseGeocodeFetcherMock.Object,
			_csvServiceMock.Object, ToolOptionFakes.Create(), new Statistics(), ConsoleWriterFakes.Valid());
	}


	private void CreateSourcePathDirectory()
	{
		_fileSystemMock.AddDirectory(SourceFolderPath);
	}

	private void VerifyNoOtherCalls()
	{
		_photoCollectorMock.VerifyNoOtherCalls();
		_exifDataAppenderMock.VerifyNoOtherCalls();
		_fileNamerMock.VerifyNoOtherCalls();
		_directoryGrouperMock.VerifyNoOtherCalls();
		_fileServiceMock.VerifyNoOtherCalls();
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

	#endregion
}
