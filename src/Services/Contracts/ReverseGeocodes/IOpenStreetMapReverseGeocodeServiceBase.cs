namespace PhotoCli.Services.Contracts.ReverseGeocodes;

public interface IOpenStreetMapReverseGeocodeServiceBase
{
	Task<ReverseGeocodeAddressResult> Get(Coordinate coordinate, PhotoFile photoFile, List<PropertyInfo> requestedAddressPropertyInfos);
	Task<OpenStreetMapResponse?> SerializeFullResponse(ReverseGeocodeRequest request);
	Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate);
}
