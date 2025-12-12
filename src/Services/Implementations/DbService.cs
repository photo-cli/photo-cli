using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace PhotoCli.Services.Implementations;

public class DbService : IDbService
{
	private readonly IArchiveDbContextProvider _archiveDbContextProvider;
	private readonly ILogger<DbService> _logger;
	private readonly ToolOptions _toolOptions;
	private readonly Statistics _statistics;
	private readonly IConsoleWriter _consoleWriter;

	private ArchiveDbContext _archiveDbContext => _archiveDbContextProvider.CreateOrGetInstance();

	public DbService(IArchiveDbContextProvider archiveDbContextProviderProvider, ToolOptions toolOptions, Statistics statistics, IConsoleWriter consoleWriter, ILogger<DbService> logger)
	{
		_archiveDbContextProvider = archiveDbContextProviderProvider;
		_toolOptions = toolOptions;
		_statistics = statistics;
		_consoleWriter = consoleWriter;
		_logger = logger;
	}

	public async Task<ArchiveResult> Archive(IEnumerable<Photo> photos, bool isDryRun = false)
	{
		const string progressName = "Archiving photos to SQLite";
		_consoleWriter.ProgressStart(progressName);
		var photoEntities = new List<PhotoEntity>();
		foreach (var photo in photos)
		{
			var exifData = photo.ExifData;
			var takenDate = exifData?.TakenDate;
			var coordinate = exifData?.Coordinate;
			var reverseGeocodes = exifData?.ReverseGeocodes?.ToList();

			if (photo.PhotoFile.TargetRelativePath.IsMissing())
				throw new PhotoCliException($"Can't archive, {nameof(PhotoFile.TargetRelativePath)} is missing for {photo.PhotoFile.SourceFullPath}");

			var photoEntity = new PhotoEntity(photo.PhotoFile.TargetRelativePath, DateTime.Now, takenDate, exifData?.ReverseGeocodeFormatted, coordinate?.Latitude, coordinate?.Longitude, takenDate?.Year,
				takenDate?.Month, takenDate?.Day,
				takenDate?.Hour, takenDate?.Minute, takenDate?.Second, reverseGeocodes?.ElementAtOrDefault(0), reverseGeocodes?.ElementAtOrDefault(1), reverseGeocodes?.ElementAtOrDefault(2),
				reverseGeocodes?.ElementAtOrDefault(3), reverseGeocodes?.ElementAtOrDefault(4), reverseGeocodes?.ElementAtOrDefault(5), reverseGeocodes?.ElementAtOrDefault(6),
				reverseGeocodes?.ElementAtOrDefault(7), photo.PhotoFile.Sha1Hash);

			photoEntities.Add(photoEntity);
		}

		if (isDryRun)
		{
			_logger.LogInformation("On dry run, {PhotoCount} photos, changes are not persist on DB", photoEntities.Count);
			return new ArchiveResult(true, []);
		}

		_logger.LogInformation("Sending {PhotoCount} photos to DB", photoEntities.Count);
		await _archiveDbContext.Photos.AddRangeAsync(photoEntities);
		var affectedRows = await _archiveDbContext.SaveChangesAsync();

		if (photoEntities.Count != affectedRows)
		{
			_logger.LogCritical("Mismatch between sent {PhotoCount} and {AffectedRows}", photoEntities.Count, affectedRows);
			return new ArchiveResult(false, []);
		}
		_logger.LogInformation("Saved photos on database with {AffectedRows} records", affectedRows);
		_consoleWriter.ProgressFinish(progressName);
		return new ArchiveResult(true, photoEntities);
	}

	public async Task<AlbumResult> NewIndividualAlbum(string name, List<PhotoEntity> photos, bool isDryRun = false)
	{
		const string progressName = "Saving individual album";
		_consoleWriter.ProgressStart(progressName);
		var existingAlbum = await GetAlbumByName(name);
		if (existingAlbum != null)
			return AlbumResult.AlbumExists;
		var newPhotoIds = photos.Select(s => s.Id).ToList();
		if (newPhotoIds.Count == 0)
			return AlbumResult.NoPhotosToAddInAlbum;
		var configuration = new AlbumConfiguration(newPhotoIds);
		var albumResult = await SaveAlbum(name, configuration, isDryRun);
		_consoleWriter.ProgressFinish(progressName);
		return albumResult;
	}

	public async Task<AlbumResult> UpdateIndividualAlbum(int albumId, List<PhotoEntity> photos, bool isDryRun = false)
	{
		const string progressName = "Updating individual album";
		_consoleWriter.ProgressStart(progressName);
		var newPhotoIds = photos.Select(s => s.Id).ToList();
		if (newPhotoIds.Count == 0)
			return AlbumResult.NoPhotosToAddInAlbum;

		var existingAlbum = await GetAlbumById(albumId);
		if (existingAlbum == null)
			return AlbumResult.AlbumNotFound;

		if (!DeserializeConfiguration<AlbumConfiguration>(existingAlbum.Configuration, out var existingConfiguration))
			return AlbumResult.ExistingConfigurationNotInCorrectFormat;

		List<long> updatedPhotoIds;
		if (existingConfiguration.PhotoIds != null)
		{
			updatedPhotoIds = new List<long>(existingConfiguration.PhotoIds);
			updatedPhotoIds.AddRange(newPhotoIds);
		}
		else
		{
			updatedPhotoIds = newPhotoIds;
		}
		var updatedConfiguration = existingConfiguration with { PhotoIds = updatedPhotoIds };
		var albumResult = await UpdateAlbum(existingAlbum, updatedConfiguration, isDryRun);
		_consoleWriter.ProgressFinish(progressName);
		return albumResult;
	}

	public async Task<AlbumResult> NewDateRangeAlbum(string name, AlbumDateRange dateRangeSession, List<PhotoEntity> photos, bool isDryRun = false)
	{
		const string progressName = "Saving new date range album";
		_consoleWriter.ProgressStart(progressName);
		var photoIdsDontHaveTakenDate = photos.Where(w => w.DateTaken == null).Select(s => s.Id).ToList();
		var existingAlbum = await GetAlbumByName(name);
		if (existingAlbum != null)
			return AlbumResult.AlbumExists;
		AlbumConfiguration configuration;
		if (photoIdsDontHaveTakenDate.Any())
			configuration = new AlbumConfiguration(photoIdsDontHaveTakenDate, dateRangeSession);
		else
			configuration = new AlbumConfiguration(DateRange: dateRangeSession);
		var albumResult = await SaveAlbum(name, configuration, isDryRun);
		_consoleWriter.ProgressFinish(progressName);
		return albumResult;
	}

	public async Task<AlbumResult> UpdateDateRangeAlbum(int albumId, AlbumDateRange? dateRangeSession, List<PhotoEntity> photos, bool isDryRun = false)
	{
		const string progressName = "Saving new date range album";
		_consoleWriter.ProgressStart(progressName);
		var photoIdsDontHaveTakenDate = photos.Where(w => w.DateTaken == null).Select(s => s.Id).ToList();
		var existingAlbum = await GetAlbumById(albumId);
		if (existingAlbum == null)
			return AlbumResult.AlbumNotFound;

		if (!DeserializeConfiguration<AlbumConfiguration>(existingAlbum.Configuration, out var existingConfiguration))
			return AlbumResult.ExistingConfigurationNotInCorrectFormat;

		AlbumDateRange? updatedDateRange;
		if (existingConfiguration.DateRange != null && dateRangeSession != null)
		{
			var olderStart = DateTimeHelper.GetEarliestDateTime(dateRangeSession.Start, existingConfiguration.DateRange.Start);
			var newerEnd = DateTimeHelper.GetLatestDateTime(dateRangeSession.End, existingConfiguration.DateRange.End);
			updatedDateRange = new AlbumDateRange(olderStart, newerEnd);
		}
		else
		{
			updatedDateRange = dateRangeSession;
		}

		List<long>? updatedPhotoIds;
		if (existingConfiguration.PhotoIds != null)
		{
			updatedPhotoIds = new List<long>(existingConfiguration.PhotoIds);
			updatedPhotoIds.AddRange(photoIdsDontHaveTakenDate);
		}
		else if (photoIdsDontHaveTakenDate.Count != 0)
		{
			updatedPhotoIds = photoIdsDontHaveTakenDate;
		}
		else
		{
			updatedPhotoIds = null;
		}

		var updatedConfiguration = new AlbumConfiguration(updatedPhotoIds, updatedDateRange);
		var albumResult = await UpdateAlbum(existingAlbum, updatedConfiguration);
		_consoleWriter.ProgressFinish(progressName);
		return albumResult;
	}

	public async Task<AlbumResult> SaveReverseGeocodeAlbums(List<PhotoEntity> photos, bool isDryRun = false)
	{
		const string progressName = "Saving reverse geocode albums";
		_consoleWriter.ProgressStart(progressName);
		var albumResult = await ProcessAddress(photos, nameof(AlbumReverseGeocode.ReverseGeocodeFormatted));
		if (albumResult != AlbumResult.Successful)
			return albumResult;

		albumResult = await ProcessAddress(photos, nameof(AlbumReverseGeocode.Address1), isDryRun);
		if (albumResult != AlbumResult.Successful)
			return albumResult;

		albumResult = await ProcessAddress(photos, nameof(AlbumReverseGeocode.Address2), isDryRun);
		if (albumResult != AlbumResult.Successful)
			return albumResult;

		albumResult = await ProcessAddress(photos, nameof(AlbumReverseGeocode.Address3), isDryRun);
		if (albumResult != AlbumResult.Successful)
			return albumResult;

		albumResult = await ProcessAddress(photos, nameof(AlbumReverseGeocode.Address4), isDryRun);
		if (albumResult != AlbumResult.Successful)
			return albumResult;

		albumResult = await ProcessAddress(photos, nameof(AlbumReverseGeocode.Address5), isDryRun);
		if (albumResult != AlbumResult.Successful)
			return albumResult;

		albumResult = await ProcessAddress(photos, nameof(AlbumReverseGeocode.Address6), isDryRun);
		if (albumResult != AlbumResult.Successful)
			return albumResult;

		albumResult = await ProcessAddress(photos, nameof(AlbumReverseGeocode.Address7), isDryRun);
		if (albumResult != AlbumResult.Successful)
			return albumResult;

		albumResult = await ProcessAddress(photos, nameof(AlbumReverseGeocode.Address8), isDryRun);
		_consoleWriter.ProgressFinish(progressName);
		return albumResult;
	}

	private async Task<AlbumResult> ProcessAddress(List<PhotoEntity> photos, string propertyName, bool isDryRun = false)
	{
		foreach (var addressIndexValue in GetGroupedAddresses(photos, propertyName))
		{
			var query = $"SELECT * FROM Albums WHERE JSON_EXTRACT(Configuration, '$.ReverseGeocode.{propertyName}') = {{0}} AND IsDeleted = 0;";
			var existingAddressAlbums = await _archiveDbContext.Albums.FromSqlRaw(query, addressIndexValue).ToListAsync();

			if (existingAddressAlbums.Count > 0)
				continue;

			var albumAddress = new AlbumReverseGeocode();
			SetPropertyStringValue(albumAddress, propertyName, addressIndexValue);
			var albumConfiguration = SerializeConfiguration(new AlbumConfiguration(ReverseGeocode: albumAddress));
			var album = new AlbumEntity(addressIndexValue, AlbumType.ReverseGeocode, DateTime.Now)
			{
				Configuration = albumConfiguration,
			};
			await _archiveDbContext.Albums.AddAsync(album);
			if (isDryRun)
			{
				_logger.LogInformation("On dry run");
				return AlbumResult.Successful;
			}

			var affectedRows = await _archiveDbContext.SaveChangesAsync();
			++_statistics.AutoAddressAlbumCreated;
			if (affectedRows != 1)
				return AlbumResult.DataInconsistency;
		}

		return AlbumResult.Successful;
	}

	public async Task<TResponse?> GetReverseGeocodeCache<TResponse>(ReverseGeocodeRequest request, ReverseGeocodeProvider provider)
	{
		var coordinate = request.Coordinate;

		var geocodeCacheEntity = await _archiveDbContext.ReverseGeocodeCache.FromSqlInterpolated(@$"
			SELECT * FROM ReverseGeocodeCache WHERE
			ROUND(Latitude, {_toolOptions.CoordinatePrecision}) = {coordinate.Latitude} AND
			ROUND(Longitude, {_toolOptions.CoordinatePrecision}) = {coordinate.Longitude} AND
			Provider = {provider} AND Precision = {_toolOptions.CoordinatePrecision} AND
			(
				{request.Language} IS NULL
			    OR
			    Language = {request.Language}
			)
		").SingleOrDefaultAsync();

		return geocodeCacheEntity != null ? JsonSerializer.Deserialize<TResponse>(geocodeCacheEntity.Response) : default;
	}

	public async Task SaveReverseGeocodeCache<TResponse>(ReverseGeocodeRequest request, TResponse response, ReverseGeocodeProvider provider)
	{
		var serializedBytes = JsonSerializer.SerializeToUtf8Bytes(response);
		var reverseGeocodeCacheEntity = new ReverseGeocodeCacheEntity(provider, request.Coordinate.Latitude, request.Coordinate.Longitude, serializedBytes, _toolOptions.CoordinatePrecision, request.Language, DateTime.Now);
		_archiveDbContext.ReverseGeocodeCache.Add(reverseGeocodeCacheEntity);
		await _archiveDbContext.SaveChangesAsync();
	}

	public Task<AlbumEntity?> GetAlbumByName(string name)
	{
		return _archiveDbContext.Albums.SingleOrDefaultAsync(s => s.Name == name);
	}

	public Task<AlbumEntity?> GetAlbumById(int albumId)
	{
		return _archiveDbContext.Albums.SingleOrDefaultAsync(s => s.Id == albumId);
	}

	public Task<List<AlbumEntity>> GetAllAlbums()
	{
		return _archiveDbContext.Albums.Where(w => !w.IsDeleted).ToListAsync(); ;
	}

	public Task<int> TotalAlbumCount()
	{
		return _archiveDbContext.Albums.CountAsync(s => !s.IsDeleted);
	}

	public Task<long> TotalPhotoCount()
	{
		return _archiveDbContext.Photos.LongCountAsync(s => !s.IsDeleted);
	}

	public Task<long> TotalReverseGeocodeCacheCount()
	{
		return _archiveDbContext.ReverseGeocodeCache.LongCountAsync();
	}

	public async Task<AlbumPhotoResult> GetAlbumPhotosById(int albumId)
	{
		var album = await GetAlbumById(albumId);
		if (album == null)
		{
			_logger.LogError("Album with id, {AlbumId} not found", albumId);
			return new AlbumPhotoResult(AlbumPhotoResultStatus.AlbumNotFound, []);
		}

		if (!DeserializeConfiguration<AlbumConfiguration>(album.Configuration, out var configuration))
		{
			_logger.LogCritical("Album with id, {AlbumId} configuration is not a valid json, manually fix from history, configuration raw {Configuration}", albumId, album.Configuration);
			return new AlbumPhotoResult(AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat, []);
		}

		var photos = new List<PhotoEntity>();
		if (configuration.PhotoIds != null)
		{
			var photosById = await _archiveDbContext.Photos.Where(p => configuration.PhotoIds.Contains(p.Id)).ToListAsync();
			photos.AddRange(photosById);
		}

		if (configuration.DateRange != null)
		{
			var photosByDateRange = await _archiveDbContext.Photos.Where(p => p.DateTaken >= configuration.DateRange.Start && p.DateTaken <= configuration.DateRange.End).ToListAsync();
			photos.AddRange(photosByDateRange);
		}

		if (configuration.ReverseGeocode != null)
		{
			var reserveGeocodeValuesByProperty = GetNonNullAlbumReverseGeocodeKeyValues(configuration.ReverseGeocode);
			foreach (var (propertyName, valueToFilter) in reserveGeocodeValuesByProperty)
			{
				var photosByAddress = await GetPhotoByDynamicKeyValue(propertyName, valueToFilter);
				photos.AddRange(photosByAddress);
			}
		}

		return new AlbumPhotoResult(AlbumPhotoResultStatus.Successful, photos);
	}

	public Task<List<PhotoEntity>> GetPhotosByDate(int? year, byte? month, byte? day)
	{
		var query = _archiveDbContext.Photos.Where(p => !p.IsDeleted);

		if (year != null)
			query = query.Where(p => p.Year == year.Value);
		if (month != null)
			query = query.Where(p => p.Month == month.Value);
		if (day != null)
			query = query.Where(p => p.Day == day.Value);

		return query.ToListAsync();
	}

	private async Task<List<PhotoEntity>> GetPhotoByDynamicKeyValue(string propertyName, string value)
	{
		var query = $"SELECT * FROM Photos WHERE {propertyName} = {{0}} AND IsDeleted = 0;";
		var filtered = await _archiveDbContext.Photos.FromSqlRaw(query, value).ToListAsync();
		return filtered;
	}

	private async Task<AlbumResult> SaveAlbum(string name, object configurationObject, bool isDryRun = false)
	{
		if (isDryRun)
		{
			_logger.LogInformation("On dry run, album: {AlbumName} are not saved on DB", name);
			return AlbumResult.Successful;
		}
		var configurationSerialized = SerializeConfiguration(configurationObject);
		var album = new AlbumEntity(name, AlbumType.UserDefined, DateTime.Now)
		{
			Configuration = configurationSerialized,
			History = new List<AlbumHistoryEntity>
			{
				new(configurationSerialized, DateTime.Now)
			},
		};
		await _archiveDbContext.Albums.AddAsync(album);
		var affectedRows = await _archiveDbContext.SaveChangesAsync();
		++_statistics.UserDefinedAlbumCreated;
		if (affectedRows != 2)
		{
			_logger.LogCritical("Doesn't able to persist album on database with {AffectedRows} records", affectedRows);
			return AlbumResult.DataInconsistency;
		}
		_logger.LogInformation("Saved album on database with {AffectedRows} records", affectedRows);
		return AlbumResult.Successful;
	}

	private async Task<AlbumResult> UpdateAlbum(AlbumEntity existingAlbum, object newConfiguration, bool isDryRun = false)
	{
		var updatedConfigurationSerialized = SerializeConfiguration(newConfiguration);
		existingAlbum.Configuration = updatedConfigurationSerialized;
		existingAlbum.ModifiedAt = DateTime.Now;
		existingAlbum.History = new List<AlbumHistoryEntity>
		{
			new(updatedConfigurationSerialized, DateTime.Now),
		};
		_archiveDbContext.Albums.Update(existingAlbum);

		if (isDryRun)
		{
			_logger.LogInformation("On dry run, album: {AlbumName} are not updated on DB", existingAlbum.Name);
			return AlbumResult.Successful;
		}

		var affectedRows = await _archiveDbContext.SaveChangesAsync();
		++_statistics.UserDefinedAlbumUpdated;
		if (affectedRows != 2)
		{
			_logger.LogCritical("Doesn't able to persist album on database with {AffectedRows} records", affectedRows);
			return AlbumResult.DataInconsistency;
		}
		_logger.LogInformation("Saved album on database with {AffectedRows} records", affectedRows);
		return AlbumResult.Successful;
	}

	private bool DeserializeConfiguration<T>(string serialized, [NotNullWhen(true)] out T? value)
	{
		try
		{
			var deserialize = JsonSerializer.Deserialize<T>(serialized);
			if (deserialize == null)
			{
				value = default!;
				return false;
			}
			value = deserialize;
			return true;
		}
		catch (JsonException jsonException)
		{
			_logger.LogCritical(jsonException, "Can't deserialize configuration with value {Configuration}", serialized);
			value = default;
			return false;
		}
	}

	private static string SerializeConfiguration(object configuration)
	{
		return JsonSerializer.Serialize(configuration, StaticOptions.AlbumConfigurationJsonSerializerOptions);
	}

	private static List<string> GetGroupedAddresses(List<PhotoEntity> photos, string propertyName)
	{
		return photos
			.GroupBy(photo => GetPropertyStringValueOrDefault(photo, propertyName))
			.Select(s => s.Key)
			.Where(w => w.IsPresent())
			.ToList()!;
	}

	private static string? GetPropertyStringValueOrDefault(PhotoEntity photo, string propertyName)
	{
		var property = typeof(PhotoEntity).GetProperty(propertyName);
		if (property == null)
			throw new Exception($"property {propertyName} not able to get");
		return property.GetValue(photo)?.ToString();
	}

	private static void SetPropertyStringValue(AlbumReverseGeocode albumReverseGeocode, string propertyName, string propertyValue)
	{
		var property = typeof(AlbumReverseGeocode).GetProperty(propertyName);
		if (property == null)
			throw new Exception($"property {propertyName} not able to set with value {propertyValue}");
		property.SetValue(albumReverseGeocode, propertyValue);
	}

	private static Dictionary<string, string> GetNonNullAlbumReverseGeocodeKeyValues(AlbumReverseGeocode albumReverseGeocode)
	{
		var nonNullProperties = new Dictionary<string, string>();
		var properties = typeof(AlbumReverseGeocode).GetProperties();
		foreach (var property in properties)
		{
			if (property.GetValue(albumReverseGeocode) is string value)
				nonNullProperties[property.Name] = value;
		}
		return nonNullProperties;
	}
}
