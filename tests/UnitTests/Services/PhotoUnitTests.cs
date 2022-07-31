namespace PhotoCli.Tests.UnitTests.Services;

public class PhotoUnitTests
{
	[Fact]
	public void Using_FileInfo_Constructor_Object_Properties_Set()
	{
		var fileInfo = new MockFileSystem().FileInfo.FromFileName("/folder/photo.jpg");
		var reverseGeocodes = ReverseGeocodeFakes.Valid();
		var exifData = ExifDataFakes.Create(DateTimeFakes.WithYear(2000), CoordinateFakes.Valid(), reverseGeocodes);
		const string targetRelativeDirectoryPath = "folder";
		var photo = new Photo(fileInfo, exifData, targetRelativeDirectoryPath);
		photo.Extension.Should().Be("jpg");
		photo.FilePath.Should().Be(MockFileSystemHelper.Path("/folder/photo.jpg"));
		photo.ReverseGeocodeFormatted.Should().Be(ReverseGeocodeFakes.Format(reverseGeocodes));
		photo.HasReverseGeocode.Should().Be(true);
		photo.PhotoExifData.Should().Be(exifData);
		photo.FileNameWithoutExtension.Should().Be("photo");
		photo.PhotoTakenDateTime.Should().Be(exifData.TakenDate);
		photo.TargetRelativeDirectoryPath.Should().Be(targetRelativeDirectoryPath);
		photo.HasPhotoTakenDateTime.Should().Be(true);
	}

	[Fact]
	public void Using_File_Without_Extension_Throws_PhotoOrganizerToolException()
	{
		var fileInfo = new MockFileSystem().FileInfo.FromFileName("/folder/photo");
		Assert.Throws<PhotoCliException>(() => { _ = new Photo(fileInfo, ExifDataFakes.Create(DateTimeFakes.WithYear(2000), CoordinateFakes.Valid()), "folder"); });
	}
}
