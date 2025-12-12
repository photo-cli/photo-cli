namespace PhotoCli.Tests.UnitTests.Services.FileServiceTests;

public class FileServiceFileSystemsUnitTests : FileServiceUnitTestsBase
{
	#region CreateOutputFolderIfNotExists

	[Fact]
	public void CreateOutputFolderIfNotExists_GivenNonExistentFolder_ShouldCreateFolder()
	{
		var mockFileSystem = new MockFileSystem();
		var outputFolder = "non-existent-folder";

		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		sut.CreateOutputFolderIfNotExists(outputFolder);

		mockFileSystem.Directory.Exists(outputFolder).Should().BeTrue();
	}

	[Fact]
	public void CreateOutputFolderIfNotExists_GivenExistentFolder_ShouldNoEffectAndContinueRunning()
	{
		var mockFileSystem = new MockFileSystem();
		const string outputFolder = "existent-folder";
		mockFileSystem.Directory.CreateDirectory(outputFolder);

		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		sut.CreateOutputFolderIfNotExists(outputFolder);

		mockFileSystem.Directory.Exists(outputFolder).Should().BeTrue();
	}

	#endregion

	#region DeletePhotoSources

	public static TheoryData<List<Photo>, ICollection<string>> PhotosAndDirectoriesToDelete = new()
	{
		{
			[ OnRoot("file1.jpg") ],
			[ DefaultSourcePath ]
		},
		{
			[
				OnRoot("file2.jpg"),
				OnSubPath("file3.jpg", "sub-path1"),
				OnSubPath("file4.jpg", "sub-path1"),
			],
			[
				DefaultSourcePath,
				SubPathOnSourcePath("sub-path1")
			]
		},
		{
			[
				OnRoot("file5.jpg"),
				OnRoot("file6.jpg"),
				OnSubPath("file7.jpg", "sub-path2"),
				OnSubPath("file8.jpg", "sub-path2"),
				OnSubPath("file9.jpg", "sub-path3")
			],
			[
				DefaultSourcePath,
				SubPathOnSourcePath("sub-path2"),
				SubPathOnSourcePath("sub-path3")
			]
		},
		{
			[
				OnSubPath("file9.jpg", "sub-path4"),
				OnSubPath("file10.jpg", "sub-path4", "second-level-sub-path"),
				OnSubPath("file11.jpg", "sub-path4", "second-level-sub-path", "third-level-sub-path"),
			],
			[
				SubPathOnSourcePath("sub-path4"),
				SubPathOnSourcePath("sub-path4", "second-level-sub-path"),
				SubPathOnSourcePath("sub-path4", "second-level-sub-path", "third-level-sub-path")
			]
		},
	};

	public static TheoryData<List<Photo>, ICollection<string>> PhotosWithCompanionFilesAndDirectoriesToDelete = new()
	{
		{
			[ WithCompanionsOnRoot("file1.jpg", ["file1.comp"]) ],
			[ DefaultSourcePath ]
		},
		{
			[
				WithCompanionsOnRoot("file2.jpg", ["file2.comp"]),
				WithCompanionsOnSubPath("file3.jpg", ["file3.comp"], "sub-path1"),
				WithCompanionsOnSubPath("file4.jpg", ["file4.comp"], "sub-path1"),
			],
			[
				DefaultSourcePath,
				SubPathOnSourcePath("sub-path1")
			]
		},
		{
			[
				WithCompanionsOnRoot("file5.jpg", ["file5.comp1", "file5.comp2"]),
				WithCompanionsOnRoot("file6.jpg", ["file6.comp"]),
				WithCompanionsOnSubPath("file7.jpg", ["file7.comp"], "sub-path2"),
				WithCompanionsOnSubPath("file8.jpg", ["file8.comp"], "sub-path2"),
				WithCompanionsOnSubPath("file9.jpg", ["file9.comp"], "sub-path3")
			],
			[
				DefaultSourcePath,
				SubPathOnSourcePath("sub-path2"),
				SubPathOnSourcePath("sub-path3")
			]
		},
		{
			[
				WithCompanionsOnSubPath("file9.jpg", ["file9.comp"], "sub-path4"),
				WithCompanionsOnSubPath("file10.jpg", ["file10.comp"], "sub-path4", "second-level-sub-path"),
				WithCompanionsOnSubPath("file11.jpg", ["file11.comp1", "file11.comp2", "file11.comp3"], "sub-path4", "second-level-sub-path", "third-level-sub-path"),
			],
			[
				SubPathOnSourcePath("sub-path4"),
				SubPathOnSourcePath("sub-path4", "second-level-sub-path"),
				SubPathOnSourcePath("sub-path4", "second-level-sub-path", "third-level-sub-path")
			]
		},
	};

	[Theory]
	[MemberData(nameof(PhotosAndDirectoriesToDelete))]
	[MemberData(nameof(PhotosWithCompanionFilesAndDirectoriesToDelete))]
	public void DeletePhotoSources_GivenPhotos_ShouldDeleteSourcePhotoFilesAndEmptyDirectories(List<Photo> photos, ICollection<string> directoriesExpectedToBeDeleted)
	{
		var mockFileSystem = new MockFileSystem();
		SetupFileSystemWithDummyFiles(photos, mockFileSystem);
		VerifyDirectoriesExistOnFileSystem(directoriesExpectedToBeDeleted, mockFileSystem);

		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		sut.DeletePhotoSources(photos);

		using (new AssertionScope())
		{
			VerifyPhotoSourceFilesNotExistOnFileSystem(photos, mockFileSystem);
			VerifyDirectoriesNotExistOnFileSystem(directoriesExpectedToBeDeleted, mockFileSystem);
		}
	}

	public static TheoryData<List<Photo>, ICollection<string>, ICollection<string>, ICollection<string>> PhotosAndOtherFilesAndDirectoriesToBeKept = new()
	{
		// scenario - file1.jpg & other-file.pdf are on source-path directory
		// expected - other-file1.ext & source-path directory should be kept
		{
			[ OnRoot("file1.jpg") ],
			[ SourcePath("other-file1.ext") ],
			[ DefaultSourcePath ],
			[]
		},

		// scenario - other-file2.ext is on source-path/sub-path1 directory
		// expected - other-file2.ext & source-path should be kept
		{
			[
				OnRoot("file2.jpg"),
				OnSubPath("file3.jpg", "sub-path1"),
				OnSubPath("file4.jpg", "sub-path1"),
			],
			[
				SubPathOnSourcePath("sub-path1", "other-file2.ext")
			],
			[
				DefaultSourcePath,
				SubPathOnSourcePath("sub-path1")
			],
			[]
		},

		// scenario - other-file3.ext on source-path directory,
		// expected - other-file3.ext & source-path should be kept but sub-path2 should be deleted
		{
			[
				OnRoot("file5.jpg"),
				OnSubPath("file6.jpg", "sub-path2"),
				OnSubPath("file7.jpg", "sub-path2"),
			],
			[
				SourcePath("other-file3.ext")
			],
			[
				DefaultSourcePath
			],
			[
				SubPathOnSourcePath("sub-path1")
			]
		},

		// scenario other-file4.ext is on sub-path3/second-level-sub-path1 & other-file5.ext on sub-path3/second-level-sub-path3/third-level-sub-path2
		// expected other-file4.ext, other-file5.ext, sub-path3/second-level-sub-path1, sub-path3/second-level-sub-path3/third-level-sub-path2, sub-path3 should be kept
		//	  but sub-path3/second-level-sub-path2 & sub-path3/second-level-sub-path3/third-level-sub-path1 should be deleted
		{
			[
				WithCompanionsOnSubPath("file9.jpg", ["file9.comp"], "sub-path3"),
				OnSubPath("file10.jpg", "sub-path3", "second-level-sub-path1"),
				OnSubPath("file11.jpg", "sub-path3", "second-level-sub-path2"),
				WithCompanionsOnSubPath("file12.jpg", ["file12.comp1", "file12.comp2"], "sub-path3", "second-level-sub-path3", "third-level-sub-path1"),
				OnSubPath("file13.jpg", "sub-path3", "second-level-sub-path3", "third-level-sub-path2"),
			],
			[
				SubPathOnSourcePath("sub-path3", "second-level-sub-path1", "other-file4.ext"),
				SubPathOnSourcePath("sub-path3", "second-level-sub-path3", "third-level-sub-path2", "other-file5.ext")
			],
			[
				DefaultSourcePath,
				SubPathOnSourcePath("sub-path3"),
				SubPathOnSourcePath("sub-path3", "second-level-sub-path1"),
				SubPathOnSourcePath("sub-path3", "second-level-sub-path3"),
				SubPathOnSourcePath("sub-path3", "second-level-sub-path3", "third-level-sub-path2"),
			],
			[
				SubPathOnSourcePath("sub-path3", "second-level-sub-path2"),
				SubPathOnSourcePath("sub-path3", "second-level-sub-path3", "third-level-sub-path1")
			]
		},
	};

	[Theory]
	[MemberData(nameof(PhotosAndOtherFilesAndDirectoriesToBeKept))]
	public void DeletePhotoSources_GivenPhotosAndOtherFiles_ShouldDeleteOnPhotosButKeepOtherFilesAndNonEmptyDirectories(List<Photo> photos, ICollection<string> nonPhotoFilesExistsOnSourcePath,
		ICollection<string> directoriesExpectedToBeKept, ICollection<string> directoriesExpectedToBeDeleted)
	{
		var mockFileSystem = new MockFileSystem();
		SetupFileSystemWithDummyFiles(photos, mockFileSystem);
		SetupFileSystemWithDummyFiles(nonPhotoFilesExistsOnSourcePath, mockFileSystem);

		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		sut.DeletePhotoSources(photos);

		using (new AssertionScope())
		{
			VerifyPhotoSourceFilesNotExistOnFileSystem(photos, mockFileSystem);
			VerifyFilesExistOnFileSystem(nonPhotoFilesExistsOnSourcePath, mockFileSystem);
			VerifyDirectoriesExistOnFileSystem(directoriesExpectedToBeKept, mockFileSystem);
			VerifyDirectoriesNotExistOnFileSystem(directoriesExpectedToBeDeleted, mockFileSystem);
		}
	}

	#endregion

	#region Helpers

	private static Photo OnRoot(string fileNameWithExtension)
	{
		return PhotoFakes.Create(fileNameWithExtension: fileNameWithExtension, sourcePath: DefaultSourcePath);
	}

	private static Photo OnSubPath(string fileNameWithExtension, params string[] subPaths)
	{
		var paths = new List<string> { DefaultSourcePath };
		paths.AddRange(subPaths);
		var path = MockFileSystemHelper.Combine(paths.ToArray());
		return PhotoFakes.Create(sourcePath: path, fileNameWithExtension: fileNameWithExtension);
	}

	private static Photo WithCompanionsOnRoot(string fileNameWithExtension, string[] companionFileNamesWithExtension)
	{
		return PhotoFakes.Create(fileNameWithExtension: fileNameWithExtension, sourcePath: DefaultSourcePath, companionFileNamesWithExtension: companionFileNamesWithExtension);
	}

	private static Photo WithCompanionsOnSubPath(string fileNameWithExtension, string[] companionFileNamesWithExtension, params string[] subPaths)
	{
		var paths = new List<string> { DefaultSourcePath };
		paths.AddRange(subPaths);
		var path = MockFileSystemHelper.Combine(paths.ToArray());
		return PhotoFakes.Create(sourcePath: path, fileNameWithExtension: fileNameWithExtension, companionFileNamesWithExtension: companionFileNamesWithExtension);
	}

	#endregion
}
