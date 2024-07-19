namespace PhotoCli.Services.Contracts;

public interface IExifDataAppenderService
{
	IReadOnlyCollection<Photo> ExtractExifData(IReadOnlyCollection<Photo> photos, out bool allPhotosAreValid, out bool allPhotosHasPhotoTaken, out bool allPhotosHasCoordinate);
}
