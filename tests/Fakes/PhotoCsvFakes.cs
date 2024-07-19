namespace PhotoCli.Tests.Fakes;

public static class PhotoCsvFakes
{
	public static PhotoCsv CreateWithFileName(ExifData exifData, string fileNameWithExtension, string sourceFolder, List<string>? reverseGeocodes = null)
	{
		var sourcePhotoPath = MockFileSystemHelper.Combine(sourceFolder, fileNameWithExtension);
		return WithExifData(exifData, sourcePhotoPath, reverseGeocodes: reverseGeocodes);
	}

	public static PhotoCsv WithTargetRelativePath(ExifData exifData, string fileNameWithExtension, string sourceFolder, string targetRelativePath, List<string>? reverseGeocodes = null)
	{
		var sourcePhotoPath = MockFileSystemHelper.Combine(sourceFolder, fileNameWithExtension);
		return WithExifData(exifData, sourcePhotoPath, targetRelativePath, reverseGeocodes: reverseGeocodes);
	}

	public static PhotoCsv WithExifData(ExifData exifData, string sourcePhotoPath, string? targetRelativePath = null, List<string>? reverseGeocodes = null, bool useRelativePathOutput = false)
	{
		var sourcePath = MockFileSystemHelper.Path(sourcePhotoPath, useRelativePathOutput);
		var photoNewPath = targetRelativePath.IsPresent() ? MockFileSystemHelper.Path(targetRelativePath, true) : string.Empty;
		var reverseGeocodeFormat = reverseGeocodes != null ? ReverseGeocodeFakes.Format(reverseGeocodes) : string.Empty;
		var takenDateTime = exifData.TakenDate;
		var coordinate = exifData.Coordinate;
		return new PhotoCsv(sourcePath, photoNewPath, takenDateTime, reverseGeocodeFormat, coordinate?.Latitude, coordinate?.Longitude, takenDateTime?.Year,
			takenDateTime?.Month, takenDateTime?.Day, takenDateTime?.Hour,
			takenDateTime?.Minute, takenDateTime?.Second, reverseGeocodes?.ElementAtOrDefault(0) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(1) ?? string.Empty,
			reverseGeocodes?.ElementAtOrDefault(2) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(3) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(4) ?? string.Empty,
			reverseGeocodes?.ElementAtOrDefault(5) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(6) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(7) ?? string.Empty);
	}

	public static PhotoCsv CreateWithoutExifData(string sourcePhotoPath, string? targetRelativePath = null, bool useRelativePathOutput = false)
	{
		var sourcePath = MockFileSystemHelper.Path(sourcePhotoPath, useRelativePathOutput);
		var photoNewPath = targetRelativePath.IsPresent() ? MockFileSystemHelper.Path(targetRelativePath, true) : string.Empty;
		return new PhotoCsv(sourcePath, photoNewPath);
	}
}
