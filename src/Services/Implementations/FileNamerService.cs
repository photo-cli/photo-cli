namespace PhotoCli.Services.Implementations;

public class FileNamerService : IFileNamerService
{
	private readonly IExifDataGrouperService _exifDataGrouperService;
	private readonly ToolOptions _options;
	private readonly ILogger<FileNamerService> _logger;
	private readonly ISequentialNumberEnumeratorService _sequentialNumberEnumeratorService;

	public FileNamerService(ISequentialNumberEnumeratorService sequentialNumberEnumeratorService, IExifDataGrouperService exifDataGrouperService, ToolOptions options, ILogger<FileNamerService> logger)
	{
		_sequentialNumberEnumeratorService = sequentialNumberEnumeratorService;
		_exifDataGrouperService = exifDataGrouperService;
		_options = options;
		_logger = logger;
	}

	public IReadOnlyCollection<Photo> SetFileName(IReadOnlyCollection<Photo> orderedPhotos, NamingStyle namingStyle, NumberNamingTextStyle numberNamingTextStyle)
	{
		orderedPhotos.ThrowIfNotOrderedByPhotoTakenDate();
		if (namingStyle is NamingStyle.Numeric)
		{
			_logger.LogTrace("Using numeric numerator for {PhotoCount}", orderedPhotos.Count);
			using var numberIterator = _sequentialNumberEnumeratorService.NumberIterator(orderedPhotos.Count, numberNamingTextStyle).GetEnumerator();
			foreach (var photoInfo in orderedPhotos)
			{
				numberIterator.MoveNext();
				photoInfo.SetNewName(numberIterator.Current);
			}
		}
		else
		{
			_logger.LogTrace("Grouping {PhotoCount} photos with {NamingStyle}", orderedPhotos.Count, namingStyle);
			var photosGroupedByDateTime = _exifDataGrouperService.Group(orderedPhotos, namingStyle);
			foreach (var (groupKey, photos) in photosGroupedByDateTime)
			{
				if (photos.Count == 1)
				{
					photos.Single().SetNewName(groupKey);
					_logger.LogTrace("Single photo grouped with key {GroupKey}. Directly used group key as name, no number iteration needed", groupKey);
				}
				else
				{
					_logger.LogTrace("{PhotoCount} photos groped with key {GroupKey}. Numerate files with number naming strategy {NumberStrategy}", photos.Count, groupKey, numberNamingTextStyle);
					using var numberIterator = _sequentialNumberEnumeratorService.NumberIterator(photos.Count, numberNamingTextStyle).GetEnumerator();
					foreach (var photoInfo in photos)
					{
						numberIterator.MoveNext();
						photoInfo.SetNewName($"{groupKey}{_options.SameNameNumberSeparator}{numberIterator.Current}");
					}
				}
			}
		}
		return orderedPhotos;
	}

	public IReadOnlyCollection<Photo> SetArchiveFileName(IReadOnlyCollection<Photo> orderedPhotos)
	{
		foreach (var photo in orderedPhotos)
		{
			if (photo.PhotoFile.Sha1Hash.IsPresent())
			{
				if (photo.TakenDateTime.HasValue)
					photo.SetNewName(photo.TakenDateTime.Value.ToString(_options.DateTimeFormatWithSeconds) + _options.ArchivePhotoTakenDateHashSeparator + photo.PhotoFile.Sha1Hash);
				else
					photo.SetNewName(photo.PhotoFile.Sha1Hash);
			}
			else
			{
				throw new PhotoCliException($"Couldn't set file name for {photo.PhotoFile.SourceFullPath} without Sha1Hash is set");
			}
		}
		return orderedPhotos;
	}
}
