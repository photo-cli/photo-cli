namespace PhotoCli.Services.Implementations;

public class ExifOrganizerService : IExifOrganizerService
{
	private readonly ILogger<ExifOrganizerService> _logger;

	public ExifOrganizerService(ILogger<ExifOrganizerService> logger)
	{
		_logger = logger;
	}

	public (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterAndSortByNoActionTypes(IReadOnlyCollection<Photo> photoInfos, CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction,
		CopyNoCoordinateAction noCoordinateAction)
	{
		IReadOnlyCollection<Photo> notToRenamePhotos = new List<Photo>();
		if (noCoordinateAction != CopyNoCoordinateAction.Continue)
		{
			var (coordinateOrderedPhotos, coordinateNotToRenamePhotos) = FilterByNoCoordinateAction(photoInfos, noCoordinateAction);
			_logger.LogTrace("Filtered by no coordinate action; To rename count {ToRenameCount}, to not rename count {ToNotRenameCount}", coordinateOrderedPhotos.Count,
				coordinateNotToRenamePhotos.Count);
			photoInfos = coordinateOrderedPhotos;
			notToRenamePhotos = coordinateNotToRenamePhotos;
		}

		if (noPhotoDateTimeTakenAction != CopyNoPhotoTakenDateAction.Continue)
		{
			var (photoTakenOrderedPhotos, photoTakenNotToRenamePhotos) = FilterAndSortByNoPhotoDateTimeTakenAction(photoInfos, notToRenamePhotos, noPhotoDateTimeTakenAction);
			_logger.LogTrace("Filtered by no taken date action; To rename count {ToRenameCount}, to not rename count {ToNotRenameCount}", photoTakenOrderedPhotos.Count, photoTakenOrderedPhotos.Count);
			photoInfos = photoTakenOrderedPhotos;
			notToRenamePhotos = photoTakenNotToRenamePhotos;
		}
		else
		{
			_logger.LogTrace("No taken date action as continue, just sort by photo date");
			photoInfos = SortByPhotoDateTime(photoInfos);
		}

		return (photoInfos, notToRenamePhotos);
	}

	private (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterAndSortByNoPhotoDateTimeTakenAction(IReadOnlyCollection<Photo> photoInfos, IReadOnlyCollection<Photo> notToRenamePhotos,
		CopyNoPhotoTakenDateAction? noPhotoDateTimeTakenAction)
	{
		var withDateOrdered = FilterAndOrderPhotosWithTakenDateTime(photoInfos);
		var noDateOrderedByFileName = FilterAndOrderPhotosWithoutTakenDateTime(photoInfos);
		List<Photo> photosOrdered;
		List<Photo> photosNotToRename = new(notToRenamePhotos);

		switch (noPhotoDateTimeTakenAction)
		{
			case CopyNoPhotoTakenDateAction.DontCopyToOutput:
				photosOrdered = withDateOrdered.ToList();
				photosNotToRename = photosNotToRename.Where(w => w.HasPhotoTakenDateTime).ToList();
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

	private (IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterByNoCoordinateAction(IReadOnlyCollection<Photo> photoInfos, CopyNoCoordinateAction? noCoordinateAction)
	{
		var withCoordinates = photoInfos.Where(w => w.HasCoordinate).ToList();
		var withoutCoordinates = photoInfos.Where(w => !w.HasCoordinate).ToList();
		switch (noCoordinateAction)
		{
			case CopyNoCoordinateAction.InSubFolder or null:
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
