namespace PhotoCli.Services.Contracts;

public interface IReverseGeocodeFetcherService
{
	Task<ReverseGeocodeResult> Fetch(IReadOnlyCollection<Photo> photos, bool parallelProcessing);
	void RateLimitWarning();
}
