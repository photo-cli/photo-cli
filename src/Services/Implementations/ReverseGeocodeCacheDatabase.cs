using System.Collections.Concurrent;

namespace PhotoCli.Services.Implementations;

public class ReverseGeocodeCacheDatabase<TResponse>(IDbService dbService, Statistics statistics, ILogger<ReverseGeocodeCacheDatabase<TResponse>> logger)
	: IReverseGeocodeCache<TResponse>
{
	private readonly ConcurrentDictionary<ReverseGeocodeRequest, TResponse?> _memoryCache = new();

	public async Task<ReverseGeocodeCacheResult<TResponse>> TryGet(ReverseGeocodeRequest request, ReverseGeocodeProvider provider)
	{
		if (_memoryCache.TryGetValue(request, out var value))
		{
			++statistics.ReserveGeocodeFromMemory;
			return new ReverseGeocodeCacheResult<TResponse>(true, value);
		}

		var databaseCacheHit = await dbService.GetReverseGeocodeCache<TResponse>(request, provider);
		if (databaseCacheHit == null)
		{
			logger.LogInformation("Cache miss for {Request} from {Provider}", request, provider);
			return new ReverseGeocodeCacheResult<TResponse>(false, default);
		}

		++statistics.ReserveGeocodeFromDatabase;
		_memoryCache[request] = databaseCacheHit;
		logger.LogInformation("Cache hit for {Request} from {Provider} with {CacheEntry}", request, provider, databaseCacheHit);
		return new ReverseGeocodeCacheResult<TResponse>(true, databaseCacheHit);
	}

	public async Task SetResponse(ReverseGeocodeRequest request, ReverseGeocodeProvider provider, TResponse? value)
	{
		_memoryCache[request] = value;
		logger.LogInformation("Setting cache for {Request} from {Provider} with {CacheEntry}", request, provider, value);
		await dbService.SaveReverseGeocodeCache(request, value, provider);
	}
}
