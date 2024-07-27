namespace PhotoCli.Services.Implementations;

public class DbService : IDbService
{
	private readonly IArchiveDbContextProvider _archiveDbContextProvider;
	private readonly ILogger<DbService> _logger;

	public DbService(IArchiveDbContextProvider archiveDbContextProviderProvider, ILogger<DbService> logger)
	{
		_archiveDbContextProvider = archiveDbContextProviderProvider;
		_logger = logger;
	}

	public async Task<int> Archive(IEnumerable<Photo> photos, bool isDryRun = false)
	{
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

		int affectedRows;
		if (isDryRun)
		{
			affectedRows = 0;
			_logger.LogInformation("On dry run, {PhotoCount} photos, changes are not persist on DB", photoEntities.Count());
		}
		else
		{
			var archiveDbContext = _archiveDbContextProvider.CreateOrGetInstance();
			_logger.LogInformation("Sending {PhotoCount} photos to DB", photoEntities.Count);
			await archiveDbContext.Photos.AddRangeAsync(photoEntities);
			affectedRows = await archiveDbContext.SaveChangesAsync();
			_logger.LogInformation("DB saved {AffectedRows} records", affectedRows);
		}
		return affectedRows;
	}
}
