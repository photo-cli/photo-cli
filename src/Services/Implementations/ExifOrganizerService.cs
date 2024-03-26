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

	public (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterAndSortByNoActionTypes(IReadOnlyCollection<Photo> photoInfos, CopyInvalidFormatAction invalidFormatAction,
		CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction, CopyNoCoordinateAction noCoordinateAction, string targetRelativeDirectoryPath)
	{
		IReadOnlyCollection<Photo> filteredAndSorted = photoInfos;
		var keptFilesNotInFilter = new List<Photo>();

		_logger.LogDebug("Start filtering and sorting on {TargetFolder} with {PhotoCount} photos by invalid format action: {InvalidFormatAction}, no coordinate action: {NoCoordinateAction}",
			targetRelativeDirectoryPath, photoInfos.Count, invalidFormatAction, noCoordinateAction);

		if (_invalidFormatActionsToFilter.Contains(invalidFormatAction))
		{
			var (filteredByValidFiles, keptInvalidFiles) = FilterByInvalidFormatAction(filteredAndSorted, invalidFormatAction);
			_logger.LogDebug("Filtered by no invalid format action: Filtered to {FilterToCount}, kept not in filter {KeptNotInFilterCount}", filteredByValidFiles.Count, keptInvalidFiles.Count);
			filteredAndSorted = filteredByValidFiles;
			keptFilesNotInFilter.AddRange(keptInvalidFiles);
		}

		if (_noCoordinateActionsToFilter.Contains(noCoordinateAction))
		{
			var (filteredByCoordinates, keptDontHaveCoordinates) = FilterByNoCoordinateAction(filteredAndSorted, noCoordinateAction);
			_logger.LogDebug("Filtered by no coordinate action: Filtered to {FilterToCount}, kept not in filter {KeptNotInFilterCount}", filteredByCoordinates.Count, keptDontHaveCoordinates.Count);
			filteredAndSorted = filteredByCoordinates;
			keptFilesNotInFilter.AddRange(keptDontHaveCoordinates);
		}

		if (_noPhotoTakenActionsToFilter.Contains(noPhotoDateTimeTakenAction))
		{
			var (photoTakenFilteredAndOrderedPhotos, keptDontHavePhotoTakenDate) = FilterAndSortByNoPhotoDateTimeTakenAction(filteredAndSorted, noPhotoDateTimeTakenAction);
			_logger.LogDebug("Filtered by no taken date action: Filtered to {FilterToCount}, kept not in filter {KeptNotInFilterCount}", photoTakenFilteredAndOrderedPhotos.Count, keptDontHavePhotoTakenDate.Count);
			filteredAndSorted = photoTakenFilteredAndOrderedPhotos;
			keptFilesNotInFilter.AddRange(keptDontHavePhotoTakenDate);
		}
		else
		{
			_logger.LogDebug("No taken date action as continue, just sort by photo date");
			filteredAndSorted = SortByPhotoDateTime(filteredAndSorted);
		}

		return (filteredAndSorted, keptFilesNotInFilter);
	}

	private (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterAndSortByNoPhotoDateTimeTakenAction(IReadOnlyCollection<Photo> photoInfos, CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction)
	{
		var withDateOrdered = FilterAndOrderPhotosWithTakenDateTime(photoInfos);
		var noDateOrderedByFileName = FilterAndOrderPhotosWithoutTakenDateTime(photoInfos);
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

	private (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterByInvalidFormatAction(IReadOnlyCollection<Photo> photoInfos, CopyInvalidFormatAction invalidFormatAction)
	{
		var validFiles = photoInfos.Where(w => w.HasExifData).ToList();
		var keptInvalidFiles = photoInfos.Where(w => !w.HasExifData).ToList();
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


	private (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterByNoCoordinateAction(IReadOnlyCollection<Photo> photoInfos, CopyNoCoordinateAction noCoordinateAction)
	{
		var withCoordinates = photoInfos.Where(w => w.HasCoordinate).ToList();
		var withoutCoordinates = photoInfos.Where(w => !w.HasCoordinate).ToList();
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

	private IReadOnlyCollection<Photo> SortByPhotoDateTime(IReadOnlyCollection<Photo> photoInfos)
	{
		var withDateOrdered = FilterAndOrderPhotosWithTakenDateTime(photoInfos).ToList();
		var noDateOrderedByFileName = FilterAndOrderPhotosWithoutTakenDateTime(photoInfos);
		withDateOrdered.AddRange(noDateOrderedByFileName);
		return withDateOrdered;
	}

	private IEnumerable<Photo> FilterAndOrderPhotosWithTakenDateTime(IEnumerable<Photo> photoInfos)
	{
		return photoInfos.Where(w => w.PhotoTakenDateTime.HasValue).OrderBy(o => o.PhotoTakenDateTime).ThenBy(t => t.FileNameWithoutExtension).ThenBy(t => t.FilePath);
	}

	private IEnumerable<Photo> FilterAndOrderPhotosWithoutTakenDateTime(IEnumerable<Photo> photoInfos)
	{
		return photoInfos.Where(w => !w.PhotoTakenDateTime.HasValue).OrderBy(o => o.FileNameWithoutExtension).ThenBy(t => t.FilePath);
	}
}
