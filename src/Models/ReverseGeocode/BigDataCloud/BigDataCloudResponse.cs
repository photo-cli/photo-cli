namespace PhotoCli.Models.ReverseGeocode.BigDataCloud;

public record BigDataCloudResponse
{
	[JsonPropertyName("localityInfo")]
	public BigDataCloudLocalityInfo? LocalityInfo { get; set; }

	[JsonPropertyName("latitude")]
	public double? Latitude { get; set; }

	[JsonPropertyName("longitude")]
	public double? Longitude { get; set; }

	[JsonPropertyName("plusCode")]
	public string? PlusCode { get; set; }

	[JsonPropertyName("localityLanguageRequested")]
	public string? LocalityLanguageRequested { get; set; }

	[JsonPropertyName("continent")]
	public string? Continent { get; set; }

	[JsonPropertyName("continentCode")]
	public string? ContinentCode { get; set; }

	[JsonPropertyName("countryName")]
	public string? CountryName { get; set; }

	[JsonPropertyName("countryCode")]
	public string? CountryCode { get; set; }

	[JsonPropertyName("principalSubdivision")]
	public string? PrincipalSubdivision { get; set; }

	[JsonPropertyName("city")]
	public string? City { get; set; }

	[JsonPropertyName("locality")]
	public string? Locality { get; set; }

	[JsonPropertyName("postcode")]
	public string? Postcode { get; set; }
}
