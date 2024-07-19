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
		var targetRelativeDirectory = string.Empty;
		var photos = new List<Photo> { PhotoFakes.Valid() };
		var groupedPhotoInfosByRelativeDirectory = new Dictionary<string, IReadOnlyCollection<Photo>> { { targetRelativeDirectory, photos } };

		Setup(copyOptions, photos, true, true, true, targetRelativeDirectory, groupedPhotoInfosByRelativeDirectory,
			verifyFolderAppendTypeMocks, verifyReverseGeocodeMocks, verifyFileIntegrity, true);

		var sut = Initialize(copyOptions);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.Success);
		var verifySaveGnuHashFileTree = verifyFileIntegrity;

		Verify(copyOptions, photos, true, true, true,
			targetRelativeDirectory, groupedPhotoInfosByRelativeDirectory, verifyFolderAppendTypeMocks, verifyReverseGeocodeMocks, verifyFileIntegrity, verifySaveGnuHashFileTree, true);
	}

	private void Setup(CopyOptions copyOptions, IReadOnlyCollection<Photo> photos, bool allPhotosAreValidMockOutValue, bool allPhotosHasPhotoTakenMockOutValue, bool allPhotosHasCoordinateMockOutValue, string targetRelativeDirectory,
		Dictionary<string, IReadOnlyCollection<Photo>> groupedPhotoInfosByRelativeDirectory, bool setupOrganizeDirectoriesByFolderProcessType, bool setupReverseGeocodeFetcher,
		bool setupVerifyFileIntegrity, bool verifyReport, bool verifyFileIntegrityResult = true)
	{
		_photoCollectorMock.Setup(s => s.Collect(copyOptions.InputPath!, It.IsAny<bool>(), It.IsAny<bool>())).Returns(() => photos);
		_exifDataAppenderMock.Setup(s => s.ExtractExifData(photos, out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenMockOutValue, out allPhotosHasCoordinateMockOutValue)).Returns(() => photos);

		if (setupReverseGeocodeFetcher)
		{
			_reverseGeocodeFetcherMock.Setup(s => s.Fetch(photos))
				.Returns(() => Task.FromResult(photos));

			_reverseGeocodeFetcherMock.Setup(s => s.RateLimitWarning());
		}

		_directoryGrouperMock.Setup(s => s.GroupFiles(photos, copyOptions.InputPath!,
				It.IsAny<FolderProcessType>(), It.IsAny<GroupByFolderType?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Returns(() => groupedPhotoInfosByRelativeDirectory);

		_organizeByNoPhotoTakenActionMock.Setup(s => s.FilterAndSortByNoActionTypes(photos,
				It.IsAny<CopyInvalidFormatAction>(), It.IsAny<CopyNoPhotoTakenDateAction>(), It.IsAny<CopyNoCoordinateAction>(), It.IsAny<string>()))
			.Returns(() => (photos, new List<Photo>()));

		_fileNamerMock.Setup(s => s.SetFileName(photos, It.IsAny<NamingStyle>(), It.IsAny<NumberNamingTextStyle>()))
			.Returns(photos);

		if (setupOrganizeDirectoriesByFolderProcessType)
		{
			_organizeDirectoriesByFolderProcessTypeMock.Setup(s =>
				s.RenameByFolderAppendType(photos, It.IsAny<FolderAppendType>(), It.IsAny<FolderAppendLocationType>(), targetRelativeDirectory))
				.Returns(photos);
		}

		_fileServiceMock.Setup(s => s.Copy(photos, OutputPath, It.IsAny<bool>())).Returns(photos);
		if (setupVerifyFileIntegrity)
		{
			_fileServiceMock.Setup(s => s.VerifyFileIntegrity(photos)).ReturnsAsync(verifyFileIntegrityResult);
			_fileServiceMock.Setup(s => s.SaveGnuHashFileTree(photos, OutputPath)).Returns(Task.CompletedTask);
		}

		if (verifyReport)
			_csvServiceMock.Setup(s => s.CreateCopyReport(It.IsAny<IEnumerable<Photo>>(), OutputPath, It.IsAny<bool>())).Returns(Task.CompletedTask);
	}

	private void Verify(CopyOptions copyOptions, IReadOnlyList<Photo> photos, bool allPhotosAreValidMockOutValue, bool allPhotosHasPhotoTakenMockOutValue, bool allPhotosHasCoordinateMockOutValue,
		string targetRelativeDirectory, Dictionary<string, IReadOnlyCollection<Photo>> groupedPhotoInfosByRelativeDirectory, bool verifyOrganizeDirectoriesByFolderProcessType, bool verifyReverseGeocodeFetcher, bool verifyVerifyFileIntegrity, bool verifySaveGnuHashFileTree, bool verifyReport)
	{
		_photoCollectorMock.Verify(v => v.Collect(SourceFolderPath, It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
		_exifDataAppenderMock.Verify(v => v.ExtractExifData(photos, out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenMockOutValue, out allPhotosHasCoordinateMockOutValue), Times.Once);

		if (verifyReverseGeocodeFetcher)
		{
			_reverseGeocodeFetcherMock.Verify(v => v.Fetch(photos), Times.Once);

			_reverseGeocodeFetcherMock.Verify(v => v.RateLimitWarning(), Times.Once);
		}

		_directoryGrouperMock.Verify(v => v.GroupFiles(photos, copyOptions.InputPath!,
			It.IsAny<FolderProcessType>(), It.IsAny<GroupByFolderType?>(),It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);

		var timesGroupPhotoFolderIteration = Times.Exactly(groupedPhotoInfosByRelativeDirectory.Count);

		_organizeByNoPhotoTakenActionMock.Verify(v => v.FilterAndSortByNoActionTypes(photos,
			It.IsAny<CopyInvalidFormatAction>(), It.IsAny<CopyNoPhotoTakenDateAction>(), It.IsAny<CopyNoCoordinateAction>(), It.IsAny<string>())
			, timesGroupPhotoFolderIteration);

		_fileNamerMock.Verify(v => v.SetFileName(photos, It.IsAny<NamingStyle>(), It.IsAny<NumberNamingTextStyle>()), timesGroupPhotoFolderIteration);

		if (verifyOrganizeDirectoriesByFolderProcessType)
		{
			_organizeDirectoriesByFolderProcessTypeMock.Verify(v =>
				v.RenameByFolderAppendType(photos, It.IsAny<FolderAppendType>(), It.IsAny<FolderAppendLocationType>(), targetRelativeDirectory), timesGroupPhotoFolderIteration);
		}

		_fileServiceMock.Verify(v => v.Copy(photos, OutputPath, It.IsAny<bool>()), timesGroupPhotoFolderIteration);

		if (verifyVerifyFileIntegrity)
			_fileServiceMock.Verify(s => s.VerifyFileIntegrity(photos), timesGroupPhotoFolderIteration);

		if(verifySaveGnuHashFileTree)
			_fileServiceMock.Verify(s => s.SaveGnuHashFileTree(photos, OutputPath), Times.Once);

		if(verifyReport)
			_csvServiceMock.Verify(s => s.CreateCopyReport(It.IsAny<IEnumerable<Photo>>(), OutputPath, It.IsAny<bool>()), Times.Once);

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
		var photos = new List<Photo> { PhotoFakes.Valid() };

		var groupedPhotoInfosByRelativeDirectory = new Dictionary<string, IReadOnlyCollection<Photo>> { { targetRelativeDirectory, photoInfos } };

		Setup(copyOptions, photos, true, true, true, targetRelativeDirectory, groupedPhotoInfosByRelativeDirectory,
			false, false, true, false, verifyFileIntegrityResult);

		var sut = Initialize(copyOptions);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.FileVerifyErrors);

		Verify(copyOptions, photos, true, true, true, targetRelativeDirectory, groupedPhotoInfosByRelativeDirectory,
			false, false, true, false, false);

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

		_exifDataAppenderMock.Setup(s => s
			.ExtractExifData(It.IsAny<IReadOnlyList<Photo>>(), out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenOutValue, out allPhotosHasCoordinateOutValue))
			.Returns(() => new[] { PhotoFakes.Valid() });

		var sut = Initialize(copyOptions);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(expectedExitCode);
		PhotoCollectorVerify();

		_exifDataAppenderMock.Verify(v => v
			.ExtractExifData(It.IsAny<IReadOnlyList<Photo>>(), out allPhotosAreValidMockOutValue, out allPhotosHasPhotoTakenOutValue, out allPhotosHasCoordinateOutValue), Times.Once);

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
		_photoCollectorMock.Setup(s => s
			.Collect(SourceFolderPath, It.IsAny<bool>(), It.IsAny<bool>()))
			.Returns(Array.Empty<Photo>);
	}

	private void PhotoCollectorSetupNonEmptyList()
	{
		_photoCollectorMock.Setup(s => s
			.Collect(SourceFolderPath, It.IsAny<bool>(), It.IsAny<bool>()))
			.Returns(() => new[] { PhotoFakes.Valid() });
	}

	private void PhotoCollectorVerify()
	{
		_photoCollectorMock.Verify(v => v
			.Collect(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
	}

	#endregion
}
