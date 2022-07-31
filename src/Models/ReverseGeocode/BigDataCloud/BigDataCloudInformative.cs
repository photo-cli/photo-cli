namespace PhotoCli.Models.ReverseGeocode.BigDataCloud;

public record BigDataCloudInformative
{
	[JsonPropertyName("isoCode")]
	public string? IsoCode { get; set; }

	[JsonPropertyName("name")]
	public string? Name { get; set; }

	[JsonPropertyName("order")]
	public int? Order { get; set; }

	[JsonPropertyName("description")]
	public string? Description { get; set; }

	[JsonPropertyName("wikidataId")]
	public string? WikiDataId { get; set; }

	[JsonPropertyName("geonameId")]
	public int? GeoNameId { get; set; }
}
