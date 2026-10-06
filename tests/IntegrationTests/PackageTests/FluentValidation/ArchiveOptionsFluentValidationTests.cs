namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class ArchiveOptionsFluentValidationTests : BaseFluentValidationTests<ArchiveOptions, ArchiveOptionsValidator>
{
	#region Valid

	public static TheoryData<ArchiveOptions> OnlyOutput = new()
	{
		ArchiveOptionsFakes.Create("output-path"),
	};

	public static TheoryData<ArchiveOptions> InputAndOutput = new()
	{
		ArchiveOptionsFakes.Create("output-path", "input-path"),
	};

	public static TheoryData<ArchiveOptions> Actions = new()
	{
		ArchiveOptionsFakes.Create(invalidFileFormatAction: ArchiveInvalidFormatAction.PreventProcess),
		ArchiveOptionsFakes.Create(noPhotoTakenDateAction: ArchiveNoPhotoTakenDateAction.PreventProcess),
		ArchiveOptionsFakes.Create(noCoordinateAction: ArchiveNoCoordinateAction.PreventProcess),
		ArchiveOptionsFakes.Create(
			invalidFileFormatAction: ArchiveInvalidFormatAction.PreventProcess,
			noPhotoTakenDateAction: ArchiveNoPhotoTakenDateAction.PreventProcess,
			noCoordinateAction: ArchiveNoCoordinateAction.PreventProcess
		),
	};

	public static TheoryData<ArchiveOptions> ReverseGeocodes = new()
	{
		ArchiveOptionsFakes.Create(reverseGeoCodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: [1]),
		ArchiveOptionsFakes.Create(reverseGeoCodeProvider: ReverseGeocodeProvider.OpenStreetMapFoundation, openStreetMapProperties: ["open-street-property"]),
		ArchiveOptionsFakes.Create(reverseGeoCodeProvider: ReverseGeocodeProvider.GoogleMaps, googleMapsAddressTypes: ["google-type"]),
		ArchiveOptionsFakes.Create(reverseGeoCodeProvider: ReverseGeocodeProvider.LocationIq, openStreetMapProperties: ["open-street-property"]),
	};

	public static TheoryData<ArchiveOptions> ExpectedDayRange = new()
	{
		ArchiveOptionsFakes.Create(expectedDayRange: 10),
	};

	public static TheoryData<ArchiveOptions> AlbumTypes = new()
	{
		ArchiveOptionsFakes.Create(albumType: ArchiveAlbumType.Individual, albumNameNew: "album-new-individual"),
		ArchiveOptionsFakes.Create(albumType: ArchiveAlbumType.Individual, albumIdUpdate: 1),

		ArchiveOptionsFakes.Create(albumType: ArchiveAlbumType.DateRange, albumNameNew: "album-new-date-range"),
		ArchiveOptionsFakes.Create(albumType: ArchiveAlbumType.DateRange, albumIdUpdate: 2),
	};

	public static TheoryData<ArchiveOptions> AutoReverseGeocodeAlbum = new()
	{
		ArchiveOptionsFakes.Create(autoReverseGeocodeAlbum: true, reverseGeoCodeProvider: ReverseGeocodeProvider.GoogleMaps, googleMapsAddressTypes: ["google-type"]),
	};

	public static TheoryData<ArchiveOptions> DeleteSource = new()
	{
		ArchiveOptionsFakes.Create(deleteSource: true),
	};

	public static TheoryData<ArchiveOptions> MissingReverseGeocode = new()
	{
		ArchiveOptionsFakes.Create(missingReverseGeocodeAction: MissingReverseGeocodeAction.PreventProcess,
			reverseGeoCodeProvider: ReverseGeocodeProvider.OpenStreetMapFoundation, openStreetMapProperties: ["open-street-property"]),
	};

	[Theory]
	[MemberData(nameof(OnlyOutput))]
	[MemberData(nameof(InputAndOutput))]
	[MemberData(nameof(Actions))]
	[MemberData(nameof(ReverseGeocodes))]
	[MemberData(nameof(ExpectedDayRange))]
	[MemberData(nameof(AlbumTypes))]
	[MemberData(nameof(AutoReverseGeocodeAlbum))]
	[MemberData(nameof(DeleteSource))]
	[MemberData(nameof(MissingReverseGeocode))]
	public void ValidOption_ShouldHaveNoError(ArchiveOptions archiveOptions)
	{
		ValidationShouldHaveNoError(archiveOptions);
	}

	#endregion

	#region Invalid

	#region Conditional

	[Fact]
	public void WhenUsingMissingReverseGeocodeActionOtherThanContinueNotUsingReverseGeocodeProvider_ShouldGivePredicateValidatorAndVerifyErrorMessage()
	{
		var options = ArchiveOptionsFakes.Create(missingReverseGeocodeAction: MissingReverseGeocodeAction.PreventProcess);
		CheckPropertyInvalidValue(options, nameof(ArchiveOptions.ReverseGeocodeProvider), MustUseMessage(ReverseGeocodeProviderInfo, MissingReverseGeocodeActionInfo));
	}

	#region ReverseGeocode Providers

	[Fact]
	public void WhenUsingBigDataCloudWithoutBigDataCloudAdminLevels_ShouldGiveNullValidatorAndVerifyErrorMessage()
	{
		var options = ArchiveOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.BigDataCloud);
		CheckPropertyNotEmpty(options, nameof(ArchiveOptions.BigDataCloudAdminLevels), MustUseMessage(BigDataCloudAdminLevelInfo, ReverseGeocodeWithBigDataCloudInfo));
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public void WhenUsingOpenStreetMapWithoutOpenStreetMapProperties_ShouldGiveNullValidatorAndVerifyErrorMessage(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		var options = ArchiveOptionsFakes.WithReverseGeocodeService(reverseGeocodeProvider);
		CheckPropertyNotEmpty(options, nameof(ArchiveOptions.OpenStreetMapProperties),
			MustUseMessage(OpenStreetMapPropertiesInfo, ReverseGeocodeProviderInfoWithValue(reverseGeocodeProvider)));
	}

	[Fact]
	public void WhenUsingGoogleMapsWithoutGoogleMapsAddressTypes_ShouldGiveNullValidatorAndVerifyErrorMessage()
	{
		var options = ArchiveOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.GoogleMaps);
		CheckPropertyNotEmpty(options, nameof(ArchiveOptions.GoogleMapsAddressTypes), MustUseMessage(GoogleMapsAddressTypeInfo, ReverseGeocodeProviderWithGoogleMapsInfo));
	}

	#endregion

	#endregion

	#region Required

	[Fact]
	public void NullOutputFolderPath_ShouldBeValid()
	{
		var commandLineOptions = ArchiveOptionsFakes.Create(null!);
		ValidationShouldHaveNoError(commandLineOptions);
	}

	#endregion

	#region Enum Value Invalid Range

	[Fact]
	public void InvalidRangeForInvalidFileFormatActionOption_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = ArchiveOptionsFakes.Create(invalidFileFormatAction: (ArchiveInvalidFormatAction)byte.MaxValue);
		CheckEnumInvalidRangeValue<ArchiveInvalidFormatAction>(options, nameof(ArchiveOptions.InvalidFileFormatAction), false);
	}

	[Fact]
	public void InvalidRangeForNoPhotoTakenDateAction_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = ArchiveOptionsFakes.Create(noPhotoTakenDateAction: (ArchiveNoPhotoTakenDateAction)byte.MaxValue);
		CheckEnumInvalidRangeValue<ArchiveNoPhotoTakenDateAction>(options, nameof(ArchiveOptions.NoPhotoTakenDateAction), false);
	}

	[Fact]
	public void InvalidRangeForNoCoordinateAction_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = ArchiveOptionsFakes.Create(noCoordinateAction: (ArchiveNoCoordinateAction)byte.MaxValue);
		CheckEnumInvalidRangeValue<ArchiveNoCoordinateAction>(options, nameof(ArchiveOptions.NoCoordinateAction), false);
	}

	[Fact]
	public void InvalidRangeForAlbumType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = ArchiveOptionsFakes.Create(albumType: (ArchiveAlbumType)byte.MaxValue);
		CheckEnumInvalidRangeValue<ArchiveAlbumType>(options, nameof(ArchiveOptions.AlbumType), true);
	}

	#endregion

	#region Enum Invalid Default Value

	[Fact]
	public void InvalidDefaultValueForAlbumType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = ArchiveOptionsFakes.Create(albumType: 0);
		CheckEnumInvalidRangeValue<ArchiveAlbumType>(options, nameof(ArchiveOptions.AlbumType), true);
	}

	#endregion

	#endregion

	protected override ArchiveOptionsValidator CreateValidator()
	{
		return new ArchiveOptionsValidator();
	}
}
