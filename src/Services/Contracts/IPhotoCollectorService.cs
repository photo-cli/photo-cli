namespace PhotoCli.Services.Contracts;

public interface IPhotoCollectorService
{
	IReadOnlyCollection<Photo> Collect(string folderPath, bool allDirectories, bool searchCompanionFiles);
}
