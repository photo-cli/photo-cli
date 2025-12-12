namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class InfoOptionsFluentValidationTests : BaseFluentValidationTests<InfoOptions, InfoOptionsValidator>
{
	#region Valid

	public static TheoryData<InfoOptions> OnlyOutput = new()
	{
		InfoOptionsFakes.WithOutputPath("report.csv"),
	};

	public static TheoryData<InfoOptions> InputPath = new()
	{
		InfoOptionsFakes.WithInputPath("valid-input-path"),
	};

	public static TheoryData<InfoOptions> ExifActions = new()
	{
		InfoOptionsFakes.Create(invalidFileFormatAction: InfoInvalidFormatAction.PreventProcess,
			noPhotoTakenDateAction: InfoNoPhotoTakenDateAction.PreventProcess, noCoordinateAction: InfoNoCoordinateAction.PreventProcess),
	};

	public static TheoryData<InfoOptions> MissingReverseGeocode = new()
	{
		InfoOptionsFakes.Create(missingReverseGeocodeAction: MissingReverseGeocodeAction.PreventProcess,
			reverseGeoCodeProvider: ReverseGeocodeProvider.GoogleMaps, googleMapsAddressTypes: ["address-type"]),
	};

	[Theory]
	[MemberData(nameof(OnlyOutput))]
	[MemberData(nameof(InputPath))]
	[MemberData(nameof(ExifActions))]
	[MemberData(nameof(MissingReverseGeocode))]
	public void ValidInfoOptions_ShouldHaveNoError(InfoOptions infoOptions)
	{
		ValidationShouldHaveNoError(infoOptions);
	}

	#endregion

	#region Invalid

	[Fact]
	public void Null_OutputFolderPath_Should_Give_NotNullValidator_Error()
	{
		var options = new InfoOptions(null!);
		CheckPropertyRequiredString(options, nameof(InfoOptions.OutputPath), Required(nameof(InfoOptions.OutputPath), "output", 'o'));
	}

	[Fact]
	public void When_Using_OutputType_OnlyExifReport_Using_OutputPath_Without_Csv_Extension_Should_Give_RegularExpressionValidator_And_Verify_Error_Message()
	{
		var options = new InfoOptions("report.txt");
		CheckPropertyRegularExpression(options, nameof(InfoOptions.OutputPath), $"{nameof(InfoOptions.OutputPath)} should have .csv extension");
	}

	[Fact]
	public void WhenUsingMissingReverseGeocodeActionOtherThanContinueNotUsingReverseGeocodeProvider_ShouldGivePredicateValidatorAndVerifyErrorMessage()
	{
		var options = new InfoOptions("report.csv", missingReverseGeocodeAction: MissingReverseGeocodeAction.PreventProcess);
		CheckPropertyInvalidValue(options, nameof(InfoOptions.ReverseGeocodeProvider), MustUseMessage(ReverseGeocodeProviderInfo, MissingReverseGeocodeActionInfo));
	}

	#region ReverseGeocode Providers

	[Fact]
	public void WhenUsingBigDataCloudWithoutBigDataCloudAdminLevels_ShouldGiveNullValidatorAndVerifyErrorMessage()
	{
		var options = InfoOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.BigDataCloud);
		CheckPropertyNotEmpty(options, nameof(InfoOptions.BigDataCloudAdminLevels), MustUseMessage(BigDataCloudAdminLevelInfo, ReverseGeocodeWithBigDataCloudInfo));
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public void WhenUsingOpenStreetMapWithoutOpenStreetMapProperties_ShouldGiveNullValidatorAndVerifyErrorMessage(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		var options = InfoOptionsFakes.WithReverseGeocodeService(reverseGeocodeProvider);
		CheckPropertyNotEmpty(options, nameof(InfoOptions.OpenStreetMapProperties), MustUseMessage(OpenStreetMapPropertiesInfo, ReverseGeocodeProviderInfoWithValue(reverseGeocodeProvider)));
	}

	[Fact]
	public void WhenUsingGoogleMapsWithoutGoogleMapsAddressTypes_ShouldGiveNullValidatorAndVerifyErrorMessage()
	{
		var options = InfoOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.GoogleMaps);
		CheckPropertyNotEmpty(options, nameof(InfoOptions.GoogleMapsAddressTypes), MustUseMessage(GoogleMapsAddressTypeInfo, ReverseGeocodeProviderWithGoogleMapsInfo));
	}

	#region Enum Value Invalid Range

	[Fact]
	public void InvalidRangeForInfoInvalidFormatAction_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = InfoOptionsFakes.WithPreventAction(invalidFormatAction: (InfoInvalidFormatAction)byte.MaxValue);
		CheckEnumInvalidRangeValue<InfoInvalidFormatAction>(options, nameof(InfoOptions.InvalidFileFormatAction), false);
	}

	[Fact]
	public void InvalidRangeForInfoNoPhotoTakenDateAction_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = InfoOptionsFakes.WithPreventAction(noPhotoDateTimeTakenAction: (InfoNoPhotoTakenDateAction)byte.MaxValue);
		CheckEnumInvalidRangeValue<InfoNoPhotoTakenDateAction>(options, nameof(InfoOptions.NoPhotoTakenDateAction), false);
	}

	[Fact]
	public void InvalidRangeForInfoNoCoordinateAction_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = InfoOptionsFakes.WithPreventAction(noCoordinateAction: (InfoNoCoordinateAction)byte.MaxValue);
		CheckEnumInvalidRangeValue<InfoNoCoordinateAction>(options, nameof(InfoOptions.NoCoordinateAction), false);
	}


	#endregion

	#endregion

	#endregion

	protected override InfoOptionsValidator CreateValidator()
	{
		return new InfoOptionsValidator();
	}
}
