namespace PhotoCli.Services.Contracts;

public interface IExifOrganizerService
{
	(IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterAndSortByNoActionTypes(IReadOnlyCollection<Photo> photoInfos, CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction,
		CopyNoCoordinateAction noCoordinateAction);
}
