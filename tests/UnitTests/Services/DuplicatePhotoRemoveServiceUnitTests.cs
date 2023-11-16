using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.UnitTests.Services;

public class DuplicatePhotoRemoveServiceUnitTests
{
	public static TheoryData<List<Photo>, List<Photo>, int, string[]> AllDuplicate = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
				PhotoFakes.WithSha1Hash("file1-duplicate.jpg", Sha1HashFakes.Sample(1)),
			},
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
			},
			1,
			new []
			{
				"Photo is skipped due to same photo has already been archived. Same photo paths: /source-path/file1.jpg, /source-path/file1-duplicate.jpg",
			}
		}
	};

	public static TheoryData<List<Photo>, List<Photo>, int, string[]> ContainsUniqueAndSingleDuplicate = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
				PhotoFakes.WithSha1Hash("file2.jpg", Sha1HashFakes.Sample(2)),
				PhotoFakes.WithSha1Hash("file2-duplicate.jpg", Sha1HashFakes.Sample(2)),
			},
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
				PhotoFakes.WithSha1Hash("file2.jpg", Sha1HashFakes.Sample(2)),
			},
			1,
			new []
			{
				"Photo is skipped due to same photo has already been archived. Same photo paths: /source-path/file2.jpg, /source-path/file2-duplicate.jpg",
			}
		}
	};

	public static TheoryData<List<Photo>, List<Photo>, int, string[]> ContainsUniqueAndMultipleDuplicatesOccursManyTimes = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file2.jpg", Sha1HashFakes.Sample(2)),
				PhotoFakes.WithSha1Hash("file3.jpg", Sha1HashFakes.Sample(3)),
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
				PhotoFakes.WithSha1Hash("file2-duplicate.jpg", Sha1HashFakes.Sample(2)),
				PhotoFakes.WithSha1Hash("file3-duplicate1.jpg", Sha1HashFakes.Sample(3)),
				PhotoFakes.WithSha1Hash("file3-duplicate2.jpg", Sha1HashFakes.Sample(3)),
			},
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
				PhotoFakes.WithSha1Hash("file2.jpg", Sha1HashFakes.Sample(2)),
				PhotoFakes.WithSha1Hash("file3.jpg", Sha1HashFakes.Sample(3)),
			},
			3,
			new []
			{
				"Photo is skipped due to same photo has already been archived. Same photo paths: /source-path/file2.jpg, /source-path/file2-duplicate.jpg",
				"Photo is skipped due to same photo has already been archived. Same photo paths: /source-path/file3.jpg, /source-path/file3-duplicate1.jpg",
				"Photo is skipped due to same photo has already been archived. Same photo paths: /source-path/file3.jpg, /source-path/file3-duplicate2.jpg",
			}
		},
	};

	[Theory]
	[MemberData(nameof(AllDuplicate))]
	[MemberData(nameof(ContainsUniqueAndSingleDuplicate))]
	[MemberData(nameof(ContainsUniqueAndMultipleDuplicatesOccursManyTimes))]
	public void GroupAndFilterByPhotoHash_GivenPhotosThatContainsDuplicate_ShouldBeGroupedAndMatchWithExpected(List<Photo> photos, List<Photo> expectedPhotos,
		int expectedPhotosExistedStatistic, string[] logStatements)
	{
		var statistic = new Statistics();
		var loggerMock = new Mock<ILogger<DuplicatePhotoRemoveService>>();
		var sut = new DuplicatePhotoRemoveService(loggerMock.Object, statistic);
		var actualPhotos = sut.GroupAndFilterByPhotoHash(photos);
		actualPhotos.Should().BeEquivalentTo(expectedPhotos);
		statistic.PhotosSame.Should().Be(expectedPhotosExistedStatistic);
		loggerMock.VerifyAllLogStatementsAtLeastOnce(LogLevel.Warning, logStatements);
	}

	public static TheoryData<List<Photo>, List<Photo>> AllUniquePhotos = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
			},
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
			}
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
				PhotoFakes.WithSha1Hash("file2.jpg", Sha1HashFakes.Sample(2)),
			},
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
				PhotoFakes.WithSha1Hash("file2.jpg", Sha1HashFakes.Sample(2)),
			}
		},
		{
			new List<Photo>(),
			new List<Photo>()
		},
	};

	[Theory]
	[MemberData(nameof(AllUniquePhotos))]
	public void GroupAndFilterByPhotoHash_GivenUniquePhotos_ShouldBeGroupedAndMatchWithExpected(List<Photo> photos, List<Photo> expectedPhotos)
	{
		var statistic = new Statistics();
		var loggerMock = new Mock<ILogger<DuplicatePhotoRemoveService>>();
		var sut = new DuplicatePhotoRemoveService(loggerMock.Object, statistic);
		var actualPhotos = sut.GroupAndFilterByPhotoHash(photos);
		actualPhotos.Should().BeEquivalentTo(expectedPhotos);
		statistic.PhotosSame.Should().Be(0);
		loggerMock.VerifyNoOtherCalls();
	}
}
