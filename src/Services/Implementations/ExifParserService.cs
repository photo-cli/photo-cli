using System.IO.Abstractions;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Directory = MetadataExtractor.Directory;

namespace PhotoCli.Services.Implementations;

public class ExifParserService : IExifParserService
{
	private readonly IFileSystem _fileSystem;
	private readonly ILogger<ExifParserService> _logger;
	private readonly ToolOptions _options;
	private readonly Statistics _statistics;

	public ExifParserService(ILogger<ExifParserService> logger, IFileSystem fileSystem, ToolOptions options, Statistics statistics)
	{
		_logger = logger;
		_fileSystem = fileSystem;
		_options = options;
		_statistics = statistics;
	}

	public ExifData Parse(string filePath, bool parseDateTime, bool parseCoordinate)
	{
		var fileStream = _fileSystem.FileStream.Create(filePath, FileMode.Open);
		IReadOnlyList<Directory> fileDataDirectories;
		using (fileStream)
			fileDataDirectories = ImageMetadataReader.ReadMetadata(fileStream);
		DateTime? photoTaken = null;
		if (parseDateTime)
			photoTaken = ParseExifSubIfdDirectory(fileDataDirectories, filePath) ?? ParseExifIfd0Directory(fileDataDirectories, filePath);

		Coordinate? coordinate = null;
		if (parseCoordinate)
			coordinate = ParseCoordinate(fileDataDirectories, filePath);

		if (photoTaken.HasValue && coordinate != null)
			++_statistics.PhotoThatHasTakenDateAndCoordinate;
		else if (photoTaken.HasValue)
			++_statistics.PhotoThatHasTakenDateButNoCoordinate;
		else if (coordinate != null)
			++_statistics.PhotoThatHasCoordinateButNoTakenDate;
		else
			++_statistics.PhotoThatNoCoordinateAndNoTakenDate;

		return new ExifData(photoTaken, coordinate, _options.AddressSeparator);
	}

	private Coordinate? ParseCoordinate(IEnumerable<Directory> fileDataDirectories, string filePath)
	{
		var gpsDirectory = fileDataDirectories.OfType<GpsDirectory>().SingleOrDefault();
		var geoLocation = gpsDirectory?.GetGeoLocation();
		if (geoLocation != null)
			return new Coordinate(geoLocation.Latitude, geoLocation.Longitude);
		_logger.LogWarning("No coordinate found on `Gps` directory for {FilePath}", filePath);
		return null;
	}

	private DateTime? ParseExifSubIfdDirectory(IEnumerable<Directory> fileDataDirectories, string filePath)
	{
		_logger.LogDebug("First looking for `ExifSubIfd` directory for {FilePath}", filePath);
		var exifSubIfdDirectory = fileDataDirectories.OfType<ExifSubIfdDirectory>().SingleOrDefault();
		if (exifSubIfdDirectory == null)
		{
			_logger.LogWarning("No `ExifSubIfd` directory found on {FilePath}", filePath);
			return null;
		}

		if (exifSubIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var parsedDateTime))
			return parsedDateTime;
		if (exifSubIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeDigitized, out parsedDateTime))
			return parsedDateTime;

		_logger.LogWarning("No datetime found on tags `TagDateTimeOriginal`, `TagDateTimeDigitized` in {FilePath}", filePath);
		return null;
	}

	private DateTime? ParseExifIfd0Directory(IEnumerable<Directory> fileDataDirectories, string filePath)
	{
		_logger.LogDebug("Alternatively looking for `ExifIfd0` directory for {FilePath}", filePath);
		var exifSubIfdDirectory = fileDataDirectories.OfType<ExifIfd0Directory>().SingleOrDefault();
		if (exifSubIfdDirectory == null)
		{
			_logger.LogWarning("No `ExifIfd0` directory found on {FilePath}", filePath);
			return null;
		}

		if (exifSubIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTime, out var parsedDateTime))
			return parsedDateTime;

		_logger.LogWarning("No datetime found on tag `TagDateTime` in {FilePath}", filePath);
		return null;
	}
}
