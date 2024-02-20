namespace PhotoCli.Tests.Fakes;

public static class ExifDataFakes
{
	private static readonly int _coordinatePrecision = ToolOptions.Default().CoordinatePrecision;

	public static ExifData Valid()
	{
		return WithDay(1);
	}

	public static ExifData WithDay(int day)
	{
		return Create(DateTimeFakes.WithDay(day));
	}

	public static ExifData WithMonth(int month)
	{
		return Create(DateTimeFakes.WithMonth(month));
	}

	public static ExifData WithPhotoTakenDate(DateTime photoTakenDate)
	{
		return Create(photoTakenDate);
	}

	public static ExifData WithYear(int year)
	{
		return Create(DateTimeFakes.WithYear(year));
	}

	public static ExifData WithSeconds(int second)
	{
		return Create(DateTimeFakes.WithSecond(second));
	}

	public static ExifData WithYearAndReverseGeocode(int year)
	{
		return Create(DateTimeFakes.WithYear(year), reverseGeocodes: ReverseGeocodeFakes.Valid());
	}

	public static ExifData WithDayAndReverseGeocodeSampleId(int day, int sampleId)
	{
		return Create(DateTimeFakes.WithDay(day), reverseGeocodes: ReverseGeocodeFakes.Sample(sampleId));
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

	public static ExifData WithCoordinateSampleId(int sampleId)
    {
    	return Create(coordinate: CoordinateFakes.Sample(sampleId));
    }

	public static ExifData WithCoordinateAndReverseGeocodeSampleId(int coordinateSampleId, int reverseGeocodeSampleId)
	{
		return Create(coordinate: CoordinateFakes.Sample(coordinateSampleId), reverseGeocodes: ReverseGeocodeFakes.Sample(reverseGeocodeSampleId));
	}

	public static ExifData Create(DateTime? takenDate = null, Coordinate? coordinate = null, IEnumerable<string>? reverseGeocodes = null)
	{
		return new ExifData(takenDate, coordinate, ToolOptionFakes.AddressSeparator) { ReverseGeocodes = reverseGeocodes };
	}

	public static ExifData Kenya()
	{
		return Create(new DateTime(2005, 8, 13, 9, 47, 23), new Coordinate(Math.Round(-0.37129999999999996, _coordinatePrecision), Math.Round(36.056416666666664, _coordinatePrecision)));
	}

	public static ExifData ItalyFlorence()
	{
		return Create(new DateTime(2005, 12, 14, 14, 39, 47), new Coordinate(Math.Round(43.78559443333333, _coordinatePrecision), Math.Round(11.234619433333334, _coordinatePrecision)));
	}

	public static ExifData ItalyArezzo1()
	{
		return Create(new DateTime(2008, 10, 22, 16, 28, 39), new Coordinate(Math.Round(43.46744833333334, _coordinatePrecision), Math.Round(11.885126666663888, _coordinatePrecision)));
	}

	public static ExifData ItalyArezzo2()
	{
		return Create(new DateTime(2008, 10, 22, 16, 29, 49), new Coordinate(Math.Round(43.46715666666389, _coordinatePrecision), Math.Round(11.885394999997223, _coordinatePrecision)));
	}

	public static ExifData ItalyArezzo3()
	{
		return Create(new DateTime(2008, 10, 22, 16, 38, 20), new Coordinate(Math.Round(43.467081666663894, _coordinatePrecision), Math.Round(11.884538333330555, _coordinatePrecision)));
	}

	public static ExifData ItalyArezzo4()
	{
		return Create(new DateTime(2008, 10, 22, 16, 43, 21), new Coordinate(Math.Round(43.468365, _coordinatePrecision), Math.Round(11.881634999972222, _coordinatePrecision)));
	}

	public static ExifData ItalyArezzo5()
	{
		return Create(new DateTime(2008, 10, 22, 16, 44, 1), new Coordinate(Math.Round(43.46844166666667, _coordinatePrecision), Math.Round(11.881515, _coordinatePrecision)));
	}

	public static ExifData ItalyArezzo6()
	{
		return Create(new DateTime(2008, 10, 22, 16, 46, 53), new Coordinate(Math.Round(43.468243333330555, _coordinatePrecision), Math.Round(11.880171666638889, _coordinatePrecision)));
	}

	public static ExifData ItalyArezzo7()
	{
		return Create(new DateTime(2008, 10, 22, 16, 52, 15), new Coordinate(Math.Round(43.467254999997223, _coordinatePrecision), Math.Round(11.879213333333334, _coordinatePrecision)));
	}

	public static ExifData ItalyArezzo8()
	{
		return Create(new DateTime(2008, 10, 22, 16, 55, 37), new Coordinate(Math.Round(43.466011666638892, _coordinatePrecision), Math.Round(11.87911166663889, _coordinatePrecision)));
	}

	public static ExifData ItalyArezzo9()
	{
		return Create(new DateTime(2008, 10, 22, 17, 0, 7), new Coordinate(Math.Round(43.464455000000001, _coordinatePrecision), Math.Round(11.881478333333334, _coordinatePrecision)));
	}

	public static ExifData ItalyArezzo9Duplicate()
	{
		return ItalyArezzo9();
	}

	public static ExifData UnitedKingdom()
	{
		return Create(new DateTime(2012, 6, 22, 19, 52, 31), new Coordinate(Math.Round(51.424838333333334, _coordinatePrecision), Math.Round(-0.67356166666666661, _coordinatePrecision)));
	}

	public static ExifData Spain1()
	{
		return Create(new DateTime(2015, 4, 10, 20, 12, 23), new Coordinate(Math.Round(40.446972222222222, _coordinatePrecision), Math.Round(-3.7247527777777778, _coordinatePrecision)));
	}

	public static ExifData Spain2()
	{
		return Spain1();
	}

	public static ExifData NoGpsCoordinate()
	{
		return Create(new DateTime(2008, 7, 16, 11, 33, 20));
	}
}
