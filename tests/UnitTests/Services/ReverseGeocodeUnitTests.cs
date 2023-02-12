using System.Reflection;

namespace PhotoCli.Tests.UnitTests.Services;

public class ReverseGeocodeUnitTests
{
	[Theory]
	[InlineData(ReverseGeocodeProvider.BigDataCloud)]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.GoogleMaps)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public async Task CopyOptions_Should_Invoke_And_Verify_Related_ReverseGeocode_Provider(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		await VerifyReverseGeocodeProviderInvocations(reverseGeocodeProvider, CopyOptionsFakes.WithReverseGeocodeService(reverseGeocodeProvider));
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.BigDataCloud)]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.GoogleMaps)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public async Task InfoOptions_Should_Invoke_And_Verify_Related_ReverseGeocode_Provider(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		await VerifyReverseGeocodeProviderInvocations(reverseGeocodeProvider, InfoOptionsFakes.WithReverseGeocodeService(reverseGeocodeProvider));
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.BigDataCloud)]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.GoogleMaps)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public async Task AddressOptions_Should_Invoke_And_Verify_Related_ReverseGeocode_Provider(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		await VerifyReverseGeocodeProviderInvocations(reverseGeocodeProvider, AddressOptionsFakes.WithAddressListType(AddressListType.SelectedProperties, reverseGeocodeProvider));
	}

	private async Task VerifyReverseGeocodeProviderInvocations(ReverseGeocodeProvider reverseGeocodeProvider, IReverseGeocodeOptions options)
	{
		var bigDataCloudMock = new Mock<IBigDataCloudReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.BigDataCloud)
		{
			bigDataCloudMock.Setup(s => s.Get(It.IsAny<Coordinate>(), It.IsAny<string>(), It.IsAny<List<int>>()))
				.ReturnsAsync(ReverseGeocodeFakes.Valid);
		}

		var openStreetMapFoundationMock = new Mock<IOpenStreetMapFoundationReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.OpenStreetMapFoundation)
		{
			openStreetMapFoundationMock.Setup(s => s.Get(It.IsAny<Coordinate>(), It.IsAny<List<PropertyInfo>>()))
				.ReturnsAsync(ReverseGeocodeFakes.Valid);
		}

		var googleMapsMock = new Mock<IGoogleMapsReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.GoogleMaps)
		{
			googleMapsMock.Setup(s => s.Get(It.IsAny<Coordinate>(), It.IsAny<string>(), It.IsAny<List<string>>()))
				.ReturnsAsync(ReverseGeocodeFakes.Valid);
		}

		var locationIqMock = new Mock<ILocationIqReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.LocationIq)
		{
			locationIqMock.Setup(s => s.Get(It.IsAny<Coordinate>(), It.IsAny<List<PropertyInfo>>()))
				.ReturnsAsync(ReverseGeocodeFakes.Valid);
		}

		var sut = new ReverseGeocodeService(options, bigDataCloudMock.Object, openStreetMapFoundationMock.Object, googleMapsMock.Object, locationIqMock.Object,
			NullLogger<ReverseGeocodeService>.Instance);

		var reverseGeocodeResult = await sut.Get(CoordinateFakes.Valid());
		reverseGeocodeResult.Should().BeEquivalentTo(ReverseGeocodeFakes.Valid());

		VerifyAll(bigDataCloudMock, openStreetMapFoundationMock, googleMapsMock, locationIqMock);
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.BigDataCloud)]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.GoogleMaps)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public async Task AddressOptions_RawResponse_Should_Invoke_And_Verify_Related_ReverseGeocode_Provider(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		await RawResponseVerifyReverseGeocodeProviderInvocations(reverseGeocodeProvider, AddressOptionsFakes.WithAddressListType(AddressListType.FullResponse, reverseGeocodeProvider));
	}

	private async Task RawResponseVerifyReverseGeocodeProviderInvocations(ReverseGeocodeProvider reverseGeocodeProvider, IReverseGeocodeOptions options)
	{
		object? response = null;
		var bigDataCloudMock = new Mock<IBigDataCloudReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.BigDataCloud)
		{
			response = new BigDataCloudResponse { City = "test" };
			bigDataCloudMock.Setup(s => s.SerializeFullResponse(It.IsAny<Coordinate>(), It.IsAny<string>()))
				.ReturnsAsync((BigDataCloudResponse)response);
		}

		var openStreetMapFoundationMock = new Mock<IOpenStreetMapFoundationReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.OpenStreetMapFoundation)
		{
			response = new OpenStreetMapResponse { DisplayName = "test" };
			openStreetMapFoundationMock.Setup(s => s.SerializeFullResponse(It.IsAny<Coordinate>()))
				.ReturnsAsync((OpenStreetMapResponse)response);
		}

		var googleMapsMock = new Mock<IGoogleMapsReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.GoogleMaps)
		{
			response = new GoogleMapsResponse { Status = "test" };
			googleMapsMock.Setup(s => s.SerializeFullResponse(It.IsAny<Coordinate>(), It.IsAny<string>()))
				.ReturnsAsync((GoogleMapsResponse)response);
		}

		var locationIqMock = new Mock<ILocationIqReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.LocationIq)
		{
			response = new OpenStreetMapResponse { DisplayName = "test" };
			locationIqMock.Setup(s => s.SerializeFullResponse(It.IsAny<Coordinate>()))
				.ReturnsAsync((OpenStreetMapResponse)response);
		}

		var sut = new ReverseGeocodeService(options, bigDataCloudMock.Object, openStreetMapFoundationMock.Object, googleMapsMock.Object, locationIqMock.Object,
			NullLogger<ReverseGeocodeService>.Instance);

		var actualSerializedResponse = await sut.RawResponse(CoordinateFakes.Valid());
		var expectedSerializedResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });

		actualSerializedResponse.Should().Be(expectedSerializedResponse);

		VerifyAll(bigDataCloudMock, openStreetMapFoundationMock, googleMapsMock, locationIqMock);
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.BigDataCloud)]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.GoogleMaps)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public async Task AddressOptions_AllAvailableReverseGeocodes_Should_Invoke_And_Verify_Related_ReverseGeocode_Provider(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		await AllAvailableReverseGeocodesVerifyReverseGeocodeServiceInvocations(reverseGeocodeProvider,
			AddressOptionsFakes.WithAddressListType(AddressListType.AllAvailableProperties, reverseGeocodeProvider));
	}

	private async Task AllAvailableReverseGeocodesVerifyReverseGeocodeServiceInvocations(ReverseGeocodeProvider reverseGeocodeProvider, IReverseGeocodeOptions options)
	{
		var allAvailableReverseGeocodesFake = new Dictionary<string, object>
		{
			{ "test-property", "test-value" }
		};
		var bigDataCloudMock = new Mock<IBigDataCloudReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.BigDataCloud)
		{
			bigDataCloudMock.Setup(s => s.AllAvailableReverseGeocodes(It.IsAny<Coordinate>(), It.IsAny<string>()))
				.ReturnsAsync(allAvailableReverseGeocodesFake);
		}

		var openStreetMapFoundationMock = new Mock<IOpenStreetMapFoundationReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.OpenStreetMapFoundation)
		{
			openStreetMapFoundationMock.Setup(s => s.AllAvailableReverseGeocodes(It.IsAny<Coordinate>()))
				.ReturnsAsync(allAvailableReverseGeocodesFake);
		}

		var googleMapsMock = new Mock<IGoogleMapsReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.GoogleMaps)
		{
			googleMapsMock.Setup(s => s.AllAvailableReverseGeocodes(It.IsAny<Coordinate>(), It.IsAny<string>()))
				.ReturnsAsync(allAvailableReverseGeocodesFake);
		}

		var locationIqMock = new Mock<ILocationIqReverseGeocodeService>(MockBehavior.Strict);
		if (reverseGeocodeProvider is ReverseGeocodeProvider.LocationIq)
		{
			locationIqMock.Setup(s => s.AllAvailableReverseGeocodes(It.IsAny<Coordinate>()))
				.ReturnsAsync(allAvailableReverseGeocodesFake);
		}

		var sut = new ReverseGeocodeService(options, bigDataCloudMock.Object, openStreetMapFoundationMock.Object, googleMapsMock.Object, locationIqMock.Object,
			NullLogger<ReverseGeocodeService>.Instance);

		var actualAllAvailableReverseGeocodes = await sut.AllAvailableReverseGeocodes(CoordinateFakes.Valid());
		actualAllAvailableReverseGeocodes.Should().BeEquivalentTo(actualAllAvailableReverseGeocodes);

		VerifyAll(bigDataCloudMock, openStreetMapFoundationMock, googleMapsMock, locationIqMock);
	}

	private void VerifyAll(Mock<IBigDataCloudReverseGeocodeService> bigDataCloudMock, Mock<IOpenStreetMapFoundationReverseGeocodeService> openStreetMapFoundationMock,
		Mock<IGoogleMapsReverseGeocodeService> googleMapsMock, Mock<ILocationIqReverseGeocodeService> locationIqMock)
	{
		bigDataCloudMock.VerifyAll();
		openStreetMapFoundationMock.VerifyAll();
		googleMapsMock.VerifyAll();
		locationIqMock.VerifyAll();

		bigDataCloudMock.VerifyNoOtherCalls();
		openStreetMapFoundationMock.VerifyNoOtherCalls();
		googleMapsMock.VerifyNoOtherCalls();
		locationIqMock.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task CommandOptions_With_Disabled_Should_Throw_PhotoOrganizerToolException()
	{
		var sut = new ReverseGeocodeService(CopyOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.Disabled), null!, null!, null!, null!, NullLogger<ReverseGeocodeService>.Instance);
		await Assert.ThrowsAsync<PhotoCliException>(async () => { await sut.Get(CoordinateFakes.Valid()); });
	}
}
