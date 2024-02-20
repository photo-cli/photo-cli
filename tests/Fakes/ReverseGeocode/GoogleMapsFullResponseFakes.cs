namespace PhotoCli.Tests.Fakes.ReverseGeocode;

public static class GoogleMapsFullResponseFakes
{
	public static GoogleMapsResponse Valid(Coordinate coordinate)
	{
		return new()
		{
			Results = new()
			{
				new()
				{
					Geometry = new()
					{
						Location = new()
						{
							Lat = coordinate.Latitude,
							Lng = coordinate.Longitude
						},
					},
				},
			}
		};
	}
}
