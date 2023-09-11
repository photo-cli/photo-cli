namespace PhotoCli.Tests.Fakes.ReverseGeocode;

public static class BigDataCloudFullResponseFakes
{
	public static BigDataCloudResponse Valid(Coordinate coordinate)
	{
		return new()
		{
			Latitude = coordinate.Latitude,
			Longitude = coordinate.Longitude,
			Continent = "Asia",
			ContinentCode = "AS",
			LocalityInfo = new BigDataCloudLocalityInfo()
			{
				Informative = new()
				{
					new()
					{
						Order = 1,
						IsoCode = "AS",
					}
				}
			}
		};
	}
}
