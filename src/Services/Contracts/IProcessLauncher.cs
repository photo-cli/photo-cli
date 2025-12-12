namespace PhotoCli.Services.Contracts;

public interface IProcessLauncher
{
	Task Launch(IEnumerable<string> filePaths);
}
