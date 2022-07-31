namespace PhotoCli.Models.ReverseGeocode.GoogleMaps;

public record GoogleMapsResult
{
	[JsonPropertyName("address_components")]
	public List<GoogleMapsAddressComponent>? AddressComponents { get; set; }

	[JsonPropertyName("formatted_address")]
	public string? FormattedAddress { get; set; }

	[JsonPropertyName("geometry")]
	public GoogleMapsGeometry? Geometry { get; set; }

	[JsonPropertyName("place_id")]
	public string? PlaceId { get; set; }

	[JsonPropertyName("plus_code")]
	public GoogleMapsPlusCode? PlusCode { get; set; }

	[JsonPropertyName("types")]
	public List<string>? Types { get; set; }
}
