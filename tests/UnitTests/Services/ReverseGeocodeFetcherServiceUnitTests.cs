using System.Diagnostics;

namespace PhotoCli.Tests.UnitTests.Services;

public class ReverseGeocodeFetcherServiceUnitTests
{
	#region Concurrent Fetch Semaphore Connection Limit

	public static TheoryData<ReverseGeocodeProvider, int, TimeSpan, Dictionary<string, ExifData>> FetchQueueIsSmallerThanConnectionLimit = new()
	{
		{
			ReverseGeocodeProvider.BigDataCloud,
			4,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(1)
		},
		{
			ReverseGeocodeProvider.GoogleMaps,
			4,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(3)
		},
		{
			ReverseGeocodeProvider.BigDataCloud,
			100,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(70)
		},
	};

	public static TheoryData<ReverseGeocodeProvider, int, TimeSpan, Dictionary<string, ExifData>> FetchQueueIsSameWithConnectionLimit = new()
	{
		{
			ReverseGeocodeProvider.BigDataCloud,
			4,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(4)
		},
		{
			ReverseGeocodeProvider.GoogleMaps,
			100,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(100)
		}
	};

	public static TheoryData<ReverseGeocodeProvider, int, TimeSpan, Dictionary<string, ExifData>> FetchQueueIsBiggerThanConnectionLimit = new()
	{
		{
			ReverseGeocodeProvider.BigDataCloud,
			4,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(5)
		},
		{
			ReverseGeocodeProvider.GoogleMaps,
			4,
			TimeSpan.FromMilliseconds(100),
			GenerateFakeExifDataByFilePaths(70)
		},
		{
			ReverseGeocodeProvider.BigDataCloud,
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
		int connectionLimit, TimeSpan fetchDuration, Dictionary<string, ExifData> sourceExifDataByFilePath)
	{
		var semaphoreMinimumCircuitCount = Math.Ceiling((float)sourceExifDataByFilePath.Count / connectionLimit);
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
		await sut.Fetch(sourceExifDataByFilePath);
		stopwatch.Stop();
		CheckElapsedTime(stopwatch, minimumFetchTime, sourceExifDataByFilePath.Count);
	}

	private void CheckElapsedTime(Stopwatch stopwatch, TimeSpan minimumFetchTime, int itemCount)
	{
		// other computation shouldn't take more 200 millisecond for each photo on average computer
		var maximumFetchTime = minimumFetchTime.Add(TimeSpan.FromMilliseconds(200) * itemCount);
		maximumFetchTime += TimeSpan.FromSeconds(2); // first TLS handshake can take long
		minimumFetchTime = minimumFetchTime.Subtract(TimeSpan.FromMilliseconds(100)); // performance tolerance
		stopwatch.Elapsed.Should().BeGreaterThan(minimumFetchTime);
		stopwatch.Elapsed.Should().BeLessThan(maximumFetchTime);
	}

	#endregion

	#region Rate Limit

	public static TheoryData<ReverseGeocodeProvider, TimeSpan, Dictionary<string, ExifData>> ObeyReverseGeocodeProvidersRateLimitData = new()
	{
		{
			ReverseGeocodeProvider.OpenStreetMapFoundation,
			TimeSpan.FromSeconds(1),
			GenerateFakeExifDataByFilePaths(3)
		},
		{
			ReverseGeocodeProvider.MapQuest,
			TimeSpan.FromSeconds(1),
			GenerateFakeExifDataByFilePaths(3)
		},
		{
			ReverseGeocodeProvider.LocationIq,
			TimeSpan.FromSeconds(1),
			GenerateFakeExifDataByFilePaths(3)
		}
	};

	[Theory]
	[MemberData(nameof(ObeyReverseGeocodeProvidersRateLimitData))]
	public async Task Check_Obeying_ReverseGeocode_Providers_Rate_Limit_By_Checking_Possible_Elapsed_Duration_Is_Between_Minimum_And_Maximum(ReverseGeocodeProvider reverseGeocodeProvider,
		TimeSpan reverseGeocodeServiceRateLimitBetweenEachRequest, Dictionary<string, ExifData> sourceExifDataByFilePath)
	{
		var minimumFetchTime = reverseGeocodeServiceRateLimitBetweenEachRequest * sourceExifDataByFilePath.Count;
		var reverseGeocodeMock = new Mock<IReverseGeocodeService>(MockBehavior.Strict);
		reverseGeocodeMock.Setup(s => s.Get(It.IsAny<Coordinate>())).ReturnsAsync((Coordinate coordinate) => ReverseGeocodeFakes.WithCoordinate(coordinate));
		var sut = new ReverseGeocodeFetcherService(reverseGeocodeMock.Object, CopyOptionsFakes.ValidReverseGeocodeService(reverseGeocodeProvider), ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(),
			ToolOptionsFakes.Valid(), NullLogger<ReverseGeocodeFetcherService>.Instance);
		var stopwatch = new Stopwatch();
		stopwatch.Start();
		await sut.Fetch(sourceExifDataByFilePath);
		stopwatch.Stop();
		CheckElapsedTime(stopwatch, minimumFetchTime, sourceExifDataByFilePath.Count);
	}

	public static TheoryData<ReverseGeocodeProvider, TimeSpan> IfUsingFreemiumRateLimitTimeSpanShouldBeMatchedData = new()
	{
		{ ReverseGeocodeProvider.OpenStreetMapFoundation, TimeSpan.FromSeconds(1) },
		{ ReverseGeocodeProvider.MapQuest, TimeSpan.FromSeconds(1) },
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
		ReverseGeocodeProvider.MapQuest, ReverseGeocodeProvider.LocationIq
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
		ReverseGeocodeProvider.BigDataCloud, ReverseGeocodeProvider.GoogleMaps
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

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, ExifData>> CoordinatesExifDataWithExpectedReverseGeocode = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ "/photo.jpeg", ExifDataFakes.WithCoordinate(0, 0) }
			},
			new Dictionary<string, ExifData>
			{
				{ "/photo.jpeg", ExifDataFakes.WithCoordinateAndReverseGeocode(0, 0, ReverseGeocodeFakes.WithCoordinate(0, 0)) }
			}
		},
		{
			new Dictionary<string, ExifData>
			{
				{ "/photo0.jpeg", ExifDataFakes.WithCoordinate(0, 0) },
				{ "/photo1.jpeg", ExifDataFakes.WithCoordinate(1, 1) }
			},
			new Dictionary<string, ExifData>
			{
				{ "/photo0.jpeg", ExifDataFakes.WithCoordinateAndReverseGeocode(0, 0, ReverseGeocodeFakes.WithCoordinate(0, 0)) },
				{ "/photo1.jpeg", ExifDataFakes.WithCoordinateAndReverseGeocode(1, 1, ReverseGeocodeFakes.WithCoordinate(1, 1)) }
			}
		},
		{
			new Dictionary<string, ExifData>
			{
				{ "/photo-no-coordinate.jpeg", ExifDataFakes.WithNoCoordinate() },
				{ "/photo1.jpeg", ExifDataFakes.WithCoordinate(1, 1) },
				{ "/photo0.jpeg", ExifDataFakes.WithCoordinate(0, 0) },
			},
			new Dictionary<string, ExifData>
			{
				{ "/photo1.jpeg", ExifDataFakes.WithCoordinateAndReverseGeocode(1, 1, ReverseGeocodeFakes.WithCoordinate(1, 1)) },
				{ "/photo-no-coordinate.jpeg", ExifDataFakes.WithNoCoordinate() },
				{ "/photo0.jpeg", ExifDataFakes.WithCoordinateAndReverseGeocode(0, 0, ReverseGeocodeFakes.WithCoordinate(0, 0)) },
			}
		}
	};

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, ExifData>> NoCoordinateShouldReturnWithNoReverseGeocodeSet = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ "/photo.jpeg", ExifDataFakes.WithNoCoordinate() }
			},
			new Dictionary<string, ExifData>
			{
				{ "/photo.jpeg", ExifDataFakes.WithNoCoordinate() }
			}
		}
	};

	[Theory]
	[MemberData(nameof(CoordinatesExifDataWithExpectedReverseGeocode))]
	[MemberData(nameof(NoCoordinateShouldReturnWithNoReverseGeocodeSet))]
	public async Task Given_Source_Dictionary_Should_Return_With_ReverseGeocode_Property_Set_On_Each_ExifData_Value(Dictionary<string, ExifData> sourceExifDataByFilePath,
		Dictionary<string, ExifData> expectedResultExifDataByFilePath)
	{
		var reverseGeocodeMock = new Mock<IReverseGeocodeService>(MockBehavior.Strict);
		reverseGeocodeMock.Setup(s => s.Get(It.IsAny<Coordinate>()))
			.ReturnsAsync((Coordinate coordinate) => ReverseGeocodeFakes.WithCoordinate(coordinate));
		var sut = new ReverseGeocodeFetcherService(reverseGeocodeMock.Object, CopyOptionsFakes.ValidReverseGeocodeService(), ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(),
			ToolOptionsFakes.Valid(), NullLogger<ReverseGeocodeFetcherService>.Instance);
		var actualExifDataByFilePath = await sut.Fetch(sourceExifDataByFilePath);
		actualExifDataByFilePath.Should().BeEquivalentTo(expectedResultExifDataByFilePath);
	}

	#endregion

	#region Utils

	private static Dictionary<string, ExifData> GenerateFakeExifDataByFilePaths(int fakeRecordCount)
	{
		var exifDataByFilePaths = new Dictionary<string, ExifData>();
		for (var i = 0; i < fakeRecordCount; i++)
			exifDataByFilePaths.Add($"/photo{i}.jpg", ExifDataFakes.WithCoordinate(i, i));
		return exifDataByFilePaths;
	}

	#endregion
}
