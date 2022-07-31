namespace PhotoCli.Models.ReverseGeocode.OpenStreetMap;

public record OpenStreetMapResponse
{
	[JsonPropertyName("address")]
	public OpenStreetMapAddress? Address { get; set; }

	[JsonPropertyName("display_name")]
	public string? DisplayName { get; set; }

	[JsonPropertyName("place_id")]
	public long? PlaceId { get; set; }

	[JsonPropertyName("osm_id")]
	public long? OsmId { get; set; }

	[JsonPropertyName("osm_type")]
	public string? OsmType { get; set; }

	[JsonPropertyName("lat")]
	public double? Lat { get; set; }

	[JsonPropertyName("lon")]
	public double? Lon { get; set; }

	[JsonPropertyName("boundingbox")]
	public List<string>? BoundingBox { get; set; }

	[JsonPropertyName("licence")]
	public string? Licence { get; set; }
}
