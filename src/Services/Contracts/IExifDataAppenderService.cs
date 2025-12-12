namespace PhotoCli.Services.Contracts;

public interface IExifDataAppenderService
{
	ExifDataResult ExtractExifData(IReadOnlyCollection<Photo> photos);
}
