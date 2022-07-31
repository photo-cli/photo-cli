namespace PhotoCli.Services.Contracts;

public interface IReverseGeocodeService
{
	Task<IEnumerable<string>> Get(Coordinate coordinate);
	Task<string> RawResponse(Coordinate coordinate);
	Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate);
}
