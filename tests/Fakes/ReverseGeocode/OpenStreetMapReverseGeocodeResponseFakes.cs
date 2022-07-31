namespace PhotoCli.Tests.Fakes.ReverseGeocode;

public static class OpenStreetMapReverseGeocodeResponseFakes
{
	public static string Ankara()
	{
		return @"{
    ""place_id"": 87984223,
    ""licence"": ""Data © OpenStreetMap contributors, ODbL 1.0. https://osm.org/copyright"",
    ""osm_type"": ""way"",
    ""osm_id"": 9779903,
    ""lat"": ""39.9266859"",
    ""lon"": ""32.836603409371946"",
    ""display_name"": ""Anıtkabir, Dumlupınar Yolu, Mebusevleri Mahallesi, Ankara, Çankaya, Ankara, İç Anadolu Bölgesi, 06580, Türkiye"",
    ""address"": {
        ""military"": ""Anıtkabir"",
        ""road"": ""Dumlupınar Yolu"",
        ""suburb"": ""Mebusevleri Mahallesi"",
        ""city"": ""Ankara"",
        ""town"": ""Çankaya"",
        ""province"": ""Ankara"",
        ""region"": ""İç Anadolu Bölgesi"",
        ""postcode"": ""06580"",
        ""country"": ""Türkiye"",
        ""country_code"": ""tr""
    },
    ""boundingbox"": [
        ""39.9216778"",
        ""39.9317837"",
        ""32.8308225"",
        ""32.8416601""
    ]
}";
	}
}
