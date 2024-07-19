namespace PhotoCli.Services.Contracts;

public interface IExifOrganizerService
{
	(IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>) FilterAndSortByNoActionTypes(IReadOnlyCollection<Photo> photos, CopyInvalidFormatAction invalidFormatAction,
		CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction, CopyNoCoordinateAction noCoordinateAction, string targetRelativeDirectoryPath);
}
