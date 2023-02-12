namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class AddressOptionsFluentValidationTests : BaseFluentValidationTests<AddressOptions, AddressOptionsValidator>
{
	[Fact]
	public void Null_InputFolderPath_Should_Give_NotNullValidator_Error()
	{
		var options = new AddressOptions(null!, ReverseGeocodeProviderFakes.Valid(), AddressListTypeFakes.Valid());
		CheckPropertyNotNull(options, nameof(AddressOptions.InputPath), Required(nameof(AddressOptions.InputPath), "input", 'i'));
	}

	[Fact]
	public void Using_InputPath_Without_Jpg_Or_Jpeg_Extension_Should_Give_RegularExpressionValidator_And_Verify_Error_Message()
	{
		var options = new AddressOptions("report.png", ReverseGeocodeProviderFakes.Valid(), AddressListTypeFakes.Valid());
		CheckPropertyRegularExpression(options, nameof(AddressOptions.InputPath), $"{nameof(AddressOptions.InputPath)} should have .jpg or .jpeg extension");
	}

	#region ReverseGeocode Providers

	[Fact]
	public void When_Using_BigDataCloud_Not_Using_BigDataCloudAdminLevels_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = AddressOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.BigDataCloud);
		CheckPropertyNotEmpty(options, nameof(AddressOptions.BigDataCloudAdminLevels), MustUseMessage(nameof(AddressOptions.BigDataCloudAdminLevels), nameof(ReverseGeocodeProvider.BigDataCloud),
			"bigdatacloud-levels", 'v'));
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public void When_Using_OpenStreetMap_Not_Using_OpenStreetMapProperties_Should_Give_NullValidator_And_Verify_Error_Message(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		var options = AddressOptionsFakes.WithReverseGeocodeService(reverseGeocodeProvider);
		CheckPropertyNotEmpty(options, nameof(AddressOptions.OpenStreetMapProperties),
			MustUseMessage(nameof(AddressOptions.OpenStreetMapProperties), reverseGeocodeProvider.ToString(), "openstreetmap-properties", 'r'));
	}

	[Fact]
	public void When_Using_GoogleMaps_Not_Using_GoogleMapsAddressTypes_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = AddressOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.GoogleMaps);
		CheckPropertyNotEmpty(options, nameof(AddressOptions.GoogleMapsAddressTypes), MustUseMessage(nameof(AddressOptions.GoogleMapsAddressTypes), nameof(ReverseGeocodeProvider.GoogleMaps),
			"googlemaps-types", 'm'));
	}

	#endregion
}
