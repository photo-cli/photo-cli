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

	public void SetFileName(IReadOnlyCollection<Photo> orderedPhotoInfos, NamingStyle namingStyle, NumberNamingTextStyle numberNamingTextStyle)
	{
		orderedPhotoInfos.ThrowIfNotOrderedByPhotoTakenDate();
		if (namingStyle is NamingStyle.Numeric)
		{
			_logger.LogTrace("Using numeric numerator for {PhotoCount}", orderedPhotoInfos.Count);
			using var numberIterator = _sequentialNumberEnumeratorService.NumberIterator(orderedPhotoInfos.Count, numberNamingTextStyle).GetEnumerator();
			foreach (var photoInfo in orderedPhotoInfos)
			{
				numberIterator.MoveNext();
				photoInfo.NewName = numberIterator.Current;
			}
		}
		else
		{
			_logger.LogTrace("Grouping {PhotoCount} photos with {NamingStyle}", orderedPhotoInfos.Count, namingStyle);
			var photosGroupedByDateTime = _exifDataGrouperService.Group(orderedPhotoInfos, namingStyle);
			foreach (var (groupKey, photoInfos) in photosGroupedByDateTime)
			{
				if (photoInfos.Count == 1)
				{
					photoInfos.Single().NewName = groupKey;
					_logger.LogTrace("Single photo grouped with key {GroupKey}. Directly used group key as name, no number iteration needed", groupKey);
				}
				else
				{
					_logger.LogTrace("{PhotoCount} photos groped with key {GroupKey}. Numerate files with number naming strategy {NumberStrategy}", photoInfos.Count, groupKey, numberNamingTextStyle);
					using var numberIterator = _sequentialNumberEnumeratorService.NumberIterator(photoInfos.Count, numberNamingTextStyle).GetEnumerator();
					foreach (var photoInfo in photoInfos)
					{
						numberIterator.MoveNext();
						photoInfo.NewName = $"{groupKey}{_options.SameNameNumberSeparator}{numberIterator.Current}";
					}
				}
			}
		}
	}
}
