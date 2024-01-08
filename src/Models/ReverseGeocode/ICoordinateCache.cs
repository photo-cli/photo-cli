namespace PhotoCli.Models.ReverseGeocode;

public interface ICoordinateCache<TResponse>
{
	bool TryGet(ReverseGeocodeRequest key, out TResponse? value);
	void SetResponse(ReverseGeocodeRequest key, TResponse? value);
}
