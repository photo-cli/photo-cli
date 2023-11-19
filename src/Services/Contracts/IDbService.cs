namespace PhotoCli.Services.Contracts;

public interface IDbService
{
	Task<int> Archive(IEnumerable<Photo> photos, bool isDryRun = false);
}
