namespace PhotoCli.Services.Contracts;

public interface IFileNamerService
{
	void SetFileName(IReadOnlyCollection<Photo> orderedPhotoInfos, NamingStyle namingStyle, NumberNamingTextStyle numberNamingTextStyle);
	void SetArchiveFileName(IReadOnlyCollection<Photo> orderedPhotoInfos);
}
