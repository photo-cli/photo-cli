namespace PhotoCli.Tests.Fakes.Options;

public static class InfoOptionsFakes
{
	private const string OutputFolder = "output-folder";

	public static InfoOptions Valid()
	{
		return new InfoOptions(OutputFolder);
	}

	public static InfoOptions WithPaths(string outputFolder, string sourceFolderPath)
	{
		return new InfoOptions(outputFolder, sourceFolderPath);
	}

	public static InfoOptions WithNoExifDataAction(string sourceFolderPath, InfoNoPhotoTakenDateAction noPhotoDateTimeTakenAction = InfoNoPhotoTakenDateAction.Continue,
		InfoNoCoordinateAction noCoordinateAction = InfoNoCoordinateAction.Continue)
	{
		return new InfoOptions(OutputFolder, sourceFolderPath, noPhotoTakenDateAction: noPhotoDateTimeTakenAction, noCoordinateAction: noCoordinateAction);
	}

	public static InfoOptions WithValidReverseGeocodeService(string outputFolder, string sourceFolderPath)
	{
		return new InfoOptions(outputFolder, sourceFolderPath, reverseGeoCodeProvider: ReverseGeocodeProvider.BigDataCloud);
	}

	public static InfoOptions WithoutReverseGeocode(string outputFolder, string sourceFolderPath)
	{
		return new InfoOptions(outputFolder, sourceFolderPath);
	}

	public static InfoOptions WithReverseGeocodeService(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		return new InfoOptions(OutputFolder, reverseGeoCodeProvider: reverseGeocodeProvider);
	}

	public static InfoOptions ValidReverseGeocodeServiceWithLicense(bool hasPaidLicense, ReverseGeocodeProvider reverseGeocodeProvider)
	{
		return new InfoOptions(OutputFolder, reverseGeoCodeProvider: reverseGeocodeProvider, hasPaidLicense: hasPaidLicense);
	}
}
