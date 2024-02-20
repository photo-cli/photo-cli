namespace PhotoCli.Tests.Fakes;

public static class PhotoEntityFakes
{
	public static PhotoEntity CreateWithExifData(string path, ExifData? exifData = null, string? sha1Hash = null)
	{
		var takenDate = exifData?.TakenDate;
		var coordinate = exifData?.Coordinate;
		var reverseGeocodes = exifData?.ReverseGeocodes?.ToList();
		var photoEntity = new PhotoEntity(path, DateTimeFakes.Valid(), takenDate, exifData?.ReverseGeocodeFormatted, coordinate?.Latitude, coordinate?.Longitude, takenDate?.Year,
			takenDate?.Month, takenDate?.Day,
			takenDate?.Hour, takenDate?.Minute, takenDate?.Second, reverseGeocodes?.ElementAtOrDefault(0), reverseGeocodes?.ElementAtOrDefault(1), reverseGeocodes?.ElementAtOrDefault(2),
			reverseGeocodes?.ElementAtOrDefault(3), reverseGeocodes?.ElementAtOrDefault(4), reverseGeocodes?.ElementAtOrDefault(5), reverseGeocodes?.ElementAtOrDefault(6),
			reverseGeocodes?.ElementAtOrDefault(7), sha1Hash);
		return photoEntity;
	}
}
