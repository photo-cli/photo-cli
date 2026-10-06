namespace PhotoCli.Services.Contracts;

/// <summary>
/// Loads a photo from the archive path and downscales / re-encodes it for inline delivery over MCP.
/// </summary>
public interface IPhotoImageService
{
	Task<PhotoImageResult> LoadAndResize(string relativePath, int maxDimension, int jpegQuality, CancellationToken cancellationToken = default);
}
