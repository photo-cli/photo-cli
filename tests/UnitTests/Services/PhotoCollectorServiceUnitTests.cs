namespace PhotoCli.Tests.UnitTests.Services;

public class PhotoCollectorServiceUnitTests
{
	private readonly MockFileSystem _fileSystem;
	private const string TestDirectoryPath = "/MockFileDirectory";

	public PhotoCollectorServiceUnitTests()
	{
		_fileSystem = new MockFileSystem();
		_fileSystem.AddDirectory(TestDirectoryPath);
	}

	#region Single Directory

	public static TheoryData<string[]> PhotoFilePathsInRootFolder = new()
	{
		{
			[
				TestFilePath("0.jpg"),
			]
		},
		{
			[
				TestFilePath("1.jpg"),
				TestFilePath("2.jpg"),
			]
		}
	};

	[Theory]
	[MemberData(nameof(PhotoFilePathsInRootFolder))]
	public void Searching_Single_Directory_Should_Return_Photo_Files_On_That_Folder(string[] photoFilePaths)
	{
		AddMockFiles(photoFilePaths);
		var photos = Collect(false);
		PhotosShouldMatchWithFilePaths(photos, photoFilePaths);
	}

	public static TheoryData<string[]> OtherFilesInRootFolder = new()
	{
		{
			[
				TestFilePath("0.txt"),
			]
		},
		{
			[
				TestFilePath("1.pdf"),
				TestFilePath("2.zip"),
			]
		}
	};

	[Theory]
	[MemberData(nameof(OtherFilesInRootFolder))]
	public void Searching_Single_Directory_Should_Not_Contain_Other_Files(string[] otherFilePaths)
	{
		AddMockFiles(otherFilePaths);
		var photos = Collect(false);
		PhotosShouldNotContainWithFilePaths(photos, otherFilePaths);
	}

	public static TheoryData<string[]> PhotoPathsOnlyInSubFolders = new()
	{
		{
			[
				TestFilePath("sub-folder/0.jpg"),
			]
		},
		{
			[
				TestFilePath("sub-folder1/2.jpg"),
				TestFilePath("sub-folder3/sub-folder4/5.jpg"),
			]
		},
	};

	[Theory]
	[MemberData(nameof(PhotoPathsOnlyInSubFolders))]
	public void Searching_Single_Directory_Should_Not_Contain_Sub_Folder_Photos(string[] photoFilePathsInSubFolders)
	{
		AddMockFiles(photoFilePathsInSubFolders);
		var photos = Collect(false);
		PhotosShouldNotContainWithFilePaths(photos, photoFilePathsInSubFolders);
	}

	#endregion

	#region All Directories

	public static TheoryData<string[]> PhotoFilePathsInSubFolders = new()
	{
		{
			[
				TestFilePath("sub-folder/0.jpg"),
			]
		},
		{
			[
				TestFilePath("1.jpg"),
				TestFilePath("sub-folder2/3.jpg"),
			]
		},
		{
			[
				TestFilePath("sub-folder4/sub-folder5/6.jpg"),
				TestFilePath("sub-folder7/sub-folder8/9.jpg"),
			]
		},
	};

	[Theory]
	[MemberData(nameof(PhotoFilePathsInSubFolders))]
	public void Searching_All_Directories_Should_Return_Photo_Files_Under_Any_Level_That_Directory(string[] photoFilePaths)
	{
		AddMockFiles(photoFilePaths);
		var photos = Collect(true);
		PhotosShouldMatchWithFilePaths(photos, photoFilePaths);
	}

	#endregion

	#region Supported Extensions

	public static TheoryData<ToolOptions, string[]> ChangedSupportedExtensionWithMatchingFiles = new()
	{
		{
			ToolOptionsFakes.WithSupportedExtensions(["tiff"]),
			[
				TestFilePath("0.tiff"),
				TestFilePath("sub-folder/1.tiff"),
			]
		},
		{
			ToolOptionsFakes.WithSupportedExtensions(["jpx", "jpf"]),
			[
				TestFilePath("2.jpx"),
				TestFilePath("sub-folder/3.jpf"),
			]
		}
	};

	[Theory]
	[MemberData(nameof(ChangedSupportedExtensionWithMatchingFiles))]
	public void Searching_With_Changed_Supported_Extensions_Should_Return_Files_With_Given_Extension(ToolOptions toolOptions, string[] matchingFilePaths)
	{
		AddMockFiles(matchingFilePaths);
		var photos = Collect(true, toolOptions: toolOptions);
		PhotosShouldMatchWithFilePaths(photos, matchingFilePaths);
	}

	public static TheoryData<ToolOptions, string[]> ChangedSupportedExtensionWithUnMatchingFiles = new()
	{
		{
			ToolOptionsFakes.WithSupportedExtensions(["tiff"]),
			[
				TestFilePath("0.jpg"),
				TestFilePath("sub-folder/1.png"),
			]
		},
		{
			ToolOptionsFakes.WithSupportedExtensions(["jpx", "jpf"]),
			[
				TestFilePath("2.jpeg"),
				TestFilePath("sub-folder/3.jpg"),
			]
		}
	};

	[Theory]
	[MemberData(nameof(ChangedSupportedExtensionWithUnMatchingFiles))]
	public void Searching_With_Changed_Supported_Extensions_Should_Not_Return_Files_With_Other_Extensions(ToolOptions toolOptions, string[] notMatchingFilePaths)
	{
		AddMockFiles(notMatchingFilePaths);
		var photos = Collect(true, toolOptions: toolOptions);
		PhotosShouldNotContainWithFilePaths(photos, notMatchingFilePaths);
	}

	#endregion

	#region Companion Files

	public static TheoryData<ToolOptions, string[]> CompanionFilesWithOnSingleDirectory = new()
	{
		{
			ToolOptionsFakes.WithCompanionExtensions(["mov"]),
			[
				TestFilePath("0.jpg"),
				TestFilePath("0.mov"),
			]
		},
		{
			ToolOptionsFakes.WithCompanionExtensions(["xmp", "mov"]),
			[
				TestFilePath("1.jpg"),
				TestFilePath("1.xmp"),
				TestFilePath("2.jpg"),
				TestFilePath("2.mov"),
			]
		}
	};

	[Theory]
	[MemberData(nameof(CompanionFilesWithOnSingleDirectory))]
	public void Searching_With_Companion_Files_In_Root_Folder_Should_Attach_Companion_Files_With_Matching_Name(ToolOptions toolOptionsWithCompanionExtension, string[] photoFileWithCompanionPaths)
	{
		AddMockFiles(photoFileWithCompanionPaths);
		var photos = Collect(false, true, toolOptionsWithCompanionExtension);
		PhotosShouldMatchWithFilePaths(photos, photoFileWithCompanionPaths);
	}

	public static TheoryData<ToolOptions,string[]> CompanionFilesNotMatchingWithPhoto = new()
	{
		{
			ToolOptionsFakes.WithCompanionExtensions(["mov"]),
			[
				TestFilePath("0.jpg"),
				TestFilePath("1.mov"),
			]
		},
		{
			ToolOptionsFakes.WithCompanionExtensions(["mov"]),
			[
				TestFilePath("2.jpg"),

				TestFilePath("2 .mov"),
				TestFilePath(" 2.mov"),
				TestFilePath("2..jpg"),
				TestFilePath("2_.mov"),
				TestFilePath("_2.mov"),
				TestFilePath("22.mov"),
				TestFilePath("2.mov2"),
			]
		},
	};

	[Theory]
	[MemberData(nameof(CompanionFilesNotMatchingWithPhoto))]
	public void Given_Companion_Files_Not_Exactly_Matching_With_Photo_Should_Not_Attach_To_Photo(ToolOptions toolOptionsWithCompanionExtension, string[] photoFileWithCompanionPaths)
	{
		AddMockFiles(photoFileWithCompanionPaths);
		var photos = Collect(false, true, toolOptionsWithCompanionExtension);
		photos.Should().AllSatisfy(a => a.CompanionFiles.Should().BeNull());
	}


	public static TheoryData<ToolOptions,string[]> CompanionFilesWithOnAllDirectories = new()
	{
		{
			ToolOptionsFakes.WithCompanionExtensions(["mov"]),
			[
				TestFilePath("sub-folder/0.jpg"),
				TestFilePath("sub-folder/0.mov"),
			]
		},
		{
			ToolOptionsFakes.WithCompanionExtensions(["xmp", "mov"]),
			[
				TestFilePath("sub-folder1/2.jpg"),
				TestFilePath("sub-folder1/2.xmp"),
				TestFilePath("sub-folder3/sub-folder4/5.jpg"),
				TestFilePath("sub-folder3/sub-folder4/5.mov"),
			]
		}
	};

	[Theory]
	[MemberData(nameof(CompanionFilesWithOnAllDirectories))]
	public void Searching_With_Companion_Files_In_All_Directories_Should_Attach_Companion_Files_With_Matching_Name(ToolOptions toolOptionsWithCompanionExtension, string[] photoFileWithCompanionPaths)
	{
		AddMockFiles(photoFileWithCompanionPaths);
		var photos = Collect(true, true, toolOptionsWithCompanionExtension);
		PhotosShouldMatchWithFilePaths(photos, photoFileWithCompanionPaths);
	}

	#endregion

	[Fact]
	public void Searching_NotExisting_Folder_Should_Throw_PhotoCliException_With_A_Informative_Message()
	{
		var notExistingFolder = MockFileSystemHelper.Path("/notExisting-folder");
		var sut = new PhotoCollectorService(_fileSystem, ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(), ToolOptionsFakes.Valid(), NullLogger<PhotoCollectorService>.Instance);
		var expectedException = Assert.Throws<PhotoCliException>(() => sut.Collect(notExistingFolder, true,  true));
		var expectedExceptionMessage = $"Directory not found, do not change the file system after start processing. -> Could not find a part of the path '{notExistingFolder}'.";
		expectedException.Message.Should().Be(expectedExceptionMessage);
	}

	[Theory]
	[InlineData(true, true)]
	[InlineData(true, false)]
	[InlineData(false, true)]
	[InlineData(false, false)]
	public void Searching_Empty_Directory_Should_Return_Empty_Photo_List(bool allDirectories, bool searchCompanionFiles)
	{
		var sut = new PhotoCollectorService(_fileSystem, ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(), ToolOptionsFakes.Valid(), NullLogger<PhotoCollectorService>.Instance);
		var photos = sut.Collect(TestDirectoryPath, allDirectories,  searchCompanionFiles);
		photos.Should().BeEmpty();
	}

	private IReadOnlyCollection<Photo> Collect(bool allDirectories, bool searchCompanionFiles = false, ToolOptions? toolOptions = null)
	{
		toolOptions ??= ToolOptionsFakes.Valid();
		var sut = new PhotoCollectorService(_fileSystem, ConsoleWriterFakes.Valid(), StatisticsFakes.Empty(), toolOptions, NullLogger<PhotoCollectorService>.Instance);
		return sut.Collect(TestDirectoryPath, allDirectories, searchCompanionFiles).ToList();
	}

	private static string TestFilePath(string fileName)
	{
		var filePathRelativeToTest =  Path.Combine(TestDirectoryPath, fileName);
		return MockFileSystemHelper.Path(filePathRelativeToTest);
	}

	private void AddMockFiles(string[] filePaths)
	{
		foreach (var filePath in filePaths)
			AddMockFile(filePath);
	}

	private void AddMockFile(string filePath)
	{
		_fileSystem.AddFile(filePath, new MockFileData(string.Empty));
	}

	private static void PhotosShouldMatchWithFilePaths(IReadOnlyCollection<Photo> photos, string[] shouldContainFilePaths)
	{
		var actualFilePaths = GetPhotoFilePaths(photos);
		actualFilePaths.Should().BeEquivalentTo(shouldContainFilePaths);
	}

	private static void PhotosShouldNotContainWithFilePaths(IReadOnlyCollection<Photo> photos, string[] notContainFilePaths)
	{
		var actualFilePaths = GetPhotoFilePaths(photos);
		actualFilePaths.Should().NotIntersectWith(notContainFilePaths);
	}

	private static List<string> GetPhotoFilePaths(IReadOnlyCollection<Photo> photos)
	{
		var actualFilePaths = new List<string>();
		var photoSourceFiles = photos.Select(s => s.PhotoFile.SourcePath);
		actualFilePaths.AddRange(photoSourceFiles);
		var companionFiles = photos.Where(w => w.CompanionFiles != null).SelectMany(s => s.CompanionFiles!).Select(s => s.SourcePath);
		actualFilePaths.AddRange(companionFiles);
		return actualFilePaths;
	}
}
