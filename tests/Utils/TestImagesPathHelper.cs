namespace PhotoCli.Tests.Utils;

public static class TestImagesPathHelper
{
	private const string RootTestImagesDirectory = "TestImages";

	private static string ExifImage(string fileName)
	{
		return $"{RootTestImagesDirectory}/Exif/{fileName}";
	}

	public static string SingleFolder(string? path = null)
	{
		return TestImagesFolderPath("SingleFolder", path);
	}

	public static string SubFolders(string? path = null)
	{
		return TestImagesFolderPath("SubFolders", path);
	}

	private static string TestImagesFolderPath(string folder, string? path)
	{
		var combined = Path.Combine(RootTestImagesDirectory, folder);
		if (path != null)
			combined = Path.Combine(combined, path);
		return combined;
	}

	public static class ExifSubIfdDirectory
	{
		public static readonly string FilePath = ExifImage("ExifSubIfd-DateTimeOriginal.jpg");
		public static readonly DateTime PhotoTakenDate = new(2004, 8, 31, 19, 52, 58);
	}

	public static class ExifIfd0Directory
	{
		public static readonly string FilePath = ExifImage("Exifd0-DateTime.jpg");
		public static readonly DateTime PhotoTakenDate = new(2008, 7, 31, 10, 50, 0);
	}

	public static class ExifIfd0DirectoryHasNoDateTimeTag
	{
		public static readonly string FilePath = ExifImage("Exifd0-Directories-Exists-But-NotHaveDateTime.jpg");
	}

	public static class HasGpsDirectoryButNotHaveCoordinate
	{
		public static readonly string FilePath = ExifImage("GpsDirectoryNotHaveCoordinate.jpg");
	}

	public static class HasGpsCoordinate
	{
		private const string FileName = "HasGpsCoordinate.jpg";
		public static readonly string FilePath = ExifImage(FileName);
		public static readonly Coordinate PhotoTakenCoordinate = new Coordinate(43.46744833333334, 11.885126666663888);
	}

	public static class DontHaveExifSubIdfAndExifIfd0Directory
	{
		public static readonly string FilePath = ExifImage("NotHaveExifSubIfd-and-Ifd0-Directories.jpg");
	}

	public static class HasNoGpsCoordinateDirectory
	{
		public static readonly string FilePath = ExifImage("NoGpsCoordinateDirectory.jpg");
		public static readonly DateTime PhotoTakenDate = new(2008, 7, 16, 11, 33, 20);
	}

	public static class HasCoordinateAndDateJPEGDirectory
	{
		public static readonly string FilePath = ExifImage("Exif-HasCoordinateAndDate.jpeg");
		public static readonly DateTime PhotoTakenDate = new(2023, 6, 1, 20, 13, 20);
		public static readonly Coordinate PhotoTakenCoordinate = new(47.502391666666668, 19.034752777777779);
	}

	public static class HasCoordinateAndDateHeicDirectory
	{
		public static readonly string FilePath = ExifImage("Exif-HasCoordinateAndDate.HEIC");
		public static readonly DateTime PhotoTakenDate = new(2023, 1, 30, 15, 21, 22);
		public static readonly Coordinate PhotoTakenCoordinate = new(48.199038888888886, 16.371333333333332);
	}
}
