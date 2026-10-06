namespace PhotoCli.Models;

public enum PhotoImageStatus
{
	Successful,
	ArchivePathMissing,
	FileNotFound,
	UnsupportedFormat,
	LoadFailed,
}

public record PhotoImageResult(PhotoImageStatus Status, byte[]? Bytes = null, string? MimeType = null, string? ErrorMessage = null)
{
	public static PhotoImageResult Success(byte[] bytes, string mimeType) => new(PhotoImageStatus.Successful, bytes, mimeType);

	public static PhotoImageResult Failure(PhotoImageStatus status, string errorMessage) => new(status, ErrorMessage: errorMessage);
}
