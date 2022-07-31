namespace PhotoCli.Tests.UnitTests.Services;

public class FileServiceUnitTests
{
	public static TheoryData<List<Photo>> FileDataWithNoNewName = new()
	{
		new List<Photo>
		{
			PhotoFakes.WithNoNewName("file1.jpg"),
			PhotoFakes.WithNoNewName("file2.jpg"),
		},
		new List<Photo>
		{
			PhotoFakes.WithNoNewName("file1.jpg", "sub-path1"),
			PhotoFakes.WithNoNewName("file2.jpg", "sub-path2"),
		},
		new List<Photo>
		{
			PhotoFakes.WithNoNewName("file1.jpg"),
			PhotoFakes.WithNoNewName("file2.jpg"),
			PhotoFakes.WithNoNewName("file3.jpg", "sub-path1"),
		},
	};

	public static TheoryData<List<Photo>> FileDataWithNewName = new()
	{
		new List<Photo>
		{
			PhotoFakes.WithNewName("file1.jpg", "new-name1"),
			PhotoFakes.WithNewName("file2.jpg", "new-name2"),
		},
		new List<Photo>
		{
			PhotoFakes.WithNewName("file1.jpg", "new-name1", "sub-path1"),
			PhotoFakes.WithNewName("file2.jpg", "new-name2", "sub-path2"),
		},
		new List<Photo>
		{
			PhotoFakes.WithNewName("file1.jpg", "new-name1"),
			PhotoFakes.WithNewName("file2.jpg", "new-name2"),
			PhotoFakes.WithNewName("file3.jpg", "new-name3", "sub-path1")
		},
	};

	[Theory]
	[MemberData(nameof(FileDataWithNoNewName))]
	[MemberData(nameof(FileDataWithNewName))]
	public void Copy_Verify_File_Exists(List<Photo> photoInfos)
	{
		const string outputFolder = "output-path";
		var mockFileSystem = new MockFileSystem();
		foreach (var photoInfo in photoInfos)
			mockFileSystem.AddFile(photoInfo.FilePath, string.Empty);
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, new Statistics());
		sut.Copy(photoInfos, outputFolder, false);
		foreach (var photoInfo in photoInfos)
			mockFileSystem.FileExists(photoInfo.DestinationPath(outputFolder)).Should().Be(true);
	}
}
