namespace PhotoCli.Tests.Fakes.Options;

public static class InfoOptionsFakes
{
	private const string ValidOutputPath = "output.csv";

	public static InfoOptions Valid()
	{
		return Create();
	}

	public static InfoOptions WithPaths(string outputPath, string inputPath)
	{
		return Create(outputPath, inputPath);
	}

	public static InfoOptions WithOutputPath(string outputPath)
	{
		return Create(outputPath);
	}

	public static InfoOptions WithInputPath(string inputPath)
	{
		return Create(inputPath: inputPath);
	}

	public static InfoOptions WithInputPathAndPreventAction(string inputPath, InfoInvalidFormatAction invalidFormatAction, InfoNoPhotoTakenDateAction noPhotoDateTimeTakenAction, InfoNoCoordinateAction noCoordinateAction)
	{
		return Create(inputPath: inputPath, invalidFileFormatAction: invalidFormatAction, noPhotoTakenDateAction: noPhotoDateTimeTakenAction, noCoordinateAction: noCoordinateAction);
	}

	public static InfoOptions WithPreventAction(InfoInvalidFormatAction invalidFormatAction = InfoInvalidFormatAction.Continue,
		InfoNoPhotoTakenDateAction noPhotoDateTimeTakenAction = InfoNoPhotoTakenDateAction.Continue, InfoNoCoordinateAction noCoordinateAction = InfoNoCoordinateAction.Continue)
	{
		return Create(invalidFileFormatAction: invalidFormatAction, noPhotoTakenDateAction: noPhotoDateTimeTakenAction, noCoordinateAction: noCoordinateAction);
	}

	public static InfoOptions WithValidReverseGeocodeService(string outputFolder, string sourceFolderPath)
	{
		return Create(outputFolder, sourceFolderPath, reverseGeoCodeProvider: ReverseGeocodeProviderFakes.Valid());
	}

	public static InfoOptions WithoutReverseGeocode(string outputFolder, string sourceFolderPath)
	{
		return Create(outputFolder, sourceFolderPath, reverseGeoCodeProvider: ReverseGeocodeProvider.Disabled);
	}

	public static InfoOptions WithReverseGeocodeService(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		return Create(reverseGeoCodeProvider: reverseGeocodeProvider);
	}

	public static InfoOptions ValidReverseGeocodeServiceWithLicense(bool hasPaidLicense, ReverseGeocodeProvider reverseGeocodeProvider)
	{
		return Create(reverseGeoCodeProvider: reverseGeocodeProvider, hasPaidLicense: hasPaidLicense);
	}

	public static InfoOptions Create(
		// Required
		string outputPath = ValidOutputPath,
		// Optional
		string? inputPath = null, bool allFolders = false,
		InfoInvalidFormatAction invalidFileFormatAction = InfoInvalidFormatAction.Continue,
		InfoNoPhotoTakenDateAction noPhotoTakenDateAction = InfoNoPhotoTakenDateAction.Continue, InfoNoCoordinateAction noCoordinateAction = InfoNoCoordinateAction.Continue,
		// ReverseGeocode - Shared
		ReverseGeocodeProvider reverseGeoCodeProvider = ReverseGeocodeProvider.Disabled, string? bigDataCloudApiKey = null, IEnumerable<int>? bigDataCloudAdminLevels = null,
		IEnumerable<string>? googleMapsAddressTypes = null,
		string? googleMapsApiKey = null, IEnumerable<string>? openStreetMapProperties = null,
		string? locationIqApiKey = null, bool? hasPaidLicense = null, string? language = null, MissingReverseGeocodeAction missingReverseGeocodeAction = MissingReverseGeocodeAction.Continue)
	{
		return new InfoOptions(outputPath, inputPath, allFolders, invalidFileFormatAction, noPhotoTakenDateAction, noCoordinateAction, reverseGeoCodeProvider,
			bigDataCloudApiKey, bigDataCloudAdminLevels, googleMapsAddressTypes, googleMapsApiKey, openStreetMapProperties, locationIqApiKey, hasPaidLicense, language, missingReverseGeocodeAction);
	}
}
