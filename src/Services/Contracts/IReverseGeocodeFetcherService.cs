namespace PhotoCli.Services.Contracts;

public interface IReverseGeocodeFetcherService
{
	Task<IReadOnlyCollection<Photo>> Fetch(IReadOnlyCollection<Photo> photos);
	void RateLimitWarning();
}
