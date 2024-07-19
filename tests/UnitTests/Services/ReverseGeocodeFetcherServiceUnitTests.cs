using System.Diagnostics;

namespace PhotoCli.Tests.UnitTests.Services;

public class ReverseGeocodeFetcherServiceUnitTests
{
	#region Concurrent Fetch Semaphore Connection Limit

	public static TheoryData<ReverseGeocodeProvider, int, TimeSpan, IReadOnlyList<Photo>> FetchQueueIsSmallerThanConnectionLimit = new()
	{
		{
			ReverseGeocodeProviderFakes.NoWaitTime,
			4,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(1)
		},
		{
			ReverseGeocodeProviderFakes.NoWaitTime,
			4,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(3)
		},
		{
			ReverseGeocodeProviderFakes.NoWaitTime,
			100,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(70)
		},
	};

	public static TheoryData<ReverseGeocodeProvider, int, TimeSpan, IReadOnlyList<Photo>> FetchQueueIsSameWithConnectionLimit = new()
	{
		{
			ReverseGeocodeProviderFakes.NoWaitTime,
			4,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(4)
		},
		{
			ReverseGeocodeProviderFakes.NoWaitTime,
			100,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(100)
		}
	};

	public static TheoryData<ReverseGeocodeProvider, int, TimeSpan, IReadOnlyList<Photo>> FetchQueueIsBiggerThanConnectionLimit = new()
	{
		{
			ReverseGeocodeProviderFakes.NoWaitTime,
			4,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(5)
		},
		{
			ReverseGeocodeProviderFakes.NoWaitTime,
			4,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(70)
		},
		{
			ReverseGeocodeProviderFakes.NoWaitTime,
			100,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(270)
		},
	};

	[Theory]
	[MemberData(nameof(FetchQueueIsSmallerThanConnectionLimit))]
	[MemberData(nameof(FetchQueueIsSameWithConnectionLimit))]
	[MemberData(nameof(FetchQueueIsBiggerThanConnectionLimit))]
	public async Task Check_Concurrent_ReverseGeocode_Requests_Obeys_ConnectionLimit_By_Checking_Possible_Elapsed_Duration_Is_Between_Minimum_And_Maximum(ReverseGeocodeProvider reverseGeocodeProvider,
		int connectionLimit, TimeSpan fetchDuration, IReadOnlyList<Photo> photos)
	{
		var semaphoreMinimumCircuitCount = Math.Ceiling((float)photos.Count / connectionLimit);
		var minimumFetchTime = fetchDuration * semaphoreMinimumCircuitCount;
		var reverseGeocodeMock = new Mock<IReverseGeocodeService>(MockBehavior.Strict);
		reverseGeocodeMock.Setup(s => s.Get(It.IsAny<Coordinate>())).Returns(async (Coordinate coordinate) =>
		{
			await Task.Delay(fetchDuration);
			return ReverseGeocodeFakes.WithCoordinate(coordinate);
		});
		var toolOptions = ToolOptionsFakes.WithConnectionLimit(connectionLimit);
		var sut = new ReverseGeocodeFetcherService(reverseGeocodeMock.Object, CopyOptionsFakes.ValidReverseGeocodeService(reverseGeocodeProvider), ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(),
			toolOptions, NullLogger<ReverseGeocodeFetcherService>.Instance);
		var stopwatch = new Stopwatch();
		stopwatch.Start();
		await sut.Fetch(photos);
		stopwatch.Stop();
		CheckElapsedTime(stopwatch, minimumFetchTime, photos.Count);
	}

	private void CheckElapsedTime(Stopwatch stopwatch, TimeSpan minimumFetchTime, int itemCount)
	{
		// other computation shouldn't take more 1 second for each photo on average computer
		var maximumFetchTime = minimumFetchTime.Add(TimeSpan.FromSeconds(1) * itemCount);
		minimumFetchTime = minimumFetchTime.Subtract(TimeSpan.FromMilliseconds(100)); // performance tolerance
		stopwatch.Elapsed.Should().BeGreaterThan(minimumFetchTime);
		stopwatch.Elapsed.Should().BeLessThan(maximumFetchTime);
	}

	#endregion

	#region Rate Limit

	public static TheoryData<ReverseGeocodeProvider, TimeSpan, IReadOnlyList<Photo>> ObeyReverseGeocodeProvidersRateLimitData = new()
	{
		{
			ReverseGeocodeProvider.OpenStreetMapFoundation,
			TimeSpan.FromSeconds(1),
			GenerateFakeExifDataByFilePaths(3)!
		},
		{
			ReverseGeocodeProvider.LocationIq,
			TimeSpan.FromSeconds(1),
			GenerateFakeExifDataByFilePaths(3)!
		}
	};

	[Theory]
	[MemberData(nameof(ObeyReverseGeocodeProvidersRateLimitData))]
	public async Task Check_Obeying_ReverseGeocode_Providers_Rate_Limit_By_Checking_Possible_Elapsed_Duration_Is_Between_Minimum_And_Maximum(ReverseGeocodeProvider reverseGeocodeProvider,
		TimeSpan reverseGeocodeServiceRateLimitBetweenEachRequest, IReadOnlyList<Photo> photos)
	{
		var minimumFetchTime = reverseGeocodeServiceRateLimitBetweenEachRequest * photos.Count;
		var reverseGeocodeMock = new Mock<IReverseGeocodeService>(MockBehavior.Strict);
		reverseGeocodeMock.Setup(s => s.Get(It.IsAny<Coordinate>())).ReturnsAsync((Coordinate coordinate) => ReverseGeocodeFakes.WithCoordinate(coordinate));
		var sut = new ReverseGeocodeFetcherService(reverseGeocodeMock.Object, CopyOptionsFakes.ValidReverseGeocodeService(reverseGeocodeProvider), ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(),
			ToolOptionsFakes.Valid(), NullLogger<ReverseGeocodeFetcherService>.Instance);
		var stopwatch = new Stopwatch();
		stopwatch.Start();
		await sut.Fetch(photos);
		stopwatch.Stop();
		CheckElapsedTime(stopwatch, minimumFetchTime, photos.Count);
	}

	public static TheoryData<ReverseGeocodeProvider, TimeSpan> IfUsingFreemiumRateLimitTimeSpanShouldBeMatchedData = new()
	{
		{ ReverseGeocodeProvider.OpenStreetMapFoundation, TimeSpan.FromSeconds(1) },
		{ ReverseGeocodeProvider.LocationIq, TimeSpan.FromSeconds(1) },
	};

	[Theory]
	[MemberData(nameof(IfUsingFreemiumRateLimitTimeSpanShouldBeMatchedData))]
	public void If_Using_Freemium_Rate_Limit_TimeSpan_Should_Be_Matched(ReverseGeocodeProvider reverseGeocodeProvider, TimeSpan rateLimit)
	{
		var sut = new ReverseGeocodeFetcherService(null!, CopyOptionsFakes.ValidReverseGeocodeServiceWithLicense(false, reverseGeocodeProvider), ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(),
			ToolOptionsFakes.Valid(), NullLogger<ReverseGeocodeFetcherService>.Instance);
		var actualRateLimit = sut.RateLimit();
		actualRateLimit.Should().Be(rateLimit);
	}

	public static TheoryData<ReverseGeocodeProvider> IfHasPaidLicenseNoRateLimitData = new()
	{
		ReverseGeocodeProvider.LocationIq
	};

	[Theory]
	[MemberData(nameof(IfHasPaidLicenseNoRateLimitData))]
	public void CopyOptions_If_Has_Paid_License_No_Rate_Limit(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		RateLimitShouldBeNull(CopyOptionsFakes.ValidReverseGeocodeServiceWithLicense(true, reverseGeocodeProvider));
	}

	[Theory]
	[MemberData(nameof(IfHasPaidLicenseNoRateLimitData))]
	public void InfoOptions_If_Has_Paid_License_No_Rate_Limit(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		RateLimitShouldBeNull(InfoOptionsFakes.ValidReverseGeocodeServiceWithLicense(true, reverseGeocodeProvider));
	}

	private void RateLimitShouldBeNull(IReverseGeocodeOptions options)
	{
		var sut = new ReverseGeocodeFetcherService(null!, options, ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(), ToolOptionsFakes.Valid(), NullLogger<ReverseGeocodeFetcherService>.Instance);
		var actualRateLimit = sut.RateLimit();
		actualRateLimit.Should().BeNull();
	}

	public static TheoryData<ReverseGeocodeProvider> NoRateLimitForGivenReverseGeocodeProvidersData = new()
	{
		ReverseGeocodeProvider.GoogleMaps
	};

	[Theory]
	[MemberData(nameof(NoRateLimitForGivenReverseGeocodeProvidersData))]
	public void No_Rate_Limit_For_Given_ReverseGeocode_Providers(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		var sut = new ReverseGeocodeFetcherService(null!, CopyOptionsFakes.ValidReverseGeocodeService(reverseGeocodeProvider), ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(),
			ToolOptionsFakes.Valid(), NullLogger<ReverseGeocodeFetcherService>.Instance);
		var actualRateLimit = sut.RateLimit();
		actualRateLimit.Should().BeNull();
	}

	#endregion

	#region Setting Exif Data

	public static TheoryData<IReadOnlyList<Photo>, IReadOnlyList<Photo>> CoordinatesExifDataWithExpectedReverseGeocode = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithCoordinate(1,1),
			},
			new List<Photo>
			{
				PhotoFakes.WithCoordinateAndReverseGeocode(1,1, ReverseGeocodeFakes.WithCoordinate(1, 1)),
			}
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithCoordinate(0,0),
				PhotoFakes.WithCoordinate(1,1),
			},
			new List<Photo>
			{
				PhotoFakes.WithCoordinateAndReverseGeocode(0,0, ReverseGeocodeFakes.WithCoordinate(0, 0)),
				PhotoFakes.WithCoordinateAndReverseGeocode(1,1, ReverseGeocodeFakes.WithCoordinate(1, 1)),
			}
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithNoCoordinate(),
				PhotoFakes.WithCoordinate(0,0),
				PhotoFakes.WithCoordinate(1,1),
			},
			new List<Photo>
			{
				PhotoFakes.WithNoCoordinate(),
				PhotoFakes.WithCoordinateAndReverseGeocode(0,0, ReverseGeocodeFakes.WithCoordinate(0, 0)),
				PhotoFakes.WithCoordinateAndReverseGeocode(1,1, ReverseGeocodeFakes.WithCoordinate(1, 1)),
			}
		}
	};

	public static TheoryData<IReadOnlyList<Photo>, IReadOnlyList<Photo>> NoCoordinateShouldReturnWithNoReverseGeocodeSet = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithNoCoordinate(),
			},
			new List<Photo>
			{
				PhotoFakes.WithNoCoordinate(),
			}
		}
	};

	[Theory]
	[MemberData(nameof(CoordinatesExifDataWithExpectedReverseGeocode))]
	[MemberData(nameof(NoCoordinateShouldReturnWithNoReverseGeocodeSet))]
	public async Task Given_Source_Dictionary_Should_Return_With_ReverseGeocode_Property_Set_On_Each_ExifData_Value(IReadOnlyList<Photo> sourcePhotos,
		IReadOnlyList<Photo> photosExpectedResult)
	{
		var reverseGeocodeMock = new Mock<IReverseGeocodeService>(MockBehavior.Strict);
		reverseGeocodeMock.Setup(s => s.Get(It.IsAny<Coordinate>()))
			.ReturnsAsync((Coordinate coordinate) => ReverseGeocodeFakes.WithCoordinate(coordinate));
		var sut = new ReverseGeocodeFetcherService(reverseGeocodeMock.Object, CopyOptionsFakes.ValidReverseGeocodeService(), ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(),
			ToolOptionsFakes.Valid(), NullLogger<ReverseGeocodeFetcherService>.Instance);
		var actualExifDataByFilePath = await sut.Fetch(sourcePhotos);
		actualExifDataByFilePath.Should().BeEquivalentTo(photosExpectedResult);
	}

	#endregion

	#region Utils

	private static IReadOnlyList<Photo> GenerateFakeExifDataByFilePaths(int fakeRecordCount)
	{
		var photos = new List<Photo>();
		for (var i = 0; i < fakeRecordCount; i++)
			photos.Add(PhotoFakes.WithCoordinate(i, i));
		return photos;
	}

	#endregion
}
