using Microsoft.EntityFrameworkCore;

namespace PhotoCli.Tests.IntegrationTests.DbContext;

[Collection(XunitSharedCollectionsToDisableParallelExecution.EndToEndTests)]
public class DbServiceUnitTests
{
	private const string OutputPath = "output-folder";

	public static TheoryData<List<Photo>, List<PhotoEntity>> PhotoTakenDate = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("date"), ExifDataFakes.WithYear(2000), Sha1HashFakes.Sample(1)),
			},
			new List<PhotoEntity>
			{
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("date"), ExifDataFakes.WithYear(2000), Sha1HashFakes.Sample(1)),
			}
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("seconds"), ExifDataFakes.WithPhotoTakenDate(DateTimeFakes.WithSecond(1)), Sha1HashFakes.Sample(1)),
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("day"), ExifDataFakes.WithDay(1), Sha1HashFakes.Sample(2)),
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("year"), ExifDataFakes.WithYear(2001), Sha1HashFakes.Sample(3)),
			},
			new List<PhotoEntity>
			{
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("seconds"), ExifDataFakes.WithSeconds(1), Sha1HashFakes.Sample(1)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("day"), ExifDataFakes.WithDay(1), Sha1HashFakes.Sample(2)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("year"), ExifDataFakes.WithYear(2001), Sha1HashFakes.Sample(3)),
			}
		},
	};

	public static TheoryData<List<Photo>, List<PhotoEntity>> Coordinate = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("1"), ExifDataFakes.WithCoordinateSampleId(1), Sha1HashFakes.Sample(1)),
			},
			new List<PhotoEntity>
			{
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("1"), ExifDataFakes.WithCoordinateSampleId(1), Sha1HashFakes.Sample(1)),
			}
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("2"), ExifDataFakes.WithCoordinateSampleId(2), Sha1HashFakes.Sample(2)),
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("3"), ExifDataFakes.WithCoordinateSampleId(3), Sha1HashFakes.Sample(3)),
			},
			new List<PhotoEntity>
			{
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("2"), ExifDataFakes.WithCoordinateSampleId(2), Sha1HashFakes.Sample(2)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("3"), ExifDataFakes.WithCoordinateSampleId(3), Sha1HashFakes.Sample(3)),
			}
		},
	};

	public static TheoryData<List<Photo>, List<PhotoEntity>> CoordinateBoundaries = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("north-east-boundary"), ExifDataFakes.WithCoordinate(CoordinateFakes.NorthEastBoundary()), Sha1HashFakes.Sample(1)),
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("south-west-boundary"), ExifDataFakes.WithCoordinate(CoordinateFakes.SouthWestBoundary()), Sha1HashFakes.Sample(2)),
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("center"), ExifDataFakes.WithCoordinate(CoordinateFakes.Center()), Sha1HashFakes.Sample(3)),
			},
			new List<PhotoEntity>
			{
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("north-east-boundary"), ExifDataFakes.WithCoordinate(CoordinateFakes.NorthEastBoundary()), Sha1HashFakes.Sample(1)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("south-west-boundary"), ExifDataFakes.WithCoordinate(CoordinateFakes.SouthWestBoundary()), Sha1HashFakes.Sample(2)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("center"), ExifDataFakes.WithCoordinate(CoordinateFakes.Center()), Sha1HashFakes.Sample(3)),
			}
		},
	};

	public static TheoryData<List<Photo>, List<PhotoEntity>> CoordinateWithReverseGeocode = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("4"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(4, 4), Sha1HashFakes.Sample(4)),
			},
			new List<PhotoEntity>
			{
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("4"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(4, 4), Sha1HashFakes.Sample(4)),
			}
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("2"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(5, 5), Sha1HashFakes.Sample(5)),
				PhotoFakes.WithTargetPathsExifDataAndSha1Hash(OutputPath, FilePathWithJpg("3"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(6, 6), Sha1HashFakes.Sample(6)),
			},
			new List<PhotoEntity>
			{
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("2"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(5, 5), Sha1HashFakes.Sample(5)),
				PhotoEntityFakes.CreateWithExifData(OutputFilePathWithJpg("3"), ExifDataFakes.WithCoordinateAndReverseGeocodeSampleId(6, 6), Sha1HashFakes.Sample(6)),
			}
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

		var photos = new List<Photo>{ PhotoFakes.WithDay(1) };
		var actualAffectedRows = await sut.Archive(photos, true);

		actualAffectedRows.Should().Be(0);
		var savedPhotoEntities = await archiveDbContextProvider.CreateOrGetInstance().Photos.ToListAsync();
		savedPhotoEntities.Should().BeEmpty();
	}

	private static string OutputFilePathWithJpg(string fileName)
	{
		return MockFileSystemHelper.Path(true, OutputPath, $"{fileName}.jpg");
	}

	private static string FilePathWithJpg(string fileName)
	{
		return $"{fileName}.jpg";
	}
}
