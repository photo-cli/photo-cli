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

	public static TheoryData<CopyOptions, bool, bool, bool> WithoutFolderAppendTypeCopyOptionsAndCodeFlowOptions = new()
	{
		{ CopyOptionsFakes.WithoutFolderAppendType(OutputPath, SourceFolderPath), false, false, false },
	};

	public static TheoryData<CopyOptions, bool, bool, bool> WithFolderAppendTypeCopyOptionsAndCodeFlowOptions = new()
	{
		{ CopyOptionsFakes.WithFolderAppendType(OutputPath, SourceFolderPath), true, false, false },
	};

	public static TheoryData<CopyOptions, bool, bool, bool> WithReverseGeocodeCopyOptionsAndCodeFlowOptions = new()
	{
		{ CopyOptionsFakes.WithReverseGeocode(OutputPath, SourceFolderPath), false, true, false },
	};

	public static TheoryData<CopyOptions, bool, bool, bool> WithDryRun = new()
	{
		{ CopyOptionsFakes.WithDryRun(OutputPath, SourceFolderPath), false, false, false },
	};

	public static TheoryData<CopyOptions, bool, bool, bool> WithVerifyFileIntegrity = new()
	{
		{ CopyOptionsFakes.WithVerifyFileIntegrity(OutputPath, SourceFolderPath), false, false, true },
	};

	[Theory]
	[MemberData(nameof(WithoutFolderAppendTypeCopyOptionsAndCodeFlowOptions))]
	[MemberData(nameof(WithFolderAppendTypeCopyOptionsAndCodeFlowOptions))]
	[MemberData(nameof(WithReverseGeocodeCopyOptionsAndCodeFlowOptions))]
	[MemberData(nameof(WithDryRun))]
	[MemberData(nameof(WithVerifyFileIntegrity))]
	public async Task Valid_Full_Workflow_Verify_All_Invocations_With_Specific_Parameters_Should_Exit_With_Success(CopyOptions copyOptions, bool verifyFolderAppendTypeMocks,
		bool verifyReverseGeocodeMocks, bool verifyFileIntegrity)
	{
		var photoInfos = new List<Photo> { PhotoFakes.WithYear(2000) };
		var targetRelativeDirectory = string.Empty;
		const string photoPath = "/photo.jpg";
		string[] photoPaths = { "/photo.jpg" };
		var photoExifDataByFilePath = new Dictionary<string, ExifData?> { { photoPath, ExifDataFakes.Create(DateTimeFakes.WithYear(2000), CoordinateFakes.Valid()) } };

		Dictionary<string, ExifData?>? reverseGeocodedPhotoExifDataByFilePath = null;
		if (verifyReverseGeocodeMocks)
		{
			reverseGeocodedPhotoExifDataByFilePath = new Dictionary<string, ExifData?>();
			foreach (var (filePathKey, exifData) in photoExifDataByFilePath)
				reverseGeocodedPhotoExifDataByFilePath.Add(filePathKey, ExifDataFakes.Create(exifData!.TakenDate, exifData.Coordinate, ReverseGeocodeFakes.Valid()));
		}

		var groupedPhotoInfosByRelativeDirectory = new Dictionary<string, List<Photo>> { { targetRelativeDirectory, photoInfos } };
		Setup(copyOptions, photoPaths, true, true, true, photoExifDataByFilePath, photoInfos, targetRelativeDirectory, groupedPhotoInfosByRelativeDirectory,
			verifyFolderAppendTypeMocks, verifyReverseGeocodeMocks, reverseGeocodedPhotoExifDataByFilePath, verifyFileIntegrity, true);

		var sut = Initialize(copyOptions);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.Success);
		var verifySaveGnuHashFileTree = verifyFileIntegrity;
		Verify(copyOptions, photoPaths, true, true, true, photoExifDataByFilePath, photoInfos, targetRelativeDirectory, groupedPhotoInfosByRelativeDirectory,
			verifyFolderAppendTypeMocks, verifyReverseGeocodeMocks, reverseGeocodedPhotoExifDataByFilePath, verifyFileIntegrity, verifySaveGnuHashFileTree, true);
	}

	private void Setup(CopyOptions copyOptions, string[] photoPaths, bool allPhotosAreValidMockOutValue, bool allPhotosHasPhotoTakenMockOutValue, bool allPhotosHasCoordinateMockOutValue,
		Dictionary<string, ExifData?> photoExifDataByFilePath, IReadOnlyCollection<Photo> photoInfos, string targetRelativeDirectory,
		Dictionary<string, List<Photo>> groupedPhotoInfosByRelativeDirectory, bool setupOrganizeDirectoriesByFolderProcessType, bool setupReverseGeocodeFetcher,
		Dictionary<string, ExifData?>? reverseGeocodedPhotoExifDataByFilePath, bool setupVerifyFileIntegrity, bool verifyReport, bool verifyFileIntegrityResult = true)
	{
		_photoCollectorMock.Setup(s => s.Collect(copyOptions.InputPath!, It.IsAny<bool>())).Returns(() => photoPaths);
		_exifDataAppenderMock.Setup(s => s.ExifDataByPath(photoPaths, out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenMockOutValue, out allPhotosHasCoordinateMockOutValue)).Returns(() => photoExifDataByFilePath);

		if (setupReverseGeocodeFetcher)
		{
			_reverseGeocodeFetcherMock.Setup(s => s.Fetch(photoExifDataByFilePath))
				.Returns(() => Task.FromResult(reverseGeocodedPhotoExifDataByFilePath)!);

			_directoryGrouperMock.Setup(s => s.GroupFiles(reverseGeocodedPhotoExifDataByFilePath!, copyOptions.InputPath!,
					It.IsAny<FolderProcessType>(), It.IsAny<GroupByFolderType?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Returns(() => groupedPhotoInfosByRelativeDirectory);

			_reverseGeocodeFetcherMock.Setup(s => s.RateLimitWarning());
		}
		else
		{
			 _directoryGrouperMock.Setup(s => s.GroupFiles(photoExifDataByFilePath, copyOptions.InputPath!,
					It.IsAny<FolderProcessType>(), It.IsAny<GroupByFolderType?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Returns(() => groupedPhotoInfosByRelativeDirectory);
		}

		_organizeByNoPhotoTakenActionMock.Setup(s => s.FilterAndSortByNoActionTypes(photoInfos,
				It.IsAny<CopyInvalidFormatAction>(), It.IsAny<CopyNoPhotoTakenDateAction>(), It.IsAny<CopyNoCoordinateAction>(), It.IsAny<string>()))
			.Returns(() => (photoInfos, new List<Photo>()));

		_fileNamerMock.Setup(s => s.SetFileName(photoInfos, It.IsAny<NamingStyle>(), It.IsAny<NumberNamingTextStyle>()));

		if (setupOrganizeDirectoriesByFolderProcessType)
		{
			_organizeDirectoriesByFolderProcessTypeMock.Setup(s =>
				s.RenameByFolderAppendType(photoInfos, It.IsAny<FolderAppendType>(), It.IsAny<FolderAppendLocationType>(), targetRelativeDirectory));
		}

		_fileServiceMock.Setup(s => s.Copy(photoInfos, OutputPath, It.IsAny<bool>()));
		if (setupVerifyFileIntegrity)
		{
			_fileServiceMock.Setup(s => s.VerifyFileIntegrity(photoInfos, OutputPath)).ReturnsAsync(verifyFileIntegrityResult);
			_fileServiceMock.Setup(s => s.SaveGnuHashFileTree(photoInfos, OutputPath)).Returns(Task.CompletedTask);
		}


		if (verifyReport)
			_csvServiceMock.Setup(s => s.Report(It.IsAny<IEnumerable<Photo>>(), OutputPath, It.IsAny<bool>())).Returns(Task.CompletedTask);
	}

	private void Verify(CopyOptions copyOptions, string[] photoPaths, bool allPhotosAreValidMockOutValue, bool allPhotosHasPhotoTakenMockOutValue, bool allPhotosHasCoordinateMockOutValue,
		Dictionary<string, ExifData?> photoExifDataByFilePath, IReadOnlyCollection<Photo> photoInfos, string targetRelativeDirectory,
		Dictionary<string, List<Photo>> groupedPhotoInfosByRelativeDirectory, bool verifyOrganizeDirectoriesByFolderProcessType, bool verifyReverseGeocodeFetcher,
		Dictionary<string, ExifData?>? reverseGeocodedPhotoExifDataByFilePath, bool verifyVerifyFileIntegrity, bool verifySaveGnuHashFileTree, bool verifyReport)
	{
		_photoCollectorMock.Verify(v => v.Collect(SourceFolderPath, It.IsAny<bool>()), Times.Once);
		_exifDataAppenderMock.Verify(v => v.ExifDataByPath(photoPaths, out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenMockOutValue, out allPhotosHasCoordinateMockOutValue), Times.Once);

		if (verifyReverseGeocodeFetcher)
		{
			_reverseGeocodeFetcherMock.Verify(v => v.Fetch(photoExifDataByFilePath), Times.Once);

			_directoryGrouperMock.Verify(v => v.GroupFiles(reverseGeocodedPhotoExifDataByFilePath!, copyOptions.InputPath!,
				It.IsAny<FolderProcessType>(), It.IsAny<GroupByFolderType?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);

			_reverseGeocodeFetcherMock.Verify(v => v.RateLimitWarning(), Times.Once);
		}
		else
		{
			_directoryGrouperMock.Verify(v => v.GroupFiles(photoExifDataByFilePath, copyOptions.InputPath!,
				It.IsAny<FolderProcessType>(), It.IsAny<GroupByFolderType?>(),It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
		}

		var timesGroupPhotoFolderIteration = Times.Exactly(groupedPhotoInfosByRelativeDirectory.Count);

		_organizeByNoPhotoTakenActionMock.Verify(v => v.FilterAndSortByNoActionTypes(photoInfos,
			It.IsAny<CopyInvalidFormatAction>(), It.IsAny<CopyNoPhotoTakenDateAction>(), It.IsAny<CopyNoCoordinateAction>(), It.IsAny<string>())
			, timesGroupPhotoFolderIteration);

		_fileNamerMock.Verify(v => v.SetFileName(photoInfos, It.IsAny<NamingStyle>(), It.IsAny<NumberNamingTextStyle>()), timesGroupPhotoFolderIteration);

		if (verifyOrganizeDirectoriesByFolderProcessType)
		{
			_organizeDirectoriesByFolderProcessTypeMock.Verify(v =>
				v.RenameByFolderAppendType(photoInfos, It.IsAny<FolderAppendType>(), It.IsAny<FolderAppendLocationType>(), targetRelativeDirectory), timesGroupPhotoFolderIteration);
		}

		_fileServiceMock.Verify(v => v.Copy(photoInfos, OutputPath, It.IsAny<bool>()), timesGroupPhotoFolderIteration);
		if (verifyVerifyFileIntegrity)
			_fileServiceMock.Verify(s => s.VerifyFileIntegrity(photoInfos, OutputPath), timesGroupPhotoFolderIteration);

		if(verifySaveGnuHashFileTree)
			_fileServiceMock.Verify(s => s.SaveGnuHashFileTree(photoInfos, OutputPath), Times.Once);

		if(verifyReport)
			_csvServiceMock.Verify(s => s.Report(It.IsAny<IEnumerable<Photo>>(), OutputPath, It.IsAny<bool>()), Times.Once);

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

	[Fact]
	public async Task When_VerifyFileIntegrity_Fails_Should_Exit_With_FileVerifyErrors()
	{
		const bool verifyFileIntegrityResult = false;
		var copyOptions = CopyOptionsFakes.WithVerifyFileIntegrity(OutputPath, SourceFolderPath);
		var photoInfos = new List<Photo> { PhotoFakes.WithYear(2000) };
		var targetRelativeDirectory = string.Empty;
		const string photoPath = "/photo.jpg";
		string[] photoPaths = { "/photo.jpg" };
		var photoExifDataByFilePath = new Dictionary<string, ExifData?> { { photoPath, ExifDataFakes.Create(DateTimeFakes.WithYear(2000), CoordinateFakes.Valid()) } };

		Dictionary<string, ExifData?>? reverseGeocodedPhotoExifDataByFilePath = null;

		var groupedPhotoInfosByRelativeDirectory = new Dictionary<string, List<Photo>> { { targetRelativeDirectory, photoInfos } };

		Setup(copyOptions, photoPaths, true, true, true, photoExifDataByFilePath, photoInfos, targetRelativeDirectory, groupedPhotoInfosByRelativeDirectory,
			false, false, reverseGeocodedPhotoExifDataByFilePath, true, false, verifyFileIntegrityResult);

		var sut = Initialize(copyOptions);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.FileVerifyErrors);

		Verify(copyOptions, photoPaths, true, true, true, photoExifDataByFilePath, photoInfos, targetRelativeDirectory, groupedPhotoInfosByRelativeDirectory,
			false, false, reverseGeocodedPhotoExifDataByFilePath, true, false, false);

		VerifyNoOtherCalls();
	}

	#region Prevent Process Actions

	[Theory]
	[InlineData(CopyNoPhotoTakenDateAction.PreventProcess, true, CopyNoCoordinateAction.PreventProcess,true)]
	[InlineData(CopyNoPhotoTakenDateAction.PreventProcess, true, CopyNoCoordinateAction.PreventProcess,false)]
	[InlineData(CopyNoPhotoTakenDateAction.PreventProcess, false, CopyNoCoordinateAction.PreventProcess,true)]
	[InlineData(CopyNoPhotoTakenDateAction.PreventProcess, false, CopyNoCoordinateAction.PreventProcess,false)]
	public async Task When_InvalidFormatAction_PreventProcess_And_AllPhotosAreValid_Is_False_Runner_Should_Exit_With_PhotosWithInvalidFileFormatPreventedProcess(
		CopyNoPhotoTakenDateAction noPhotoTakenDateAction, bool allPhotosHasPhotoTaken, CopyNoCoordinateAction noCoordinateAction, bool allPhotosHasCoordinate)
	{
		var options = CopyOptionsFakes.WithPreventAction(SourceFolderPath, CopyInvalidFormatAction.PreventProcess, noPhotoTakenDateAction, noCoordinateAction);
		await CheckPreventActions(false, allPhotosHasPhotoTaken, allPhotosHasCoordinate, options, ExitCode.PhotosWithInvalidFileFormatPreventedProcess);
	}

	[Theory]
	[InlineData(CopyInvalidFormatAction.PreventProcess, true, CopyNoCoordinateAction.PreventProcess,true)]
	[InlineData(CopyInvalidFormatAction.PreventProcess, true, CopyNoCoordinateAction.DontCopyToOutput,true)]
	[InlineData(CopyInvalidFormatAction.PreventProcess, true, CopyNoCoordinateAction.DontCopyToOutput,false)]
	[InlineData(CopyInvalidFormatAction.DontCopyToOutput, false, CopyNoCoordinateAction.PreventProcess,true)]
	[InlineData(CopyInvalidFormatAction.DontCopyToOutput, false, CopyNoCoordinateAction.DontCopyToOutput,true)]
	public async Task When_NoPhotoDateTimeTakenAction_PreventProcess_And_AllPhotosHasPhotoTaken_Is_False_Runner_Should_Exit_With_PhotosWithNoDatePreventedProcess(
		CopyInvalidFormatAction invalidFormatAction, bool allPhotosAreValid, CopyNoCoordinateAction noCoordinateAction, bool allPhotosHasCoordinate)
	{
		var options = CopyOptionsFakes.WithPreventAction(SourceFolderPath, invalidFormatAction, CopyNoPhotoTakenDateAction.PreventProcess, noCoordinateAction);
		await CheckPreventActions(allPhotosAreValid, false, allPhotosHasCoordinate, options, ExitCode.PhotosWithNoDatePreventedProcess);
	}

	[Theory]
	[InlineData(CopyInvalidFormatAction.PreventProcess, true, CopyNoPhotoTakenDateAction.PreventProcess,true)]
	[InlineData(CopyInvalidFormatAction.PreventProcess, true, CopyNoPhotoTakenDateAction.DontCopyToOutput,true)]
	[InlineData(CopyInvalidFormatAction.PreventProcess, true, CopyNoPhotoTakenDateAction.DontCopyToOutput,false)]
	[InlineData(CopyInvalidFormatAction.DontCopyToOutput, false, CopyNoPhotoTakenDateAction.PreventProcess,true)]
	[InlineData(CopyInvalidFormatAction.DontCopyToOutput, false, CopyNoPhotoTakenDateAction.DontCopyToOutput,true)]
	public async Task When_NoPhotoCoordinateAction_PreventProcess_And_AllPhotosHasCoordinate_Is_False_Runner_Should_Exit_With_PhotosWithNoCoordinatePreventedProcess(
		CopyInvalidFormatAction invalidFormatAction, bool allPhotosAreValid, CopyNoPhotoTakenDateAction noPhotoTakenDateAction, bool allPhotosHasPhotoTaken)
	{
		var options = CopyOptionsFakes.WithPreventAction(SourceFolderPath, invalidFormatAction, noPhotoTakenDateAction, CopyNoCoordinateAction.PreventProcess);
		await CheckPreventActions(allPhotosAreValid, allPhotosHasPhotoTaken, false, options, ExitCode.PhotosWithNoCoordinatePreventedProcess);
	}

	[Theory]
	[InlineData(CopyInvalidFormatAction.PreventProcess, true)]
	[InlineData(CopyInvalidFormatAction.DontCopyToOutput, true)]
	public async Task When_NoPhotoDateTimeAction_And_NoCoordinateAction_PreventProcess_And_Both_AllPhotosHasPhotoTaken_AllPhotosHasCoordinate_Are_False_Runner_Should_Exit_With_PhotosWithNoCoordinateAndNoDatePreventedProcess(
		CopyInvalidFormatAction invalidFormatAction, bool allPhotosAreValid)
	{
		var options = CopyOptionsFakes.WithPreventAction(SourceFolderPath, invalidFormatAction, CopyNoPhotoTakenDateAction.PreventProcess, CopyNoCoordinateAction.PreventProcess);
		await CheckPreventActions(allPhotosAreValid, false, false, options, ExitCode.PhotosWithNoCoordinateAndNoDatePreventedProcess);
	}

	private async Task CheckPreventActions(bool allPhotosAreValidMockOutValue, bool allPhotosHasPhotoTakenOutValue, bool allPhotosHasCoordinateOutValue, CopyOptions copyOptions, ExitCode expectedExitCode)
	{
		PhotoCollectorSetupNonEmptyList();
		_exifDataAppenderMock.Setup(s => s.ExifDataByPath(It.IsAny<string[]>(), out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenOutValue, out allPhotosHasCoordinateOutValue))
			.Returns(() => new Dictionary<string, ExifData?> { { "/photo.jpg", ExifDataFakes.Valid() } });
		var sut = Initialize(copyOptions);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(expectedExitCode);
		PhotoCollectorVerify();
		_exifDataAppenderMock.Verify(v => v.ExifDataByPath(It.IsAny<string[]>(), out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenOutValue, out allPhotosHasCoordinateOutValue));
		VerifyNoOtherCalls();
	}

	#endregion

	#endregion

	#region File System

	[Fact]
	public async Task Not_Existed_Folder_Should_Be_Created()
	{
		var directoryBefore = _fileSystemMock.DirectoryInfo.New(OutputPath);
		directoryBefore.Exists.Should().Be(false);
		PhotoCollectorSetupEmptyList();
		var sut = Initialize(CopyOptionsFakes.WithPaths(OutputPath, SourceFolderPath));
		await sut.Execute();
		var directoryAfter = _fileSystemMock.DirectoryInfo.New(OutputPath);
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
