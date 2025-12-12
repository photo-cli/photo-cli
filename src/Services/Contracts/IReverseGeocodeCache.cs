namespace PhotoCli.Services.Contracts;

public interface IReverseGeocodeCache<TResponse>
{
	Task<ReverseGeocodeCacheResult<TResponse>> TryGet(ReverseGeocodeRequest request, ReverseGeocodeProvider provider);
	Task SetResponse(ReverseGeocodeRequest request, ReverseGeocodeProvider provider, TResponse? value);
}
