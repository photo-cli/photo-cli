using System.IO.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace PhotoCli.Services.Implementations;

public class PhotoImageService : IPhotoImageService
{
	private const string JpegMimeType = "image/jpeg";

	private readonly IFileSystem _fileSystem;
	private readonly McpOptions _mcpOptions;
	private readonly ILogger<PhotoImageService> _logger;

	public PhotoImageService(IFileSystem fileSystem, McpOptions mcpOptions, ILogger<PhotoImageService> logger)
	{
		_fileSystem = fileSystem;
		_mcpOptions = mcpOptions;
		_logger = logger;
	}

	public async Task<PhotoImageResult> LoadAndResize(string relativePath, int maxDimension, int jpegQuality, CancellationToken cancellationToken = default)
	{
		if (_mcpOptions.ArchivePath.IsMissing())
			return PhotoImageResult.Failure(PhotoImageStatus.ArchivePathMissing, "Archive path is missing.");

		var fullPath = _fileSystem.Path.Combine(_mcpOptions.ArchivePath, relativePath);
		if (!_fileSystem.File.Exists(fullPath))
			return PhotoImageResult.Failure(PhotoImageStatus.FileNotFound, $"Photo file not found on disk: {relativePath}");

		try
		{
			await using var inputStream = _fileSystem.File.OpenRead(fullPath);
			using var image = await Image.LoadAsync(inputStream, cancellationToken);

			if (image.Width > maxDimension || image.Height > maxDimension)
			{
				image.Mutate(x => x.Resize(new ResizeOptions
				{
					Size = new Size(maxDimension, maxDimension),
					Mode = ResizeMode.Max,
				}));
			}

			using var outputStream = new MemoryStream();
			await image.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = jpegQuality }, cancellationToken);
			return PhotoImageResult.Success(outputStream.ToArray(), JpegMimeType);
		}
		catch (Exception ex) when (ex is ImageFormatException or NotSupportedException)
		{
			_logger.LogWarning(ex, "Unsupported image format, cannot inline {RelativePath}", relativePath);
			return PhotoImageResult.Failure(PhotoImageStatus.UnsupportedFormat, $"Unsupported image format (cannot inline): {relativePath}");
		}
		catch (Exception ex) when (ex is IOException or ImageProcessingException)
		{
			_logger.LogWarning(ex, "Failed to load image {RelativePath}", relativePath);
			return PhotoImageResult.Failure(PhotoImageStatus.LoadFailed, $"Failed to load image: {relativePath}");
		}
	}
}
