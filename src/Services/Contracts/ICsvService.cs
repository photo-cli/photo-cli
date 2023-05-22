namespace PhotoCli.Services.Contracts;

public interface ICsvService
{
	Task Report(IEnumerable<Photo> photos, string outputPath, bool isDryRun = false);
	Task WriteExifDataToCsvOutput(Dictionary<string, ExifData?> photoExifDataByPath, string outputFile);
}
