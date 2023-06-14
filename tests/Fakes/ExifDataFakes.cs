namespace PhotoCli.Tests.Fakes;

public static class ExifDataFakes
{
	private const int YearDefault = 2000;
	private const int MonthDefault = 1;
	private const int DayDefault = 1;

	public static ExifData Valid()
	{
		return WithDay(1);
	}

	public static ExifData WithDay(int day)
	{
		return Create(new DateTime(YearDefault, MonthDefault, day));
	}

	public static ExifData WithMonth(int month)
	{
		return Create(new DateTime(YearDefault, month, DayDefault));
	}

	public static ExifData WithPhotoTakenDate(DateTime photoTakenDate)
	{
		return Create(photoTakenDate);
	}

	public static ExifData WithYear(int year)
	{
		return Create(new DateTime(year, MonthDefault, DayDefault));
	}

	public static ExifData WithYearAndReverseGeocode(int year)
	{
		return Create(new DateTime(year, MonthDefault, DayDefault), reverseGeocodes: ReverseGeocodeFakes.Valid());
	}

	public static ExifData WithDayAndReverseGeocodeSampleId(int day, int sampleId)
	{
		return Create(new DateTime(YearDefault, MonthDefault, day), reverseGeocodes: ReverseGeocodeFakes.Sample(sampleId));
	}

	public static ExifData WithReverseGeocodeSampleId(int sampleId)
	{
		return Create(reverseGeocodes: ReverseGeocodeFakes.Sample(sampleId));
	}

	public static ExifData WithNoPhotoTakenDate()
	{
		return new ExifData(null, CoordinateFakes.Valid(), ToolOptionFakes.AddressSeparator) { ReverseGeocodes = ReverseGeocodeFakes.Valid()};
	}

	public static ExifData WithNoCoordinate()
	{
		return new ExifData(DateTimeFakes.Valid(), null, ToolOptionFakes.AddressSeparator);
	}

	public static ExifData WithNoReverseGeocode()
	{
		return new ExifData(DateTimeFakes.Valid(), null, ToolOptionFakes.AddressSeparator);
	}

	public static ExifData WithNoReverseGeocodeAndNoTakenDate()
	{
		return new ExifData(null, null, ToolOptionFakes.AddressSeparator);;
	}

	public static ExifData? WithInvalidFileFormat()
	{
		return null;
	}

	public static ExifData WithCoordinate(Coordinate coordinate)
	{
		return Create(coordinate: coordinate);
	}

	public static ExifData WithCoordinate(double latitude, double longitude)
	{
		return Create(coordinate: new Coordinate(latitude, longitude));
	}

	public static ExifData WithCoordinateAndReverseGeocode(double latitude, double longitude, List<string> reverseGeocodes)
	{
		return Create(null, new Coordinate(latitude, longitude), reverseGeocodes);
	}

	public static ExifData Create(DateTime? takenDate = null, Coordinate? coordinate = null, IEnumerable<string>? reverseGeocodes = null)
	{
		return new ExifData(takenDate, coordinate, "-") { ReverseGeocodes = reverseGeocodes };
	}
}
