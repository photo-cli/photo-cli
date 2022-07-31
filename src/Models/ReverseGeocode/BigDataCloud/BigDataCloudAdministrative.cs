namespace PhotoCli.Models.ReverseGeocode.BigDataCloud;

public record BigDataCloudAdministrative
{
	[JsonPropertyName("adminLevel")]
	public int? AdminLevel { get; set; }

	[JsonPropertyName("isoName")]
	public string? IsoName { get; set; }

	[JsonPropertyName("name")]
	public string? Name { get; set; }

	[JsonPropertyName("isoCode")]
	public string? IsoCode { get; set; }

	[JsonPropertyName("description")]
	public string? Description { get; set; }

	[JsonPropertyName("order")]
	public int? Order { get; set; }

	[JsonPropertyName("wikidataId")]
	public string? WikiDataId { get; set; }

	[JsonPropertyName("geoNameId")]
	public int? GeoNameId { get; set; }
}
