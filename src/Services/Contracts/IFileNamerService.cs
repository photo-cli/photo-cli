namespace PhotoCli.Services.Contracts;

public interface IFileNamerService
{
	IReadOnlyCollection<Photo> SetFileName(IReadOnlyCollection<Photo> orderedPhotos, NamingStyle namingStyle, NumberNamingTextStyle numberNamingTextStyle);
	IReadOnlyCollection<Photo> SetArchiveFileName(IReadOnlyCollection<Photo> orderedPhotos);
}
