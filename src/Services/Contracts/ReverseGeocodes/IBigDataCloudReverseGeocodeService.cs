namespace PhotoCli.Services.Contracts.ReverseGeocodes;

public interface IBigDataCloudReverseGeocodeService
{
	Task<IEnumerable<string>> Get(Coordinate coordinate, string? language, IEnumerable<int> adminLevels);

	Task<BigDataCloudResponse?> SerializeFullResponse(Coordinate coordinate, string? language);
	Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate, string? language);
}
