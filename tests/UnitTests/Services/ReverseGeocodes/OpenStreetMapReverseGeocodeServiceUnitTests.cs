using System.Reflection;

namespace PhotoCli.Tests.UnitTests.Services.ReverseGeocodes;

public class OpenStreetMapReverseGeocodeServiceUnitTests
{
	public static TheoryData<List<string>, List<string>> WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData = new()
	{
		{ new List<string> { "country" }, new List<string> { "Türkiye" } },
		{ new List<string> { "city" }, new List<string> { "Ankara" } },
		{ new List<string> { "country", "city" }, new List<string> { "Türkiye", "Ankara" } },
		{ new List<string> { "country", "city", "town" }, new List<string> { "Türkiye", "Ankara", "Çankaya" } },
		{ new List<string> { "country", "city", "town", "suburb" }, new List<string> { "Türkiye", "Ankara", "Çankaya", "Mebusevleri Mahallesi" } },
	};

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task OpenStreetMapNominatim_With_Given_Options_ReverseGeocode_Result_Should_Be_Given_Expected_Address_Data(List<string> properties, List<string> expectedAddresses)
	{
		var sut = new OpenStreetMapFoundationReverseGeocodeService(CreateMockHttpClient(), NullLogger<OpenStreetMapFoundationReverseGeocodeService>.Instance);
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), PrepareRequestedAddressProperties(properties));
		actualReverseGeocode.Should().BeEquivalentTo(expectedAddresses);
	}

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task MapQuest_With_Given_Options_ReverseGeocode_Result_Should_Be_Given_Expected_Address_Data(List<string> properties, List<string> expectedAddresses)
	{
		var sut = new MapQuestReverseGeocodeService(CreateMockHttpClient(), NullLogger<MapQuestReverseGeocodeService>.Instance, ApiKeyStoreFakes.MapQuestValid());
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), PrepareRequestedAddressProperties(properties));
		actualReverseGeocode.Should().BeEquivalentTo(expectedAddresses);
	}

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task LocationIq_With_Given_Options_ReverseGeocode_Result_Should_Be_Expected_Address_Data(List<string> properties, List<string> expectedAddresses)
	{
		var sut = new LocationIqReverseGeocodeService(CreateMockHttpClient(), NullLogger<LocationIqReverseGeocodeService>.Instance, ApiKeyStoreFakes.LocationIqValid());
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), PrepareRequestedAddressProperties(properties));
		actualReverseGeocode.Should().BeEquivalentTo(expectedAddresses);
	}

	[Fact]
	public async Task MapQuest_Service_Error_Should_Give_Empty_List()
	{
		var sut = new MapQuestReverseGeocodeService(MockHttpClient.WithError(), NullLogger<MapQuestReverseGeocodeService>.Instance, ApiKeyStoreFakes.MapQuestValid());
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), OpenStreetMapAddressPropertiesFakes.Valid());
		actualReverseGeocode.Should().BeEquivalentTo(ArraySegment<string>.Empty);
	}

	[Fact]
	public async Task LocationIq_Service_Error_Should_Give_Empty_List()
	{
		var sut = new LocationIqReverseGeocodeService(MockHttpClient.WithError(), NullLogger<LocationIqReverseGeocodeService>.Instance, ApiKeyStoreFakes.LocationIqValid());
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), OpenStreetMapAddressPropertiesFakes.Valid());
		actualReverseGeocode.Should().BeEquivalentTo(ArraySegment<string>.Empty);
	}

	public static TheoryData<Dictionary<string, object>> ExpectedAvailableReverseGeocodes = new()
	{
		new Dictionary<string, object>
		{
			{ "CountryCode", "tr" },
			{ "Country", "Türkiye" },
			{ "Region", "İç Anadolu Bölgesi" },
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
		var sut = new OpenStreetMapFoundationReverseGeocodeService(CreateMockHttpClient(), NullLogger<OpenStreetMapFoundationReverseGeocodeService>.Instance);
		var actualAllAvailableReverseGeocodes = await sut.AllAvailableReverseGeocodes(CoordinateFakes.Ankara());
		actualAllAvailableReverseGeocodes.Should().BeEquivalentTo(expectedAllAvailableReverseGeocodes);
	}

	private HttpClient CreateMockHttpClient()
	{
		return MockHttpClient.WithResponse(OpenStreetMapReverseGeocodeResponseFakes.Ankara());
	}

	private List<PropertyInfo> PrepareRequestedAddressProperties(List<string> properties)
	{
		var allLowerCaseRequestedProperties = properties.Select(s => s.ToLowerInvariant());
		var requestedAddressProperties = typeof(OpenStreetMapAddress).GetProperties().Where(w => allLowerCaseRequestedProperties.Contains(w.Name.ToLowerInvariant())).ToList();
		return requestedAddressProperties;
	}
}
