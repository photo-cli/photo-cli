namespace PhotoCli.Tests.Utils;

public static class ReverseGeocodeResponseAssertionExtensions
{
	public static void Verify(this BigDataCloudResponse? response)
	{
		if (response == null)
			throw new ArgumentNullException(nameof(response));

		using (new AssertionScope())
		{
			response.LocalityInfo.Should().NotBeNull();
			response.LocalityInfo?.Administrative.Should().NotBeNull().And.NotBeEmpty();
			response.LocalityInfo?.Informative.Should().NotBeNull().And.NotBeEmpty();

			response.LocalityInfo?.Administrative?.FirstOrDefault()?.Description.Should();
			response.LocalityInfo?.Administrative?.FirstOrDefault()?.Name.Should();
			response.LocalityInfo?.Administrative?.FirstOrDefault()?.Order.Should();
			response.LocalityInfo?.Administrative?.FirstOrDefault()?.AdminLevel.Should();
			response.LocalityInfo?.Administrative?.FirstOrDefault()?.IsoCode.Should();
			response.LocalityInfo?.Administrative?.FirstOrDefault()?.IsoName.Should();
			response.LocalityInfo?.Administrative?.FirstOrDefault()?.GeoNameId.Should();
			response.LocalityInfo?.Administrative?.FirstOrDefault()?.WikiDataId.Should();

			response.LocalityInfo?.Informative?.FirstOrDefault()?.Description.Should().NotBeNull();
			response.LocalityInfo?.Informative?.FirstOrDefault()?.Name.Should().NotBeNull();
			response.LocalityInfo?.Informative?.FirstOrDefault()?.Order.Should().NotBeNull();
			response.LocalityInfo?.Informative?.FirstOrDefault()?.IsoCode.Should().NotBeNull();
			response.LocalityInfo?.Informative?.FirstOrDefault()?.GeoNameId.Should().NotBeNull();
			response.LocalityInfo?.Informative?.FirstOrDefault()?.WikiDataId.Should().NotBeNull();

			response.Latitude.Should().NotBeNull();
			response.Longitude.Should().NotBeNull();
			response.PlusCode.Should().NotBeNull();
			response.LocalityLanguageRequested.Should().NotBeNull();
			response.Continent.Should().NotBeNull();
			response.ContinentCode.Should().NotBeNull();
			response.Longitude.Should().NotBeNull();
			response.CountryName.Should().NotBeNull();
			response.CountryCode.Should().NotBeNull();
			response.PrincipalSubdivision.Should().NotBeNull();
			response.City.Should().NotBeNull();
			response.Locality.Should().NotBeNull();
			response.Postcode.Should().NotBeNull();
		}
	}

	public static void Verify(this OpenStreetMapResponse? response, ReverseGeocodeProvider reverseGeocodeProvider)
	{
		if (response == null)
			throw new ArgumentNullException(nameof(response));

		using (new AssertionScope())
		{
			response.PlaceId.Should().NotBeNull();
			response.Lat.Should().NotBeNull();
			response.Lon.Should().NotBeNull();
			response.DisplayName.Should().NotBeNull();
			response.OsmType.Should().NotBeNull();
			response.Licence.Should().NotBeNull();
			response.OsmId.Should().NotBeNull();
			response.BoundingBox.Should().NotBeNull().And.NotBeEmpty();
			response.Address.Should().NotBeNull();
			response.Address?.Country.Should().NotBeNull();

			response.Address?.Postcode.Should().NotBeNull();
			response.Address?.Suburb.Should().NotBeNull();
			response.Address?.CountryCode.Should().NotBeNull();

			if (reverseGeocodeProvider is not ReverseGeocodeProvider.MapQuest and not ReverseGeocodeProvider.LocationIq)
			{
				response.Address?.Military.Should().NotBeNull();
			}

			if (reverseGeocodeProvider is not ReverseGeocodeProvider.MapQuest)
			{
				response.Address?.Province.Should().NotBeNull();
				response.Address?.Region.Should().NotBeNull();
				response.Address?.Road.Should().NotBeNull();
			}
		}
	}

	public static void Verify(this GoogleMapsResponse? response)
	{
		if (response == null)
			throw new ArgumentNullException(nameof(response));

		using (new AssertionScope())
		{
			response.Status.Should().Be("OK");

			response.Results.Should().NotBeNull().And.NotBeEmpty();

			response.Results?.FirstOrDefault()?.FormattedAddress.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.PlaceId.Should().NotBeNull();

			response.Results?.FirstOrDefault()?.AddressComponents.Should().NotBeNull().And.NotBeEmpty();
			response.Results?.FirstOrDefault()?.AddressComponents?.FirstOrDefault()?.LongName.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.AddressComponents?.FirstOrDefault()?.ShortName.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.AddressComponents?.FirstOrDefault()?.Types.Should().NotBeNull().And.NotBeEmpty();

			response.Results?.FirstOrDefault()?.Geometry.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.LocationType.Should().NotBeNull();

			response.Results?.FirstOrDefault()?.Geometry?.Bounds.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Bounds?.Northeast.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Bounds?.Northeast?.Lat.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Bounds?.Northeast?.Lng.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Bounds?.Southwest.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Bounds?.Southwest?.Lat.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Bounds?.Southwest?.Lng.Should().NotBeNull();

			response.Results?.FirstOrDefault()?.Geometry?.Location.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Location?.Lat.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Location?.Lng.Should().NotBeNull();

			response.Results?.FirstOrDefault()?.Geometry?.Viewport.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Viewport?.Northeast.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Viewport?.Northeast?.Lat.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Viewport?.Northeast?.Lng.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Viewport?.Southwest.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Viewport?.Southwest?.Lat.Should().NotBeNull();
			response.Results?.FirstOrDefault()?.Geometry?.Viewport?.Southwest?.Lng.Should().NotBeNull();

			response.Results?.FirstOrDefault()?.Types.Should().NotBeNull().And.NotBeEmpty();

			response.PlusCode.Should().NotBeNull();
			response.PlusCode?.CompoundCode.Should().NotBeNull();
			response.PlusCode?.GlobalCode.Should().NotBeNull();
		}
	}
}
