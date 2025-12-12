namespace PhotoCli.Tests.Fakes.Options;

public static class ArchiveOptionsFakes
{
	private const string OutputFolder = "output-folder";

	public static ArchiveOptions Valid()
	{
		return new ArchiveOptions(OutputFolder);
	}

	public static ArchiveOptions WithPaths(string outputFolder, string sourceFolderPath)
	{
		return new ArchiveOptions(outputFolder, sourceFolderPath);
	}

	public static ArchiveOptions WithPreventAction(string sourceFolderPath, ArchiveInvalidFormatAction invalidFormatAction, ArchiveNoPhotoTakenDateAction noPhotoDateTimeTakenAction, ArchiveNoCoordinateAction noCoordinateAction)
	{
		return new ArchiveOptions(OutputFolder, sourceFolderPath, invalidFileFormatAction: invalidFormatAction, noPhotoTakenDateAction: noPhotoDateTimeTakenAction, noCoordinateAction: noCoordinateAction);
	}

	public static ArchiveOptions WithValidReverseGeocodeService(string outputFolder, string sourceFolderPath)
	{
		return new ArchiveOptions(outputFolder, sourceFolderPath, reverseGeoCodeProvider: ReverseGeocodeProviderFakes.Valid());
	}

	public static ArchiveOptions WithReverseGeocodeService(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		return Create(reverseGeoCodeProvider: reverseGeocodeProvider);
	}

	public static ArchiveOptions WithoutReverseGeocode(string outputFolder, string sourceFolderPath)
	{
		return new ArchiveOptions(outputFolder, sourceFolderPath);
	}

	public static ArchiveOptions WithDryRun(string outputFolderPath, string sourceFolderPath)
	{
		return new ArchiveOptions(outputFolderPath, sourceFolderPath, isDryRun: true);
	}

	public static ArchiveOptions WithAlbumNew(string outputFolder, string sourceFolderPath, string albumName, ArchiveAlbumType albumType)
	{
		return new ArchiveOptions(outputFolder, sourceFolderPath, albumNameNew: albumName, albumType: albumType);
	}

	public static ArchiveOptions WithAlbumUpdate(string outputFolder, string sourceFolderPath, int albumId, ArchiveAlbumType albumType)
	{
		return new ArchiveOptions(outputFolder, sourceFolderPath, albumIdUpdate: albumId, albumType: albumType);
	}

	public static ArchiveOptions WithAutoReverseGeocodeAlbum(string outputFolderPath, string sourceFolderPath)
	{
		return new ArchiveOptions(outputFolderPath, sourceFolderPath, autoReverseGeocodeAlbum: true, reverseGeoCodeProvider: ReverseGeocodeProviderFakes.Valid());
	}

	public static ArchiveOptions WithAutoReverseGeocodeAndNewIndividualAlbum(string outputFolderPath, string sourceFolderPath, string albumName, ArchiveAlbumType albumType)
	{
		return new ArchiveOptions(outputFolderPath, sourceFolderPath, autoReverseGeocodeAlbum: true, reverseGeoCodeProvider: ReverseGeocodeProviderFakes.Valid(),
			albumNameNew: albumName, albumType: albumType);
	}

	public static ArchiveOptions WithAutoReverseGeocodeAndUpdateIndividualAlbum(string outputFolderPath, string sourceFolderPath, int albumId, ArchiveAlbumType albumType)
	{
		return new ArchiveOptions(outputFolderPath, sourceFolderPath, autoReverseGeocodeAlbum: true, reverseGeoCodeProvider: ReverseGeocodeProviderFakes.Valid(),
			albumIdUpdate: albumId, albumType: albumType);
	}

	public static ArchiveOptions WithExpectedDayRange(string sourceFolderPath, short expectedDayRange)
	{
		return new ArchiveOptions(OutputFolder, sourceFolderPath, expectedDayRange: expectedDayRange);
	}

	public static ArchiveOptions WithDeleteSource(string outputFolderPath, string sourceFolderPath)
	{
		return new ArchiveOptions(outputFolderPath, sourceFolderPath, deleteSource: true);
	}

	public static ArchiveOptions Create(
		// Required
		string outputPath = ValidOutputPath,
		// Optional
		string? inputPath = null,
		bool isDryRun = false, ArchiveInvalidFormatAction invalidFileFormatAction = ArchiveInvalidFormatAction.Continue,
		ArchiveNoPhotoTakenDateAction noPhotoTakenDateAction = ArchiveNoPhotoTakenDateAction.Continue, ArchiveNoCoordinateAction noCoordinateAction = ArchiveNoCoordinateAction.Continue,
		short? expectedDayRange = null, ArchiveAlbumType? albumType = null, string? albumNameNew = null, int? albumIdUpdate = null, bool autoReverseGeocodeAlbum = false, bool deleteSource = false,
		// ReverseGeocode - Shared
		ReverseGeocodeProvider reverseGeoCodeProvider = ReverseGeocodeProvider.Disabled, string? bigDataCloudApiKey = null, IEnumerable<int>? bigDataCloudAdminLevels = null,
		IEnumerable<string>? googleMapsAddressTypes = null, string? googleMapsApiKey = null, IEnumerable<string>? openStreetMapProperties = null,
		string? locationIqApiKey = null, bool? hasPaidLicense = null, string? language = null, MissingReverseGeocodeAction missingReverseGeocodeAction = MissingReverseGeocodeAction.Continue
		)
	{
		return new ArchiveOptions(outputPath, inputPath, isDryRun, invalidFileFormatAction, noPhotoTakenDateAction, noCoordinateAction, expectedDayRange, albumType, albumNameNew, albumIdUpdate,
			autoReverseGeocodeAlbum, deleteSource, reverseGeoCodeProvider, bigDataCloudApiKey, bigDataCloudAdminLevels, googleMapsAddressTypes, googleMapsApiKey, openStreetMapProperties,
			locationIqApiKey, hasPaidLicense, language, missingReverseGeocodeAction);
	}

	private const string ValidOutputPath = "output-folder";
}
