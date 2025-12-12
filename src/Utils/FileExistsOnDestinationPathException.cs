namespace PhotoCli.Utils;

public class FileExistsOnDestinationPathException : IOException
{
	public PhotoFile PhotoFile { get; }

	public FileExistsOnDestinationPathException(PhotoFile photoFile)
	{
		PhotoFile = photoFile;
	}
}
