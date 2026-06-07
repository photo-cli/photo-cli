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
	private readonly Mock<IConsoleWriter> _consoleWriterMock = new(MockBehavior.Loose);
	private readonly MockFileSystem _fileSystemMock = new();
	private const string OutputPath = "output-folder";
	private const string SourceFolderPath = "source-folder";
	private const string NewAlbumNameFake = "valid album name";
	private const int UpdateAlbumIdFake = 1;

	#region Expected Code Flow

	public static TheoryData<ArchiveOptions> WithoutReverseGeocodeOptions = new()
	{
		ArchiveOptionsFakes.WithoutReverseGeocode(OutputPath, SourceFolderPath),
	};

	public static TheoryData<ArchiveOptions> WithReverseGeocodeOptions = new()
	{
		ArchiveOptionsFakes.WithValidReverseGeocodeService(OutputPath, SourceFolderPath)
	};

	public static TheoryData<ArchiveOptions> WithDryRun = new()
	{
		ArchiveOptionsFakes.WithDryRun(OutputPath, SourceFolderPath),
	};

	public static TheoryData<ArchiveOptions> WithAlbumNew = new()
	{
		ArchiveOptionsFakes.WithAlbumNew(OutputPath, SourceFolderPath, "Individual Album to Create New", ArchiveAlbumType.Individual),
		ArchiveOptionsFakes.WithAlbumNew(OutputPath, SourceFolderPath, "Date Range Album to Create New", ArchiveAlbumType.DateRange),
	};

	public static TheoryData<ArchiveOptions> WithAlbumUpdate = new()
	{
		ArchiveOptionsFakes.WithAlbumUpdate(OutputPath, SourceFolderPath, 1, ArchiveAlbumType.Individual),
		ArchiveOptionsFakes.WithAlbumUpdate(OutputPath, SourceFolderPath, 2, ArchiveAlbumType.DateRange),
	};

	public static TheoryData<ArchiveOptions> WithAutoReverseGeocodeAlbum = new()
	{
		ArchiveOptionsFakes.WithAutoReverseGeocodeAlbum(OutputPath, SourceFolderPath),
		ArchiveOptionsFakes.WithAutoReverseGeocodeAndNewIndividualAlbum(OutputPath, SourceFolderPath, "Auto Reverse Geocode and New Individual Album", ArchiveAlbumType.Individual),
		ArchiveOptionsFakes.WithAutoReverseGeocodeAndNewIndividualAlbum(OutputPath, SourceFolderPath, "Auto Reverse Geocode and New Date Range Album", ArchiveAlbumType.DateRange),
		ArchiveOptionsFakes.WithAutoReverseGeocodeAndUpdateIndividualAlbum(OutputPath, SourceFolderPath, 1, ArchiveAlbumType.Individual),
		ArchiveOptionsFakes.WithAutoReverseGeocodeAndUpdateIndividualAlbum(OutputPath, SourceFolderPath, 2, ArchiveAlbumType.DateRange),
	};

	public static TheoryData<ArchiveOptions> WithDeleteSources = new()
	{
		ArchiveOptionsFakes.WithDeleteSource(OutputPath, SourceFolderPath),
	};

	[Theory]
	[MemberData(nameof(WithoutReverseGeocodeOptions))]
	[MemberData(nameof(WithReverseGeocodeOptions))]
	[MemberData(nameof(WithDryRun))]
	[MemberData(nameof(WithAlbumNew))]
	[MemberData(nameof(WithAlbumUpdate))]
	[MemberData(nameof(WithAutoReverseGeocodeAlbum))]
	[MemberData(nameof(WithDeleteSources))]
	public async Task Execute_ValidWorkflow_ShouldExitWithSuccessWithVerifyingAllMockedServices(ArchiveOptions options)
	{
		var (photos, photoEntities) = Setup(options);
		var sut = Initialize(options);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.Success);
		Verify(options, photos, photoEntities);
		VerifyNoOtherCalls();
	}

	#endregion

	#region Breaking Code Flow

	#region File System

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

	#endregion

	#region Date Range

	public static TheoryData<short, AlbumDateRange> ExpectedDayRangesWithExceedingDateRanges = new()
	{
		{ 1, AlbumDateRangeFakes.WithStartEnd(new DateTime(2000, 1, 1), new DateTime(2000, 1, 3))}, // expected 1 day, actual 2 day
		{ 50, AlbumDateRangeFakes.WithStartEnd(new DateTime(2001, 2, 1), new DateTime(2001, 3, 30))}, // expected 50 day, actual 57 day
		{ 365, AlbumDateRangeFakes.WithStartEnd(new DateTime(2002, 1, 1), new DateTime(2003, 3, 17))}, // expected 365 day, actual 440
	};

	[Theory]
	[MemberData(nameof(ExpectedDayRangesWithExceedingDateRanges))]
	public async Task Execute_WithAlbumRangeExceedTheExpectedDay_ShouldExitWithPhotosWithUnexpectedDateRangePreventedProcess(short expectedDayRange, AlbumDateRange albumDateRange)
	{
		var options = ArchiveOptionsFakes.WithExpectedDayRange(SourceFolderPath, expectedDayRange);
		var photos = new[] { PhotoFakes.Valid() };
		_photoCollectorMock.Setup(s => s.Collect(SourceFolderPath, It.IsAny<bool>(), It.IsAny<bool>())).Returns(() => photos);

		_exifDataAppenderMock.Setup(s => s
				.ExtractExifData(It.IsAny<IReadOnlyList<Photo>>()))
			.Returns(() => new ExifDataResult(photos, true, true, true, albumDateRange));

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.PhotosWithUnexpectedDateRangePreventedProcess);
		VerifyPhotoCollectorAndExifDataAppender();
	}

	public static TheoryData<ArchiveOptions> DateRangeAlbumOptions = new()
	{
		ArchiveOptionsFakes.WithAlbumNew(OutputPath, SourceFolderPath, "New Date Range Album", ArchiveAlbumType.DateRange),
		ArchiveOptionsFakes.WithAlbumUpdate(OutputPath, SourceFolderPath, 1, ArchiveAlbumType.DateRange),
	};

	[Theory]
	[MemberData(nameof(DateRangeAlbumOptions))]
	public async Task Execute_WithoutExtractedDateRangeOnAlbumRangeOptions_ShouldExitWithNoDataRangeFoundOnPhotos(ArchiveOptions dateRangeOptions)
	{
		var photos = new[] { PhotoFakes.Valid() };
		_photoCollectorMock.Setup(s => s.Collect(SourceFolderPath, It.IsAny<bool>(), It.IsAny<bool>())).Returns(() => photos);

		_exifDataAppenderMock.Setup(s => s
			.ExtractExifData(It.IsAny<IReadOnlyList<Photo>>()))
			.Returns(() => new ExifDataResult(photos, true, true, true, null));

		var sut = Initialize(dateRangeOptions);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.NoDataRangeFoundOnPhotos);
		VerifyPhotoCollectorAndExifDataAppender();
	}

	#endregion

	#region Database Inconsistencies

	[Theory]
	[InlineData(ArchiveAlbumType.Individual)]
	[InlineData(ArchiveAlbumType.DateRange)]
	public async Task Execute_WithNewAlbumNameExistsOnDb_ShouldExitWithAlbumNameMustBeUniqueWhileAddingOrUseUpdate(ArchiveAlbumType albumType)
	{
		const string existingAlbumWithSameName = "existing album with same name";
		var options = ArchiveOptionsFakes.WithAlbumNew(OutputPath, SourceFolderPath, existingAlbumWithSameName, albumType);
		SetupValidPhotoCollectorAndExifDataAppender();
		_dbServiceMock.Setup(s => s.GetAlbumByName(existingAlbumWithSameName)).ReturnsAsync(AlbumEntityFakes.Valid);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.AlbumNameMustBeUniqueWhileAddingOrUseUpdate);
		_dbServiceMock.Verify(v => v.GetAlbumByName(existingAlbumWithSameName), Times.Once);
		VerifyPhotoCollectorAndExifDataAppender();
	}

	[Theory]
	[InlineData(ArchiveAlbumType.Individual)]
	[InlineData(ArchiveAlbumType.DateRange)]
	public async Task Execute_WithUpdateAlbumByNotExistingAlbumIdOnDb_ShouldExitWithAlbumNameMustBeUniqueWhileAddingOrUseUpdate(ArchiveAlbumType albumType)
	{
		const short notExistingAlbumId = 1;
		var options = ArchiveOptionsFakes.WithAlbumUpdate(OutputPath, SourceFolderPath, notExistingAlbumId, albumType);
		SetupValidPhotoCollectorAndExifDataAppender();
		_dbServiceMock.Setup(s => s.GetAlbumById(notExistingAlbumId)).ReturnsAsync((AlbumEntity?)null);

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(ExitCode.AlbumNotFoundById);
		_dbServiceMock.Verify(v => v.GetAlbumById(notExistingAlbumId), Times.Once);
		VerifyPhotoCollectorAndExifDataAppender();
	}

	public static TheoryData<ArchiveOptions> ValidOptions = new()
	{
		ArchiveOptionsFakes.WithoutReverseGeocode(OutputPath, SourceFolderPath),
		ArchiveOptionsFakes.WithAlbumNew(OutputPath, SourceFolderPath, NewAlbumNameFake, ArchiveAlbumType.Individual),
		ArchiveOptionsFakes.WithAlbumUpdate(OutputPath, SourceFolderPath, 1, ArchiveAlbumType.Individual),
		ArchiveOptionsFakes.WithAutoReverseGeocodeAlbum(OutputPath, SourceFolderPath),
	};

	[Theory]
	[MemberData(nameof(ValidOptions))]
	public async Task Execute_ArchivingFailure_ShouldExitWithInconsistencyOnSavingPhotosToDatabase(ArchiveOptions options)
	{
		var (photos, photoEntities) = Setup(options, false);
		var sut = Initialize(options);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.InconsistencyOnSavingPhotosToDatabase);
		Verify(options, photos, photoEntities, false, false);
		VerifyNoOtherCalls();
	}

	[Theory]
	[InlineData(AlbumResult.AlbumExists, ExitCode.AlbumExist)]
	[InlineData(AlbumResult.NoPhotosToAddInAlbum, ExitCode.NoPhotosToAddInAlbum)]
	[InlineData(AlbumResult.DataInconsistency, ExitCode.InconsistencyOnSavingUserDefinedAlbumToDatabase)]
	public async Task Execute_NewIndividualAlbumSaveFailureByAlbumResult_ShouldExitMatchingExitCode(AlbumResult albumResult, ExitCode expectedExitCode)
	{
		var options = ArchiveOptionsFakes.WithAlbumNew(OutputPath, SourceFolderPath, NewAlbumNameFake, ArchiveAlbumType.Individual);
		var (photos, photoEntities) = Setup(options, newIndividualAlbumResult: albumResult);
		var sut = Initialize(options);
		var actualExitCode = await sut.Execute();
		actualExitCode.Should().Be(expectedExitCode);
		Verify(options, photos, photoEntities, true, false);
		VerifyNoOtherCalls();
	}

	[Theory]
	[InlineData(AlbumResult.AlbumExists, ExitCode.AlbumExist)]
	[InlineData(AlbumResult.DataInconsistency, ExitCode.InconsistencyOnSavingUserDefinedAlbumToDatabase)]
	public async Task Execute_NewDateRangeAlbumSaveFailureByAlbumResult_ShouldExitMatchingExitCode(AlbumResult albumResult, ExitCode expectedExitCode)
	{
		var options = ArchiveOptionsFakes.WithAlbumNew(OutputPath, SourceFolderPath, NewAlbumNameFake, ArchiveAlbumType.DateRange);
		var (photos, photoEntities) = Setup(options, newDateRangeAlbumResult: albumResult);
		var sut = Initialize(options);
		var actualExitCode = await sut.Execute();
		actualExitCode.Should().Be(expectedExitCode);
		Verify(options, photos, photoEntities, true, false);
		VerifyNoOtherCalls();
	}

	[Theory]
	[InlineData(AlbumResult.AlbumExists, ExitCode.AlbumExist)]
	[InlineData(AlbumResult.NoPhotosToAddInAlbum, ExitCode.NoPhotosToAddInAlbum)]
	[InlineData(AlbumResult.DataInconsistency, ExitCode.InconsistencyOnSavingUserDefinedAlbumToDatabase)]
	[InlineData(AlbumResult.AlbumNotFound, ExitCode.AlbumNotFoundById)]
	[InlineData(AlbumResult.ExistingConfigurationNotInCorrectFormat, ExitCode.ExistingAlbumConfigurationNotValid)]
	public async Task Execute_UpdateIndividualAlbumSaveFailureByAlbumResult_ShouldExitMatchingExitCode(AlbumResult albumResult, ExitCode expectedExitCode)
	{
		var options = ArchiveOptionsFakes.WithAlbumUpdate(OutputPath, SourceFolderPath, UpdateAlbumIdFake, ArchiveAlbumType.Individual);
		var (photos, photoEntities) = Setup(options, updateIndividualAlbumResult: albumResult);
		var sut = Initialize(options);
		var actualExitCode = await sut.Execute();
		actualExitCode.Should().Be(expectedExitCode);
		Verify(options, photos, photoEntities, true, false);
		VerifyNoOtherCalls();
	}

	[Theory]
	[InlineData(AlbumResult.AlbumExists, ExitCode.AlbumExist)]
	[InlineData(AlbumResult.DataInconsistency, ExitCode.InconsistencyOnSavingUserDefinedAlbumToDatabase)]
	[InlineData(AlbumResult.AlbumNotFound, ExitCode.AlbumNotFoundById)]
	[InlineData(AlbumResult.ExistingConfigurationNotInCorrectFormat, ExitCode.ExistingAlbumConfigurationNotValid)]
	public async Task Execute_UpdateDateRangeAlbumSaveFailureByAlbumResult_ShouldExitMatchingExitCode(AlbumResult albumResult, ExitCode expectedExitCode)
	{
		var options = ArchiveOptionsFakes.WithAlbumUpdate(OutputPath, SourceFolderPath, UpdateAlbumIdFake, ArchiveAlbumType.DateRange);
		var (photos, photoEntities) = Setup(options, updateDateRangeAlbumResult: albumResult);
		var sut = Initialize(options);
		var actualExitCode = await sut.Execute();
		actualExitCode.Should().Be(expectedExitCode);
		Verify(options, photos, photoEntities, true, false);
		VerifyNoOtherCalls();
	}

	[Theory]
	[InlineData(AlbumResult.DataInconsistency, ExitCode.InconsistencyOnSavingUserDefinedAlbumToDatabase)]
	public async Task Execute_SaveReverseGeocodeAlbumsFailureByAlbumResult_ShouldExitMatchingExitCode(AlbumResult albumResult, ExitCode expectedExitCode)
	{
		var options = ArchiveOptionsFakes.WithAutoReverseGeocodeAlbum(OutputPath, SourceFolderPath);
		var (photos, photoEntities) = Setup(options, saveReverseGeocodeAlbumsResult: albumResult);
		var sut = Initialize(options);
		var actualExitCode = await sut.Execute();
		actualExitCode.Should().Be(expectedExitCode);
		Verify(options, photos, photoEntities);
		VerifyNoOtherCalls();
	}

	#endregion

	#region Prevent Process Actions

	[Theory]
	[InlineData(ArchiveNoPhotoTakenDateAction.PreventProcess, true, ArchiveNoCoordinateAction.PreventProcess, true)]
	[InlineData(ArchiveNoPhotoTakenDateAction.PreventProcess, true, ArchiveNoCoordinateAction.PreventProcess, false)]
	[InlineData(ArchiveNoPhotoTakenDateAction.PreventProcess, false, ArchiveNoCoordinateAction.PreventProcess, true)]
	[InlineData(ArchiveNoPhotoTakenDateAction.PreventProcess, false, ArchiveNoCoordinateAction.PreventProcess, false)]
	public async Task When_InvalidFormatAction_PreventProcess_And_AllPhotosAreValid_Is_False_Runner_Should_Exit_With_PhotosWithInvalidFileFormatPreventedProcess(
		ArchiveNoPhotoTakenDateAction noPhotoTakenDateAction, bool allPhotosHasPhotoTaken, ArchiveNoCoordinateAction noCoordinateAction, bool allPhotosHasCoordinate)
	{
		var options = ArchiveOptionsFakes.WithPreventAction(SourceFolderPath, ArchiveInvalidFormatAction.PreventProcess, noPhotoTakenDateAction, noCoordinateAction);
		await CheckPreventActions(false, allPhotosHasPhotoTaken, allPhotosHasCoordinate, options, ExitCode.PhotosWithInvalidFileFormatPreventedProcess);
	}

	[Theory]
	[InlineData(ArchiveInvalidFormatAction.PreventProcess, true, ArchiveNoCoordinateAction.PreventProcess, true)]
	public async Task When_NoPhotoDateTimeTakenAction_PreventProcess_And_AllPhotosHasPhotoTaken_Is_False_Runner_Should_Exit_With_PhotosWithNoDatePreventedProcess(
		ArchiveInvalidFormatAction invalidFormatAction, bool allPhotosAreValid, ArchiveNoCoordinateAction noCoordinateAction, bool allPhotosHasCoordinate)
	{
		var options = ArchiveOptionsFakes.WithPreventAction(SourceFolderPath, invalidFormatAction, ArchiveNoPhotoTakenDateAction.PreventProcess, noCoordinateAction);
		await CheckPreventActions(allPhotosAreValid, false, allPhotosHasCoordinate, options, ExitCode.PhotosWithNoDatePreventedProcess);
	}

	[Theory]
	[InlineData(ArchiveInvalidFormatAction.PreventProcess, true, ArchiveNoPhotoTakenDateAction.PreventProcess, true)]
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

		_exifDataAppenderMock.Setup(s => s
			.ExtractExifData(It.IsAny<IReadOnlyList<Photo>>()))
			.Returns(() => new ExifDataResult(photos, allPhotosAreValidMockOutValue, allPhotosHasPhotoTakenOutValue, allPhotosHasCoordinateOutValue, null));

		var sut = Initialize(options);
		var exitCode = await sut.Execute();

		exitCode.Should().Be(expectedExitCode);
		VerifyPhotoCollectorAndExifDataAppender();
	}

	#endregion

	#endregion

	#region Shared

	private (List<Photo>, List<PhotoEntity>) Setup(ArchiveOptions options, bool archiveResultSuccess = true,
		AlbumResult newIndividualAlbumResult = AlbumResult.Successful, AlbumResult newDateRangeAlbumResult = AlbumResult.Successful,
		AlbumResult updateIndividualAlbumResult = AlbumResult.Successful, AlbumResult updateDateRangeAlbumResult = AlbumResult.Successful,
		AlbumResult saveReverseGeocodeAlbumsResult = AlbumResult.Successful)
	{
		var exifData = ExifDataFakes.WithYear(2000);
		var photos = new List<Photo> { PhotoFakes.WithExifData(exifData) };
		var photoEntities = new List<PhotoEntity> { PhotoEntityFakes.WithExifData(exifData) };
		const string targetRelativeDirectory = "2000/01/01";
		var groupedPhotoInfosByRelativeDirectory = new Dictionary<string, IReadOnlyCollection<Photo>> { { targetRelativeDirectory, photos } };

		_photoCollectorMock.Setup(s => s.Collect(options.InputPath!, It.IsAny<bool>(), It.IsAny<bool>())).Returns(() => photos);

		_exifDataAppenderMock.Setup(s => s
			.ExtractExifData(photos))
			.Returns(() => new ExifDataResult(photos, true, true, true, AlbumDateRangeFakes.Valid()));

		_fileServiceMock.Setup(s => s.CalculateFileHash(photos)).ReturnsAsync(photos);
		if (options.ReverseGeocodeProvider != ReverseGeocodeProvider.Disabled)
		{
			_reverseGeocodeFetcherMock.Setup(s => s.RateLimitWarning());

			_reverseGeocodeFetcherMock.Setup(s => s
				.Fetch(photos, It.IsAny<bool>()))
				.Returns(() => Task.FromResult(new ReverseGeocodeResult(photos, true)));
		}

		_directoryGrouperServiceMock.Setup(s => s.GroupFiles(photos, options.InputPath!,
				FolderProcessType.FlattenAllSubFolders, GroupByFolderType.YearMonthDay, true, true, false))
			.Returns(() => groupedPhotoInfosByRelativeDirectory);

		_duplicatePhotoRemoveServiceMock.Setup(s => s.GroupAndFilterByPhotoHash(photos)).Returns(() => photos);
		_fileNamerServiceMock.Setup(s => s.SetArchiveFileName(photos)).Returns(photos);
		_fileServiceMock.Setup(s => s.CopyIfNotExists(photos, options.OutputPath, options.IsDryRun)).Returns(photos);
		if (!options.IsDryRun)
			_fileServiceMock.Setup(s => s.VerifyFileIntegrity(photos)).ReturnsAsync(true);

		_dbServiceMock.Setup(s => s
			.Archive(photos, options.IsDryRun))
			.ReturnsAsync(new ArchiveResult(archiveResultSuccess, photoEntities));

		if (options.AlbumNameNew.IsPresent())
		{
			_dbServiceMock.Setup(s => s
				.GetAlbumByName(options.AlbumNameNew))
				.ReturnsAsync(() => null);

			switch (options.AlbumType)
			{
				case ArchiveAlbumType.Individual:
					_dbServiceMock.Setup(s => s
						.NewIndividualAlbum(options.AlbumNameNew, photoEntities, options.IsDryRun))
						.ReturnsAsync(newIndividualAlbumResult);
					break;
				case ArchiveAlbumType.DateRange:
					_dbServiceMock.Setup(s => s
						.NewDateRangeAlbum(options.AlbumNameNew, It.IsAny<AlbumDateRange>(), photoEntities, options.IsDryRun))
						.ReturnsAsync(newDateRangeAlbumResult);
					break;
			}
		}
		else if (options.AlbumIdUpdate != null)
		{
			var albumIdUpdate = options.AlbumIdUpdate.Value;
			_dbServiceMock.Setup(s => s
				.GetAlbumById(albumIdUpdate))
				.ReturnsAsync(AlbumEntityFakes.Valid);

			switch (options.AlbumType)
			{
				case ArchiveAlbumType.Individual:
					_dbServiceMock.Setup(s => s
						.UpdateIndividualAlbum(albumIdUpdate, photoEntities, options.IsDryRun))
						.ReturnsAsync(updateIndividualAlbumResult);
					break;
				case ArchiveAlbumType.DateRange:
					_dbServiceMock.Setup(s => s
						.UpdateDateRangeAlbum(albumIdUpdate, It.IsAny<AlbumDateRange>(), photoEntities, options.IsDryRun))
						.ReturnsAsync(updateDateRangeAlbumResult);
					break;
			}
		}

		if (options.AutoReverseGeocodeAlbum)
		{
			_dbServiceMock.Setup(s => s
				.SaveReverseGeocodeAlbums(photoEntities, options.IsDryRun))
				.ReturnsAsync(saveReverseGeocodeAlbumsResult);
		}

		if (options.DeleteSource)
			_fileServiceMock.Setup(s => s.DeletePhotoSources(photos, options.IsDryRun));

		return (photos, photoEntities);
	}

	private void Verify(ArchiveOptions options, IReadOnlyList<Photo> photos, List<PhotoEntity> photoEntities, bool verifySaveUserDefinedAlbumDbOperations = true,
		bool verifySaveReverseGeocodeAlbums = true)
	{
		_photoCollectorMock.Verify(s => s.Collect(options.InputPath!, It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
		_exifDataAppenderMock.Verify(s => s.ExtractExifData(photos), Times.Once);

		if (options.ReverseGeocodeProvider != ReverseGeocodeProvider.Disabled)
		{
			_reverseGeocodeFetcherMock.Verify(v => v.RateLimitWarning(), Times.Once);
			_reverseGeocodeFetcherMock.Verify(s => s.Fetch(photos, It.IsAny<bool>()), Times.Once);
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

		if (options.AlbumNameNew.IsPresent())
		{
			_dbServiceMock.Verify(v => v.GetAlbumByName(options.AlbumNameNew), Times.Once);

			if (verifySaveUserDefinedAlbumDbOperations)
			{
				switch (options.AlbumType)
				{
					case ArchiveAlbumType.Individual:
						_dbServiceMock.Verify(v => v.NewIndividualAlbum(options.AlbumNameNew, photoEntities, options.IsDryRun), Times.Once);
						break;
					case ArchiveAlbumType.DateRange:
						_dbServiceMock.Verify(v => v.NewDateRangeAlbum(options.AlbumNameNew, It.IsAny<AlbumDateRange>(), photoEntities, options.IsDryRun), Times.Once);
						break;
				}
			}
		}
		else if (options.AlbumIdUpdate != null)
		{
			var albumIdUpdate = options.AlbumIdUpdate.Value;
			_dbServiceMock.Verify(v => v.GetAlbumById(albumIdUpdate), Times.Once);

			if (verifySaveUserDefinedAlbumDbOperations)
			{
				switch (options.AlbumType)
				{
					case ArchiveAlbumType.Individual:
						_dbServiceMock.Verify(v => v.UpdateIndividualAlbum(albumIdUpdate, photoEntities, options.IsDryRun), Times.Once);
						break;
					case ArchiveAlbumType.DateRange:
						_dbServiceMock.Verify(v => v.UpdateDateRangeAlbum(albumIdUpdate, It.IsAny<AlbumDateRange>(), photoEntities, options.IsDryRun), Times.Once);
						break;
				}
			}
		}

		if (verifySaveReverseGeocodeAlbums && options.AutoReverseGeocodeAlbum)
			_dbServiceMock.Verify(v => v.SaveReverseGeocodeAlbums(photoEntities, options.IsDryRun), Times.Once);

		if (options.DeleteSource)
			_fileServiceMock.Verify(v => v.DeletePhotoSources(photos, options.IsDryRun));
	}

	private ArchiveRunner Initialize(ArchiveOptions options, bool createSourcePath = true)
	{
		if (createSourcePath)
			CreateSourcePathDirectory();

		return new ArchiveRunner(NullLogger<ArchiveRunner>.Instance, options, _photoCollectorMock.Object, _exifDataAppenderMock.Object,
			_directoryGrouperServiceMock.Object, _fileNamerServiceMock.Object, _fileServiceMock.Object, _fileSystemMock, new Statistics(), _reverseGeocodeFetcherMock.Object,
			_consoleWriterMock.Object, _duplicatePhotoRemoveServiceMock.Object, new ArchiveDatabaseOptions(options.OutputPath), _dbServiceMock.Object);
	}

	private void CreateSourcePathDirectory()
	{
		_fileSystemMock.AddDirectory(SourceFolderPath);
	}

	private void SetupValidPhotoCollectorAndExifDataAppender()
	{
		var photos = new[] { PhotoFakes.Valid() };
		_photoCollectorMock.Setup(s => s.Collect(SourceFolderPath, It.IsAny<bool>(), It.IsAny<bool>())).Returns(() => photos);

		_exifDataAppenderMock.Setup(s => s
			.ExtractExifData(It.IsAny<IReadOnlyList<Photo>>()))
			.Returns(() => new ExifDataResult(photos, true, true, true, AlbumDateRangeFakes.Valid()));
	}

	private void VerifyPhotoCollectorAndExifDataAppender()
	{
		_photoCollectorMock.Verify(v => v.Collect(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
		_exifDataAppenderMock.Verify(v => v.ExtractExifData(It.IsAny<IReadOnlyList<Photo>>()), Times.Once);
		VerifyNoOtherCalls();
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
