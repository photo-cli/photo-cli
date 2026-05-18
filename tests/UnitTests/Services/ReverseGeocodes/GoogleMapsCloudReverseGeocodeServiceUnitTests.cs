using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.UnitTests.Services.ReverseGeocodes;

public class GoogleMapsCloudReverseGeocodeServiceUnitTests() : ReverseGeocodeServiceUnitTestBase<GoogleMapsResponse>(ReverseGeocodeProvider.GoogleMaps)
{
	#region Matching AddressType

	public static TheoryData<List<string>, List<string>> RequestedAddressTypesAllFoundWithValues = new()
	{
		{ ["country"], ["Turkey"] },
		{ ["administrative_area_level_1"], ["Ankara"] },
		{ ["country", "administrative_area_level_1"], ["Turkey", "Ankara"] },
		{ ["country", "administrative_area_level_1", "administrative_area_level_2"], ["Turkey", "Ankara", "Çankaya"] },
		{
			["country", "administrative_area_level_1", "administrative_area_level_2", "administrative_area_level_4"],
			["Turkey", "Ankara", "Çankaya", "Mebusevleri"]
		},
	};

	[Theory]
	[MemberData(nameof(RequestedAddressTypesAllFoundWithValues))]
	public async Task Get_AllRequestedAddressTypesFound_ShouldMatchWithExpectedAddress(List<string> requestedAddressTypes, List<string> expectedAddresses)
	{
		await MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResult(requestedAddressTypes, expectedAddresses, true);
	}

	public static TheoryData<List<string>, List<string>, string[]> RequestedAddressTypesSomeMissingWithValuesAndExpectedLogs = new()
	{
		{
			["not_existing_address_type"],
			[],
			[RequestedAddressTypesNotFoundLogStatement("not_existing_address_type", 1)]
		},
		{
			["country", "not_existing_address_type"],
			["Turkey"],
			[RequestedAddressTypesNotFoundLogStatement("not_existing_address_type", 2)]
		},
		{
			["not_existing_address_type", "administrative_area_level_1"],
			["Ankara"],
			[RequestedAddressTypesNotFoundLogStatement("not_existing_address_type", 1)]
		},
		{
			["country", "administrative_area_level_1", "administrative_area_level_2", "not_existing_address_type_1", "administrative_area_level_4", "not_existing_address_type_2", "route"],
			["Turkey", "Ankara", "Çankaya", "Mebusevleri", "Anıtkabir"],
			[
				RequestedAddressTypesNotFoundLogStatement("not_existing_address_type_1", 4),
				RequestedAddressTypesNotFoundLogStatement("not_existing_address_type_2", 6),
			]
		},
	};

	[Theory]
	[MemberData(nameof(RequestedAddressTypesSomeMissingWithValuesAndExpectedLogs))]
	public async Task Get_SomeRequestedAddressTypesMissing_ShouldReturnAllPhotosHasReverseGeocodedAsRequestedAsFalseWithMatchWithAddressAndLogs(List<string> requestedAddressTypes,
		List<string> expectedAddresses, string[] expectedLogStatements)
	{
		await MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResultAndLogs(requestedAddressTypes, expectedAddresses, false, expectedLogStatements);
	}

	#endregion

	#region Fallback AddressType

	#region Fallback as All Types Are Found

	public static TheoryData<List<string>, List<string>> SingleRequestedAddressTypeWithFallbackTypesWithAllRequestedAddressTypesFound = new()
	{
		{
			["not_existing_address_type,country"],
			["Turkey"]
		},
		{
			["administrative_area_level_4,not_existing_address_type"],
			["Mebusevleri"]
		},
		{
			["not_existing_address_type_1,not_existing_address_type_2,administrative_area_level_1"],
			["Ankara"]
		},
		{
			["not_existing_address_type_1,administrative_area_level_2,not_existing_address_type_2"],
			["Çankaya"]
		},
	};

	public static TheoryData<List<string>, List<string>> MultipleRequestedAddressTypesWithFallbackTypesWithAllRequestedAddressTypesFound = new()
	{
		{
			["country", "not_existing_address_type,administrative_area_level_1"],
			["Turkey", "Ankara"]
		},
		{
			["administrative_area_level_1", "not_existing_address_type_1,not_existing_address_type_2,administrative_area_level_2", ",administrative_area_level_4"],
			["Ankara", "Çankaya", "Mebusevleri"]
		},
	};

	[Theory]
	[MemberData(nameof(SingleRequestedAddressTypeWithFallbackTypesWithAllRequestedAddressTypesFound))]
	[MemberData(nameof(MultipleRequestedAddressTypesWithFallbackTypesWithAllRequestedAddressTypesFound))]
	public async Task Get_RequestedAddressTypesWithFallbackTypesWithAllRequestedAddressTypesFound_ShouldWithExpectedReverseGeocodeAddressResult(List<string> requestedAddressTypes,
		List<string> expectedAddresses)
	{
		await MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResultAndLogs(requestedAddressTypes, expectedAddresses, true);
	}

	#endregion

	#region Fallback as Some Types Are Missing

	public static TheoryData<List<string>, List<string>, string[]> SingleRequestedAddressTypeWithFallbackTypesWithSomeRequestedAddressTypesMissing = new()
	{
		{
			["not_existing_address_type_1,not_existing_address_type_2"],
			[],
			[RequestedAddressTypesNotFoundLogStatement("not_existing_address_type_1,not_existing_address_type_2", 1)]
		},
		{
			["not_existing_address_type_1,not_existing_address_type_2,not_existing_address_type_3"],
			[],
			[RequestedAddressTypesNotFoundLogStatement("not_existing_address_type_1,not_existing_address_type_2,not_existing_address_type_3", 1)]
		},
	};

	public static TheoryData<List<string>, List<string>, string[]> MultipleRequestedAddressTypesWithFallbackTypesWithSomeRequestedAddressTypesMissing = new()
	{
		{
			["country", "not_existing_address_type_1,not_existing_address_type_2"],
			["Turkey"],
			[RequestedAddressTypesNotFoundLogStatement("not_existing_address_type_1,not_existing_address_type_2", 2)]
		},
		{
			["administrative_area_level_1", "not_existing_address_type_3,not_existing_address_type_4", ",administrative_area_level_2,not_existing_address_type_5", "not_existing_address_type_6"],
			["Ankara", "Çankaya"],
			[
				RequestedAddressTypesNotFoundLogStatement("not_existing_address_type_3,not_existing_address_type_4", 2),
				RequestedAddressTypesNotFoundLogStatement("not_existing_address_type_6", 4),
			]
		},
	};

	[Theory]
	[MemberData(nameof(SingleRequestedAddressTypeWithFallbackTypesWithSomeRequestedAddressTypesMissing))]
	[MemberData(nameof(MultipleRequestedAddressTypesWithFallbackTypesWithSomeRequestedAddressTypesMissing))]
	public async Task Get_RequestedAddressTypesWithFallbackTypesWithSomeRequestedAddressTypesMissing_ShouldWithExpectedReverseGeocodeAddressResult(List<string> requestedAddressTypes,
		List<string> expectedAddresses, string[] expectedLogStatements)
	{
		await MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResultAndLogs(requestedAddressTypes, expectedAddresses, false, expectedLogStatements);
	}

	#endregion

	#endregion

	[Fact]
	public async Task Response_Serialization_Verify()
	{
		var (sut, _) = MockServiceAndLoggerWithValidResponseNoCache(GoogleMapsReverseGeocodeResponseFakes.Ankara());
		var googleMapsRequest = new ReverseGeocodeRequest(CoordinateFakes.Ankara(), "tr");
		var googleMapsResponse = await sut.SerializeFullResponse(googleMapsRequest);
		googleMapsResponse.Verify();
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
		var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
		var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);

		var cacheResponseFakeExpected = GoogleMapsFullResponseFakes.Valid(coordinate);
		var mockGoogleMapsResponseCache = ReverseGeocodeCacheMock(coordinate, cacheResponseFakeExpected);

		var sut = new GoogleMapsReverseGeocodeService(mockHttpClient, ApiKeyStoreFakes.GoogleMapsValid(), NullLogger<GoogleMapsReverseGeocodeService>.Instance,
			mockGoogleMapsResponseCache, StatisticsFakes.Empty());

		var cacheResponseActual = await sut.SerializeFullResponse(new ReverseGeocodeRequest(coordinate));

		cacheResponseActual.Should().Be(cacheResponseFakeExpected);
		VerifyMockMessageHandlerSendRequestExactly(Times.Never(), mockHttpMessageHandler);
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
		var (httpMessageHandlerMock, httpClientMock) = MockHttpMessageHandler.WithResponse(GoogleMapsReverseGeocodeResponseFakes.Ankara());
		var mockGoogleMapsResponseCache = ReverseGeocodeCacheMock(cacheCoordinate, GoogleMapsFullResponseFakes.Valid(cacheCoordinate));

		var sut = new GoogleMapsReverseGeocodeService(httpClientMock, ApiKeyStoreFakes.GoogleMapsValid(), NullLogger<GoogleMapsReverseGeocodeService>.Instance,
			mockGoogleMapsResponseCache, StatisticsFakes.Empty());

		foreach (var requestCoordinate in requestCoordinates)
			_ = await sut.SerializeFullResponse(new ReverseGeocodeRequest(requestCoordinate));

		VerifyMockMessageHandlerSendRequestExactly(Times.Exactly(requestCoordinates.Length), httpMessageHandlerMock);
	}

	[Fact]
	public async Task Service_Error_Should_Give_Empty_List()
	{
		var mockHttpClient = MockHttpClient.WithError();
		var reverseGeocodeCacheAlwaysMissMock = ReverseGeocodeCacheAlwaysMiss();

		var sut = new GoogleMapsReverseGeocodeService(mockHttpClient, ApiKeyStoreFakes.GoogleMapsValid(), NullLogger<GoogleMapsReverseGeocodeService>.Instance,
			reverseGeocodeCacheAlwaysMissMock, StatisticsFakes.Empty());

		var reverseGeocodeResponseActual = await sut.Get(CoordinateFakes.Ankara(), PhotoFileValid, "en", GoogleMapsPropertiesFakes.Valid());
		reverseGeocodeResponseActual.Should().BeEquivalentTo(new ReverseGeocodeAddressResult(ArraySegment<string>.Empty, false));
	}

	public static TheoryData<Dictionary<string, object>> ExpectedAvailableReverseGeocodes = new()
	{
		new Dictionary<string, object>
		{
			{ "route", "Anıtkabir" },
			{ "administrative_area_level_4", "Mebusevleri" },
			{ "administrative_area_level_2", "Çankaya" },
			{ "administrative_area_level_1", "Ankara" },
			{ "country", "Turkey" },
			{ "postal_code", "06570" },
			{ "street_number", "108" },
			{ "plus_code", "WRGM+2W" },
			{ "locality", "Ankara" },
		}
	};

	[Theory]
	[MemberData(nameof(ExpectedAvailableReverseGeocodes))]
	public async Task AllAvailableReverseGeocodes_Equivalent_To_Expected(Dictionary<string, object> expectedAllAvailableReverseGeocodes)
	{
		var (sut, _) = MockServiceAndLoggerWithValidResponseNoCache(GoogleMapsReverseGeocodeResponseFakes.Ankara());
		var actualAllAvailableReverseGeocodes = await sut.AllAvailableReverseGeocodes(CoordinateFakes.Ankara(), "tr");
		actualAllAvailableReverseGeocodes.Should().BeEquivalentTo(expectedAllAvailableReverseGeocodes);
	}

	private (GoogleMapsReverseGeocodeService, Mock<ILogger<GoogleMapsReverseGeocodeService>>) MockServiceAndLoggerWithValidResponseNoCache(string responseMock)
	{
		var httpClientMock = MockHttpClient.WithResponse(responseMock);
		var loggerMock = new Mock<ILogger<GoogleMapsReverseGeocodeService>>();

		var reverseGeocodeService = new GoogleMapsReverseGeocodeService(httpClientMock, ApiKeyStoreFakes.GoogleMapsValid(),
			loggerMock.Object, ReverseGeocodeCacheAlwaysMiss(), StatisticsFakes.Empty());

		return (reverseGeocodeService, loggerMock);
	}

	private Task MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResult(List<string> requestedAddressTypes, List<string> expectedAddresses,
		bool expectedAllPhotosHasReverseGeocodedAsRequested)
	{
		return MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResultAndLogs(requestedAddressTypes, expectedAddresses, expectedAllPhotosHasReverseGeocodedAsRequested);
	}

	private async Task MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResultAndLogs(List<string> requestedAddressTypes,
		List<string> expectedAddresses, bool expectedAllPhotosHasReverseGeocodedAsRequested, string[]? expectedLogStatements = null)
	{
		var (sut, logger) = MockServiceAndLoggerWithValidResponseNoCache(GoogleMapsReverseGeocodeResponseFakes.Ankara());
		var reverseGeocodeResponseActual = await sut.Get(CoordinateFakes.Ankara(), PhotoFileValid, "en", requestedAddressTypes);

		using (new AssertionScope())
		{
			reverseGeocodeResponseActual.Should().BeEquivalentTo(new ReverseGeocodeAddressResult(expectedAddresses, expectedAllPhotosHasReverseGeocodedAsRequested));
			if (expectedLogStatements != null)
				logger.VerifyAllLogStatementsAtLeastOnce(LogLevel.Error, false, expectedLogStatements);
		}
	}

	private static string RequestedAddressTypesNotFoundLogStatement(string requestedAddressType, int addressIndex)
	{
		return $"Requested address types: {requestedAddressType} on index #{addressIndex}, not found on Google's response. Try adding fallback types like : type1,type2,type3. " +
			   $"Available types found: {GoogleMapsReverseGeocodeResponseFakes.AnkaraAvailableTypesLogOutput()}. " +
			   $"Path:<{PhotoPathValid}>";
	}
}
