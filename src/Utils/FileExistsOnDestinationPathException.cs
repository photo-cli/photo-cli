namespace PhotoCli.Utils;

public class FileExistsOnDestinationPathException : PhotoCliException
{
	public string FilePath { get; }

	public FileExistsOnDestinationPathException(string filePath)
		: base("File exists on destination path")
	{
		FilePath = filePath;
	}
}
