namespace PhotoCli.Services.Contracts.ReverseGeocodes;

public interface IGoogleMapsReverseGeocodeService
{
	Task<IEnumerable<string>> Get(Coordinate coordinate, string? language, IEnumerable<string> requestedAddressTypes);
	Task<GoogleMapsResponse?> SerializeFullResponse(ReverseGeocodeRequest request);
	Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate, string? language);
}
