namespace PhotoCli.Tests.Fakes;

public static class ReverseGeocodeCacheEntityFakes
{
	public static ReverseGeocodeCacheEntity Sample(byte sampleId)
	{
		return new ReverseGeocodeCacheEntity(ReverseGeocodeProviderFakes.Valid(), double.MinValue, double.MaxValue, [1, 2, 3], sampleId, string.Empty, DateTime.Today);
	}
}
