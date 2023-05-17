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

	public async Task Report(IEnumerable<Photo> photos, string outputPath, bool isDryRun = false)
	{
		_consoleWriter.ProgressStart(ProgressName);
		string reportFile;
		if (isDryRun)
		{
			reportFile = _options.DryRunCsvReportFileName;
		}
		else
		{
			var directory = _fileSystem.DirectoryInfo.FromDirectoryName(outputPath);
			if (!directory.Exists)
				directory.Create();
			reportFile = Path.Combine(outputPath, _options.CsvReportFileName);
		}

		await WritePhotoToCsvOutput(photos, reportFile, outputPath);
		_consoleWriter.ProgressFinish(ProgressName);
	}

	public async Task WriteExifDataToCsvOutput(Dictionary<string, ExifData> photoExifDataByPath, string outputFile)
	{
		_consoleWriter.ProgressStart(ProgressName);
		var photoCsvModels = new List<PhotoCsv>();
		foreach (var (photoPath, exifData) in photoExifDataByPath)
			photoCsvModels.Add(ExifDataToPhotoCsv(exifData, photoPath));
		await WritePhotoCsvReport(outputFile, photoCsvModels);
		_consoleWriter.ProgressFinish(ProgressName);
	}

	private async Task WritePhotoToCsvOutput(IEnumerable<Photo> photos, string outputFile, string outputFolder)
	{
		var photoCsvModels = photos.Select(photo => ExifDataToPhotoCsv(photo.PhotoExifData, photo.FilePath, photo.DestinationPath(outputFolder), photo.Sha1Hash));
		await WritePhotoCsvReport(outputFile, photoCsvModels);
	}

	private async Task WritePhotoCsvReport(string outputFile, IEnumerable<PhotoCsv> photoCsvModels)
	{
		await using var fileStreamWrite = _fileSystem.File.OpenWrite(outputFile);
		await using var writer = new StreamWriter(fileStreamWrite);
		await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
		await csv.WriteRecordsAsync(photoCsvModels);
	}

	private PhotoCsv ExifDataToPhotoCsv(ExifData exifData, string photoPath, string? photoNewPath = null, string? sha1Hash = null)
	{
		var takenDate = exifData.TakenDate;
		var coordinate = exifData.Coordinate;
		var reverseGeocodes = exifData.ReverseGeocodes?.ToList();
		var photoCsv = new PhotoCsv(photoPath, photoNewPath, takenDate, exifData.ReverseGeocodeFormatted, coordinate?.Latitude, coordinate?.Longitude, takenDate?.Year,
			takenDate?.Month, takenDate?.Day,
			takenDate?.Hour, takenDate?.Minute, takenDate?.Second, reverseGeocodes?.ElementAtOrDefault(0), reverseGeocodes?.ElementAtOrDefault(1), reverseGeocodes?.ElementAtOrDefault(2),
			reverseGeocodes?.ElementAtOrDefault(3), reverseGeocodes?.ElementAtOrDefault(4), reverseGeocodes?.ElementAtOrDefault(5), reverseGeocodes?.ElementAtOrDefault(6),
			reverseGeocodes?.ElementAtOrDefault(7), sha1Hash);
		return photoCsv;
	}
}
