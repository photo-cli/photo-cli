namespace PhotoCli.Services.Contracts;

public interface ICsvService
{
	Task CreateCopyReport(IEnumerable<Photo> photos, string outputPath, bool isDryRun = false);
	Task CreateInfoReport(IEnumerable<Photo> photos, string outputFile);
}
