namespace PhotoCli.Models.ReverseGeocode.GoogleMaps;

public record GoogleMapsResponse
{
	[JsonPropertyName("plus_code")]
	public GoogleMapsPlusCode? PlusCode { get; set; }

	[JsonPropertyName("results")]
	public List<GoogleMapsResult>? Results { get; set; }

	[JsonPropertyName("status")]
	public string? Status { get; set; }
}
