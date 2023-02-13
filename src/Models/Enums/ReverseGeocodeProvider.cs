namespace PhotoCli.Models.Enums;

public enum ReverseGeocodeProvider : byte
{
	Disabled = 0,

	/// <summary>
	///     BigDataCloud - https://www.bigdatacloud.com/
	///     Needs api key - has free tier
	///     Api key can be set on:
	///     - Application argument: -b [api-key] or --bigdatacloud-key [api-key]
	///     - Environment variable: PHOTO_CLI_BIG_DATA_CLOUD_API_KEY
	///     Free tier count limit : 50.000req/month
	///     Api docs : https://www.bigdatacloud.com/geocoding-apis/reverse-geocode-to-city-api/
	///     Support localized results. language code parameter value : https://www.bigdatacloud.com/supported-languages/
	/// </summary>
	BigDataCloud = 1,

	/// <summary>
	///     OpenStreetMap Foundation - https://www.openstreetmap.org/
	///     Free but limited to maximum 1 request per second
	///     Api docs : https://nominatim.org/release-docs/latest/api/Reverse/
	///     Usage policy : https://operations.osmfoundation.org/policies/nominatim/
	/// </summary>
	OpenStreetMapFoundation = 2,

	/// <summary>
	///     Google Maps - https://maps.google.com/
	///     Needs api key - no free tier
	///     Api key can be set on:
	///     - Application argument: -k [api-key] or --googlemaps-key [api-key]
	///     - Environment variable: PHOTO_CLI_GOOGLE_MAPS_API_KEY
	///     - appsettings.json as root property: GoogleMapsApiKey
	///     Api docs: https://developers.google.com/maps/documentation/geocoding/overview/
	///     Support localized results. language code parameter value : https://developers.google.com/maps/faq#languagesupport
	/// </summary>
	GoogleMaps = 3,

	// MapQuest = 4, MapQuest terminated theirs Nominatim product. We don't want to change the existing enum values, this 4 may used for future integrations

	/// <summary>
	///     LocationIq - https://locationiq.com/
	///     Needs api key - has free tier
	///     Free tier count limit : 5000req/day
	///     Free tier rate limit: 1req/sec
	///     Api key can be set on:
	///     - Application argument: -q [api-key] or --locationiq-key [api-key]
	///     - Environment variable: PHOTO_CLI_LOCATIONIQ_API_KEY
	///     - appsettings.json as root property: LocationIqApiKey
	///     Map testing : https://locationiq.com/sandbox/geocoding/reverse/
	///     Api docs : https://locationiq.com/docs/
	/// </summary>
	LocationIq = 5,
}
