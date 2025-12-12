namespace PhotoCli.Tests.Fakes;

public static class PhotoFileFakes
{
	public static PhotoFile Create(string photoPath)
	{
		var filePath = MockFileSystemHelper.Path(photoPath);
		var mockFileSystem = new MockFileSystem();
		var fileInfo = mockFileSystem.FileInfo.New(filePath);
		return new PhotoFile(fileInfo);
	}
}
