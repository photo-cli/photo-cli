namespace PhotoCli.Services.Implementations;

public class DuplicatePhotoRemoveService : IDuplicatePhotoRemoveService
{
	private readonly ILogger<DuplicatePhotoRemoveService> _logger;
	private readonly Statistics _statistics;

	public DuplicatePhotoRemoveService(ILogger<DuplicatePhotoRemoveService> logger, Statistics statistics)
	{
		_logger = logger;
		_statistics = statistics;
	}

	public IReadOnlyCollection<Photo> GroupAndFilterByPhotoHash(IReadOnlyCollection<Photo> photos)
	{
		var uniquePhotos = new List<Photo>();
		foreach (var photosGroupedByHash in photos.GroupBy(g => g.Sha1Hash))
		{
			if (photosGroupedByHash.Count() == 1)
			{
				uniquePhotos.Add(photosGroupedByHash.Single());
				continue;
			}

			var firstPhoto = photosGroupedByHash.First();
			uniquePhotos.Add(firstPhoto);
			foreach (var photo in photosGroupedByHash.Skip(1))
			{
				++_statistics.PhotosSame;
				_logger.LogWarning("Photo is skipped due to same photo has already been archived. Same photo paths: {Path1}, {Path2}", firstPhoto.FilePath, photo.FilePath);
			}
		}
		return uniquePhotos;
	}
}
