namespace PhotoCli.Models;

public record ExifData
{
	private readonly string _reverseGeocodeSeparator;

	public ExifData(DateTime? takenDate, Coordinate? coordinate, string reverseGeocodeSeparator)
	{
		(TakenDate, Coordinate, _reverseGeocodeSeparator) = (takenDate, coordinate, reverseGeocodeSeparator);
	}

	public DateTime? TakenDate { get; }
	public Coordinate? Coordinate { get; }

	public IEnumerable<string>? ReverseGeocodes { get; set; }
	public string? ReverseGeocodeFormatted => ReverseGeocodes != null ? string.Join(_reverseGeocodeSeparator, ReverseGeocodes) : null;
}
