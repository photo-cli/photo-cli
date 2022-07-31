namespace PhotoCli.Models.ReverseGeocode.GoogleMaps;

public record GoogleMapsLocation
{
	[JsonPropertyName("lat")]
	public double? Lat { get; set; }

	[JsonPropertyName("lng")]
	public double? Lng { get; set; }
}
