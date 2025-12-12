namespace PhotoCli.Services.Contracts.ReverseGeocodes;

public interface IGoogleMapsReverseGeocodeService
{
	Task<ReverseGeocodeAddressResult> Get(Coordinate coordinate, PhotoFile photoFile, string? language, IEnumerable<string> requestedAddressTypes);
	Task<GoogleMapsResponse?> SerializeFullResponse(ReverseGeocodeRequest request);
	Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate, string? language);
}
