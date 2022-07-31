namespace PhotoCli.Services.Contracts;

public interface IExifParserService
{
	ExifData Parse(string filePath, bool parseDateTime, bool parseCoordinate);
}
