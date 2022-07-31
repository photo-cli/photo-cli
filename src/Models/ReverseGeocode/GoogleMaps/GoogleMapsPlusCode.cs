namespace PhotoCli.Models.ReverseGeocode.GoogleMaps;

public record GoogleMapsPlusCode
{
	[JsonPropertyName("compound_code")]
	public string? CompoundCode { get; set; }

	[JsonPropertyName("global_code")]
	public string? GlobalCode { get; set; }
}
