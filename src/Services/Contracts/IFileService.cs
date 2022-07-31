namespace PhotoCli.Services.Contracts;

public interface IFileService
{
	void Copy(IReadOnlyCollection<Photo> photos, string outputFolder, bool isDryRun);
}
