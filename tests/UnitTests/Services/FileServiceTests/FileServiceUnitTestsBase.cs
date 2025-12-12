using System.Runtime.InteropServices;

namespace PhotoCli.Tests.UnitTests.Services.FileServiceTests;

public class FileServiceUnitTestsBase
{
	#region Helpers

	protected const string DefaultOutputPath = "output-path";
	protected const string DefaultSourcePath = "source-path";

	protected static string SourcePath(string fileName)
	{
		return MockFileSystemHelper.Path(Path.Combine(DefaultSourcePath, fileName));
	}

	protected static string SubPathOnSourcePath(params string[] subPaths)
	{
		var paths = new List<string> { DefaultSourcePath };
		paths.AddRange(subPaths);
		return MockFileSystemHelper.Combine(paths.ToArray());
	}

	protected static List<Photo> CreatePhotosOnMockFileSystem(Dictionary<Photo, byte[]> inputFileContentByPhoto, IMockFileDataAccessor mockFileSystem)
	{
		var photos = new List<Photo>();
		foreach (var (photo, fileContent) in inputFileContentByPhoto)
		{
			photos.Add(photo);
			mockFileSystem.AddFile(photo.PhotoFile.SourcePath, new MockFileData(fileContent));
		}
		return photos;
	}

	protected static void CreateFilesOnMockFileSystem(Dictionary<string, byte[]> fileContentByPhoto, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var (filePath, fileContent) in fileContentByPhoto)
			mockFileSystem.AddFile(filePath, new MockFileData(fileContent));
	}

	protected static void SetupFileSystemWithDummyFiles(IEnumerable<Photo> photos, IMockFileDataAccessor mockFileSystem)
	{
		var filePaths = new List<string>();
		foreach (var photo in photos)
		{
			filePaths.Add(photo.PhotoFile.SourcePath);
			if (photo.CompanionFiles == null)
				continue;
			foreach (var companionFile in photo.CompanionFiles)
				filePaths.Add(companionFile.SourcePath);
		}
		SetupFileSystemWithDummyFiles(filePaths, mockFileSystem);
	}

	protected static void SetupFileSystemWithDummyFiles(IEnumerable<string> filePaths, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var filePath in filePaths)
			mockFileSystem.AddFile(filePath, string.Empty);
	}

	protected static void VerifyDirectoriesExistOnFileSystem(IEnumerable<string> directories, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var directory in directories)
		{
			mockFileSystem.Directory.Exists(directory).Should().BeTrue();
		}
	}

	protected static void VerifyDirectoriesNotExistOnFileSystem(IEnumerable<string> directories, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var directory in directories)
		{
			mockFileSystem.Directory.Exists(directory).Should().BeFalse();
		}
	}

	protected static void VerifyPhotoSourceFilesNotExistOnFileSystem(IEnumerable<Photo> photos, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var photo in photos)
		{
			mockFileSystem.FileExists(photo.PhotoFile.SourceFullPath).Should().BeFalse();
			if (photo.CompanionFiles == null)
				continue;
			foreach (var companionFile in photo.CompanionFiles)
				mockFileSystem.FileExists(companionFile.SourceFullPath).Should().BeFalse();
		}
	}

	protected static void VerifyFilesExistOnFileSystem(IEnumerable<string> files, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var file in files)
		{
			mockFileSystem.File.Exists(file).Should().BeTrue();
		}
	}

	protected static string RootForMockFileSystem()
	{
		return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\" : "";
	}

	#endregion
}
