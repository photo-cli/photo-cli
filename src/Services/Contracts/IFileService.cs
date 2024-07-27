namespace PhotoCli.Services.Contracts;

public interface IFileService
{
	IReadOnlyCollection<Photo> Copy(IReadOnlyCollection<Photo> photos, string outputFolder, bool isDryRun);
	IReadOnlyCollection<Photo> CopyIfNotExists(IReadOnlyCollection<Photo> photos, string outputFolder, bool isDryRun);
	Task<bool> VerifyFileIntegrity(IEnumerable<Photo> photos);
	Task SaveGnuHashFileTree(IEnumerable<Photo> photos, string outputFolder);
	Task<IReadOnlyCollection<Photo>> CalculateFileHash(IReadOnlyCollection<Photo> photos);
}
