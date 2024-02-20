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
		return new ArchiveOptions(outputFolder, sourceFolderPath, reverseGeoCodeProvider: ReverseGeocodeProvider.BigDataCloud);
	}

	public static ArchiveOptions WithoutReverseGeocode(string outputFolder, string sourceFolderPath)
	{
		return new ArchiveOptions(outputFolder, sourceFolderPath);
	}

	public static ArchiveOptions WithDryRun(string outputFolderPath, string sourceFolderPath)
	{
		return new ArchiveOptions(outputFolderPath, sourceFolderPath, isDryRun: true);
	}
}
