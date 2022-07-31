namespace PhotoCli.Models.ReverseGeocode.OpenStreetMap;

public record OpenStreetMapAddress
{
	[JsonPropertyName("country_code")]
	public string? CountryCode { get; set; }

	[JsonPropertyName("country")]
	public string? Country { get; set; }

	[JsonPropertyName("region")]
	public string? Region { get; set; }

	[JsonPropertyName("province")]
	public string? Province { get; set; }

	[JsonPropertyName("city")]
	public string? City { get; set; }

	[JsonPropertyName("town")]
	public string? Town { get; set; }

	[JsonPropertyName("postcode")]
	public string? Postcode { get; set; }

	[JsonPropertyName("suburb")]
	public string? Suburb { get; set; }

	[JsonPropertyName("road")]
	public string? Road { get; set; }

	[JsonPropertyName("military")]
	public string? Military { get; set; }

	[JsonPropertyName("barracks")]
	public string? Barracks { get; set; }
}
