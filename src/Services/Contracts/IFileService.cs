namespace PhotoCli.Services.Contracts;

public interface IFileService
{
	void Copy(IReadOnlyCollection<Photo> photos, string outputFolder, bool isDryRun);
	Task<bool> VerifyFileIntegrity(IEnumerable<Photo> photos, string outputFolder);
	Task SaveGnuHashFileTree(IEnumerable<Photo> photos, string outputFolder);
}
