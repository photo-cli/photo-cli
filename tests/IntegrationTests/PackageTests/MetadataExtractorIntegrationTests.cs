namespace PhotoCli.Tests.IntegrationTests.PackageTests;

public class PhotoExifParserIntegrationTests
{
	public static TheoryData<string, DateTime?> FilePathWithExpectedPhotoTakenDate = new()
	{
		{ TestImagesPathHelper.ExifSubIfdDirectory.FilePath, TestImagesPathHelper.ExifSubIfdDirectory.PhotoTakenDate },
		{ TestImagesPathHelper.ExifIfd0Directory.FilePath, TestImagesPathHelper.ExifIfd0Directory.PhotoTakenDate },
		{ TestImagesPathHelper.ExifIfd0DirectoryHasNoDateTimeTag.FilePath, null },
		{ TestImagesPathHelper.DontHaveExifSubIdfAndExifIfd0Directory.FilePath, null },
		{ TestImagesPathHelper.HasCoordinateAndDateJPEGDirectory.FilePath, TestImagesPathHelper.HasCoordinateAndDateJPEGDirectory.PhotoTakenDate },
		{ TestImagesPathHelper.HasCoordinateAndDateHeicDirectory.FilePath, TestImagesPathHelper.HasCoordinateAndDateHeicDirectory.PhotoTakenDate }
	};

	public static TheoryData<string, Coordinate?> PhotosWithGpsDataWithExpectedCoordinate = new()
	{
		{ TestImagesPathHelper.HasGpsCoordinate.FilePath, TestImagesPathHelper.HasGpsCoordinate.PhotoTakenCoordinate },
		{ TestImagesPathHelper.HasGpsDirectoryButNotHaveCoordinate.FilePath, null },
		{ TestImagesPathHelper.HasNoGpsCoordinateDirectory.FilePath, null },
		{ TestImagesPathHelper.HasCoordinateAndDateJPEGDirectory.FilePath, TestImagesPathHelper.HasCoordinateAndDateJPEGDirectory.PhotoTakenCoordinate },
		{ TestImagesPathHelper.HasCoordinateAndDateHeicDirectory.FilePath, TestImagesPathHelper.HasCoordinateAndDateHeicDirectory.PhotoTakenCoordinate}
	};

	private readonly IExifParserService _sut;

	public PhotoExifParserIntegrationTests()
	{
		_sut = new ExifParserService(NullLogger<ExifParserService>.Instance, new System.IO.Abstractions.FileSystem(), ToolOptionFakes.Create(), new Statistics());
	}

	[Theory]
	[MemberData(nameof(FilePathWithExpectedPhotoTakenDate))]
	public void Photo_Taken_Date_Equals_To_Expected(string filePath, DateTime? dateExpected)
	{
		var photoExifData = _sut.Parse(filePath, true, false);
		photoExifData?.TakenDate.Should().Be(dateExpected);
	}

	[Theory]
	[MemberData(nameof(PhotosWithGpsDataWithExpectedCoordinate))]
	public void Coordinate_Equals_To_Expected(string filePath, Coordinate? coordinateExpected)
	{
		var exifData = _sut.Parse(filePath, false, true);
		exifData?.Coordinate.Should().Be(coordinateExpected);
	}

	[Theory]
	[InlineData("not-existing-photo-path")]
	public void Parsing_A_Non_Existing_Photo_Path_Throws_FileNotFoundException(string filePath)
	{
		Assert.Throws<FileNotFoundException>(() => _sut.Parse(filePath, It.IsAny<bool>(), It.IsAny<bool>()));
	}
}
