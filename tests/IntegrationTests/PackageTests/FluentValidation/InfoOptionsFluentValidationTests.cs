namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class InfoOptionsFluentValidationTests : BaseFluentValidationTests<InfoOptions, InfoOptionsValidator>
{
	[Fact]
	public void Null_OutputFolderPath_Should_Give_NotNullValidator_Error()
	{
		var options = new InfoOptions(null!);
		CheckPropertyNotNull(options, nameof(InfoOptions.OutputPath), Required(nameof(InfoOptions.OutputPath), "output", 'o'));
	}

	[Fact]
	public void When_Using_OutputType_OnlyExifReport_Using_OutputPath_Without_Csv_Extension_Should_Give_RegularExpressionValidator_And_Verify_Error_Message()
	{
		var options = new InfoOptions("report.txt");
		CheckPropertyRegularExpression(options, nameof(InfoOptions.OutputPath), $"{nameof(InfoOptions.OutputPath)} should have .csv extension");
	}

	#region ReverseGeocode Providers

	[Fact]
	public void When_Using_BigDataCloud_Not_Using_BigDataCloudAdminLevels_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = InfoOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.BigDataCloud);
		CheckPropertyNotEmpty(options, nameof(InfoOptions.BigDataCloudAdminLevels), MustUseMessage(nameof(InfoOptions.BigDataCloudAdminLevels), nameof(ReverseGeocodeProvider.BigDataCloud),
			"bigdatacloud-levels", 'u'));
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public void When_Using_OpenStreetMap_Not_Using_OpenStreetMapProperties_Should_Give_NullValidator_And_Verify_Error_Message(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		var options = InfoOptionsFakes.WithReverseGeocodeService(reverseGeocodeProvider);
		CheckPropertyNotEmpty(options, nameof(InfoOptions.OpenStreetMapProperties),
			MustUseMessage(nameof(InfoOptions.OpenStreetMapProperties), reverseGeocodeProvider.ToString(), "openstreetmap-properties", 'r'));
	}

	[Fact]
	public void When_Using_GoogleMaps_Not_Using_GoogleMapsAddressTypes_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = InfoOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.GoogleMaps);
		CheckPropertyNotEmpty(options, nameof(InfoOptions.GoogleMapsAddressTypes), MustUseMessage(nameof(InfoOptions.GoogleMapsAddressTypes), nameof(ReverseGeocodeProvider.GoogleMaps),
			"googlemaps-types", 'm'));
	}

	#endregion
}
