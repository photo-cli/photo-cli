namespace PhotoCli.Models.ReverseGeocode.GoogleMaps;

public record GoogleMapsGeometry
{
	[JsonPropertyName("bounds")]
	public GoogleMapsBoundary? Bounds { get; set; }

	[JsonPropertyName("location")]
	public GoogleMapsLocation? Location { get; set; }

	[JsonPropertyName("location_type")]
	public string? LocationType { get; set; }

	[JsonPropertyName("viewport")]
	public GoogleMapsBoundary? Viewport { get; set; }
}
