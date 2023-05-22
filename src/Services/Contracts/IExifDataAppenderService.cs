namespace PhotoCli.Services.Contracts;

public interface IExifDataAppenderService
{
	Dictionary<string, ExifData?> ExifDataByPath(IEnumerable<string> photoPaths, out bool allPhotosAreValid, out bool allPhotosHasPhotoTaken, out bool allPhotosHasCoordinate);
}
