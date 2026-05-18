using System.Reflection;
using System.Text.Json.Serialization;

namespace PhotoCli.Tests.Fakes;

public static class AlbumEntityFakes
{
	public static AlbumEntity Valid()
	{
		return Create(AlbumNameFakes.Valid(), AlbumType.UserDefined, new AlbumConfiguration());
	}
	public static AlbumEntity WithPhotoIdsAndName(string name, List<long> photoIds)
	{
		return Create(name, AlbumType.UserDefined, new AlbumConfiguration(photoIds));
	}

	public static AlbumEntity WithPhotoIdsAndId(int id, List<long> photoIds)
	{
		var albumEntity = WithPhotoIdsAndName(AlbumNameFakes.Sample(id), photoIds);
		albumEntity = SetId(id, albumEntity);
		return albumEntity;
	}

	public static AlbumEntity WithReverseGeocodeAndName(string name, AlbumReverseGeocode albumReverseGeocode)
	{
		return Create(name, AlbumType.ReverseGeocode, new AlbumConfiguration(ReverseGeocode: albumReverseGeocode));
	}

	public static AlbumEntity WithReverseGeocodeAndId(int id, AlbumReverseGeocode albumReverseGeocode)
	{
		var albumEntity = WithReverseGeocodeAndName(AlbumNameFakes.Sample(id), albumReverseGeocode);
		albumEntity = SetId(id, albumEntity);
		return albumEntity;
	}

	public static AlbumEntity WithDateRangeAndName(string name, AlbumDateRange albumDateRange)
	{
		return Create(name, AlbumType.UserDefined, new AlbumConfiguration(DateRange: albumDateRange));
	}

	public static AlbumEntity WithDateRangeAndId(int id, AlbumDateRange albumDateRange)
	{
		var albumEntity = WithDateRangeAndName(AlbumNameFakes.Sample(id), albumDateRange);
		albumEntity = SetId(id, albumEntity);
		return albumEntity;
	}

	public static AlbumEntity WithDateRangeAndPhotoIds(string name, AlbumDateRange albumDateRange, List<long> photoIds)
	{
		return Create(name, AlbumType.UserDefined, new AlbumConfiguration(photoIds, albumDateRange));
	}

	public static AlbumEntity WithRawConfigurationAndId(string rawConfiguration, int id)
	{
		var albumEntity = WithSpecificId(id);
		albumEntity.Configuration = rawConfiguration;
		return albumEntity;
	}

	public static AlbumEntity WithRawConfiguration(string rawConfiguration)
	{
		return new AlbumEntity(AlbumNameFakes.Valid(), AlbumType.UserDefined, DateTime.Now) { Configuration = rawConfiguration };
	}

	public static AlbumEntity WithConfigurationId(int id, AlbumConfiguration albumConfiguration)
	{
		var albumEntity = Create(AlbumNameFakes.Sample(id), AlbumType.UserDefined, albumConfiguration);
		albumEntity = SetId(id, albumEntity);
		return albumEntity;
	}

	public static AlbumEntity Sample(int sampleId)
	{
		return Create(AlbumNameFakes.Sample(sampleId), AlbumType.UserDefined, new AlbumConfiguration());
	}

	public static AlbumEntity Deleted(int sampleId)
	{
		var album = Create(AlbumNameFakes.Sample(sampleId), AlbumType.UserDefined, new AlbumConfiguration());
		album.IsDeleted = true;
		return album;
	}

	private static AlbumEntity Create(string name, AlbumType albumType, object configuration)
	{
		return new AlbumEntity(name, albumType, DateTime.Now)
		{
			Configuration = SerializeConfiguration(configuration),
		};
	}

	private static string SerializeConfiguration(object configuration)
	{
		return JsonSerializer.Serialize(configuration, new JsonSerializerOptions
		{
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		});
	}

	public static AlbumEntity WithSpecificId(int id)
	{
		var albumEntity = Create(AlbumNameFakes.Sample(id), AlbumType.UserDefined, new AlbumConfiguration());
		albumEntity = SetId(id, albumEntity);
		return albumEntity;
	}

	private static AlbumEntity SetId(int id, AlbumEntity albumEntity)
	{
		var idProperty = typeof(AlbumEntity).GetProperty(nameof(AlbumEntity.Id));
		if (idProperty == null || idProperty.CanWrite)
			throw new Exception("AlbumEntity Id not found or not readonly");

		// setting field directly because of a private setter
		var backingField = typeof(AlbumEntity).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
		backingField?.SetValue(albumEntity, id);

		return albumEntity;
	}
}
