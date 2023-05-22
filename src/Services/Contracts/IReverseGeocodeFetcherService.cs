namespace PhotoCli.Services.Contracts;

public interface IReverseGeocodeFetcherService
{
	Task<Dictionary<string, ExifData?>> Fetch(Dictionary<string, ExifData?> exifDataByFilePath);
	void RateLimitWarning();
}
