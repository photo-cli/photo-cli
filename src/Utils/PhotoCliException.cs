namespace PhotoCli.Utils;

public class PhotoCliException : Exception
{
	public PhotoCliException()
	{
	}

	public PhotoCliException(string message) : base(message)
	{
	}

	public PhotoCliException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
