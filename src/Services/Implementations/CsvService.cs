using System.IO.Abstractions;
using CsvHelper;

namespace PhotoCli.Services.Implementations;

public class CsvService : ICsvService
{
	private const string ProgressName = "Writing csv report";
	private readonly IFileSystem _fileSystem;
	private readonly ILogger<CsvService> _logger;
	private readonly ToolOptions _options;
	private readonly IConsoleWriter _consoleWriter;

	public CsvService(IFileSystem fileSystem, ILogger<CsvService> logger, ToolOptions options, IConsoleWriter consoleWriter)
	{
		_fileSystem = fileSystem;
		_logger = logger;
		_options = options;
		_consoleWriter = consoleWriter;
	}

	public async Task CreateCopyReport(IEnumerable<Photo> photos, string outputPath, bool isDryRun = false)
	{
		_consoleWriter.ProgressStart(ProgressName);
		string reportFile;
		if (isDryRun)
		{
			reportFile = _options.DryRunCsvReportFileName;
		}
		else
		{
			var directory = _fileSystem.DirectoryInfo.New(outputPath);
			if (!directory.Exists)
				directory.Create();
			reportFile = Path.Combine(outputPath, _options.CsvReportFileName);
		}

		await WritePhotoToCsvOutput(photos, reportFile);
		_consoleWriter.ProgressFinish(ProgressName);
	}

	public async Task CreateInfoReport(IEnumerable<Photo> photos, string outputFile)
	{
		_consoleWriter.ProgressStart(ProgressName);
		var photoCsvModels = new List<PhotoCsv>();
		foreach (var photo in photos)
			photoCsvModels.Add(Map(photo, false));
		await WritePhotoCsvReport(outputFile, photoCsvModels);
		_consoleWriter.ProgressFinish(ProgressName);
	}

	private async Task WritePhotoToCsvOutput(IEnumerable<Photo> photos, string outputFile)
	{
		var photoCsvModels = photos.Select(s => Map(s, true));
		await WritePhotoCsvReport(outputFile, photoCsvModels);
	}

	private async Task WritePhotoCsvReport(string outputFile, IEnumerable<PhotoCsv> photoCsvModels)
	{
		await using var fileStreamWrite = _fileSystem.File.OpenWrite(outputFile);
		await using var writer = new StreamWriter(fileStreamWrite);
		await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
		await csv.WriteRecordsAsync(photoCsvModels);
	}

	private static PhotoCsv Map(Photo photo, bool mapNewPath)
	{
		var takenDate = photo.ExifData?.TakenDate;
		var coordinate = photo.ExifData?.Coordinate;
		var reverseGeocodes = photo.ExifData?.ReverseGeocodes?.ToList();
		var newPath = mapNewPath ? photo.PhotoFile.TargetRelativePath : null;

		var photoCsv = new PhotoCsv(photo.PhotoFile.SourceFullPath, newPath, takenDate,
			photo.ExifData?.ReverseGeocodeFormatted, coordinate?.Latitude, coordinate?.Longitude,
			takenDate?.Year, takenDate?.Month, takenDate?.Day, takenDate?.Hour, takenDate?.Minute,
			takenDate?.Second, reverseGeocodes?.ElementAtOrDefault(0), reverseGeocodes?.ElementAtOrDefault(1), reverseGeocodes?.ElementAtOrDefault(2),
			reverseGeocodes?.ElementAtOrDefault(3), reverseGeocodes?.ElementAtOrDefault(4), reverseGeocodes?.ElementAtOrDefault(5), reverseGeocodes?.ElementAtOrDefault(6),
			reverseGeocodes?.ElementAtOrDefault(7), photo.PhotoFile.Sha1Hash);

		return photoCsv;
	}
}
