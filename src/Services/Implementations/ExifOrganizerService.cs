namespace PhotoCli.Services.Implementations;

public class ExifOrganizerService : IExifOrganizerService
{
	private readonly ILogger<ExifOrganizerService> _logger;
	private readonly CopyInvalidFormatAction[] _invalidFormatActionsToFilter = { CopyInvalidFormatAction.DontCopyToOutput, CopyInvalidFormatAction.InSubFolder };
	private readonly CopyNoCoordinateAction[] _noCoordinateActionsToFilter = { CopyNoCoordinateAction.DontCopyToOutput, CopyNoCoordinateAction.InSubFolder };
	private readonly CopyNoPhotoTakenDateAction[] _noPhotoTakenActionsToFilter = {
		CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoPhotoTakenDateAction.InSubFolder,
		CopyNoPhotoTakenDateAction.AppendToEndOrderByFileName, CopyNoPhotoTakenDateAction.InsertToBeginningOrderByFileName
	};

	public ExifOrganizerService(ILogger<ExifOrganizerService> logger)
	{
		_logger = logger;
	}

	public (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterAndSortByNoActionTypes(IReadOnlyCollection<Photo> photos, CopyInvalidFormatAction invalidFormatAction,
		CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction, CopyNoCoordinateAction noCoordinateAction, string targetRelativeDirectoryPath)
	{
		IReadOnlyCollection<Photo> filteredAndSortedInternal = photos;
		var keptFilesNotInFilterInternal = new List<Photo>();

		_logger.LogDebug("Start filtering and sorting on {TargetFolder} with {PhotoCount} photos by invalid format action: {InvalidFormatAction}, no coordinate action: {NoCoordinateAction}",
			targetRelativeDirectoryPath, photos.Count, invalidFormatAction, noCoordinateAction);

		if (_invalidFormatActionsToFilter.Contains(invalidFormatAction))
		{
			var (filteredByValidFiles, keptInvalidFiles) = FilterByInvalidFormatAction(filteredAndSortedInternal, invalidFormatAction);
			_logger.LogDebug("Filtered by no invalid format action: Filtered to {FilterToCount}, kept not in filter {KeptNotInFilterCount}", filteredByValidFiles.Count, keptInvalidFiles.Count);
			filteredAndSortedInternal = filteredByValidFiles;
			keptFilesNotInFilterInternal.AddRange(keptInvalidFiles);
		}

		if (_noCoordinateActionsToFilter.Contains(noCoordinateAction))
		{
			var (filteredByCoordinates, keptDontHaveCoordinates) = FilterByNoCoordinateAction(filteredAndSortedInternal, noCoordinateAction);
			_logger.LogDebug("Filtered by no coordinate action: Filtered to {FilterToCount}, kept not in filter {KeptNotInFilterCount}", filteredByCoordinates.Count, keptDontHaveCoordinates.Count);
			filteredAndSortedInternal = filteredByCoordinates;
			keptFilesNotInFilterInternal.AddRange(keptDontHaveCoordinates);
		}

		if (_noPhotoTakenActionsToFilter.Contains(noPhotoDateTimeTakenAction))
		{
			var (photoTakenFilteredAndOrderedPhotos, keptDontHavePhotoTakenDate) = FilterAndSortByNoPhotoDateTimeTakenAction(filteredAndSortedInternal, noPhotoDateTimeTakenAction);
			_logger.LogDebug("Filtered by no taken date action: Filtered to {FilterToCount}, kept not in filter {KeptNotInFilterCount}", photoTakenFilteredAndOrderedPhotos.Count, keptDontHavePhotoTakenDate.Count);
			filteredAndSortedInternal = photoTakenFilteredAndOrderedPhotos;
			keptFilesNotInFilterInternal.AddRange(keptDontHavePhotoTakenDate);
		}
		else
		{
			_logger.LogDebug("No taken date action as continue, just sort by photo date");
			filteredAndSortedInternal = SortByPhotoDateTime(filteredAndSortedInternal);
		}

		var filteredAndSortedReadOnly = filteredAndSortedInternal.ToList().AsReadOnly();
		var keptFilesNotInFilterReadOnly = keptFilesNotInFilterInternal.AsReadOnly();
		return (filteredAndSortedReadOnly, keptFilesNotInFilterReadOnly);
	}

	private (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterAndSortByNoPhotoDateTimeTakenAction(IReadOnlyCollection<Photo> photos, CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction)
	{
		var withDateOrdered = FilterAndOrderPhotosWithTakenDateTime(photos);
		var noDateOrderedByFileName = FilterAndOrderPhotosWithoutTakenDateTime(photos);
		List<Photo> photosOrdered;
		List<Photo> photosNotToRename = new List<Photo>();

		switch (noPhotoDateTimeTakenAction)
		{
			case CopyNoPhotoTakenDateAction.DontCopyToOutput:
				photosOrdered = withDateOrdered.ToList();
				break;
			case CopyNoPhotoTakenDateAction.AppendToEndOrderByFileName or CopyNoPhotoTakenDateAction.InSubFolder:
				photosOrdered = withDateOrdered.ToList();
				photosOrdered.AddRange(noDateOrderedByFileName);
				break;
			case CopyNoPhotoTakenDateAction.InsertToBeginningOrderByFileName:
				photosOrdered = noDateOrderedByFileName.ToList();
				photosOrdered.AddRange(withDateOrdered);
				break;
			default:
				throw new PhotoCliException($"Not implemented {nameof(CopyNoPhotoTakenDateAction)}: {noPhotoDateTimeTakenAction}");
		}

		return (photosOrdered, photosNotToRename);
	}

	private (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterByInvalidFormatAction(IReadOnlyCollection<Photo> photos, CopyInvalidFormatAction invalidFormatAction)
	{
		var validFiles = photos.Where(w => w.HasExifData).ToList();
		var keptInvalidFiles = photos.Where(w => !w.HasExifData).ToList();
		switch (invalidFormatAction)
		{
			case CopyInvalidFormatAction.InSubFolder:
				return (validFiles, keptInvalidFiles);
			case CopyInvalidFormatAction.DontCopyToOutput:
				return (validFiles, new List<Photo>());
			default:
				throw new PhotoCliException($"Not implemented {nameof(CopyInvalidFormatAction)}: {invalidFormatAction}");
		}
	}


	private (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterByNoCoordinateAction(IReadOnlyCollection<Photo> photos, CopyNoCoordinateAction noCoordinateAction)
	{
		var withCoordinates = photos.Where(w => w.HasCoordinate).ToList();
		var withoutCoordinates = photos.Where(w => !w.HasCoordinate).ToList();
		switch (noCoordinateAction)
		{
			case CopyNoCoordinateAction.InSubFolder:
				return (withCoordinates, withoutCoordinates);
			case CopyNoCoordinateAction.DontCopyToOutput:
				return (withCoordinates, new List<Photo>());
			default:
				throw new PhotoCliException($"Not implemented {nameof(CopyNoCoordinateAction)}: {noCoordinateAction}");
		}
	}

	private IReadOnlyCollection<Photo> SortByPhotoDateTime(IReadOnlyCollection<Photo> photos)
	{
		var withDateOrdered = FilterAndOrderPhotosWithTakenDateTime(photos).ToList();
		var noDateOrderedByFileName = FilterAndOrderPhotosWithoutTakenDateTime(photos);
		withDateOrdered.AddRange(noDateOrderedByFileName);
		return withDateOrdered;
	}

	private IOrderedEnumerable<Photo> FilterAndOrderPhotosWithTakenDateTime(IEnumerable<Photo> photos)
	{
		return photos.Where(w => w.HasTakenDateTime).OrderBy(o => o.TakenDateTime).ThenBy(t => t.PhotoFile.FileName).ThenBy(t => t.PhotoFile.SourcePath);
	}

	private IOrderedEnumerable<Photo> FilterAndOrderPhotosWithoutTakenDateTime(IEnumerable<Photo> photos)
	{
		return photos.Where(w => !w.HasTakenDateTime).OrderBy(o => o.PhotoFile.FileName).ThenBy(t => t.PhotoFile.SourcePath);
	}
}
