namespace PhotoCli.Tests.Fakes.ReverseGeocode;

public static class BigDataCloudReverseGeocodeResponseFakes
{
	public static string Ankara()
	{
		return @"{
    ""latitude"": 39.925052642822266,
    ""longitude"": 32.834754943847656,
    ""lookupSource"": ""coordinates"",
    ""plusCode"": ""8GFJWRGM+2W"",
    ""localityLanguageRequested"": ""tr"",
    ""continent"": ""Asya"",
    ""continentCode"": ""AS"",
    ""countryName"": ""Türkiye"",
    ""countryCode"": ""TR"",
    ""principalSubdivision"": ""Ankara"",
    ""principalSubdivisionCode"": ""TR-06"",
    ""city"": """",
    ""locality"": ""Mebusevleri Mahallesi"",
    ""postcode"": """",
    ""localityInfo"": {
        ""administrative"": [
            {
                ""order"": 2,
                ""adminLevel"": 2,
                ""name"": ""Türkiye"",
                ""description"": ""Güneydoğu Avrupa ve Batı Asya'da yer alan ülke"",
                ""isoName"": ""Turkey"",
                ""isoCode"": ""TR"",
                ""wikidataId"": ""Q43"",
                ""geonameId"": 298795
            },
            {
                ""order"": 4,
                ""adminLevel"": 3,
                ""name"": ""İç Anadolu Bölgesi"",
                ""description"": ""Türkiye'nin ortasındaki coğrafi bölgesi"",
                ""wikidataId"": ""Q155526""
            },
            {
                ""order"": 5,
                ""adminLevel"": 4,
                ""name"": ""Ankara"",
                ""description"": ""Türkiye'nin bir ili"",
                ""isoName"": ""Ankara"",
                ""isoCode"": ""TR-06"",
                ""wikidataId"": ""Q2297724"",
                ""geonameId"": 323784
            },
            {
                ""order"": 6,
                ""adminLevel"": 6,
                ""name"": ""Çankaya"",
                ""description"": ""Ankara'da bir ilçe"",
                ""wikidataId"": ""Q1020646"",
                ""geonameId"": 6955677
            },
            {
                ""order"": 7,
                ""adminLevel"": 8,
                ""name"": ""Mebusevleri Mahallesi""
            }
        ],
        ""informative"": [
            {
                ""order"": 1,
                ""name"": ""Asya"",
                ""description"": ""Dünya'nın doğu yarıküresindeki bir kıta"",
                ""isoCode"": ""AS"",
                ""wikidataId"": ""Q48"",
                ""geonameId"": 6255147
            },
            {
                ""order"": 3,
                ""name"": ""Anadolu"",
                ""description"": ""Türkiye topraklarının büyük bölümünü oluşturan Batı Asya yarımadası"",
                ""wikidataId"": ""Q51614"",
                ""geonameId"": 323835
            }
        ]
    }
}";
	}

	public static string MultipleAdminLevel()
	{
		return @"{
    ""latitude"": 39.925052642822266,
    ""longitude"": 32.834754943847656,
    ""lookupSource"": ""coordinates"",
    ""plusCode"": ""8GFJWRGM+2W"",
    ""localityLanguageRequested"": ""tr"",
    ""continent"": ""Asya"",
    ""continentCode"": ""AS"",
    ""countryName"": ""Türkiye"",
    ""countryCode"": ""TR"",
    ""principalSubdivision"": ""Ankara"",
    ""principalSubdivisionCode"": ""TR-06"",
    ""city"": """",
    ""locality"": ""Mebusevleri Mahallesi"",
    ""postcode"": """",
    ""localityInfo"": {
        ""administrative"": [
            {
                ""order"": 2,
                ""adminLevel"": 2,
                ""name"": ""Türkiye"",
                ""description"": ""Güneydoğu Avrupa ve Batı Asya'da yer alan ülke"",
                ""isoName"": ""Turkey"",
                ""isoCode"": ""TR"",
                ""wikidataId"": ""Q43"",
                ""geonameId"": 298795
            },
            {
                ""order"": 4,
                ""adminLevel"": 3,
                ""name"": ""First value on level3"",
                ""description"": ""Türkiye'nin ortasındaki coğrafi bölgesi"",
                ""wikidataId"": ""Q155526""
            },
            {
                ""order"": 5,
                ""adminLevel"": 4,
                ""name"": ""Ankara"",
                ""description"": ""Türkiye'nin bir ili"",
                ""isoName"": ""Ankara"",
                ""isoCode"": ""TR-06"",
                ""wikidataId"": ""Q2297724"",
                ""geonameId"": 323784
            },
            {
                ""order"": 6,
                ""adminLevel"": 6,
                ""name"": ""Çankaya"",
                ""description"": ""Ankara'da bir ilçe"",
                ""wikidataId"": ""Q1020646"",
                ""geonameId"": 6955677
            },
            {
                ""order"": 7,
                ""adminLevel"": 8,
                ""name"": ""Mebusevleri Mahallesi""
            },
			{
                ""order"": 999,
                ""adminLevel"": 3,
                ""name"": ""Duplicate value on level3"",
                ""description"": ""Türkiye'nin ortasındaki coğrafi bölgesi"",
                ""wikidataId"": ""Q155526""
            }
        ],
        ""informative"": [
            {
                ""order"": 1,
                ""name"": ""Asya"",
                ""description"": ""Dünya'nın doğu yarıküresindeki bir kıta"",
                ""isoCode"": ""AS"",
                ""wikidataId"": ""Q48"",
                ""geonameId"": 6255147
            },
            {
                ""order"": 3,
                ""name"": ""Anadolu"",
                ""description"": ""Türkiye topraklarının büyük bölümünü oluşturan Batı Asya yarımadası"",
                ""wikidataId"": ""Q51614"",
                ""geonameId"": 323835
            }
        ]
    }
}";
	}
}
