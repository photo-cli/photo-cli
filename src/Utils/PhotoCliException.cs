namespace PhotoCli.Utils;

public class PhotoCliException : Exception
{
	public PhotoCliException()
	{
	}

	public PhotoCliException(string? message) : base(message)
	{
	}
}
