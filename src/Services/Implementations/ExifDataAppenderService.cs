namespace PhotoCli.Services.Implementations;

public class ExifDataAppenderService : IExifDataAppenderService
{
	private readonly IExifParserService _exifParserService;

	public ExifDataAppenderService(IExifParserService exifParserService)
	{
		_exifParserService = exifParserService;
	}

	public ExifDataResult ExtractExifData(IReadOnlyCollection<Photo> photos)
	{
		var photosAreValid = true;
		var photosHasPhotoTaken = true;
		var photosHasCoordinate = true;

		var minimumDate = DateTime.MaxValue;
		var maximumDate = DateTime.MinValue;

		foreach (var photo in photos)
		{
			var exifData = _exifParserService.Parse(photo.PhotoFile.SourcePath, true, true);
			if (exifData == null)
				photosAreValid = false;
			if (photosHasPhotoTaken && exifData?.TakenDate == null)
				photosHasPhotoTaken = false;
			if (photosHasCoordinate && exifData?.Coordinate == null)
				photosHasCoordinate = false;

			if (exifData != null)
			{
				photo.SetExifData(exifData);

				if (exifData.TakenDate != null)
				{
					if (exifData.TakenDate < minimumDate)
						minimumDate = exifData.TakenDate.Value;
					if (exifData.TakenDate > maximumDate)
						maximumDate = exifData.TakenDate.Value;
				}
			}
		}

		AlbumDateRange? dateRange;
		if (minimumDate != DateTime.MaxValue && maximumDate != DateTime.MinValue)
			dateRange = new AlbumDateRange(minimumDate, maximumDate);
		else
			dateRange = null;

		return new ExifDataResult(photos, photosAreValid, photosHasPhotoTaken, photosHasCoordinate, dateRange);
	}
}
