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
	public void Copy_GivenPhotos_ShouldCreatedOnMockedFileSystem(List<Photo> photos)
	{
		const string outputFolder = "/output-path";
		var mockFileSystem = new MockFileSystem();

		var inputPhotoFilePathsToBeCreated = photos.Select(s => s.FilePath);
		SetupFileSystemWithDummyFiles(inputPhotoFilePathsToBeCreated, mockFileSystem);

		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, new Statistics());
		sut.Copy(photos, outputFolder, false);

		var expectedFilePathsToBeCreated = photos.Select(s => s.DestinationPath(outputFolder));
		VerifyFilesExistOnFileSystem(expectedFilePathsToBeCreated, mockFileSystem);
	}

	public static TheoryData<List<string>, List<Photo>, Statistics> CopyExistingFilesOnOutputPathWithSameOverrides = new()
	{
		{
			new List<string>
			{
				"fileExistOnOutput.jpg",
			},
			new List<Photo>
			{
				PhotoFakes.WithNewName("input-1.jpg", "fileExistOnOutput"),
			},
			new Statistics { PhotosExisted = 1, PhotosCopied = 0 }
		},
		{
			new List<string>
			{
				"fileExistOnOutput.jpg",
			},
			new List<Photo>
			{
				PhotoFakes.WithNewName("input-1.jpg", "fileExistOnOutput"),
				PhotoFakes.WithNewName("input-2.jpg", "output-2"),
			},
			new Statistics { PhotosExisted = 1, PhotosCopied = 1 }
		},
		{
			new List<string>
			{
				"fileExistOnOutput-2.jpg",
				"fileExistOnOutput-3.jpg",
				"fileExistOnOutput-5.jpg",
			},
			new List<Photo>
			{
				PhotoFakes.WithNewName("input-1.jpg", "new-1"),
				PhotoFakes.WithNewName("input-2.jpg", "fileExistOnOutput-2"),
				PhotoFakes.WithNewName("input-3.jpg", "fileExistOnOutput-3"),
				PhotoFakes.WithNewName("input-4.jpg", "new-4"),
				PhotoFakes.WithNewName("input-5.jpg", "fileExistOnOutput-5"),
			},
			new Statistics { PhotosExisted = 3, PhotosCopied = 2 }
		}
	};

	public static TheoryData<List<string>, List<Photo>, Statistics> CopyExistingFilesOnOutputPathWithoutOverride = new()
	{
		{
			new List<string>
			{
				"fileExistOnOutput.jpg",
			},
			new List<Photo>
			{
				PhotoFakes.WithNewName("input-1.jpg", "output-1"),
			},
			new Statistics { PhotosExisted = 0, PhotosCopied = 1 }
		},
		{
			new List<string>
			{
				"fileExistOnOutput.jpg",
			},
			new List<Photo>(),
			new Statistics { PhotosExisted = 0, PhotosCopied = 0 }
		}
	};

	[Theory]
	[MemberData(nameof(CopyExistingFilesOnOutputPathWithSameOverrides))]
	[MemberData(nameof(CopyExistingFilesOnOutputPathWithoutOverride))]
	public void CopyIfNotExists_GivenExistingFileSystemAndPhotos_ShouldCompleteWithStatisticsMatchedWithExpectedFileSystemState(List<string> existingFilesOnOutput,
		List<Photo> photos, Statistics statisticsExpected)
	{
		const string outputFolder = "/output-path";
		var mockFileSystem = new MockFileSystem();

		var existingFilePathsToBeCreated = existingFilesOnOutput.Select(s => Path.Combine(outputFolder, s));
		var inputPhotoFilePathsToBeCreated = photos.Select(s => s.FilePath);

		SetupFileSystemWithDummyFiles(existingFilePathsToBeCreated, mockFileSystem);
		SetupFileSystemWithDummyFiles(inputPhotoFilePathsToBeCreated, mockFileSystem);

		var statistics = new Statistics();
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, statistics);
		sut.CopyIfNotExists(photos, outputFolder, false);

		var expectedFilePathsToBeCreated = photos.Select(s => s.DestinationPath(outputFolder));
		VerifyFilesExistOnFileSystem(expectedFilePathsToBeCreated, mockFileSystem);

		statistics.PhotosExisted.Should().Be(statisticsExpected.PhotosExisted);
		statistics.PhotosCopied.Should().Be(statisticsExpected.PhotosCopied);
		statistics.FileIoErrors.Should().BeEmpty();
	}

	public static TheoryData<List<string>, List<Photo>> CopyExistingFilesOnOutputPathThrowException = new()
	{
		{
			new List<string>
			{
				"fileExistOnOutput.jpg",
			},
			new List<Photo>
			{
				PhotoFakes.WithNewName("input-1.jpg", "fileExistOnOutput"),
			}
		},
		{
			new List<string>
			{
				"fileExistOnOutput.jpg",
			},
			new List<Photo>
			{
				PhotoFakes.WithNewName("input-1.jpg", "fileExistOnOutput"),
				PhotoFakes.WithNewName("input-2.jpg", "output-2"),
			}
		}
	};

	[Theory]
	[MemberData(nameof(CopyExistingFilesOnOutputPathThrowException))]
	public void Copy_WithParameterBreakDestinationExistsAsTrue_ShouldThrowException(List<string> existingFilesOnOutput, List<Photo> photos)
	{
		const string outputFolder = "/output-path";
		var mockFileSystem = new MockFileSystem();
		var existingFilePathsToBeCreated = existingFilesOnOutput.Select(s => Path.Combine(outputFolder, s));
		var inputPhotoFilePathsToBeCreated = photos.Select(s => s.FilePath);

		SetupFileSystemWithDummyFiles(existingFilePathsToBeCreated, mockFileSystem);
		SetupFileSystemWithDummyFiles(inputPhotoFilePathsToBeCreated, mockFileSystem);

		var statistics = new Statistics();
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, statistics);

		Assert.Throws<FileExistsOnDestinationPathException>(() =>
		{
			sut.Copy(photos, outputFolder, false);
		});
	}

	public static TheoryData<string, bool, Dictionary<Photo, byte[]>, Dictionary<string, byte[]>> DataIntegrityFailed = new()
	{
		{
			"output-path",
			false,
			new Dictionary<Photo, byte[]>
			{
				{ PhotoFakes.WithNoNewName("file1.jpg"), new byte[] { 1 } }
			},
			new Dictionary<string, byte[]>
			{
				{ "output-path/file1.jpg", new byte[] { 2 } }
			}
		},
		{
			"output-path",
			false,
			new Dictionary<Photo, byte[]>
			{
				{ PhotoFakes.WithNoNewName("file1.jpg"), new byte[] { 1 } },
				{ PhotoFakes.WithNoNewName("file2.jpg"), new byte[] { 2 } }
			},
			new Dictionary<string, byte[]>
			{
				{ "output-path/file1.jpg", new byte[] { 1 } },
				{ "output-path/file2.jpg", new byte[] { 3 } },
			}
		},
	};

	public static TheoryData<string, bool, Dictionary<Photo, byte[]>, Dictionary<string, byte[]>> DataIntegritySuccessful = new()
	{
		{
			"output-path",
			true,
			new Dictionary<Photo, byte[]>
			{
				{ PhotoFakes.WithNoNewName("file1.jpg"), new byte[] { 1 } }
			},
			new Dictionary<string, byte[]>
			{
				{ "output-path/file1.jpg", new byte[] { 1 } }
			}
		},
		{
			"output-path",
			true,
			new Dictionary<Photo, byte[]>
			{
				{ PhotoFakes.WithNoNewName("file1.jpg"), new byte[] { 1 } },
				{ PhotoFakes.WithNoNewName("file2.jpg"), new byte[] { 2 } }
			},
			new Dictionary<string, byte[]>
			{
				{ "output-path/file1.jpg", new byte[] { 1 } },
				{ "output-path/file2.jpg", new byte[] { 2 } },
			}
		},
	};

	[Theory]
	[MemberData(nameof(DataIntegrityFailed))]
	[MemberData(nameof(DataIntegritySuccessful))]
	public async Task VerifyFileIntegrity_MockedFileSystemWithInputAndOutputFolders_ShouldMatchWithExpectedResult(string outputFolder, bool expectedResult, Dictionary<Photo, byte[]> inputFileContentByPhoto, Dictionary<string, byte[]> outputFileContentByPhoto)
	{
		var mockFileSystem = new MockFileSystem();

		var photos = CreatePhotosOnMockFileSystem(inputFileContentByPhoto, mockFileSystem);
		foreach (var (filePath, fileContent) in outputFileContentByPhoto)
		{
			mockFileSystem.AddFile(filePath, new MockFileData(fileContent));
		}

		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty());
		var verifySuccessful = await sut.VerifyFileIntegrity(photos, outputFolder);
		verifySuccessful.Should().Be(expectedResult);
	}

	public static TheoryData<List<Photo>, string> PhotosWithSha1HashOnRootFolder = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
			},
			$"{Sha1HashFakes.Sample(1)}  file1.jpg{Environment.NewLine}"
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
				PhotoFakes.WithSha1Hash("file2.jpg", Sha1HashFakes.Sample(2)),
			},
			$"{Sha1HashFakes.Sample(1)}  file1.jpg{Environment.NewLine}{Sha1HashFakes.Sample(2)}  file2.jpg{Environment.NewLine}"
		},
	};

	public static TheoryData<List<Photo>, string> PhotosWithSha1HashOnSubFolders = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(3), "sub1/"),
			},
			$"{Sha1HashFakes.Sample(3)}  sub1/file1.jpg{Environment.NewLine}"
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(3), "sub1/"),
				PhotoFakes.WithSha1Hash("file2.jpg", Sha1HashFakes.Sample(4), "sub1/sub2"),
			},
			$"{Sha1HashFakes.Sample(3)}  sub1/file1.jpg{Environment.NewLine}{Sha1HashFakes.Sample(4)}  sub1/sub2/file2.jpg{Environment.NewLine}"
		}
	};

	[Theory]
	[MemberData(nameof(PhotosWithSha1HashOnRootFolder))]
	[MemberData(nameof(PhotosWithSha1HashOnSubFolders))]
	public async Task SaveGnuHashFileTree_GivenPhotos_CreatedShaHashFileTreeShouldMatchWithExpected(List<Photo> photos, string expectedHashFileTreeContent)
	{
		var mockFileSystem = new MockFileSystem();
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty());
		await sut.SaveGnuHashFileTree(photos, "");

		await using var fileStream = mockFileSystem.FileStream.Create("sha1.lst", FileMode.Open);
		var streamReader = new StreamReader(fileStream);
		var text = await streamReader.ReadToEndAsync();
		text.Should().Be(expectedHashFileTreeContent);
	}

	public static TheoryData<Dictionary<Photo, byte[]>, List<Photo>> CalculateFileHashData = new()
	{
		{
			new Dictionary<Photo, byte[]>
			{
				{ PhotoFakes.WithNoNewName("file1.jpg"), new byte[] { 1 } },
			},
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
			}
		},
		{
			new Dictionary<Photo, byte[]>
			{
				{ PhotoFakes.WithNoNewName("file1.jpg"), new byte[] { 1 } },
				{ PhotoFakes.WithNoNewName("file2.jpg"), new byte[] { 2 } },
			},
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", Sha1HashFakes.Sample(1)),
				PhotoFakes.WithSha1Hash("file2.jpg", Sha1HashFakes.Sample(2)),
			}
		},
		{
			new Dictionary<Photo, byte[]>(),
			new List<Photo>()
		},
	};

	[Theory]
	[MemberData(nameof(CalculateFileHashData))]
	public async Task CalculateFileHash_GivenPhotos_ShouldCalculateAndMatchWithSHA1Hashes(Dictionary<Photo, byte[]> inputFileContentByPhoto, List<Photo> expectedPhotos)
	{
		var mockFileSystem = new MockFileSystem();
		var photos = CreatePhotosOnMockFileSystem(inputFileContentByPhoto, mockFileSystem);
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty());
		await sut.CalculateFileHash(photos);
		photos.Should().BeEquivalentTo(expectedPhotos);
	}

	private static List<Photo> CreatePhotosOnMockFileSystem(Dictionary<Photo, byte[]> inputFileContentByPhoto, IMockFileDataAccessor mockFileSystem)
	{
		var photoInfos = new List<Photo>();
		foreach (var (photoInfo, fileContent) in inputFileContentByPhoto)
		{
			photoInfos.Add(photoInfo);
			mockFileSystem.AddFile(photoInfo.FilePath, new MockFileData(fileContent));
		}
		return photoInfos;
	}

	private static void SetupFileSystemWithDummyFiles(IEnumerable<string> filePaths, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var filePath in filePaths)
			mockFileSystem.AddFile(filePath, string.Empty);
	}

	private static void VerifyFilesExistOnFileSystem(IEnumerable<string> filePaths, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var filePath in filePaths)
			mockFileSystem.FileExists(filePath).Should().Be(true);
	}
}
