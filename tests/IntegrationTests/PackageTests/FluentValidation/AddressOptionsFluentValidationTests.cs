namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class AddressOptionsFluentValidationTests : BaseFluentValidationTests<AddressOptions, AddressOptionsValidator>
{
	#region Invalid

	[Fact]
	public void Null_InputFolderPath_Should_Give_NotNullValidator_Error()
	{
		var options = new AddressOptions(null!, ReverseGeocodeProviderFakes.Valid(), AddressListTypeFakes.Valid());
		CheckPropertyNotNull(options, nameof(AddressOptions.InputPath), Required(nameof(AddressOptions.InputPath), "input", 'i'));
	}

	[Fact]
	public void Using_InputPath_Without_Jpg_Or_Jpeg_Extension_Should_Give_RegularExpressionValidator_And_Verify_Error_Message()
	{
		var options = new AddressOptions(FileNameFakes.InvalidInputPhotoPath, ReverseGeocodeProviderFakes.Valid(), AddressListTypeFakes.Valid());
		CheckPropertyRegularExpression(options, nameof(AddressOptions.InputPath), $"{nameof(AddressOptions.InputPath)} should have .jpg or .jpeg extension");
	}

	#region AddressListType SelectedProperties Require Additional Vendor Specific Properties

	[Fact]
	public void When_Using_BigDataCloud_With_AddressListType_As_SelectedProperties_Not_Using_BigDataCloudAdminLevels_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = AddressOptionsFakes.WithReverseGeocodeServiceAndAddressListType(ReverseGeocodeProvider.BigDataCloud, AddressListType.SelectedProperties);
		CheckPropertyNotEmpty(options, nameof(AddressOptions.BigDataCloudAdminLevels), MustUseMessage(nameof(AddressOptions.BigDataCloudAdminLevels), nameof(ReverseGeocodeProvider.BigDataCloud),
			"bigdatacloud-levels", 'u'));
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public void When_Using_OpenStreetMap_With_AddressListType_As_SelectedProperties_Not_Using_OpenStreetMapProperties_Should_Give_NullValidator_And_Verify_Error_Message(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		var options = AddressOptionsFakes.WithReverseGeocodeServiceAndAddressListType(reverseGeocodeProvider, AddressListType.SelectedProperties);
		CheckPropertyNotEmpty(options, nameof(AddressOptions.OpenStreetMapProperties),
			MustUseMessage(nameof(AddressOptions.OpenStreetMapProperties), reverseGeocodeProvider.ToString(), "openstreetmap-properties", 'r'));
	}

	[Fact]
	public void When_Using_GoogleMaps_With_AddressListType_As_SelectedProperties_Not_Using_GoogleMapsAddressTypes_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = AddressOptionsFakes.WithReverseGeocodeServiceAndAddressListType(ReverseGeocodeProvider.GoogleMaps, AddressListType.SelectedProperties);
		CheckPropertyNotEmpty(options, nameof(AddressOptions.GoogleMapsAddressTypes), MustUseMessage(nameof(AddressOptions.GoogleMapsAddressTypes), nameof(ReverseGeocodeProvider.GoogleMaps),
			"googlemaps-types", 'm'));
	}

	#endregion

	#endregion

	#region Valid

	public static TheoryData<AddressListType> ValidOptionsThatRequireNoAdditionalReverseGeocodeParameter = new()
	{
		AddressListType.FullResponse, AddressListType.AllAvailableProperties
	};

	[Theory]
	[MemberData(nameof(ValidOptionsThatRequireNoAdditionalReverseGeocodeParameter))]
	public void Valid_AddressOptions_With_No_Additional_ReverseGeocode_Parameter_Should_Have_No_Error(AddressListType addressListType)
	{
		var options = new AddressOptions(FileNameFakes.ValidInputPhotoPath, ReverseGeocodeProviderFakes.Valid(), addressListType);
		ValidationShouldHaveNoError(options);
	}

	[Fact]
	public void Valid_AddressOptions_For_GoogleMaps_With_AddressListType_As_SelectedProperties()
	{
		var options = new AddressOptions(FileNameFakes.ValidInputPhotoPath, ReverseGeocodeProvider.GoogleMaps, AddressListType.SelectedProperties, googleMapsAddressTypes: GoogleMapsPropertiesFakes.Valid());
		ValidationShouldHaveNoError(options);
	}

	[Fact]
	public void Valid_AddressOptions_For_BigDataCloud_With_AddressListType_As_SelectedProperties()
	{
		var options = new AddressOptions(FileNameFakes.ValidInputPhotoPath, ReverseGeocodeProvider.BigDataCloud, AddressListType.SelectedProperties, bigDataCloudAdminLevels: BigDataCloudAdminLevelsFakes.Valid());
		ValidationShouldHaveNoError(options);
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public void Valid_AddressOptions_For_OpenStreetMap_With_AddressListType_As_SelectedProperties(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		var options = new AddressOptions(FileNameFakes.ValidInputPhotoPath, reverseGeocodeProvider, AddressListType.SelectedProperties, openStreetMapProperties: OpenStreetMapAddressPropertiesFakes.Valid());
		ValidationShouldHaveNoError(options);
	}

	#endregion
}
