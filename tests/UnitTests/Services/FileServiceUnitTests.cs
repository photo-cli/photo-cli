using System.Runtime.InteropServices;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.UnitTests.Services;

public class FileServiceUnitTests
{
	#region Copy

	public static TheoryData<List<Photo>> FileDataWithNoNewNameOnlyMainPhotoFile = new()
	{
		new List<Photo>
		{
			SingleOnRoot("file1.jpg"),
			SingleOnRoot("file2.jpg"),
		},
		new List<Photo>
		{
			SingleOnSubPath("file3.jpg", "sub-path1"),
			SingleOnSubPath("file4.jpg", "sub-path2"),
		},
		new List<Photo>
		{
			SingleOnRoot("file5.jpg"),
			SingleOnRoot("file6.jpg"),
			SingleOnSubPath("file7.jpg", "sub-path3"),
		},
	};

	public static TheoryData<List<Photo>>? FileDataWithNewNameOnlyMainPhotoFile = new()
	{
		new List<Photo>
		{
			SingleWithNewNameOnRoot("file1.jpg", "new-name1"),
			SingleWithNewNameOnRoot("file2.jpg", "new-name2"),
		},
		new List<Photo>
		{
			SingleWithNewNameOnSubPath("file3.jpg", "new-name3", "sub-path1"),
			SingleWithNewNameOnSubPath("file4.jpg", "new-name4", "sub-path2"),
		},
		new List<Photo>
		{
			SingleWithNewNameOnRoot("file5.jpg", "new-name5"),
			SingleWithNewNameOnRoot("file6.jpg", "new-name6"),
			SingleWithNewNameOnSubPath("file7.jpg", "new-name7", "sub-path3"),
		},
	};

	public static TheoryData<List<Photo>> FileDataWithNoNewNameWithCompanion = new()
	{
		new List<Photo>
		{
			WithCompanionsOnRoot("file1.jpg", ["file1.comp"]),
			WithCompanionsOnRoot("file2.jpg", ["file2.comp1", "file2.comp2"]),
		},
		new List<Photo>
		{
			WithCompanionsOnSubPath("file3.jpg", ["file3.comp"], "sub-path1"),
			WithCompanionsOnSubPath("file4.jpg", ["file4.comp1", "file4.comp2"], "sub-path2"),
		},
		new List<Photo>
		{
			WithCompanionsOnRoot("file5.jpg", ["file5.comp"]),
			WithCompanionsOnRoot("file6.jpg", ["file6.comp"]),
			WithCompanionsOnSubPath("file7.jpg", ["file7.comp1", "file7.comp2"], "sub-path3"),
		},
	};

	public static TheoryData<List<Photo>> FileDataWithNewNameWithCompanion = new()
	{
		new List<Photo>
		{
			WithCompanionsAndNewNameOnRoot("file1.jpg", "new-name1", ["file1.comp"]),
			WithCompanionsAndNewNameOnRoot("file2.jpg", "new-name2", ["file2.comp1", "file2.comp2"]),
		},
		new List<Photo>
		{
			WithCompanionsAndNewNameOnSubPath("file3.jpg","new-name3", ["file3.comp"], "sub-path1"),
			WithCompanionsAndNewNameOnSubPath("file4.jpg","new-name4", ["file4.comp1", "file4.comp2"], "sub-path2"),
		},
		new List<Photo>
		{
			WithCompanionsAndNewNameOnRoot("file5.jpg","new-name5", ["file5.comp"]),
			WithCompanionsAndNewNameOnRoot("file6.jpg","new-name6", ["file6.comp"]),
			WithCompanionsAndNewNameOnSubPath("file7.jpg","new-name7", ["file7.comp1", "file7.comp2"], "sub-path3"),
		},
	};

	[Theory]
	[MemberData(nameof(FileDataWithNoNewNameOnlyMainPhotoFile))]
	[MemberData(nameof(FileDataWithNewNameOnlyMainPhotoFile))]
	[MemberData(nameof(FileDataWithNoNewNameWithCompanion))]
	[MemberData(nameof(FileDataWithNewNameWithCompanion))]
	public void Copy_GivenPhotos_ShouldCreatedOnMockedFileSystem(List<Photo> photos)
	{
		var mockFileSystem = new MockFileSystem();
		SetupFileSystemWithDummyFiles(photos, mockFileSystem);
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, new Statistics(), ConsoleWriterFakes.Valid());
		var copiedPhotos = sut.Copy(photos, DefaultOutputPath, false);
		VerifyFilesExistOnFileSystem(copiedPhotos, mockFileSystem);
	}

	public static TheoryData<List<string>, List<Photo>> CopyExistingMainPhotoFileOnOutputPathThrowException = new()
	{
		{
			[
				"fileExistOnOutput-1.jpg",
			],
			[
				SingleWithNewNameOnRoot("input-1.jpg", "fileExistOnOutput-1"),
			]
		},
		{
			[
				"fileExistOnOutput-2.jpg",
			],
			[
				SingleWithNewNameOnRoot("input-2.jpg", "fileExistOnOutput-2"),
				SingleWithNewNameOnRoot("input-3.jpg", "output-3"),
			]
		},
		{
			[
				"fileExistOnOutput-3.jpg",
			],
			[
				WithCompanionsAndNewNameOnRoot("input-4.jpg", "fileExistOnOutput-3", ["input-4.comp1"]),
				SingleWithNewNameOnRoot("input-5.jpg", "output-5"),
			]
		}
	};

	public static TheoryData<List<string>, List<Photo>> CopyExistingCompanionFilesOnOutputPathThrowException = new()
    {
    	{
    		[
    			"fileExistOnOutput-1.comp1",
    		],
    		[
			    WithCompanionsAndNewNameOnRoot("input-1.jpg", "fileExistOnOutput-1", ["fileExistOnOutput-1.comp1"]),
    		]
    	},
    	{
    		[
    			"fileExistOnOutput-2.comp2",
    		],
    		[
			    SingleWithNewNameOnRoot("input-1.jpg", "output-1"),
			    WithCompanionsAndNewNameOnRoot("input-2.jpg", "fileExistOnOutput-2", ["fileExistOnOutput-2.comp2"]),
    		]
    	}
    };

	[Theory]
	[MemberData(nameof(CopyExistingMainPhotoFileOnOutputPathThrowException))]
	[MemberData(nameof(CopyExistingCompanionFilesOnOutputPathThrowException))]
	public void Copy_WithParameterBreakDestinationExistsAsTrue_ShouldThrowException(List<string> existingFilesOnOutput, List<Photo> photos)
	{
		const string outputFolder = "/output-path";
		var mockFileSystem = new MockFileSystem();
		var existingFilePathsToBeCreated = existingFilesOnOutput.Select(s => Path.Combine(outputFolder, s));
		var inputPhotoFilePathsToBeCreated = photos.Select(s => s.PhotoFile.SourcePath);

		SetupFileSystemWithDummyFiles(existingFilePathsToBeCreated, mockFileSystem);
		SetupFileSystemWithDummyFiles(inputPhotoFilePathsToBeCreated, mockFileSystem);

		var statistics = new Statistics();
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, statistics, ConsoleWriterFakes.Valid());

		Assert.Throws<FileExistsOnDestinationPathException>(() =>
		{
			sut.Copy(photos, outputFolder, false);
		});
	}

	#endregion

	#region CopyIfNotExists

	public static TheoryData<List<string>, List<Photo>, Statistics> CopyMainPhotoFilesExistingOnOutput = new()
	{
		{
			[
				"fileExistOnOutput-1.jpg",
			],
			[
				SingleWithNewNameOnRoot("input-1.jpg", "fileExistOnOutput-1"),
			],
			new Statistics { PhotosExisted = 1, PhotosCopied = 0 }
		},
		{
			[
				"fileExistOnOutput-2.jpg"
			],
			[
				SingleWithNewNameOnRoot("input-1.jpg", "fileExistOnOutput-2"),
				SingleWithNewNameOnRoot("input-2.jpg", "output-2"),
			],
			new Statistics { PhotosExisted = 1, PhotosCopied = 1 }
		},
		{
			[
				"fileExistOnOutput-2.jpg",
				"fileExistOnOutput-3.jpg",
				"fileExistOnOutput-5.jpg",
			],
			[
				SingleWithNewNameOnRoot("input-1.jpg", "new-1"),
				SingleWithNewNameOnRoot("input-2.jpg", "fileExistOnOutput-2"),
				SingleWithNewNameOnRoot("input-3.jpg", "fileExistOnOutput-3"),
				SingleWithNewNameOnRoot("input-4.jpg", "new-4"),
				SingleWithNewNameOnRoot("input-5.jpg", "fileExistOnOutput-5"),
			],
			new Statistics { PhotosExisted = 3, PhotosCopied = 2 }
		}
	};

	public static TheoryData<List<string>, List<Photo>, Statistics> CopyMainPhotoFilesWithoutExistingOnOutput = new()
	{
		{
			[],
			[
				SingleWithNewNameOnRoot("input-1.jpg", "output-1"),
			],
			new Statistics { PhotosExisted = 0, PhotosCopied = 1 }
		},
		{
			[
				"fileExistOnOutput-1.jpg",
			],
			[
				SingleWithNewNameOnRoot("input-2.jpg", "output-2"),
				SingleWithNewNameOnRoot("input-3.jpg", "output-3"),
			],
			new Statistics { PhotosExisted = 0, PhotosCopied = 2 }
		},
		{
			[
				"fileExistOnOutput-2.jpg"
			],
			[],
			new Statistics { PhotosExisted = 0, PhotosCopied = 0 }
		}
	};

	public static TheoryData<List<string>, List<Photo>, Statistics> CopyPhotosWithCompanionFilesExistingOnOutputPath = new()
	{
		{
			[
				"fileExistOnOutput-1.comp1",
			],
			[
				WithCompanionsAndNewNameOnRoot("input-1.jpg", "fileExistOnOutput-1", ["input.comp1"]),
			],
			new Statistics { CompanionFilesExisted = 1, PhotosCopied = 1, CompanionFilesCopied =  0, PhotosExisted = 0 }
		},
		{
			[
				"fileExistOnOutput-2.comp2",
				"fileExistOnOutput-3.comp3",
			],
			[
				WithCompanionsAndNewNameOnRoot("input-2.jpg", "fileExistOnOutput-2", ["input-2.comp2"]),
				WithCompanionsAndNewNameOnRoot("input-3.jpg", "fileExistOnOutput-3", ["input-3.comp3", "input-3.comp4"]),
			],
			new Statistics { CompanionFilesExisted = 2, PhotosCopied = 2, CompanionFilesCopied =  1, PhotosExisted = 0 }
		},
		{
			[
				"fileExistOnOutput-4.jpg",
				"fileExistOnOutput-5.comp1",
			],
			[
				SingleWithNewNameOnRoot("input-3.jpg", "output-1"),
				WithCompanionsAndNewNameOnRoot("input-4.jpg", "fileExistOnOutput-4", ["input.comp1"]),
				WithCompanionsAndNewNameOnRoot("input-5.jpg", "output-5", ["fileExistOnOutput-5.comp1", "output-5.comp1"]),
			],
			new Statistics { CompanionFilesExisted = 1, PhotosCopied = 2, CompanionFilesCopied =  1, PhotosExisted = 1 }
		},
	};

	public static TheoryData<List<string>, List<Photo>, Statistics> CopyPhotosWithCompanionFilesWithoutExistingOnOutput = new()
	{
		{
			[],
			[
				WithCompanionsAndNewNameOnRoot("input-1.jpg", "output-1", ["input-1.comp1"]),
			],
			new Statistics { CompanionFilesCopied = 1, PhotosCopied = 1, CompanionFilesExisted = 0, PhotosExisted = 0 }
		},
		{
			[
				"fileExistOnOutput-1.jpg",
			],
			[
				WithCompanionsAndNewNameOnRoot("input-1.jpg", "output-1", ["input-1.comp1"]),
				WithCompanionsAndNewNameOnRoot("input-2.jpg", "output-2", ["input-2.comp2", "input-2.comp3"]),
			],
			new Statistics { CompanionFilesCopied = 3, PhotosCopied = 2, CompanionFilesExisted = 0, PhotosExisted = 0 }
		},
	};

	[Theory]
	[MemberData(nameof(CopyMainPhotoFilesExistingOnOutput))]
	[MemberData(nameof(CopyMainPhotoFilesWithoutExistingOnOutput))]
	[MemberData(nameof(CopyPhotosWithCompanionFilesExistingOnOutputPath))]
	[MemberData(nameof(CopyPhotosWithCompanionFilesWithoutExistingOnOutput))]
	public void CopyIfNotExists_GivenExistingFileSystemAndPhotos_ShouldCompleteWithStatisticsMatchedWithExpectedFileSystemState(List<string> existingFilesOnOutput,
		List<Photo> photos, Statistics statisticsExpected)
	{
		var mockFileSystem = new MockFileSystem();
		var existingFilePathsToBeCreated = existingFilesOnOutput.Select(s => Path.Combine(DefaultOutputPath, s));
		SetupFileSystemWithDummyFiles(existingFilePathsToBeCreated, mockFileSystem);
		SetupFileSystemWithDummyFiles(photos, mockFileSystem);

		var statistics = new Statistics();
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, statistics, ConsoleWriterFakes.Valid());
		var copiedPhotos = sut.CopyIfNotExists(photos, DefaultOutputPath, false);

		using (new AssertionScope())
		{
			VerifyFilesExistOnFileSystem(copiedPhotos, mockFileSystem);

			statistics.PhotosExisted.Should().Be(statisticsExpected.PhotosExisted);
			statistics.PhotosCopied.Should().Be(statisticsExpected.PhotosCopied);
			statistics.CompanionFilesCopied.Should().Be(statisticsExpected.CompanionFilesCopied);
			statistics.CompanionFilesExisted.Should().Be(statisticsExpected.CompanionFilesExisted);
			statistics.FileIoErrors.Should().BeEmpty();
		}
	}

	#endregion

	#region VerifyFileIntegrity

	#region Hash Match

	public static TheoryData<List<Photo>, Dictionary<string, byte[]>> DataIntegritySuccessfulMainPhotoOnly = new()
	{
		{
			[
				SingleOnRoot("file1.jpg"),
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file1.jpg"), [1] },
				{ OutputPath("file1.jpg"), [1] },
			}
		},
		{
			[
				SingleOnRoot("file1.jpg"),
				SingleOnRoot("file2.jpg")
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file1.jpg"), [1] },
				{ SourcePath("file2.jpg"), [2] },

				{ OutputPath("file1.jpg"), [1] },
				{ OutputPath("file2.jpg"), [2] },
			}
		},
	};

	public static TheoryData<List<Photo>, Dictionary<string, byte[]>> DataIntegritySuccessfulWithCompanionFiles = new()
	{
		{
			[
				WithCompanionsOnRoot("file1.jpg", ["file1.comp1"]),
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file1.jpg"), [1] },
				{ SourcePath("file1.comp1"), [2] },

				{ OutputPath("file1.jpg"), [1] },
				{ OutputPath("file1.comp1"), [2] },
			}
		},
		{
			[
				WithCompanionsOnRoot("file2.jpg", ["file2.comp2"]),
				WithCompanionsOnRoot("file3.jpg", ["file3.comp2", "file3.comp3"]),
				WithCompanionsOnRoot("file4.jpg", ["file4.comp4"])
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file2.jpg"), [3] },
				{ SourcePath("file2.comp2"), [4] },

				{ SourcePath("file3.jpg"), [5] },
				{ SourcePath("file3.comp2"), [6] },
				{ SourcePath("file3.comp3"), [7] },

				{ SourcePath("file4.jpg"), [8] },
				{ SourcePath("file4.comp4"), [9] },

				{ OutputPath("file2.jpg"), [3] },
				{ OutputPath("file2.comp2"), [4] },

				{ OutputPath("file3.jpg"), [5] },
				{ OutputPath("file3.comp2"), [6] },
				{ OutputPath("file3.comp3"), [7] },

				{ OutputPath("file4.jpg"), [8] },
				{ OutputPath("file4.comp4"), [9] },
			}
		},
	};

	[Theory]
	[MemberData(nameof(DataIntegritySuccessfulMainPhotoOnly))]
	[MemberData(nameof(DataIntegritySuccessfulWithCompanionFiles))]
	public async Task VerifyFileIntegrity_MockedFileSystemMatchingWithInputAndOutputFolders_ShouldReturnTrue(List<Photo> photos, Dictionary<string, byte[]> fileContentByPhoto)
	{
		await VerifyFileIntegrityExpected(photos, fileContentByPhoto, true);
	}

	#endregion

	#region Hash Not Match

	public static TheoryData<List<Photo>, Dictionary<string, byte[]>, string> DataIntegrityFailedMainPhotoOnly = new()
	{
		{
			[
				SingleOnRoot("file1.jpg"),
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file1.jpg"), [1] },
				{ OutputPath("file1.jpg"), [2] },
			},
			MainPhotoIntegrityNotSuccessfulLog("file1.jpg")
		},
		{
			[
				SingleOnRoot("file1.jpg"),
				SingleOnRoot("file2.jpg"),
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file1.jpg"), [1] },
				{ SourcePath("file2.jpg"), [1] },

				{ OutputPath("file1.jpg"), [1] },
				{ OutputPath("file2.jpg"), [3] },
			},
			MainPhotoIntegrityNotSuccessfulLog("file2.jpg")
		},
	};

	public static TheoryData<List<Photo>, Dictionary<string, byte[]>, string> DataIntegrityFailedWithCompanionFiles = new()
	{
		{
			[
				WithCompanionsOnRoot("file1.jpg", ["file1.comp1"]),
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file1.jpg"), [1] },
				{ SourcePath("file1.comp1"), [2] },

				{ OutputPath("file1.jpg"), [1] },
				{ OutputPath("file1.comp1"), [3] },
			},
			CompanionFileIntegrityNotSuccessfulLog("file1.comp1")
		},
		{
			[
				WithCompanionsOnRoot("file2.jpg", ["file2.comp2"]),
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file2.jpg"), [4] },
				{ SourcePath("file2.comp2"), [5] },

				{ OutputPath("file2.jpg"), [6] },
				{ OutputPath("file2.comp2"), [5] },
			},
			MainPhotoIntegrityNotSuccessfulLog("file2.jpg")
		},
	};

	[Theory]
	[MemberData(nameof(DataIntegrityFailedMainPhotoOnly))]
	[MemberData(nameof(DataIntegrityFailedWithCompanionFiles))]
	public async Task VerifyFileIntegrity_MockedFileSystemNotMatchingWithInputAndOutputFolders_ShouldReturnFalse(List<Photo> photos, Dictionary<string, byte[]> fileContentByPhoto, string expectedLogStatement)
	{
		var loggerMock = new Mock<ILogger<FileService>>();
		using (new AssertionScope())
		{
			await VerifyFileIntegrityExpected(photos, fileContentByPhoto, false, loggerMock);
			loggerMock.VerifyAllLogStatementsAtLeastOnce(LogLevel.Critical, expectedLogStatement);
		}
	}

	private static string MainPhotoIntegrityNotSuccessfulLog(string fileName)
	{
		return $"Source main photo file content path: {SourcePath(fileName)} doesn't match with target photo's hash: {OutputPath(fileName)}";
	}

	private static string CompanionFileIntegrityNotSuccessfulLog(string fileName)
	{
		return $"Source companion file content path: {SourcePath(fileName)} doesn't match with target companion file's hash: {OutputPath(fileName)}";
	}

	#endregion

	#region Missing Destination

	public static TheoryData<List<Photo>, Dictionary<string, byte[]>, string> DataIntegrityMissingDestinationMainPhotoOnly = new()
	{
		{
			[
				SingleOnRoot("file1.jpg"),
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file1.jpg"), [1] },
			},
			TargetPhotoNotExistOnFileSystemLog("file1.jpg")
		},
		{
			[
				SingleOnRoot("file1.jpg"),
				SingleOnRoot("file2.jpg"),
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file1.jpg"), [1] },
				{ SourcePath("file2.jpg"), [2] },

				{ OutputPath("file1.jpg"), [1] },
			},
			TargetPhotoNotExistOnFileSystemLog("file2.jpg")
		},
	};

	public static TheoryData<List<Photo>, Dictionary<string, byte[]>, string> DataIntegrityMissingDestinationWithCompanionFiles = new()
	{
		{
			[
				WithCompanionsOnRoot("file1.jpg", ["file1.comp1"]),
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file1.jpg"), [1] },
				{ SourcePath("file1.comp1"), [2] },

				{ OutputPath("file1.jpg"), [1] },
			},
			CompanionFileNotExistOnFileSystemLog("file1.comp1")
		},
		{
			[
				WithCompanionsOnRoot("file2.jpg", ["file2.comp2"]),
				WithCompanionsOnRoot("file3.jpg", ["file3.comp2", "file3.comp3"]),
			],
			new Dictionary<string, byte[]>
			{
				{ SourcePath("file2.jpg"), [3] },
				{ SourcePath("file2.comp2"), [4] },

				{ SourcePath("file3.jpg"), [5] },
				{ SourcePath("file3.comp2"), [6] },
				{ SourcePath("file3.comp3"), [7] },

				{ OutputPath("file2.jpg"), [3] },
				{ OutputPath("file2.comp2"), [4] },

				{ OutputPath("file3.jpg"), [5] },
				{ OutputPath("file3.comp2"), [6] },
			},
			CompanionFileNotExistOnFileSystemLog("file3.comp3")
		},
	};

	[Theory]
	[MemberData(nameof(DataIntegrityMissingDestinationMainPhotoOnly))]
	[MemberData(nameof(DataIntegrityMissingDestinationWithCompanionFiles))]
	public async Task VerifyFileIntegrity_MockedFileSystemMissingDestinationFile_ShouldReturnFalse(List<Photo> photos, Dictionary<string, byte[]> fileContentByPhoto, string expectedLogStatement)
	{
		var loggerMock = new Mock<ILogger<FileService>>();
		using (new AssertionScope())
		{
			await VerifyFileIntegrityExpected(photos, fileContentByPhoto, false, loggerMock);
			loggerMock.VerifyAllLogStatementsAtLeastOnce(LogLevel.Critical, expectedLogStatement);
		}
	}

	private static string TargetPhotoNotExistOnFileSystemLog(string fileName)
	{
		return $"Target photo doesn't exists at path : {OutputPath(fileName)} for a source photo path: {SourcePath(fileName)}";
	}

	private static string CompanionFileNotExistOnFileSystemLog(string fileName)
	{
		return $"Target companion file doesn't exists at path : {OutputPath(fileName)} for a source companion file path: {SourcePath(fileName)}";
	}

	#endregion

	private static async Task VerifyFileIntegrityExpected(List<Photo> photos, Dictionary<string, byte[]> fileContentByPhoto, bool expected, Mock<ILogger<FileService>>? loggerMock = null)
	{
		var mockFileSystem = new MockFileSystem();
		CreateFilesOnMockFileSystem(fileContentByPhoto, mockFileSystem);
		var logger = loggerMock != null ? loggerMock.Object : NullLogger<FileService>.Instance;
		var sut = new FileService(mockFileSystem, logger, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		var verifySuccessful = await sut.VerifyFileIntegrity(photos);
		verifySuccessful.Should().Be(expected);
	}

	[Fact]
	public async Task VerifyFileIntegrity_NoTargetSetForMainFile_ShouldThrowPhotoCliException()
	{
		var (noTargetSetPhoto, sourceFullPath) = PhotoFakes.SourceAndFileNameWithExtensionWithFullSourcePath("source-path", "no-target-set.jpg");
		var photos = new List<Photo>{ noTargetSetPhoto };
		var sut = new FileService(new MockFileSystem(), NullLogger<FileService>.Instance, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		var photoCliException = await Assert.ThrowsAsync<PhotoCliException>(async () => await sut.VerifyFileIntegrity(photos));
		photoCliException.Message.Should().Be($"Couldn't copy a photo don't have a TargetFullPath on {sourceFullPath}");
	}

	[Fact]
	public async Task VerifyFileIntegrity_NoTargetSetForCompanionFil_ShouldThrowPhotoCliException()
	{
		var (noTargetSetPhoto, sourceFullPath) = PhotoFakes.SourceAndFileNameWithExtensionWithFullSourcePath("source-path", "no-target-set.jpg");
		var photos = new List<Photo>{ noTargetSetPhoto };
		var sut = new FileService(new MockFileSystem(), NullLogger<FileService>.Instance, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		var photoCliException = await Assert.ThrowsAsync<PhotoCliException>(async () => await sut.VerifyFileIntegrity(photos));
		photoCliException.Message.Should().Be($"Couldn't copy a photo don't have a TargetFullPath on {sourceFullPath}");
	}

	#endregion

	#region SaveGnuHashFileTree

	public static TheoryData<List<Photo>, string> HashFileMainPhotoOnRootFolder = new()
	{
		{
			[
				SingleWithSha1HashOnRoot("file1.jpg", 1)
			],
			GnuHashFileOutput(new GnuHashFormat(1, "file1.jpg"))
		},
		{
			[
				SingleWithSha1HashOnRoot("file1.jpg", 1),
				SingleWithSha1HashOnRoot("file2.jpg", 2),
			],
			GnuHashFileOutput(new GnuHashFormat(1, "file1.jpg"), new GnuHashFormat(2, "file2.jpg"))
		},
	};

	public static TheoryData<List<Photo>, string> HashFileMainPhotoOnSubFolders = new()
	{
		{
			[
				SingleWithSha1HashOnSubPath("file1.jpg", 3, "sub1"),
			],
			GnuHashFileOutput(new GnuHashFormat(3, "sub1/file1.jpg"))
		},
		{
			[
				SingleWithSha1HashOnSubPath("file1.jpg", 3, "sub1"),
				SingleWithSha1HashOnSubPath("file2.jpg", 4, "sub1/sub2"),
			],
			GnuHashFileOutput(new GnuHashFormat(3, "sub1/file1.jpg"), new GnuHashFormat(4, "sub1/sub2/file2.jpg")
			)
		}
	};

	public static TheoryData<List<Photo>, string> HashFilePhotoWithCompanionOnRootFolder = new()
	{
		{
			[
				WithCompanionsAndSha1HashOnRoot("file1.jpg", 1, new CompanionFileNameHashPair("file1.comp1", 2))
			],
			GnuHashFileOutput(new GnuHashFormat(1, "file1.jpg"), new GnuHashFormat(2, "file1.comp1"))
		},
		{
			[
				WithCompanionsAndSha1HashOnRoot("file2.jpg", 3, new CompanionFileNameHashPair("file2.comp2", 4)),
				WithCompanionsAndSha1HashOnRoot("file3.jpg", 5, new CompanionFileNameHashPair("file3.comp2", 6), new CompanionFileNameHashPair("file3.comp3", 7)),
			],
			GnuHashFileOutput(
				new GnuHashFormat(3, "file2.jpg"),
				new GnuHashFormat(4, "file2.comp2"),

				new GnuHashFormat(5, "file3.jpg"),
				new GnuHashFormat(6, "file3.comp2"),
				new GnuHashFormat(7, "file3.comp3")
			)
		},
	};

	[Theory]
	[MemberData(nameof(HashFileMainPhotoOnRootFolder))]
	[MemberData(nameof(HashFileMainPhotoOnSubFolders))]
	[MemberData(nameof(HashFilePhotoWithCompanionOnRootFolder))]
	public async Task SaveGnuHashFileTree_GivenPhotos_CreatedShaHashFileTreeShouldMatchWithExpected(List<Photo> photos, string expectedHashFileTreeContent)
	{
		var mockFileSystem = new MockFileSystem();
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		await sut.SaveGnuHashFileTree(photos, "");

		await using var fileStream = mockFileSystem.FileStream.New("sha1.lst", FileMode.Open);
		var streamReader = new StreamReader(fileStream);
		var text = await streamReader.ReadToEndAsync();
		text.Should().Be(expectedHashFileTreeContent);
	}

	#endregion

	#region CalculateFileHash

	public static TheoryData<Dictionary<Photo, byte[]>, List<Photo>> CalculateFileMatchesHashData = new()
    {
    	{
    		new Dictionary<Photo, byte[]>
    		{
			    { SingleOnRoot("file1.jpg"), [1] },
    		},
    		[
			    SingleWithDataOnRoot("file1.jpg", [1]),
    		]
    	},
    	{
    		new Dictionary<Photo, byte[]>
    		{
			    { SingleOnRoot("file2.jpg"), [2] },
			    { SingleOnRoot("file3.jpg"), [100, 101, 102] },
    		},
    		[
			    SingleWithDataOnRoot("file2.jpg", [2]),
			    SingleWithDataOnRoot("file3.jpg", [100, 101, 102]),
    		]
    	},
    	{
    		new Dictionary<Photo, byte[]>(),
    		[]
    	},
    };

	[Theory]
	[MemberData(nameof(CalculateFileMatchesHashData))]
	public async Task CalculateFileHash_GivenPhotosWithMatchedSHA1_ShouldEquivalentToExpected(Dictionary<Photo, byte[]> inputFileContentByPhoto, List<Photo> expectedPhotos)
	{
		var mockFileSystem = new MockFileSystem();
		var photos = CreatePhotosOnMockFileSystem(inputFileContentByPhoto, mockFileSystem);
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		var photosHashed = await sut.CalculateFileHash(photos);
		photosHashed.Should().BeEquivalentTo(expectedPhotos);
	}

	public static TheoryData<Dictionary<Photo, byte[]>, List<Photo>> CalculateFileNotMatchingHashData = new()
	{
		{
			new Dictionary<Photo, byte[]>
			{
				{ SingleOnRoot("file1.jpg"), [1] },
			},
			[
				SingleWithDataOnRoot("file1.jpg", [2]),
			]
		},
		{
			new Dictionary<Photo, byte[]>
			{
				{ SingleOnRoot("file2.jpg"), [2] },
				{ SingleOnRoot("file3.jpg"), [100, 101, 102] },
			},
			[
				SingleWithDataOnRoot("file2.jpg", [2]),
				SingleWithDataOnRoot("file3.jpg", [100, 102, 102]),
			]
		},
	};

	[Theory]
	[MemberData(nameof(CalculateFileNotMatchingHashData))]
	public async Task CalculateFileHash_GivenPhotosWithNotMatchedSHA1_ShouldNotEquivalent(Dictionary<Photo, byte[]> inputFileContentByPhoto, List<Photo> mismatchingPhotos)
	{
		var mockFileSystem = new MockFileSystem();
		var photos = CreatePhotosOnMockFileSystem(inputFileContentByPhoto, mockFileSystem);
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		var photosHashed = await sut.CalculateFileHash(photos);
		photosHashed.Should().NotBeEquivalentTo(mismatchingPhotos);
	}

	#endregion

	#region Helpers

	private const string DefaultOutputPath = "output-path";
	private const string DefaultSourcePath = "source-path";

	private static Photo SingleOnRoot(string fileNameWithExtension)
	{
		return SingleOnSubPath(fileNameWithExtension, "");
	}

	private static Photo SingleOnSubPath(string fileNameWithExtension, string subPath)
	{
		return Photo(fileNameWithExtension, subPath);
	}

	private static Photo SingleWithNewNameOnRoot(string fileNameWithExtension, string newName)
	{
		return SingleWithNewNameOnSubPath(fileNameWithExtension, newName, "");
	}

	private static Photo SingleWithNewNameOnSubPath(string fileNameWithExtension, string newName, string subPath)
	{
		return Photo(fileNameWithExtension, subPath, newName);
	}

	private static Photo WithCompanionsOnRoot(string fileNameWithExtension, string[] companionFileNamesWithExtension)
	{
		return WithCompanionsOnSubPath(fileNameWithExtension, companionFileNamesWithExtension, "");
	}

	private static Photo WithCompanionsAndNewNameOnRoot(string fileNameWithExtension, string newName, string[] companionFileNamesWithExtension)
	{
		return WithCompanionsAndNewNameOnSubPath(fileNameWithExtension, newName, companionFileNamesWithExtension, "");
	}

	private static Photo WithCompanionsAndNewNameOnSubPath(string fileNameWithExtension, string newName, string[] companionFileNamesWithExtension, string subPath)
	{
		return Photo(fileNameWithExtension, subPath, newName, companionFileNamesWithExtension);
	}

	private static Photo WithCompanionsOnSubPath(string fileNameWithExtension, string[] companionFileNamesWithExtension, string subPath)
	{
		return Photo(fileNameWithExtension, subPath, companionFileNamesWithExtension: companionFileNamesWithExtension);
	}

	private static Photo Photo(string fileNameWithExtension, string targetRelativeDirectoryPath, string? newName = null, string[]? companionFileNamesWithExtension = null, string? sha1Hash = null)
	{
		return PhotoFakes.Create(fileNameWithExtension, targetRelativeDirectoryPath: targetRelativeDirectoryPath, newName: newName,
			companionFileNamesWithExtension: companionFileNamesWithExtension, sha1Hash: sha1Hash, outputFolder: DefaultOutputPath, sourcePath: DefaultSourcePath);
	}

	private static string SourcePath(string fileName)
	{
		return MockFileSystemHelper.Path(Path.Combine(DefaultSourcePath, fileName));
	}

	private static string OutputPath(string fileName)
	{
		return MockFileSystemHelper.Path(Path.Combine(DefaultOutputPath, fileName), true);
	}

	private static Photo WithCompanionsAndSha1HashOnRoot(string fileNameWithExtension, int sha1HashSampleId, params CompanionFileNameHashPair[] companionFileNamesWithExtension)
	{
		return WithCompanionsAndSha1HashAndOutput(fileNameWithExtension, sha1HashSampleId,RootForMockFileSystem(), companionFileNamesWithExtension);
	}

	private static Photo SingleWithSha1HashOnRoot(string fileNameWithExtension, int sha1HashSampleId)
	{
		return WithSha1HashAndOutput(fileNameWithExtension, sha1HashSampleId,RootForMockFileSystem());
	}

	private static Photo SingleWithDataOnRoot(string fileNameWithExtension, byte[] data)
	{
		var sha1Hash = Sha1HashHelper.CalculateArray(data);
		return Photo(fileNameWithExtension, sha1Hash: sha1Hash, targetRelativeDirectoryPath: "");
	}

	private static Photo SingleWithSha1HashOnSubPath(string fileNameWithExtension, int sha1HashSampleId, string subPath)
	{
		return WithSha1HashAndOutput(fileNameWithExtension, sha1HashSampleId, MockFileSystemHelper.Path(subPath));
	}

	private static Photo WithSha1HashAndOutput(string fileNameWithExtension, int sha1HashSampleId, string targetRelativeDirectoryPath)
	{
		return Photo(fileNameWithExtension, targetRelativeDirectoryPath: targetRelativeDirectoryPath, sha1Hash: Sha1HashFakes.Sample(sha1HashSampleId));
	}

	private static Photo WithCompanionsAndSha1HashAndOutput(string fileNameWithExtension, int sha1HashSampleId, string targetRelativeDirectoryPath, params CompanionFileNameHashPair[] companionFileNameHashPairs)
	{
		var companionFileNamesWithExtension = companionFileNameHashPairs.Select(s => s.CompanionFileName).ToArray();
		var photo = Photo(fileNameWithExtension, targetRelativeDirectoryPath: targetRelativeDirectoryPath, sha1Hash: Sha1HashFakes.Sample(sha1HashSampleId), companionFileNamesWithExtension: companionFileNamesWithExtension);

		if (photo.CompanionFiles == null)
			throw new ApplicationException("Companion files should be set");

		if (photo.CompanionFiles.Count != companionFileNameHashPairs.Length)
			throw new ApplicationException($"{nameof(photo.CompanionFiles)} array should be same size as {nameof(companionFileNamesWithExtension)}");

		for (var hashPairIndex = 0; hashPairIndex < companionFileNameHashPairs.Length; hashPairIndex++)
		{
			var companionFile = photo.CompanionFiles.ElementAt(hashPairIndex);
			var companionFileNameHashPair = companionFileNameHashPairs.ElementAt(hashPairIndex);
			companionFile.Sha1Hash = Sha1HashFakes.Sample(companionFileNameHashPair.Sha1SampleId);
		}

		return photo;
	}

	private static List<Photo> CreatePhotosOnMockFileSystem(Dictionary<Photo, byte[]> inputFileContentByPhoto, IMockFileDataAccessor mockFileSystem)
	{
		var photos = new List<Photo>();
		foreach (var (photo, fileContent) in inputFileContentByPhoto)
		{
			photos.Add(photo);
			mockFileSystem.AddFile(photo.PhotoFile.SourcePath, new MockFileData(fileContent));
		}
		return photos;
	}

	private static void CreateFilesOnMockFileSystem(Dictionary<string, byte[]> fileContentByPhoto, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var (filePath, fileContent) in fileContentByPhoto)
			mockFileSystem.AddFile(filePath, new MockFileData(fileContent));
	}

	private static void SetupFileSystemWithDummyFiles(IEnumerable<Photo> photos, IMockFileDataAccessor mockFileSystem)
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

	private static void SetupFileSystemWithDummyFiles(IEnumerable<string> filePaths, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var filePath in filePaths)
			mockFileSystem.AddFile(filePath, string.Empty);
	}

	private static void VerifyFilesExistOnFileSystem(IEnumerable<Photo> photos, IMockFileDataAccessor mockFileSystem)
	{
		foreach (var photo in photos)
		{
			mockFileSystem.FileExists(photo.PhotoFile.TargetFullPath).Should().Be(true);
			if(photo.CompanionFiles == null)
				continue;
			foreach (var companionFile in photo.CompanionFiles)
				mockFileSystem.FileExists(companionFile.TargetFullPath).Should().Be(true);
		}
	}

	private record GnuHashFormat(int Sha1SampleId, string Path);

	private static string GnuHashFileOutput(params GnuHashFormat[] gnuHashFormats)
	{
		return gnuHashFormats.Aggregate(string.Empty, (current, gnuHashFormat) =>
			current + $"{Sha1HashFakes.Sample(gnuHashFormat.Sha1SampleId)}  {MockFileSystemHelper.Path(gnuHashFormat.Path)}{Environment.NewLine}");
	}

	private static string RootForMockFileSystem()
	{
		return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\" : "";
	}

	private record CompanionFileNameHashPair(string CompanionFileName, int Sha1SampleId);

	#endregion
}
