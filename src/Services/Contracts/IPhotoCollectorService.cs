namespace PhotoCli.Services.Contracts;

public interface IPhotoCollectorService
{
	string[] Collect(string folderPath, bool allDirectories);
}
