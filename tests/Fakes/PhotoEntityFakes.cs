namespace PhotoCli.Tests.Fakes;

public static class PhotoEntityFakes
{
	public static PhotoEntity WithId(long id)
	{
		return CreateWithExifData(id: id);
	}

	public static PhotoEntity WithExifDataAndId(ExifData exifData, long id)
	{
		return CreateWithExifData(exifData: exifData, id: id);
	}

	public static PhotoEntity WithExifData(ExifData exifData)
	{
		return CreateWithExifData(exifData: exifData);
	}

	public static PhotoEntity WithPhotoTakenDate(DateTime photoTakenDate)
	{
		return CreateWithExifData(exifData: ExifDataFakes.WithPhotoTakenDate(photoTakenDate));
	}

	public static PhotoEntity WithPhotoTakenDateAndId(DateTime photoTakenDate, long id)
	{
		return CreateWithExifData(exifData: ExifDataFakes.WithPhotoTakenDate(photoTakenDate), id: id);
	}

	public static PhotoEntity WithPhotoTakenDateAndIdAdIsDeleted(DateTime photoTakenDate, long id, bool isDeleted)
	{
		var photo = CreateWithExifData(exifData: ExifDataFakes.WithPhotoTakenDate(photoTakenDate), id: id);
		return photo with { IsDeleted = isDeleted };
	}

	public static PhotoEntity Sample(int sampleId)
	{
		return WithId(sampleId);
	}

	public static PhotoEntity Deleted(int sampleId)
	{
		var photo = Sample(sampleId);
		return photo with { IsDeleted = true };
	}

	public static PhotoEntity WithoutPhotoTakenDateAndId(long id)
	{
		return CreateWithExifData(exifData: ExifDataFakes.WithNoPhotoTakenDate(), id: id);
	}

	public static PhotoEntity WithReverseGeocodes(IEnumerable<string> reverseGeocodes)
	{
		return CreateWithExifData(exifData: ExifDataFakes.WithReverseGeocodes(reverseGeocodes));
	}

	public static PhotoEntity WithReverseGeocodesAndId(IEnumerable<string> reverseGeocodes, long id)
	{
		return CreateWithExifData(exifData: ExifDataFakes.WithReverseGeocodes(reverseGeocodes), id: id);
	}

	public static PhotoEntity WithoutReverseGeocodes()
	{
		return CreateWithExifData(exifData: ExifDataFakes.WithoutReverseGeocodes());
	}

	public static PhotoEntity CreateWithExifData(string? path = null, ExifData? exifData = null, string? sha1Hash = null, long? id = null)
	{
		var takenDate = exifData?.TakenDate;
		var coordinate = exifData?.Coordinate;
		var reverseGeocodes = exifData?.ReverseGeocodes?.ToList();

		string pathEntity;
		if (path != null)
			pathEntity = path;
		else if (id != null)
			pathEntity = PathFakeById(id.Value);
		else
			pathEntity = "path.jpg";

		var photoEntity = new PhotoEntity(pathEntity, DateTimeFakes.Valid(), takenDate, exifData?.ReverseGeocodeFormatted, coordinate?.Latitude, coordinate?.Longitude, takenDate?.Year,
			takenDate?.Month, takenDate?.Day,
			takenDate?.Hour, takenDate?.Minute, takenDate?.Second, reverseGeocodes?.ElementAtOrDefault(0), reverseGeocodes?.ElementAtOrDefault(1), reverseGeocodes?.ElementAtOrDefault(2),
			reverseGeocodes?.ElementAtOrDefault(3), reverseGeocodes?.ElementAtOrDefault(4), reverseGeocodes?.ElementAtOrDefault(5), reverseGeocodes?.ElementAtOrDefault(6),
			reverseGeocodes?.ElementAtOrDefault(7), sha1Hash);

		if (id != null)
			photoEntity.Id = id.Value;
		return photoEntity;
	}

	public static List<PhotoEntity> ValidList()
	{
		return [WithId(1), WithId(2), WithId(3)];
	}

	public static List<PhotoEntity> ValidListWithAllPhotoTakenDate()
	{
		return [
			WithExifDataAndId(ExifDataFakes.WithDay(1), 1),
			WithExifDataAndId(ExifDataFakes.WithDay(2), 2),
			WithExifDataAndId(ExifDataFakes.WithDay(3), 3),
		];
	}

	private static string PathFakeById(long id) => $"{id}.jpg";
}
