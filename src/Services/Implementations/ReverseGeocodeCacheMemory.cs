using System.Collections.Concurrent;

namespace PhotoCli.Services.Implementations;

public class ReverseGeocodeCacheMemory<TResponse>(Statistics statistics, ILogger<ReverseGeocodeCacheMemory<TResponse>> logger) : IReverseGeocodeCache<TResponse>
{
	private readonly ConcurrentDictionary<ReverseGeocodeRequest, TResponse?> _memoryCache = new();

	public Task<ReverseGeocodeCacheResult<TResponse>> TryGet(ReverseGeocodeRequest request, ReverseGeocodeProvider provider)
	{
		if (!_memoryCache.TryGetValue(request, out var value))
		{
			logger.LogInformation("Cache miss for {Request} from {Provider}", request, provider);
			return Task.FromResult(new ReverseGeocodeCacheResult<TResponse>(false, default));
		}

		++statistics.ReserveGeocodeFromMemory;
		logger.LogInformation("Cache hits for {Request} from {Provider} with {CacheEntry}", request, provider, value);
		return Task.FromResult(new ReverseGeocodeCacheResult<TResponse>(true, value));
	}

	public Task SetResponse(ReverseGeocodeRequest request, ReverseGeocodeProvider provider, TResponse? value)
	{
		_memoryCache[request] = value;
		logger.LogInformation("Setting cache for {Request} from {Provider} with {CacheEntry}", request, provider, value);
		return Task.CompletedTask;
	}
}
