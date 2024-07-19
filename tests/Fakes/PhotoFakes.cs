using System.IO.Abstractions;

namespace PhotoCli.Tests.Fakes;

public static class PhotoFakes
{
	public static Photo WithNewFileNameDay(int day, int hourForSorting, string filePostFix)
	{
		var photoTakenDateTime = DateTimeFakes.WithDayHour(day, hourForSorting);
		var dateTimeFormat = DateTimeFakes.FormatDay(day);
		return Create(takenDate: photoTakenDateTime, newName: dateTimeFormat + filePostFix);
	}

	public static Photo WithNewFileNameMinute(int minute, int secondForSorting, string filePostFix)
	{
		var photoTakenDateTime = DateTimeFakes.WithMinuteSecond(minute, secondForSorting);
		var dateTimeFormat = DateTimeFakes.FormatMinute(minute);
		return Create(takenDate: photoTakenDateTime, newName: dateTimeFormat + filePostFix);
	}

	public static Photo WithNewFileNameSecond(int second, string filePostFix)
	{
		var photoTakenDateTime = DateTimeFakes.WithSecond(second);
		var dateTimeFormat = DateTimeFakes.FormatSecond(second);
		return Create(takenDate: photoTakenDateTime, newName: dateTimeFormat + filePostFix);
	}

	public static Photo WithDayHour(int day, int hour)
	{
		return Create(takenDate: DateTimeFakes.WithDayHour(day, hour));
	}

	public static Photo WithDay(int day)
	{
		return Create(takenDate: DateTimeFakes.WithDay(day));
	}

	public static Photo WithMonth(int month)
	{
		return Create(takenDate: DateTimeFakes.WithMonth(month));
	}

	public static Photo WithYear(int year)
	{
		return Create(takenDate: DateTimeFakes.WithYear(year));
	}

	public static Photo NoPhotoTakenDate()
	{
		return Create();
	}

	public static Photo NoReverseGeocode()
	{
		return Create();
	}

	public static Photo Valid(int sampleId = 1)
	{
		return ValidFileWithDay(sampleId);
	}

	public static Photo ValidFileWithDay(int day)
	{
		return Create(takenDate: DateTimeFakes.WithDay(day));
	}

	public static Photo WithMinuteSecond(int minute, int second)
	{
		return Create(takenDate: DateTimeFakes.WithMinuteSecond(minute, second));
	}

	public static Photo WithMinute(int minute)
	{
		return Create(takenDate: DateTimeFakes.WithMinute(minute));
	}

	public static Photo WithSecond(int second)
	{
		return Create(takenDate: DateTimeFakes.WithSecond(second));
	}

	public static Photo WithReverseGeocodes(params string[] reverseGeocodes)
	{
		return Create(reverseGeocodes: reverseGeocodes);
	}

	public static Photo WithReverseGeocodeSampleId(int sampleId)
	{
		return CreatePhotoWithReverseGeocodeAndDateTime(sampleId);
	}

	public static Photo WithReverseGeocodeAndDay(int reverseGeocodeSampleId, int day)
	{
		return CreatePhotoWithReverseGeocodeAndDateTime(reverseGeocodeSampleId, DateTimeFakes.WithDay(day));
	}

	public static Photo WithReverseGeocodeAndMinute(int reverseGeocodeSampleId, int minute)
	{
		return CreatePhotoWithReverseGeocodeAndDateTime(reverseGeocodeSampleId, DateTimeFakes.WithMinute(minute));
	}

	public static Photo WithReverseGeocodeAndSecond(int reverseGeocodeSampleId, int second)
	{
		return CreatePhotoWithReverseGeocodeAndDateTime(reverseGeocodeSampleId, DateTimeFakes.WithSecond(second));
	}

	private static Photo CreatePhotoWithReverseGeocodeAndDateTime(int reverseGeocodeSampleId, DateTime? photoTakenDate = null)
	{
		return Create(takenDate: photoTakenDate, coordinate: CoordinateFakes.Valid(), reverseGeocodes: ReverseGeocodeFakes.Sample(reverseGeocodeSampleId));
	}

	public static IReadOnlyCollection<Photo> DummyOrderedListWithCount(int count)
	{
		var list = new List<Photo>();
		for (var i = 1; i <= count; i++)
			list.Add(WithDay(i));
		return list;
	}

	public static Photo WithArchiveFileName(int second, string sha1Hash)
	{
		var newName = DateTimeFakes.FormatSecond(second) + ToolOptionFakes.ArchivePhotoTakenDateHashSeparator + sha1Hash;
		return Create(newName: newName, takenDate: DateTimeFakes.WithSecond(second), sha1Hash: sha1Hash);
	}

	public static Photo WithInvalidFileFormat()
	{
		return InvalidFileFormat();
	}

	public static (Photo, string) SourceAndFileNameWithExtensionWithFullSourcePath(string sourcePath, string fileNameWithExtension)
	{
		var sourceFullPath = MockFileSystemHelper.Path($"/{sourcePath}/{fileNameWithExtension}");
		var photo = Create(sourcePath: sourcePath, fileNameWithExtension: fileNameWithExtension);
		return (photo, sourceFullPath);
	}

	public static Photo WithValidFilePathInvalidFileFormat(string sourcePath, string fileNameWithExtension, string targetRelativeDirectoryPath)
	{
		return InvalidFileFormat(fileNameWithExtension, targetRelativeDirectoryPath: targetRelativeDirectoryPath, sourcePath: sourcePath);
	}

	public static Photo WithValidFilePathAndExifData(string sourcePath, string fileNameWithExtension, ExifData? photoExifData, string targetRelativeDirectoryPath)
	{
		return Create(fileNameWithExtension, photoExifData?.TakenDate, photoExifData?.Coordinate, targetRelativeDirectoryPath, sourcePath: sourcePath,
			reverseGeocodes: photoExifData?.ReverseGeocodes);
	}

	public static Photo WithSha1Hash(string fileNameWithExtension, string sha1Hash, string? targetRelativeDirectoryPath = "")
	{
		return Create(fileNameWithExtension, targetRelativeDirectoryPath: targetRelativeDirectoryPath, sha1Hash: sha1Hash);
	}

	public static Photo WithSecondAndSha1Hash(int second, string sha1Hash)
	{
		return Create(takenDate: DateTimeFakes.WithSecond(second), sha1Hash: sha1Hash);
	}

	public static Photo WithNoCoordinate()
	{
		return Create();
	}

	public static Photo WithCoordinate(double latitude, double longitude)
	{
		return Create(coordinate: new Coordinate(latitude, longitude));
	}

	public static Photo WithCoordinateAndReverseGeocode(double latitude, double longitude, IEnumerable<string> reverseGeocodes)
	{
		return Create(coordinate: new Coordinate(latitude, longitude), reverseGeocodes: reverseGeocodes);
	}

	public static Photo Create(string? fileNameWithExtension = null, DateTime? takenDate = null, Coordinate? coordinate = null, string? targetRelativeDirectoryPath = null, string? newName = null,
		IEnumerable<string>? reverseGeocodes = null, string? sourcePath = null, string? sha1Hash = null, string? outputFolder = null, string[]? companionFileNamesWithExtension = null)
	{
		var exifData = ExifDataFakes.Create(takenDate, coordinate, reverseGeocodes?.ToList());
		return CreateWithExifData(exifData, fileNameWithExtension, targetRelativeDirectoryPath, newName, sourcePath, sha1Hash, outputFolder, companionFileNamesWithExtension);
	}

	private static Photo InvalidFileFormat(string? fileNameWithExtension = null, string? targetRelativeDirectoryPath = null, string? newName = null, string? sourcePath = null, string? sha1Hash = null)
	{
		return CreateWithExifData(null, fileNameWithExtension, targetRelativeDirectoryPath, newName, sourcePath, sha1Hash);
	}

	public static Photo WithSourcePathAndExifData(string fileNameWithExtension, ExifData? exifData)
	{
		return CreateWithExifData(exifData, fileNameWithExtension);
	}

	public static Photo WithTargetRelativePathAndOutput(string targetRelativeDirectoryPath, string outputFolder)
	{
		return CreateWithExifData(targetRelativeDirectoryPath: targetRelativeDirectoryPath, outputFolder: outputFolder);
	}

	public static Photo CreateWithExifData(ExifData? exifData = null, string? fileNameWithExtension = null, string? targetRelativeDirectoryPath = null, string? newName = null,
		string? sourcePath = null, string? sha1Hash = null, string? outputFolder = null, string[]? companionFileNamesWithExtension = null)
	{
		sourcePath ??= DefaultSourcePath;
		var crossPlatformSourcePath = MockFileSystemHelper.Path(sourcePath);
		fileNameWithExtension ??= "dummy.jpg";
		var mainPhotoMockFileInfo = new MockFileInfo(new MockFileSystem(), Path.Combine(crossPlatformSourcePath, fileNameWithExtension));
		List<IFileInfo>? companionFileInfos = null;
		if (companionFileNamesWithExtension != null)
		{
			companionFileInfos = new List<IFileInfo>();
			foreach (var companionFileNameWithExtension in companionFileNamesWithExtension)
				companionFileInfos.Add(new MockFileInfo(new MockFileSystem(), Path.Combine(crossPlatformSourcePath, companionFileNameWithExtension)));
		}
		var photo = new Photo(mainPhotoMockFileInfo, companionFileInfos?.ToArray());
		if (exifData != null)
			photo.SetExifData(exifData);
		if(newName.IsPresent())
			photo.SetNewName(newName);
		if(targetRelativeDirectoryPath != null)
			photo.SetTargetRelativePath(targetRelativeDirectoryPath);
		if (outputFolder.IsPresent())
			photo.SetTarget(outputFolder);
		if (sha1Hash != null)
			photo.PhotoFile.Sha1Hash = sha1Hash;
		return photo;
	}

	public const string DefaultSourcePath = "/source-path";
}
