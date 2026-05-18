namespace PhotoCli.Services.Contracts;

public interface IReverseGeocodeService
{
	Task<ReverseGeocodeAddressResult> Get(Coordinate coordinate, PhotoFile photoFile);
	Task<string> RawResponse(Coordinate coordinate);
	Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate);
}
