using System.Reflection;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.UnitTests.Services.ReverseGeocodes;

public class OpenStreetMapReverseGeocodeServiceUnitTests() : ReverseGeocodeServiceUnitTestBase<OpenStreetMapResponse>(ReverseGeocodeProvider.OpenStreetMapFoundation)
{
	public static TheoryData<List<string>, List<string>> WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData = new()
	{
		{ ["country"], ["Türkiye"] },
		{ ["city"], ["Ankara"] },
		{ ["country", "city"], ["Türkiye", "Ankara"] },
		{ ["country", "city", "town"], ["Türkiye", "Ankara", "Çankaya"] },
		{ ["country", "city", "town", "suburb"], ["Türkiye", "Ankara", "Çankaya", "Mebusevleri Mahallesi"] },
	};

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task OpenStreetMapNominatim_With_Given_Options_ReverseGeocode_Result_Should_Be_Given_Expected_Address_Data(List<string> properties, List<string> expectedAddresses)
	{
		var sut = MockServiceWithValidResponseNoCache(OpenStreetMapReverseGeocodeResponseFakes.Ankara());
		var reverseGeocodeResponseActual = await sut.Get(CoordinateFakes.Ankara(), PhotoFileValid, PrepareRequestedAddressProperties(properties));
		reverseGeocodeResponseActual.Should().BeEquivalentTo(new ReverseGeocodeAddressResult(expectedAddresses, true));
	}

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task LocationIq_With_Given_Options_ReverseGeocode_Result_Should_Be_Expected_Address_Data(List<string> properties, List<string> expectedAddresses)
	{
		var sut = MockServiceWithValidResponseNoCache(OpenStreetMapReverseGeocodeResponseFakes.Ankara());
		var reverseGeocodeResponseActual = await sut.Get(CoordinateFakes.Ankara(), PhotoFileValid, PrepareRequestedAddressProperties(properties));
		reverseGeocodeResponseActual.Should().BeEquivalentTo(new ReverseGeocodeAddressResult(expectedAddresses, true));
	}

	[Fact]
	public async Task LocationIq_Service_Error_Should_Give_Empty_List()
	{
		var coordinateCacheMock = new Mock<IReverseGeocodeCache<OpenStreetMapResponse>>();

		var sut = new LocationIqReverseGeocodeService(MockHttpClient.WithError(), NullLogger<LocationIqReverseGeocodeService>.Instance, ApiKeyStoreFakes.LocationIqValid(),
			coordinateCacheMock.Object, StatisticsFakes.Empty());

		var reverseGeocodeResponseActual = await sut.Get(CoordinateFakes.Ankara(), PhotoFileValid, OpenStreetMapAddressPropertiesFakes.ValidPropertyInfos());
		reverseGeocodeResponseActual.Should().BeEquivalentTo(new ReverseGeocodeAddressResult(ArraySegment<string>.Empty, false));
	}

	public static TheoryData<Dictionary<string, object>> ExpectedAvailableReverseGeocodes = new()
	{
		new Dictionary<string, object>
		{
			{ "CountryCode", "tr" },
			{ "Country", "Türkiye" },
			{ "Province", "Ankara" },
			{ "City", "Ankara" },
			{ "Town", "Çankaya" },
			{ "Postcode", "06580" },
			{ "Suburb", "Mebusevleri Mahallesi" },
			{ "Road", "Dumlupınar Yolu" },
			{ "Military", "Anıtkabir" },
		}
	};

	[Theory]
	[MemberData(nameof(ExpectedAvailableReverseGeocodes))]
	public async Task AllAvailableReverseGeocodes_Equivalent_To_Expected(Dictionary<string, object> expectedAllAvailableReverseGeocodes)
	{
		var sut = MockServiceWithValidResponseNoCache(OpenStreetMapReverseGeocodeResponseFakes.Ankara());
		var actualAllAvailableReverseGeocodes = await sut.AllAvailableReverseGeocodes(CoordinateFakes.Ankara());
		actualAllAvailableReverseGeocodes.Should().BeEquivalentTo(expectedAllAvailableReverseGeocodes);
	}

	public static TheoryData<Coordinate> CacheHitData = new()
	{
		new Coordinate(-89.1234567, -179.1234567),
		new Coordinate(89.1234567, 179.1234567),
	};

	[Theory]
	[MemberData(nameof(CacheHitData))]
	public async Task SerializeFullResponse_Should_Return_From_Cache(Coordinate coordinate)
	{
		var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
		var httpClientMock = new HttpClient(httpMessageHandlerMock.Object);

		var cacheResponseFakeExpected = OpenStreetMapFullResponseFakes.Valid(coordinate);
		var mockOpenStreetMapResponseCache = ReverseGeocodeCacheMock(coordinate, cacheResponseFakeExpected);

		var sut = new OpenStreetMapFoundationReverseGeocodeService(httpClientMock, NullLogger<OpenStreetMapFoundationReverseGeocodeService>.Instance,
			mockOpenStreetMapResponseCache, StatisticsFakes.Empty());

		var cacheResponseActual = await sut.SerializeFullResponse(new ReverseGeocodeRequest(coordinate));

		cacheResponseActual.Should().Be(cacheResponseFakeExpected);
		VerifyMockMessageHandlerSendRequestExactly(Times.Never(), httpMessageHandlerMock);
	}

	public static TheoryData<Coordinate, Coordinate[]> CacheMissData = new()
	{
		{
			new Coordinate(-89.1234567, -179.1234567),
			[
				new Coordinate(-89.1234568, -179.1234568)
			]
		},
		{
			new Coordinate(89.1234567, 179.1234567),
			[
				new Coordinate(89.1234566, 179.1234566),
				new Coordinate(45.1234566, 91.1234566)
			]
		},
	};

	[Theory]
	[MemberData(nameof(CacheMissData))]
	public async Task SerializeFullResponse_Should_Requested_From_HttpClient(Coordinate cacheCoordinate, Coordinate[] requestCoordinates)
	{
		var (httpMessageHandlerMock, mockHttpClient) = MockHttpMessageHandler.WithResponse(OpenStreetMapReverseGeocodeResponseFakes.Ankara());

		var mockOpenStreetMapResponseCache = ReverseGeocodeCacheMock(cacheCoordinate, OpenStreetMapFullResponseFakes.Valid(cacheCoordinate));

		var sut = new OpenStreetMapFoundationReverseGeocodeService(mockHttpClient, NullLogger<OpenStreetMapFoundationReverseGeocodeService>.Instance,
			mockOpenStreetMapResponseCache, StatisticsFakes.Empty());

		foreach (var requestCoordinate in requestCoordinates)
			_ = await sut.SerializeFullResponse(new ReverseGeocodeRequest(requestCoordinate));

		VerifyMockMessageHandlerSendRequestExactly(Times.Exactly(requestCoordinates.Length), httpMessageHandlerMock);
	}

	private OpenStreetMapFoundationReverseGeocodeService MockServiceWithValidResponseNoCache(string responseMock)
	{
		var httpClientMock = MockHttpClient.WithResponse(responseMock);

		return new OpenStreetMapFoundationReverseGeocodeService(httpClientMock, NullLogger<OpenStreetMapFoundationReverseGeocodeService>.Instance,
			ReverseGeocodeCacheAlwaysMiss(), StatisticsFakes.Empty());
	}

	private static readonly string NotExistingAddressPropertyOnFakeResponseBarracks = nameof(OpenStreetMapAddress.Barracks);
	private static readonly string NotExistingAddressPropertyOnFakeResponseRegion = nameof(OpenStreetMapAddress.Region);

	public static TheoryData<List<string>, List<string>, string[]> RequestedAddressTypesSomeMissingWithValuesAndExpectedLogs = new()
	{
		{
			[NotExistingAddressPropertyOnFakeResponseBarracks],
			[],
			[RequestedAddressTypesNotFoundLogStatement(NotExistingAddressPropertyOnFakeResponseBarracks, 1)]
		},
		{
			["country", NotExistingAddressPropertyOnFakeResponseRegion],
			["Türkiye"],
			[RequestedAddressTypesNotFoundLogStatement(NotExistingAddressPropertyOnFakeResponseRegion, 2)]
		},
		{
			[NotExistingAddressPropertyOnFakeResponseBarracks, "military"],
			["Anıtkabir"],
			[RequestedAddressTypesNotFoundLogStatement(NotExistingAddressPropertyOnFakeResponseBarracks, 1)]
		},
		{
			["country", "city", "town", NotExistingAddressPropertyOnFakeResponseBarracks, "suburb", NotExistingAddressPropertyOnFakeResponseRegion, "military"],
			["Türkiye", "Ankara", "Çankaya", "Mebusevleri Mahallesi", "Anıtkabir"],
			[
				RequestedAddressTypesNotFoundLogStatement(NotExistingAddressPropertyOnFakeResponseBarracks, 4),
				RequestedAddressTypesNotFoundLogStatement(NotExistingAddressPropertyOnFakeResponseRegion, 6),
			]
		},
	};

	[Theory]
	[MemberData(nameof(RequestedAddressTypesSomeMissingWithValuesAndExpectedLogs))]
	public async Task Get_SomeRequestedAddressLevelsMissing_ShouldReturnAllPhotosHasReverseGeocodedAsRequestedAsFalseWithMatchWithAddressAndLogs(List<string> properties,
		List<string> expectedAddresses, string[] expectedLogStatements)
	{
		await MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResultAndLogs(properties, expectedAddresses, false, expectedLogStatements);
	}


	private static List<PropertyInfo> PrepareRequestedAddressProperties(List<string> properties)
	{
		var requestedAddressProperties = new List<PropertyInfo>();
		var allProperties = typeof(OpenStreetMapAddress).GetProperties();
		foreach (var property in properties)
		{
			var propertyInfo = allProperties.SingleOrDefault(w => string.Equals(w.Name, property, StringComparison.InvariantCultureIgnoreCase));
			if (propertyInfo == null)
				throw new ArgumentException($"Property '{property}' does not exist in {nameof(OpenStreetMapAddress)}.");
			requestedAddressProperties.Add(propertyInfo);
		}
		return requestedAddressProperties;
	}

	private async Task MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResultAndLogs(List<string> properties, List<string> expectedAddresses,
		bool expectedAllPhotosHasReverseGeocodedAsRequested, string[]? expectedLogStatements = null)
	{
		var (sut, logger) = MockServiceAndLoggerWithValidResponseNoCache(OpenStreetMapReverseGeocodeResponseFakes.Ankara());
		var reverseGeocodeResponseActual = await sut.Get(CoordinateFakes.Ankara(), PhotoFileValid, PrepareRequestedAddressProperties(properties));

		using (new AssertionScope())
		{
			reverseGeocodeResponseActual.Should().BeEquivalentTo(new ReverseGeocodeAddressResult(expectedAddresses, expectedAllPhotosHasReverseGeocodedAsRequested));
			if (expectedLogStatements != null)
				logger.VerifyAllLogStatementsAtLeastOnce(LogLevel.Error, false, expectedLogStatements);
		}
	}

	private (OpenStreetMapFoundationReverseGeocodeService, Mock<ILogger<OpenStreetMapFoundationReverseGeocodeService>>) MockServiceAndLoggerWithValidResponseNoCache(string responseMock)
	{
		var httpClientMock = MockHttpClient.WithResponse(responseMock);
		var loggerMock = new Mock<ILogger<OpenStreetMapFoundationReverseGeocodeService>>();
		var reverseGeocodeService = new OpenStreetMapFoundationReverseGeocodeService(httpClientMock, loggerMock.Object, ReverseGeocodeCacheAlwaysMiss(), StatisticsFakes.Empty());
		return (reverseGeocodeService, loggerMock);
	}

	private static string RequestedAddressTypesNotFoundLogStatement(string property, int addressIndex)
	{
		return $"Requested address types: {property} on index #{addressIndex}, not found on OpenStreetMap's response. " +
			   $"Available types found: OpenStreetMapAddress {{ {OpenStreetMapReverseGeocodeResponseFakes.AnkaraAddressTypesLogOutput()} }}. " +
			   $"Path:<{PhotoPathValid}>";
	}
}
