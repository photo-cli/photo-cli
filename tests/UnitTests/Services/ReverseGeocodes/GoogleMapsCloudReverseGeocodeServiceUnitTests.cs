namespace PhotoCli.Tests.UnitTests.Services.ReverseGeocodes;

public class GoogleMapsCloudReverseGeocodeServiceUnitTests
{
	public static TheoryData<List<string>, List<string>> WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData = new()
	{
		{ new List<string> { "country" }, new List<string> { "Turkey" } },
		{ new List<string> { "administrative_area_level_1" }, new List<string> { "Ankara" } },
		{ new List<string> { "country", "administrative_area_level_1" }, new List<string> { "Turkey", "Ankara" } },
		{ new List<string> { "country", "administrative_area_level_1", "administrative_area_level_2" }, new List<string> { "Turkey", "Ankara", "Çankaya" } },
		{
			new List<string> { "country", "administrative_area_level_1", "administrative_area_level_2", "administrative_area_level_4" },
			new List<string> { "Turkey", "Ankara", "Çankaya", "Mebusevleri" }
		},
	};

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task With_Given_Options_ReverseGeocode_Result_Should_Be_Given_Expected_Address_Data(List<string> properties, List<string> expectedAddresses)
	{
		var sut = MockServiceWithValidResponse();
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), "en", properties);
		actualReverseGeocode.Should().BeEquivalentTo(expectedAddresses);
	}

	[Fact]
	public async Task Response_Serialization_Verify()
	{
		var sut = MockServiceWithValidResponse();
		var googleMapsResponse = await sut.SerializeFullResponse(CoordinateFakes.Ankara(), "tr");
		googleMapsResponse.Verify();
	}

	[Fact]
	public async Task Service_Error_Should_Give_Empty_List()
	{
		var mockHttpClient = MockHttpClient.WithError();
		var sut = new GoogleMapsReverseGeocodeService(mockHttpClient, ApiKeyStoreFakes.GoogleMapsValid(), NullLogger<GoogleMapsReverseGeocodeService>.Instance);
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), "en", GoogleMapsPropertiesFakes.Valid());
		actualReverseGeocode.Should().BeEquivalentTo(ArraySegment<string>.Empty);
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
		}
	};

	[Theory]
	[MemberData(nameof(ExpectedAvailableReverseGeocodes))]
	public async Task AllAvailableReverseGeocodes_Equivalent_To_Expected(Dictionary<string, object> expectedAllAvailableReverseGeocodes)
	{
		var sut = MockServiceWithValidResponse();
		var actualAllAvailableReverseGeocodes = await sut.AllAvailableReverseGeocodes(CoordinateFakes.Ankara(), "tr");
		actualAllAvailableReverseGeocodes.Should().BeEquivalentTo(expectedAllAvailableReverseGeocodes);
	}

	private GoogleMapsReverseGeocodeService MockServiceWithValidResponse()
	{
		var mockHttpClient = MockHttpClient.WithResponse(GoogleMapsReverseGeocodeResponseFakes.Ankara());
		return new GoogleMapsReverseGeocodeService(mockHttpClient, ApiKeyStoreFakes.GoogleMapsValid(), NullLogger<GoogleMapsReverseGeocodeService>.Instance);
	}
}
