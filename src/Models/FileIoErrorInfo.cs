namespace PhotoCli.Models;

public record FileIoErrorInfo
{
	public required string FilePath { get; init; }
	public required string ExceptionMessage { get; init; }
	public required string ExceptionType { get; init; }
}
