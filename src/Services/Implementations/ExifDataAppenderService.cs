namespace PhotoCli.Services.Implementations;

public class ExifDataAppenderService : IExifDataAppenderService
{
	private const string ProgressName = "Parsing photo exif information";
	private readonly IConsoleWriter _consoleWriter;
	private readonly IExifParserService _exifParserService;
	private readonly Statistics _statistics;

	public ExifDataAppenderService(IExifParserService exifParserService, Statistics statistics, IConsoleWriter consoleWriter)
	{
		_exifParserService = exifParserService;
		_statistics = statistics;
		_consoleWriter = consoleWriter;
	}

	public Dictionary<string, ExifData> ExifDataByPath(IEnumerable<string> photoPaths, out bool allPhotosHasPhotoTaken, out bool allPhotosHasCoordinate)
	{
		_consoleWriter.ProgressStart(ProgressName, _statistics.PhotosFound);
		var photosHasPhotoTaken = true;
		var photosHasCoordinate = true;
		var filePathInfo = new Dictionary<string, ExifData>();
		foreach (var photoPath in photoPaths)
		{
			var photoExifData = _exifParserService.Parse(photoPath, true, true);
			if (photosHasPhotoTaken && !photoExifData.TakenDate.HasValue)
				photosHasPhotoTaken = false;
			if (photosHasCoordinate && photoExifData.Coordinate == null)
				photosHasCoordinate = false;
			filePathInfo.Add(photoPath, photoExifData);
			_consoleWriter.InProgressItemComplete(ProgressName);
		}

		_consoleWriter.ProgressFinish(ProgressName);
		allPhotosHasPhotoTaken = photosHasPhotoTaken;
		allPhotosHasCoordinate = photosHasCoordinate;
		return filePathInfo;
	}
}
