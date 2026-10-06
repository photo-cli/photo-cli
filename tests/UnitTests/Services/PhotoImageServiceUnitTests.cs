using System.IO.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace PhotoCli.Tests.UnitTests.Services;

public class PhotoImageServiceUnitTests
{
	private const int JpegQuality = 80;

	private static string ArchivePath => MockFileSystemHelper.Path("/archive");

	private static PhotoImageService CreateSut(IFileSystem fileSystem, string? archivePath)
	{
		return new PhotoImageService(fileSystem, new McpOptions(archivePath), NullLogger<PhotoImageService>.Instance);
	}

	private static byte[] CreateJpeg(int width, int height)
	{
		using var image = new Image<Rgba32>(width, height);
		using var memoryStream = new MemoryStream();
		image.SaveAsJpeg(memoryStream, new JpegEncoder { Quality = 90 });
		return memoryStream.ToArray();
	}

	private static (MockFileSystem FileSystem, string RelativePath) FileSystemWithPhoto(byte[] bytes)
	{
		const string relativePath = "2020/06/15/photo.jpg";
		var fullPath = MockFileSystemHelper.Combine("/archive", "2020", "06", "15", "photo.jpg");
		var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { [fullPath] = new(bytes) });
		return (fileSystem, relativePath);
	}

	[Fact]
	public async Task LoadAndResize_GivenMissingArchivePath_ShouldReturnArchivePathMissing()
	{
		var sut = CreateSut(new MockFileSystem(), null);

		var result = await sut.LoadAndResize("photo.jpg", 1280, JpegQuality);

		result.Status.Should().Be(PhotoImageStatus.ArchivePathMissing);
	}

	[Fact]
	public async Task LoadAndResize_GivenFileNotOnDisk_ShouldReturnFileNotFound()
	{
		var sut = CreateSut(new MockFileSystem(), ArchivePath);

		var result = await sut.LoadAndResize("missing.jpg", 1280, JpegQuality);

		result.Status.Should().Be(PhotoImageStatus.FileNotFound);
	}

	[Fact]
	public async Task LoadAndResize_GivenImageLargerThanMaxDimension_ShouldDownscaleAndReturnJpeg()
	{
		var (fileSystem, relativePath) = FileSystemWithPhoto(CreateJpeg(4000, 3000));
		var sut = CreateSut(fileSystem, ArchivePath);

		var result = await sut.LoadAndResize(relativePath, 1280, JpegQuality);

		result.Status.Should().Be(PhotoImageStatus.Successful);
		result.MimeType.Should().Be("image/jpeg");
		using var resized = Image.Load(result.Bytes!);
		resized.Width.Should().Be(1280);
		resized.Height.Should().Be(960);
	}

	[Fact]
	public async Task LoadAndResize_GivenImageSmallerThanMaxDimension_ShouldNotUpscale()
	{
		var (fileSystem, relativePath) = FileSystemWithPhoto(CreateJpeg(800, 600));
		var sut = CreateSut(fileSystem, ArchivePath);

		var result = await sut.LoadAndResize(relativePath, 1280, JpegQuality);

		result.Status.Should().Be(PhotoImageStatus.Successful);
		using var output = Image.Load(result.Bytes!);
		output.Width.Should().Be(800);
		output.Height.Should().Be(600);
	}

	[Fact]
	public async Task LoadAndResize_GivenNonImageBytes_ShouldReturnUnsupportedFormat()
	{
		var (fileSystem, relativePath) = FileSystemWithPhoto("this is not an image"u8.ToArray());
		var sut = CreateSut(fileSystem, ArchivePath);

		var result = await sut.LoadAndResize(relativePath, 1280, JpegQuality);

		result.Status.Should().Be(PhotoImageStatus.UnsupportedFormat);
	}
}
