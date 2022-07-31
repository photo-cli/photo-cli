namespace PhotoCli.Models.ReverseGeocode.GoogleMaps;

public record GoogleMapsAddressComponent
{
	[JsonPropertyName("long_name")]
	public string? LongName { get; set; }

	[JsonPropertyName("short_name")]
	public string? ShortName { get; set; }

	[JsonPropertyName("types")]
	public List<string>? Types { get; set; }
}
