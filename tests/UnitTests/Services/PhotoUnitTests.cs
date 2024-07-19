using System.IO.Abstractions;

namespace PhotoCli.Tests.UnitTests.Services;

public class PhotoUnitTests
{
	#region Constructor -  Only Main Photo File

	[Fact]
	public void Given_Valid_Main_Photo_Path_Should_Set_File_Properties()
	{
		var (mainPhotoFileInfo, mainPhotoFilePath) = CreateValidFileOnMockFileSystem(SamplePhotoExtension, SampleFileName);
		var photo = new Photo(mainPhotoFileInfo);
		using (new AssertionScope())
		{
			VerifyPhotoAfterInitializing(photo, new PhotoFileValues(mainPhotoFilePath, SamplePhotoExtension, SampleFileName));
			photo.CompanionFiles.Should().BeNull();
		}
	}

	[Fact]
	public void Given_Invalid_Main_Photo_FilePath_Should_Throw_File_Path_Format_Exception()
	{
		var invalidMainPhotoFileInfo = CreateInvalidFileOnMockFileSystem();
		ShouldThrowFilePathFormatException(() => _ = new Photo(invalidMainPhotoFileInfo));
	}

	#endregion

	#region Constructor - With Companion Files

	#region Valid

	[Fact]
	public void Given_Valid_Main_Photo_And_One_Valid_Companion_Path_Should_Set_File_Properties()
	{
		var (mainPhotoFileInfo, mainPhotoFilePath) = CreateValidFileOnMockFileSystem(SamplePhotoExtension, SampleFileName);
		var (companionFileInfo, companionFilePath) = CreateValidFileOnMockFileSystem(SampleCompanionExtension, SampleFileName);

		var photo = new Photo(mainPhotoFileInfo, [companionFileInfo]);
		using (new AssertionScope())
		{
			VerifyPhotoAfterInitializing(photo, new PhotoFileValues(mainPhotoFilePath, SamplePhotoExtension, SampleFileName));
			VerifyCompanionFileProperties(photo, new PhotoFileValues(companionFilePath, SampleCompanionExtension, SampleFileName));
		}
	}

	[Fact]
	public void Given_Valid_Main_Photo_And_Multiple_Valid_Companion_Path_Should_Set_File_Properties()
	{
		var (mainPhotoFileInfo, mainPhotoFilePath) = CreateValidFileOnMockFileSystem(SamplePhotoExtension, SampleFileName);

		const string companionExtension1 = "comp1";
		var (companionFileInfo1, companionFilePath1) = CreateValidFileOnMockFileSystem(companionExtension1, SampleFileName);

		const string companionExtension2 = "comp2";
		var (companionFileInfo2, companionFilePath2) = CreateValidFileOnMockFileSystem(companionExtension2, SampleFileName);

		const string companionExtension3 = "comp3";
		var (companionFileInfo3, companionFilePath3) = CreateValidFileOnMockFileSystem(companionExtension3, SampleFileName);

		var photo = new Photo(mainPhotoFileInfo, [companionFileInfo1, companionFileInfo2, companionFileInfo3]);
		using (new AssertionScope())
		{
			VerifyPhotoAfterInitializing(photo, new PhotoFileValues(mainPhotoFilePath, SamplePhotoExtension, SampleFileName));
			var photoFileValues = new PhotoFileValues[]
			{
				new(companionFilePath1, companionExtension1, SampleFileName),
				new(companionFilePath2, companionExtension2, SampleFileName),
				new(companionFilePath3, companionExtension3, SampleFileName),
			};
			VerifyCompanionFileProperties(photo, photoFileValues);
		}
	}

	#endregion

	#region Invalid

	[Fact]
	public void Given_Valid_Main_Photo_And_One_Invalid_Companion_Path_Should_Throw_File_Path_Format_Exception()
	{
		var (validMainPhotoFileInfo, _) = CreateValidFileOnMockFileSystem();
		var invalidCompanionFileInfo = CreateInvalidFileOnMockFileSystem();
		ShouldThrowFilePathFormatException(() => _ = new Photo(validMainPhotoFileInfo, [invalidCompanionFileInfo]));
	}

	[Fact]
	public void Given_Invalid_Main_Photo_And_One_Valid_Companion_Path_Should_Throw_File_Path_Format_Exception()
	{
		var invalidMainPhotoFileInfo = CreateInvalidFileOnMockFileSystem();
		var (validCompanionFileInfo, _) = CreateValidFileOnMockFileSystem();
		ShouldThrowFilePathFormatException(() => _ = new Photo(invalidMainPhotoFileInfo, [validCompanionFileInfo]));
	}

	[Fact]
	public void Given_Invalid_Main_Photo_And_One_Invalid_Companion_Path_Should_Throw_File_Path_Format_Exception()
	{
		var invalidMainPhotoFileInfo = CreateInvalidFileOnMockFileSystem("invalid-main-photo-file");
		var invalidCompanionFileInfo = CreateInvalidFileOnMockFileSystem("invalid-companion-file");
		ShouldThrowFilePathFormatException(() => _ = new Photo(invalidMainPhotoFileInfo, [invalidCompanionFileInfo]));
	}

	[Fact]
	public void Given_Valid_Main_Photo_And_One_Invalid_Path_In_Multiple_Companions_Should_Throw_File_Path_Format_Exception()
	{
		var (validMainPhotoFileInfo, _) = CreateValidFileOnMockFileSystem();
		var (validCompanionFileInfo1, _) = CreateValidFileOnMockFileSystem();
		var invalidCompanionFileInfo = CreateInvalidFileOnMockFileSystem();
		var (validCompanionFileInfo2, _) = CreateValidFileOnMockFileSystem();
		ShouldThrowFilePathFormatException(() =>
		{
			_ = new Photo(validMainPhotoFileInfo,
				[
					validCompanionFileInfo1,
					invalidCompanionFileInfo,
					validCompanionFileInfo2,
				]);
		});
	}

	#endregion

	#endregion

	#region Set Target

	#region Valid

	[Fact]
	public void SetTarget_With_Valid_Paths_On_Only_Main_Photo_File_Should_Match_Target_Paths()
	{
		var (mainPhotoFileInfo, _) = CreateValidFileOnMockFileSystem(SamplePhotoExtension, SampleFileName);
		var photo = new Photo(mainPhotoFileInfo);

		photo.SetTargetRelativePath(SampleTargetRelativePath);
		photo.SetTarget(SampleValidOutput);

		using (new AssertionScope())
		{
			var photoFile = photo.PhotoFile;
			var expectedTargetRelativePath = Path.Combine(SampleTargetRelativePath, $"{SampleFileName}.{SamplePhotoExtension}");
			photoFile.TargetRelativePath.Should().Be(expectedTargetRelativePath);
			var expectedTargetFullPath = Path.Combine(SampleValidOutput, expectedTargetRelativePath);
			photoFile.TargetFullPath.Should().Be(expectedTargetFullPath);
		}
	}

	[Fact]
	public void SetTarget_With_Valid_Paths_On_With_Single_Companion_Should_Match_Target_Paths()
	{
		var (mainPhotoFileInfo, _) = CreateValidFileOnMockFileSystem(SamplePhotoExtension, SampleFileName);
		var (companionFileInfo, _) = CreateValidFileOnMockFileSystem(SampleCompanionExtension, SampleFileName);

		var photo = new Photo(mainPhotoFileInfo, [companionFileInfo]);

		photo.SetTargetRelativePath(SampleTargetRelativePath);
		photo.SetTarget(SampleValidOutput);

		var companionFile = photo.CompanionFiles!.First();
		using (new AssertionScope())
		{
			CheckPhotoFile(photo.PhotoFile, SampleTargetRelativePath, SampleFileName, SamplePhotoExtension, SampleValidOutput);
			CheckPhotoFile(companionFile, SampleTargetRelativePath, SampleFileName, SampleCompanionExtension, SampleValidOutput);
		}
	}

	[Fact]
	public void SetTarget_With_Valid_Paths_On_With_Multiple_Companion_Should_Match_Target_Paths()
	{
		var (mainPhotoFileInfo, _) = CreateValidFileOnMockFileSystem(SamplePhotoExtension, SampleFileName);

		const string companionExtension1 = "comp1";
		var (companionFileInfo1, _) = CreateValidFileOnMockFileSystem(companionExtension1, SampleFileName);

		const string companionExtension2 = "comp2";
		var (companionFileInfo2, _) = CreateValidFileOnMockFileSystem(companionExtension2, SampleFileName);

		var photo = new Photo(mainPhotoFileInfo, [companionFileInfo1, companionFileInfo2]);

		photo.SetTargetRelativePath(SampleTargetRelativePath);
		photo.SetTarget(SampleValidOutput);

		var companionFile1 = photo.CompanionFiles!.First();
		var companionFile2 = photo.CompanionFiles!.ElementAt(1);
		using (new AssertionScope())
		{
			CheckPhotoFile(photo.PhotoFile, SampleTargetRelativePath, SampleFileName, SamplePhotoExtension, SampleValidOutput);
			CheckPhotoFile(companionFile1, SampleTargetRelativePath, SampleFileName, companionExtension1, SampleValidOutput);
			CheckPhotoFile(companionFile2, SampleTargetRelativePath, SampleFileName, companionExtension2, SampleValidOutput);
		}
	}

	#endregion

	#region Invalid

	[Fact]
	public void SetTarget_On_Which_Dont_Have_TargetRelativePath_Should_Throw_PhotoCliException()
	{
		var photo = ValidPhoto();
		var photoCliException = Assert.Throws<PhotoCliException>(() => photo.SetTarget(SampleValidOutput));
		photoCliException.Message.Should().Be("Can't SetTarget before setting TargetRelativePath");
	}

	#endregion

	#endregion

	#region Basic Properties

	[Fact]
	public void SetExifData_Should_Set_ExifData_Property()
	{
		var photo = ValidPhoto();
		var exifDataFake = ExifDataFakes.Valid();
		photo.SetExifData(exifDataFake);
		photo.ExifData.Should().Be(exifDataFake);
	}

	[Fact]
	public void SetNewName_Should_Set_NewName_Property()
	{
		var photo = ValidPhoto();
		const string newNameFake = "new-name";
		photo.SetNewName(newNameFake);
		photo.NewName.Should().Be(newNameFake);
	}

	[Fact]
	public void SetTargetRelativePath_Should_Set_TargetRelativePath_Property()
	{
		var photo = ValidPhoto();
		photo.SetTargetRelativePath(SampleTargetRelativePath);
		photo.TargetRelativePath.Should().Be(SampleTargetRelativePath);
	}

	#endregion

	#region Helpers

	private const string SampleFileName = "photo";
	private const string SamplePhotoExtension = "jpg";
	private const string SampleCompanionExtension = "mov";
	private const string SampleTargetRelativePath = "target-relative";
	private const string SampleValidOutput = "valid-output";

	private readonly MockFileSystem _mockFileSystem = new();
	private record PhotoFileValues(string FilePath, string Extension, string FileName);

	private static void VerifyCompanionFileProperties(Photo photo, params PhotoFileValues[] expectedCompanionFilePath)
	{
		var actualCompanionFiles = photo.CompanionFiles!.Select(s => new PhotoFileValues(s.SourcePath, s.Extension, s.FileName));
		actualCompanionFiles.Should().BeEquivalentTo(expectedCompanionFilePath);
	}

	private static void VerifyPhotoAfterInitializing(Photo photo, PhotoFileValues mainPhotoFileValues)
	{
		var photoFile = photo.PhotoFile;

		photoFile.Should().NotBeNull();
		photoFile.SourcePath.Should().Be(mainPhotoFileValues.FilePath);
		photoFile.SourceFullPath.Should().Be(mainPhotoFileValues.FilePath);
		photoFile.FileName.Should().Be(mainPhotoFileValues.FileName);
		photoFile.Extension.Should().Be(mainPhotoFileValues.Extension);
		photoFile.Sha1Hash.Should().BeNull();
		photoFile.TargetRelativePath.Should().BeNull();
		photoFile.TargetFullPath.Should().BeNull();

		photo.NewName.Should().BeNull();
		photo.ExifData.Should().BeNull();
		photo.HasExifData.Should().BeFalse();
		photo.TakenDateTime.Should().BeNull();
		photo.ReverseGeocodeFormatted.Should().BeNull();
		photo.Coordinate.Should().BeNull();
	}

	private static void CheckPhotoFile(PhotoFile photoFile, string targetRelativePath, string fileName, string extension, string outputFolder)
	{
		var expectedTargetRelativePath = Path.Combine(targetRelativePath, $"{fileName}.{extension}");
		photoFile.TargetRelativePath.Should().Be(expectedTargetRelativePath);

		var expectedTargetFullPath = Path.Combine(outputFolder, expectedTargetRelativePath);
		photoFile.TargetFullPath.Should().Be(expectedTargetFullPath);
	}

	private (IFileInfo, string) CreateValidFileOnMockFileSystem(string extension = "ext", string? fileName = "valid")
	{
		var filePath = MockFileSystemHelper.Path($"/folder/{fileName}.{extension}");
		return (_mockFileSystem.FileInfo.New(filePath), filePath);
	}

	private IFileInfo CreateInvalidFileOnMockFileSystem(string fileName = "invalid-file")
	{
		var filePath = MockFileSystemHelper.Path($"/folder/{fileName}");
		return _mockFileSystem.FileInfo.New(filePath);
	}

	private static void ShouldThrowFilePathFormatException(Action testCode)
	{
		var photoCliException = Assert.Throws<PhotoCliException>(testCode);
		photoCliException.Message.Should().Be("File path format is not correct");
	}

	private Photo ValidPhoto()
	{
		var (mainPhotoFileInfo, _) = CreateValidFileOnMockFileSystem();
		var photo = new Photo(mainPhotoFileInfo);
		return photo;
	}

	#endregion
}
