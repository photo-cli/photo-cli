using LogLevel = Microsoft.Extensions.Logging.LogLevel;

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
		var sut = MockGeocodeService(BigDataCloudReverseGeocodeResponseFakes.Ankara());
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
		var sut = MockGeocodeService(BigDataCloudReverseGeocodeResponseFakes.Ankara());
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
		var sut = MockGeocodeService(BigDataCloudReverseGeocodeResponseFakes.Ankara());
		var actualAllAvailableReverseGeocodes = await sut.AllAvailableReverseGeocodes(CoordinateFakes.Ankara(), "tr");
		actualAllAvailableReverseGeocodes.Should().BeEquivalentTo(expectedAllAvailableReverseGeocodes);
	}

	[Fact]
	public async Task AllAvailableReverseGeocodes_MultipleAdminLevelResponseFromService_ShouldLogWarningMessageOfInconsistentData()
	{
		var loggerMock = new Mock<ILogger<BigDataCloudReverseGeocodeService>>();
		var sut = MockGeocodeService(BigDataCloudReverseGeocodeResponseFakes.MultipleAdminLevel(), loggerMock.Object);
		await sut.AllAvailableReverseGeocodes(CoordinateFakes.Valid(), "tr");
		var logStatements = new[]
		{
			"BigDataCloud returned inconsistent/duplicate data. Multiple admin level on 3 found. Used value: First value on level3, duplicate value: Duplicate value on level3"
		};
		loggerMock.VerifyAllLogStatementsAtLeastOnce(LogLevel.Warning, logStatements);
	}

	private static BigDataCloudReverseGeocodeService MockGeocodeService(string response, ILogger<BigDataCloudReverseGeocodeService>? logger = null)
	{
		var mockHttpClient = MockHttpClient.WithResponse(response);
		logger ??= NullLogger<BigDataCloudReverseGeocodeService>.Instance;
		return new BigDataCloudReverseGeocodeService(mockHttpClient, logger, ApiKeyStoreFakes.BigDataCloudValid());
	}
}
