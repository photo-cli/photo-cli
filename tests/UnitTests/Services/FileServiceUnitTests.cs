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
	public async Task Verify_File_Integrity(string outputFolder, bool expectedResult, Dictionary<Photo, byte[]> inputFileContentByPhoto, Dictionary<string, byte[]> outputFileContentByPhoto)
	{
		var mockFileSystem = new MockFileSystem();

		var photoInfos = new List<Photo>();
		foreach (var (photoInfo, fileContent) in inputFileContentByPhoto)
		{
			photoInfos.Add(photoInfo);
			mockFileSystem.AddFile(photoInfo.FilePath, new MockFileData(fileContent));
		}
		foreach (var (filePath, fileContent) in outputFileContentByPhoto)
		{
			mockFileSystem.AddFile(filePath, new MockFileData(fileContent));
		}

		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty());
		var verifySuccessful = await sut.VerifyFileIntegrity(photoInfos, outputFolder);
		verifySuccessful.Should().Be(expectedResult);
	}


	public static TheoryData<List<Photo>, string> PhotosWithSha1HashOnRootFolder = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", "5842c73cfdc5f347551bb6016e00c71bb1393160"),
			},
			$"5842c73cfdc5f347551bb6016e00c71bb1393160  file1.jpg{Environment.NewLine}"
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", "5842c73cfdc5f347551bb6016e00c71bb1393160"),
				PhotoFakes.WithSha1Hash("file2.jpg", "bb649a18b3e7bb3df3701587a13f833749091817"),
			},
			$"5842c73cfdc5f347551bb6016e00c71bb1393160  file1.jpg{Environment.NewLine}bb649a18b3e7bb3df3701587a13f833749091817  file2.jpg{Environment.NewLine}"
		},
	};

	public static TheoryData<List<Photo>, string> PhotosWithSha1HashOnSubFolders = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", "5842c73cfdc5f347551bb6016e00c71bb1393160", "sub1/"),
			},
			$"5842c73cfdc5f347551bb6016e00c71bb1393160  sub1/file1.jpg{Environment.NewLine}"
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithSha1Hash("file1.jpg", "5842c73cfdc5f347551bb6016e00c71bb1393160", "sub1/"),
				PhotoFakes.WithSha1Hash("file2.jpg", "bb649a18b3e7bb3df3701587a13f833749091817", "sub1/sub2"),
			},
			$"5842c73cfdc5f347551bb6016e00c71bb1393160  sub1/file1.jpg{Environment.NewLine}bb649a18b3e7bb3df3701587a13f833749091817  sub1/sub2/file2.jpg{Environment.NewLine}"
		}
	};

	[Theory]
	[MemberData(nameof(PhotosWithSha1HashOnRootFolder))]
	[MemberData(nameof(PhotosWithSha1HashOnSubFolders))]
	public async Task GnuHashFileTree_Should_Match_Contents(List<Photo> l1, string expectedHashFileTreeContent)
	{
		var mockFileSystem = new MockFileSystem();
		var sut = new FileService(mockFileSystem, NullLogger<FileService>.Instance, StatisticsFakes.Empty());
		await sut.SaveGnuHashFileTree(l1, "");

		await using var fileStream = mockFileSystem.FileStream.Create("sha1.lst", FileMode.Open);
		var streamReader = new StreamReader(fileStream);
		var text = await streamReader.ReadToEndAsync();
		text.Should().Be(expectedHashFileTreeContent);
	}
}
