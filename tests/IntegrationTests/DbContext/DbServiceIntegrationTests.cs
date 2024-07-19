using Microsoft.EntityFrameworkCore;

namespace PhotoCli.Tests.IntegrationTests.DbContext;

[Collection(XunitSharedCollectionsToDisableParallelExecution.EndToEndTests)]
public class DbServiceUnitTests
{
	private const string OutputPath = "output-folder";

	public static TheoryData<List<Photo>, List<PhotoEntity>> PhotoTakenDate = new()
	{
		{
			[
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("date"), ExifDataFakes.WithYear(2000), Sha1HashFakes.Sample(1))
			],
			[
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("date"), ExifDataFakes.WithYear(2000), Sha1HashFakes.Sample(1))
			]
		},
		{
			[
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("seconds"), ExifDataFakes.WithPhotoTakenDate(DateTimeFakes.WithSecond(1)), Sha1HashFakes.Sample(1)),
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("day"), ExifDataFakes.WithDay(1), Sha1HashFakes.Sample(2)),
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("year"), ExifDataFakes.WithYear(2001), Sha1HashFakes.Sample(3))
			],
			[
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("seconds"), ExifDataFakes.WithSeconds(1), Sha1HashFakes.Sample(1)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("day"), ExifDataFakes.WithDay(1), Sha1HashFakes.Sample(2)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("year"), ExifDataFakes.WithYear(2001), Sha1HashFakes.Sample(3))
			]
		},
	};

	public static TheoryData<List<Photo>, List<PhotoEntity>> Coordinate = new()
	{
		{
			[
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("1"), ExifDataFakes.WithCoordinateSampleId(1), Sha1HashFakes.Sample(1))
			],
			[
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("1"), ExifDataFakes.WithCoordinateSampleId(1), Sha1HashFakes.Sample(1))
			]
		},
		{
			[
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("2"), ExifDataFakes.WithCoordinateSampleId(2), Sha1HashFakes.Sample(2)),
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("3"), ExifDataFakes.WithCoordinateSampleId(3), Sha1HashFakes.Sample(3))
			],
			[
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("2"), ExifDataFakes.WithCoordinateSampleId(2), Sha1HashFakes.Sample(2)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("3"), ExifDataFakes.WithCoordinateSampleId(3), Sha1HashFakes.Sample(3))
			]
		},
	};

	public static TheoryData<List<Photo>, List<PhotoEntity>> CoordinateBoundaries = new()
	{
		{
			[
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("north-east-boundary"), ExifDataFakes.WithCoordinate(CoordinateFakes.NorthEastBoundary()),
					Sha1HashFakes.Sample(1)),
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("south-west-boundary"), ExifDataFakes.WithCoordinate(CoordinateFakes.SouthWestBoundary()),
					Sha1HashFakes.Sample(2)),
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("center"), ExifDataFakes.WithCoordinate(CoordinateFakes.Center()), Sha1HashFakes.Sample(3))
			],
			[
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("north-east-boundary"), ExifDataFakes.WithCoordinate(CoordinateFakes.NorthEastBoundary()), Sha1HashFakes.Sample(1)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("south-west-boundary"), ExifDataFakes.WithCoordinate(CoordinateFakes.SouthWestBoundary()), Sha1HashFakes.Sample(2)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("center"), ExifDataFakes.WithCoordinate(CoordinateFakes.Center()), Sha1HashFakes.Sample(3))
			]
		},
	};

	public static TheoryData<List<Photo>, List<PhotoEntity>> CoordinateWithReverseGeocode = new()
	{
		{
			[
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("4"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(4, 4), Sha1HashFakes.Sample(4))
			],
			[
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("4"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(4, 4), Sha1HashFakes.Sample(4))
			]
		},
		{
			[
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("2"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(5, 5), Sha1HashFakes.Sample(5)),
				WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("3"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(6, 6), Sha1HashFakes.Sample(6))
			],
			[
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("2"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(5, 5), Sha1HashFakes.Sample(5)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("3"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(6, 6), Sha1HashFakes.Sample(6))
			]
		},
	};

	[Theory]
	[MemberData(nameof(PhotoTakenDate))]
	[MemberData(nameof(Coordinate))]
	[MemberData(nameof(CoordinateBoundaries))]
	[MemberData(nameof(CoordinateWithReverseGeocode))]
	public async Task Archive_GivenPhotosSavedInMemorySqLiteDb_ShouldMatchAllPropertiesMatched(List<Photo> photos, List<PhotoEntity> expectedPhotoEntities)
	{
		var archiveDbContextProvider = new ArchiveDbContextProvider(new InMemorySQLiteConnectionStringProvider(), NullLogger<ArchiveDbContextProvider>.Instance);
		var sut = new DbService(archiveDbContextProvider, NullLogger<DbService>.Instance);

		var actualAffectedRows = await sut.Archive(photos);

		actualAffectedRows.Should().Be(expectedPhotoEntities.Count);
		var savedPhotoEntities = await archiveDbContextProvider.CreateOrGetInstance().Photos.ToListAsync();

		savedPhotoEntities.Should().BeEquivalentTo(expectedPhotoEntities, c => c
			.Excluding(e => e.Id)
			.Excluding(e => e.CreatedAt));
	}

	[Fact]
	public async Task Archive_DryRun_ShouldNotPersistRecords()
	{
		var archiveDbContextProvider = new ArchiveDbContextProvider(new InMemorySQLiteConnectionStringProvider(), NullLogger<ArchiveDbContextProvider>.Instance);
		var sut = new DbService(archiveDbContextProvider, NullLogger<DbService>.Instance);

		var photos = new List<Photo>{ PhotoFakes.WithTargetRelativePathAndOutput("", OutputPath) };
		var actualAffectedRows = await sut.Archive(photos, true);

		actualAffectedRows.Should().Be(0);
		var savedPhotoEntities = await archiveDbContextProvider.CreateOrGetInstance().Photos.ToListAsync();
		savedPhotoEntities.Should().BeEmpty();
	}

	[Fact]
	public async Task Archive_PhotosWithoutTargetRelativePath_ShouldThrowPhotoCliException()
	{
		var (withoutTargetRelativePathPhoto, sourceFullPath) = PhotoFakes.SourceAndFileNameWithExtensionWithFullSourcePath("source-path", "photo-without-target-relative-path.jpg");
		var photos = new List<Photo>{ withoutTargetRelativePathPhoto };

		var archiveDbContextProvider = new ArchiveDbContextProvider(new InMemorySQLiteConnectionStringProvider(), NullLogger<ArchiveDbContextProvider>.Instance);
		var sut = new DbService(archiveDbContextProvider, NullLogger<DbService>.Instance);
		var photoCliException = await Assert.ThrowsAsync<PhotoCliException>(async () => await sut.Archive(photos));

		photoCliException.Message.Should().Be($"Can't archive, TargetRelativePath is missing for {sourceFullPath}");
	}

	private static Photo WithTargetPathsExifDataAndSha1Hash(string targetRelativeDirectoryPath, string fileNameWithExtension, ExifData exifData, string sha1Hash)
	{
		return PhotoFakes.CreateWithExifData(exifData, fileNameWithExtension, targetRelativeDirectoryPath, sha1Hash: sha1Hash, outputFolder: OutputPath);
	}

	private static string OutputFilePathWithJpg(string fileName)
	{
		return MockFileSystemHelper.Combine(true, OutputPath, $"{fileName}.jpg");
	}

	private static string FilePathWithJpg(string fileName)
	{
		return $"{fileName}.jpg";
	}
}
