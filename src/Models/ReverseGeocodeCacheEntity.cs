namespace PhotoCli.Models;

public record ReverseGeocodeCacheEntity(ReverseGeocodeProvider Provider, double Latitude, double Longitude, byte[] Response, byte Precision, string? Language, DateTime CreatedAt)
{
	public long Id { get; }
}
