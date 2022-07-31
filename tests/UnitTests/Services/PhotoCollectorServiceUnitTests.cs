namespace PhotoCli.Tests.UnitTests.Services;

public class PhotoCollectorServiceUnitTests
{
	private readonly MockFileSystem _fileSystem;
	private readonly string _currentDirectory;
	private const string TestDirectoryPath = "MockFileDirectory";

	public PhotoCollectorServiceUnitTests()
	{
		_fileSystem = new MockFileSystem();
		_fileSystem.AddDirectory(TestDirectoryPath);
		_currentDirectory = _fileSystem.Directory.GetCurrentDirectory();
	}

	[Fact]
	public void EmptyDirectory_Should_Return_Empty()
	{
		var sut = new PhotoCollectorService(_fileSystem, ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(), NullLogger<PhotoCollectorService>.Instance);
		var filePaths = sut.Collect(TestDirectoryPath, false);
		filePaths.Should().BeEmpty();
	}

	[Theory]
	[InlineData("/t1.jpg")]
	public void Photos_Only_Items_Verified(string filePath)
	{
		filePath = MockFileSystemHelper.Path(filePath);
		AddMockFile(filePath);
		var sut = new PhotoCollectorService(_fileSystem, ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(), NullLogger<PhotoCollectorService>.Instance);
		var filePaths = sut.Collect(_currentDirectory, false);
		filePaths.Should().OnlyContain(p => p == filePath);
	}


	public static TheoryData<string[], string[]> WithOtherFiles = new()
	{
		{
			new[] { MockFileSystemHelper.Path("/t1.jpg"), MockFileSystemHelper.Path("/t2.jpg") },
			new[] { MockFileSystemHelper.Path("/t3.txt"), MockFileSystemHelper.Path("/t4.pdf") }
		}
	};

	[Theory]
	[MemberData(nameof(WithOtherFiles))]
	public void Photos_With_Other_Files_Filtered_Items_Verified(string[] photoFilePaths, string[] otherFilePaths)
	{
		foreach (var filePath in photoFilePaths)
			AddMockFile(filePath);
		foreach (var filePath in otherFilePaths)
			AddMockFile(filePath);
		var sut = new PhotoCollectorService(_fileSystem, ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(), NullLogger<PhotoCollectorService>.Instance);
		var photoPaths = sut.Collect(_currentDirectory, false);
		photoPaths.Should().OnlyContain(p => photoFilePaths.Contains(p));
		photoPaths.Should().NotContain(p => otherFilePaths.Contains(p));
	}

	private void AddMockFile(string filePath)
	{
		_fileSystem.AddFile(filePath, new MockFileData(string.Empty));
	}
}
