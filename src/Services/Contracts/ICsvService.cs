namespace PhotoCli.Services.Contracts;

public interface ICsvService
{
	Task Report(Dictionary<string, IReadOnlyCollection<Photo>> groupedPhotoInfosByRelativeDirectory, string outputPath, bool isDryRun = false);
	Task WriteExifDataToCsvOutput(Dictionary<string, ExifData> photoExifDataByPath, string outputFile);
}
