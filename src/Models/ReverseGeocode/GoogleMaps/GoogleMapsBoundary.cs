namespace PhotoCli.Models.ReverseGeocode.GoogleMaps;

public record GoogleMapsBoundary
{
	[JsonPropertyName("northeast")]
	public GoogleMapsLocation? Northeast { get; set; }

	[JsonPropertyName("southwest")]
	public GoogleMapsLocation? Southwest { get; set; }
}
