using System.Collections.Concurrent;

namespace PhotoCli.Models.ReverseGeocode;

public class CoordinateCache<TResponse> : ICoordinateCache<TResponse>
{
	private readonly ConcurrentDictionary<ReverseGeocodeRequest, TResponse?> _cache = new();

	public bool TryGet(ReverseGeocodeRequest key, out TResponse? value)
	{
		return _cache.TryGetValue(key, out value);
	}

	public void SetResponse(ReverseGeocodeRequest key, TResponse? value)
	{
		_cache[key] = value;
	}
}
