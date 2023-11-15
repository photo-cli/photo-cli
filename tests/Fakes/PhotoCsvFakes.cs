namespace PhotoCli.Tests.Fakes;

public static class PhotoCsvFakes
{
	public static PhotoCsv CreateWithFileName(string fileNameWithExtension, string sourceFolder, DateTime? takenDateTime = null, List<string>? reverseGeocodes = null, double? latitude = null,
		double? longitude = null, string? outputPath = null)
	{
		var sourcePhotoPath = Path.Combine(sourceFolder, fileNameWithExtension);
		var outputPhotoNewPath = outputPath != null ? Path.Combine(outputPath, fileNameWithExtension) : string.Empty;
		return Create(sourcePhotoPath, outputPhotoNewPath, takenDateTime, reverseGeocodes, latitude, longitude);
	}

	public static PhotoCsv Create(string sourcePhotoPath, ExifData exifData, string? outputPhotoPath = null, List<string>? reverseGeocodes = null, bool useRelativePathOutput = false)
	{
		var sourcePath = MockFileSystemHelper.Path(sourcePhotoPath, useRelativePathOutput);
		var photoNewPath = outputPhotoPath.IsPresent() ? MockFileSystemHelper.Path(outputPhotoPath!, useRelativePathOutput) : string.Empty;
		var reverseGeocodeFormat = reverseGeocodes != null ? ReverseGeocodeFakes.Format(reverseGeocodes) : string.Empty;
		var takenDateTime = exifData.TakenDate;
		var coordinate = exifData.Coordinate;
		return new PhotoCsv(sourcePath, photoNewPath, takenDateTime, reverseGeocodeFormat, coordinate?.Latitude, coordinate?.Longitude, takenDateTime?.Year,
			takenDateTime?.Month, takenDateTime?.Day, takenDateTime?.Hour,
			takenDateTime?.Minute, takenDateTime?.Second, reverseGeocodes?.ElementAtOrDefault(0) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(1) ?? string.Empty,
			reverseGeocodes?.ElementAtOrDefault(2) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(3) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(4) ?? string.Empty,
			reverseGeocodes?.ElementAtOrDefault(5) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(6) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(7) ?? string.Empty);
	}

	public static PhotoCsv Create(string sourcePhotoPath, string? outputPhotoPath = null, DateTime? takenDateTime = null, List<string>? reverseGeocodes = null, double? latitude = null,
		double? longitude = null, bool useRelativePathOutput = false)
	{
		var sourcePath = MockFileSystemHelper.Path(sourcePhotoPath, useRelativePathOutput);
		var photoNewPath = outputPhotoPath.IsPresent() ? MockFileSystemHelper.Path(outputPhotoPath!, useRelativePathOutput) : string.Empty;
		var reverseGeocodeFormat = reverseGeocodes != null ? ReverseGeocodeFakes.Format(reverseGeocodes) : string.Empty;
		return new PhotoCsv(sourcePath, photoNewPath, takenDateTime, reverseGeocodeFormat, latitude, longitude, takenDateTime?.Year,
			takenDateTime?.Month, takenDateTime?.Day, takenDateTime?.Hour,
			takenDateTime?.Minute, takenDateTime?.Second, reverseGeocodes?.ElementAtOrDefault(0) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(1) ?? string.Empty,
			reverseGeocodes?.ElementAtOrDefault(2) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(3) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(4) ?? string.Empty,
			reverseGeocodes?.ElementAtOrDefault(5) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(6) ?? string.Empty, reverseGeocodes?.ElementAtOrDefault(7) ?? string.Empty);
	}
}
