namespace PhotoCli.Services.Contracts.ReverseGeocodes;

public interface IOpenStreetMapReverseGeocodeServiceBase
{
	Task<IEnumerable<string>> Get(Coordinate coordinate, List<PropertyInfo> requestedAddressPropertyInfos);
	Task<OpenStreetMapResponse?> SerializeFullResponse(ReverseGeocodeRequest request);
	Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate);
}
