namespace PhotoCli.Tests.UnitTests.Services.ReverseGeocodes;

public class BigDataCloudReverseGeocodeServiceUnitTests
{
	public static TheoryData<List<int>, List<string>> WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData = new()
	{
		{ new List<int> { 2 }, new List<string> { "Türkiye" } },
		{ new List<int> { 2, 4 }, new List<string> { "Türkiye", "Ankara" } },
		{ new List<int> { 2, 4, 6 }, new List<string> { "Türkiye", "Ankara", "Çankaya" } },
		{ new List<int> { 2, 4, 6, 8 }, new List<string> { "Türkiye", "Ankara", "Çankaya", "Mebusevleri Mahallesi" } }
	};

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task With_Given_Options_ReverseGeocode_Result_Should_Be_Given_Expected_Address_Data(List<int> adminLevels, List<string> expectedAddresses)
	{
		var sut = MockServiceWithValidResponse();
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), "tr", adminLevels);
		actualReverseGeocode.Should().BeEquivalentTo(expectedAddresses);
	}

	[Fact]
	public async Task Service_Error_Should_Give_Empty_List()
	{
		var mockHttpClient = MockHttpClient.WithError();
		var sut = new BigDataCloudReverseGeocodeService(mockHttpClient, NullLogger<BigDataCloudReverseGeocodeService>.Instance, ApiKeyStoreFakes.BigDataCloudValid());
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), "tr", BigDataCloudAdminLevelsFakes.Valid());
		actualReverseGeocode.Should().BeEquivalentTo(ArraySegment<string>.Empty);
	}

	[Fact]
	public async Task Response_Serialization_Verify()
	{
		var sut = MockServiceWithValidResponse();
		var bigDataCloudResponse = await sut.SerializeFullResponse(CoordinateFakes.Ankara(), "tr");
		bigDataCloudResponse.Verify();
	}

	public static TheoryData<Dictionary<string, object>> ExpectedAvailableReverseGeocodes = new()
	{
		new Dictionary<string, object>
		{
			{ "AdminLevel2", "Türkiye" },
			{ "AdminLevel3", "İç Anadolu Bölgesi" },
			{ "AdminLevel4", "Ankara" },
			{ "AdminLevel6", "Çankaya" },
			{ "AdminLevel8", "Mebusevleri Mahallesi" },
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

	private BigDataCloudReverseGeocodeService MockServiceWithValidResponse()
	{
		var mockHttpClient = MockHttpClient.WithResponse(BigDataCloudReverseGeocodeResponseFakes.Ankara());
		return new BigDataCloudReverseGeocodeService(mockHttpClient, NullLogger<BigDataCloudReverseGeocodeService>.Instance, ApiKeyStoreFakes.BigDataCloudValid());
	}
}
