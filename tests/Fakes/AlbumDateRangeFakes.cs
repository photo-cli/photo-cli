namespace PhotoCli.Tests.Fakes;

public static class AlbumDateRangeFakes
{
	public static AlbumDateRange Valid()
	{
		return new AlbumDateRange(DateTime.Today.AddDays(-30), DateTime.Today);
	}

	public static AlbumDateRange ByExifData(params ExifData[] exifData)
	{
		var minimumDate = DateTime.MaxValue;
		var maximumDate = DateTime.MinValue;

		foreach (var data in exifData)
		{
			if (data.TakenDate == null)
				throw new ArgumentException("ExifData must have a TakenDate");

			if (data.TakenDate < minimumDate)
				minimumDate = data.TakenDate.Value;
			if (data.TakenDate > maximumDate)
				maximumDate = data.TakenDate.Value;
		}

		if (minimumDate == DateTime.MaxValue || maximumDate == DateTime.MinValue)
			throw new ArgumentException("ExifData must have a TakenDate");

		return new AlbumDateRange(minimumDate, maximumDate);
	}

	public static AlbumDateRange WithStartEnd(DateTime startDate, DateTime endDate)
	{
		return new AlbumDateRange(startDate, endDate);
	}
}
