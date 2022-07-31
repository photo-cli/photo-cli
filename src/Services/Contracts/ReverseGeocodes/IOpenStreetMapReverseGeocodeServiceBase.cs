namespace PhotoCli.Services.Contracts.ReverseGeocodes;

public interface IOpenStreetMapReverseGeocodeServiceBase
{
	Task<IEnumerable<string>> Get(Coordinate coordinate, List<PropertyInfo> requestedAddressPropertyInfos);
	Task<OpenStreetMapResponse?> SerializeFullResponse(Coordinate coordinate);
	Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate);
}
