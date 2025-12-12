namespace PhotoCli.Services.Contracts.ReverseGeocodes;

public interface IBigDataCloudReverseGeocodeService
{
	Task<ReverseGeocodeAddressResult> Get(Coordinate coordinate, PhotoFile photoFile, string? language, IEnumerable<int> adminLevels);
	Task<BigDataCloudResponse?> SerializeFullResponse(ReverseGeocodeRequest request);
	Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate, string? language);
}
