using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace PhotoCli.Tests.EndToEndTests;

[Collection(XunitSharedCollectionsToDisableParallelExecution.EndToEndTests)]
public class ArchiveVerbEndToEndTests : BaseEndToEndTests
{
	public ArchiveVerbEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	#region Without Reverse Geocode

	public static TheoryData<ICollection<string>, List<PhotoEntity>, ConsoleOutputValues> SingleFolderWithoutReverseGeocoding = new()
	{
		{
			CommandLineArgumentsFakes.ArchiveBuildCommandLineOptionsWithoutOutputPath(TestImagesPathHelper.SingleFolder()),
			new List<PhotoEntity>
			{
				Kenya(),
				ItalyFlorence(),
				ItalyArezzo1(),
				ItalyArezzo2(),
				ItalyArezzo3(),
				ItalyArezzo4(),
				ItalyArezzo5(),
				ItalyArezzo6(),
				ItalyArezzo7(),
				ItalyArezzo8(),
				ItalyArezzo9(),
				UnitedKingdom(),
				Spain1(),
				Spain2(),
				NoPhotoTakenDate(),
				NoGpsCoordinate(),
			},
			new ConsoleOutputValues(18, 16, 15, 1, 2, 7)
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoEntity>, ConsoleOutputValues> SubFoldersWithoutReverseGeocoding = new()
	{
		{
			CommandLineArgumentsFakes.ArchiveBuildCommandLineOptionsWithoutOutputPath(TestImagesPathHelper.SubFolders()),
			new List<PhotoEntity>
			{
				Kenya(),
				ItalyFlorence(),
				ItalyArezzo1(),
				ItalyArezzo2(),
				ItalyArezzo3(),
				ItalyArezzo4(),
				ItalyArezzo5(),
				ItalyArezzo6(),
				ItalyArezzo7(),
				ItalyArezzo8(),
				ItalyArezzo9(),
				UnitedKingdom(),
				Spain1(),
				Spain2(),
				NoPhotoTakenDate(),
				NoGpsCoordinate(),
			},
			new ConsoleOutputValues(18, 16, 15, 1, 2, 7)
		}
	};

	[Theory]
	[MemberData(nameof(SingleFolderWithoutReverseGeocoding))]
	[MemberData(nameof(SubFoldersWithoutReverseGeocoding))]
	public async Task Run_WithoutReverseGeocodeGivingArguments_ShouldCreateAndVerifyPhotosOnFileSystem(ICollection<string> args, List<PhotoEntity> expectedPhotoEntities, ConsoleOutputValues expectedConsoleOutput)
	{
		var outputFolder = OutputFolderForE2ETestPrivateToEachTest();

		CommandLineArgumentsFakes.AddOutputPathOptions(outputFolder, args);
		var actualOutput = await RunMain(args);

		var actualConsoleOutput = ParseConsoleOutput(actualOutput);
		var actualPhotoEntities = await ReadPhotoEntitiesFromSqLite(outputFolder);

		using (new AssertionScope())
		{
			actualPhotoEntities.Should().BeEquivalentTo(expectedPhotoEntities, c => c
				.Excluding(e => e.Id)
				.Excluding(e => e.CreatedAt));

			actualConsoleOutput.Should().Be(expectedConsoleOutput);
		}

		DeleteOutput(outputFolder);
	}

	#endregion

	#region With ReverseGeocode

	public static TheoryData<ICollection<string>, List<PhotoEntity>, ConsoleOutputValues> SingleFolderWithReverseGeocoding = new()
	{
		{
			CommandLineArgumentsFakes.ArchiveBuildCommandLineOptionsWithoutOutputPath(TestImagesPathHelper.SingleFolder(),
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoEntity>
			{
				Kenya(),
				ItalyFlorence(),
				ItalyArezzo1(),
				ItalyArezzo2(),
				ItalyArezzo3(),
				ItalyArezzo4(),
				ItalyArezzo5(),
				ItalyArezzo6(),
				ItalyArezzo7(),
				ItalyArezzo8(),
				ItalyArezzo9(),
				UnitedKingdom(),
				Spain1(),
				Spain2(),
				NoPhotoTakenDate(),
				NoGpsCoordinate(),
			},
			new ConsoleOutputValues(18, 16, 15, 1, 2, 7)
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoEntity>, ConsoleOutputValues> SubFoldersWithReverseGeocoding = new()
	{
		{
			CommandLineArgumentsFakes.ArchiveBuildCommandLineOptionsWithoutOutputPath(TestImagesPathHelper.SubFolders(),
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoEntity>
			{
				Kenya(),
				ItalyFlorence(),
				ItalyArezzo1(),
				ItalyArezzo2(),
				ItalyArezzo3(),
				ItalyArezzo4(),
				ItalyArezzo5(),
				ItalyArezzo6(),
				ItalyArezzo7(),
				ItalyArezzo8(),
				ItalyArezzo9(),
				UnitedKingdom(),
				Spain1(),
				Spain2(),
				NoPhotoTakenDate(),
				NoGpsCoordinate(),
			},
			new ConsoleOutputValues(18, 16, 15, 1, 2, 7)
		}
	};

	[Theory]
	[MemberData(nameof(SingleFolderWithReverseGeocoding))]
	[MemberData(nameof(SubFoldersWithReverseGeocoding))]
	public async Task Run_WithReverseGeocodeGivingArguments_ShouldCreateAndVerifyPhotosOnFileSystem(ICollection<string> args, List<PhotoEntity> expectedPhotoEntities, ConsoleOutputValues expectedConsoleOutput)
	{
		var outputFolder = OutputFolderForE2ETestPrivateToEachTest();

		CommandLineArgumentsFakes.AddOutputPathOptions(outputFolder, args);
		var actualOutput = await RunMain(args);

		var actualConsoleOutput = ParseConsoleOutput(actualOutput);
		var actualPhotoEntities = await ReadPhotoEntitiesFromSqLite(outputFolder);

		using (new AssertionScope())
		{
			actualPhotoEntities.Should().BeEquivalentTo(expectedPhotoEntities, c => c
				.Excluding(e => e.Id).Excluding(e => e.CreatedAt).Excluding(e => e.ReverseGeocodeFormatted)
				.Excluding(e => e.Address1).Excluding(e => e.Address2).Excluding(e => e.Address3).Excluding(e => e.Address4)
				.Excluding(e => e.Address5).Excluding(e => e.Address6).Excluding(e => e.Address7).Excluding(e => e.Address8));

			foreach (var actualPhotoEntity in actualPhotoEntities)
			{
				if (actualPhotoEntity is { Latitude: not null, Longitude: not null })
				{
					actualPhotoEntity.ReverseGeocodeFormatted.Should().NotBeNullOrEmpty();
					actualPhotoEntity.Address1.Should().NotBeNullOrEmpty();
				}
				else
				{
					actualPhotoEntity.ReverseGeocodeFormatted.Should().BeNullOrEmpty();
					actualPhotoEntity.Address1.Should().BeNullOrEmpty();
				}
			}

			actualConsoleOutput.Should().Be(expectedConsoleOutput);
		}

		DeleteOutput(outputFolder);
	}

	#endregion

	#region No Photo Taken Date Actions & No Coordinate Action

	public static TheoryData<ICollection<string>, ExitCode, ConsoleOutputValues> NoPhotoTakenDateActionWithPreventProcessOption = new()
	{
		{
			CommandLineArgumentsFakes.ArchiveBuildCommandLineOptionsWithoutOutputPath(TestImagesPathHelper.SingleFolder(), noPhotoTakenDateAction: ArchiveNoPhotoTakenDateAction.PreventProcess),
			ExitCode.PhotosWithNoDatePreventedProcess,
			new ConsoleOutputValues(18)
		}
	};

	public static TheoryData<ICollection<string>, ExitCode, ConsoleOutputValues> NoCoordinateActionWithPreventProcessOption = new()
	{
		{
			CommandLineArgumentsFakes.ArchiveBuildCommandLineOptionsWithoutOutputPath(TestImagesPathHelper.SingleFolder(), noCoordinateAction: ArchiveNoCoordinateAction.PreventProcess),
			ExitCode.PhotosWithNoCoordinatePreventedProcess,
			new ConsoleOutputValues(18)
		}
	};

	public static TheoryData<ICollection<string>, ExitCode, ConsoleOutputValues> NoPhotoTakenDateAndNoCoordinateActionWithPreventProcessOption = new()
	{
		{
			CommandLineArgumentsFakes.ArchiveBuildCommandLineOptionsWithoutOutputPath(TestImagesPathHelper.SingleFolder(),
				noPhotoTakenDateAction: ArchiveNoPhotoTakenDateAction.PreventProcess, noCoordinateAction: ArchiveNoCoordinateAction.PreventProcess),
			ExitCode.PhotosWithNoCoordinateAndNoDatePreventedProcess,
			new ConsoleOutputValues(18)
		}
	};

	[Theory]
	[MemberData(nameof(NoPhotoTakenDateActionWithPreventProcessOption))]
	[MemberData(nameof(NoCoordinateActionWithPreventProcessOption))]
	[MemberData(nameof(NoPhotoTakenDateAndNoCoordinateActionWithPreventProcessOption))]
	public async Task Run_NoExifDataPreventActionsWithPreventOptions_ShouldExitWithSpecificExitCodeWithoutCreatingOutputFolder(ICollection<string> args, ExitCode expectedExitCode, ConsoleOutputValues
			expectedConsoleOutput)
	{
		var outputFolder = OutputFolderForE2ETestPrivateToEachTest();

		CommandLineArgumentsFakes.AddOutputPathOptions(outputFolder, args);
		var actualOutput = await RunMain(args, expectedExitCode);

		var actualConsoleOutput = ParseConsoleOutput(actualOutput);
		var outputFolderDryRunIsExist = Directory.Exists(outputFolder);

		using (new AssertionScope())
		{
			actualConsoleOutput.Should().Be(expectedConsoleOutput);
			outputFolderDryRunIsExist.Should().Be(false);
		}

		DeleteOutput(outputFolder);
	}

	#endregion

	#region Dry Run

	public static TheoryData<ICollection<string>, ConsoleOutputValues> DryRunSingleFolder = new()
	{
		{
			CommandLineArgumentsFakes.ArchiveBuildCommandLineOptionsWithoutOutputPath(TestImagesPathHelper.SingleFolder(), true),
			new ConsoleOutputValues(18, 16, 15, 1, 2, 7)
		}
	};

	public static TheoryData<ICollection<string>, ConsoleOutputValues> DryRunSubFolders = new()
	{
		{
			CommandLineArgumentsFakes.ArchiveBuildCommandLineOptionsWithoutOutputPath(TestImagesPathHelper.SubFolders(), true),
			new ConsoleOutputValues(18, 16, 15, 1, 2, 7)
		}
	};

	[Theory]
	[MemberData(nameof(DryRunSingleFolder))]
	[MemberData(nameof(DryRunSubFolders))]
	public async Task Run_DryRun_ShouldCreateAndVerifyPhotosOnFileSystem(ICollection<string> args, ConsoleOutputValues expectedConsoleOutput)
	{
		var outputFolder = OutputFolderForE2ETestPrivateToEachTest();

		CommandLineArgumentsFakes.AddOutputPathOptions(outputFolder, args);
		var actualOutput = await RunMain(args);

		var actualConsoleOutput = ParseConsoleOutput(actualOutput);
		var outputFolderDryRunIsExist = Directory.Exists(outputFolder);

		using (new AssertionScope())
		{
			actualConsoleOutput.Should().Be(expectedConsoleOutput);
			outputFolderDryRunIsExist.Should().Be(false);
		}
	}

	#endregion

	private static async Task<List<PhotoEntity>> ReadPhotoEntitiesFromSqLite(string outputPath)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ArchiveDbContext>();
		var sqliteFilePath = Path.Combine(outputPath, Constants.ArchiveSQLiteDatabaseFileName);
		optionsBuilder.UseSqlite($"Filename={sqliteFilePath}");
		List<PhotoEntity> photoEntities;
		await using (var dbContext = new ArchiveDbContext(optionsBuilder.Options))
		{
			photoEntities = await dbContext.Photos.ToListAsync();
		}
		SqliteConnection.ClearAllPools();
		return photoEntities;
	}

	#region Test Images

	private static PhotoEntity Kenya()
	{
		return CreatePhotoEntity($"2005/08/13/2005.08.13_09.47.23-{Sha1HashFakes.Kenya}.jpg", Sha1HashFakes.Kenya, ExifDataFakes.Kenya());
	}

	private static PhotoEntity ItalyFlorence()
	{
		return CreatePhotoEntity($"2005/12/14/2005.12.14_14.39.47-{Sha1HashFakes.ItalyFlorence}.jpg", Sha1HashFakes.ItalyFlorence, ExifDataFakes.ItalyFlorence());
	}

	private static PhotoEntity ItalyArezzo1()
	{
		return CreatePhotoEntity($"2008/10/22/2008.10.22_16.28.39-{Sha1HashFakes.ItalyArezzo1}.jpg", Sha1HashFakes.ItalyArezzo1, ExifDataFakes.ItalyArezzo1());
	}

	private static PhotoEntity ItalyArezzo2()
	{
		return CreatePhotoEntity($"2008/10/22/2008.10.22_16.29.49-{Sha1HashFakes.ItalyArezzo2}.jpg", Sha1HashFakes.ItalyArezzo2, ExifDataFakes.ItalyArezzo2());
	}

	private static PhotoEntity ItalyArezzo3()
	{
		return CreatePhotoEntity($"2008/10/22/2008.10.22_16.38.20-{Sha1HashFakes.ItalyArezzo3}.jpg", Sha1HashFakes.ItalyArezzo3, ExifDataFakes.ItalyArezzo3());
	}

	private static PhotoEntity ItalyArezzo4()
	{
		return CreatePhotoEntity($"2008/10/22/2008.10.22_16.43.21-{Sha1HashFakes.ItalyArezzo4}.jpg", Sha1HashFakes.ItalyArezzo4, ExifDataFakes.ItalyArezzo4());
	}

	private static PhotoEntity ItalyArezzo5()
	{
		return CreatePhotoEntity($"2008/10/22/2008.10.22_16.44.01-{Sha1HashFakes.ItalyArezzo5}.jpg", Sha1HashFakes.ItalyArezzo5, ExifDataFakes.ItalyArezzo5());
	}

	private static PhotoEntity ItalyArezzo6()
	{
		return CreatePhotoEntity($"2008/10/22/2008.10.22_16.46.53-{Sha1HashFakes.ItalyArezzo6}.jpg", Sha1HashFakes.ItalyArezzo6, ExifDataFakes.ItalyArezzo6());
	}

	private static PhotoEntity ItalyArezzo7()
	{
		return CreatePhotoEntity($"2008/10/22/2008.10.22_16.52.15-{Sha1HashFakes.ItalyArezzo7}.jpg", Sha1HashFakes.ItalyArezzo7, ExifDataFakes.ItalyArezzo7());
	}

	private static PhotoEntity ItalyArezzo8()
	{
		return CreatePhotoEntity($"2008/10/22/2008.10.22_16.55.37-{Sha1HashFakes.ItalyArezzo8}.jpg", Sha1HashFakes.ItalyArezzo8, ExifDataFakes.ItalyArezzo8());
	}

	private static PhotoEntity ItalyArezzo9()
	{
		return CreatePhotoEntity($"2008/10/22/2008.10.22_17.00.07-{Sha1HashFakes.ItalyArezzo9}.jpg", Sha1HashFakes.ItalyArezzo9, ExifDataFakes.ItalyArezzo9());
	}

	private static PhotoEntity UnitedKingdom()
	{
		return CreatePhotoEntity($"2012/06/22/2012.06.22_19.52.31-{Sha1HashFakes.UnitedKingdom}.jpg", Sha1HashFakes.UnitedKingdom, ExifDataFakes.UnitedKingdom());
	}

	private static PhotoEntity Spain1()
	{
		return CreatePhotoEntity($"2015/04/10/2015.04.10_20.12.23-{Sha1HashFakes.Spain1}.jpg", Sha1HashFakes.Spain1, ExifDataFakes.Spain1());
	}

	private static PhotoEntity Spain2()
	{
		return CreatePhotoEntity($"2015/04/10/2015.04.10_20.12.23-{Sha1HashFakes.Spain2}.jpg", Sha1HashFakes.Spain2, ExifDataFakes.Spain2());
	}

	private static PhotoEntity NoPhotoTakenDate()
	{
		return CreatePhotoEntity($"no-photo-taken-date/{Sha1HashFakes.NoPhotoTakenDate}.jpg", Sha1HashFakes.NoPhotoTakenDate);
	}

	private static PhotoEntity NoGpsCoordinate()
	{
		return CreatePhotoEntity($"2008/07/16/2008.07.16_11.33.20-{Sha1HashFakes.NoGpsCoordinate}.jpg", Sha1HashFakes.NoGpsCoordinate, ExifDataFakes.NoGpsCoordinate());
	}

	private static PhotoEntity CreatePhotoEntity(string path, string sha1Hash, ExifData? exifData = null)
	{
		return PhotoEntityFakes.CreateWithExifData(MockFileSystemHelper.Path(path, true), exifData, sha1Hash);
	}

	#endregion
}
