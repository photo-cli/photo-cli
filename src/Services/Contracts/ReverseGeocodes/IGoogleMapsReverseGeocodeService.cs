namespace PhotoCli.Services.Contracts.ReverseGeocodes;

public interface IGoogleMapsReverseGeocodeService
{
	Task<IEnumerable<string>> Get(Coordinate coordinate, string? language, IEnumerable<string> requestedAddressTypes);
	Task<GoogleMapsResponse?> SerializeFullResponse(Coordinate coordinate, string? language);
	Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate, string? language);
}
