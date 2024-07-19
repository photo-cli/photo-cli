namespace PhotoCli.Services.Implementations;

public class FolderRenamerService : IFolderRenamerService
{
	private readonly List<string> _ignoredFolders;
	private readonly ToolOptions _options;
	private readonly ILogger<FolderRenamerService> _logger;

	public FolderRenamerService(ToolOptions options, ILogger<FolderRenamerService> logger)
	{
		_options = options;
		_logger = logger;
		_ignoredFolders = new List<string> { _options.NoAddressFolderName, _options.NoPhotoTakenDateFolderName, _options.NoAddressAndPhotoTakenDateFolderName };
	}

	public IReadOnlyCollection<Photo> RenameByFolderAppendType(IReadOnlyCollection<Photo> orderedPhotos, FolderAppendType folderAppendType, FolderAppendLocationType folderAppendLocationType,
		string targetRelativeDirectoryPath)
	{
		if (_ignoredFolders.Any(targetRelativeDirectoryPath.EndsWith) || targetRelativeDirectoryPath == string.Empty)
		{
			_logger.LogDebug("Ignored folder: {Path} found, skipping folder renaming", targetRelativeDirectoryPath);
			return orderedPhotos;
		}

		string? appendValue;
		switch (folderAppendType)
		{
			case FolderAppendType.FirstYearMonthDay:
			{
				if (HasNoPhotoTakenDate(orderedPhotos, targetRelativeDirectoryPath))
					return orderedPhotos;
				var firstDateTime = GetFirstPhotoTakenDate(orderedPhotos, targetRelativeDirectoryPath);
				appendValue = firstDateTime.ToString(_options.DateFormatWithDay);
				break;
			}
			case FolderAppendType.FirstYearMonth:
			{
				if (HasNoPhotoTakenDate(orderedPhotos, targetRelativeDirectoryPath))
					return orderedPhotos;
				var firstDateTime = GetFirstPhotoTakenDate(orderedPhotos, targetRelativeDirectoryPath);
				appendValue = firstDateTime.ToString(_options.DateFormatWithMonth);
				break;
			}
			case FolderAppendType.FirstYear:
			{
				if (HasNoPhotoTakenDate(orderedPhotos, targetRelativeDirectoryPath))
					return orderedPhotos;
				var firstDateTime = GetFirstPhotoTakenDate(orderedPhotos, targetRelativeDirectoryPath);
				appendValue = firstDateTime.ToString(_options.YearFormat);
				break;
			}
			case FolderAppendType.DayRange:
			{
				if (HasNoPhotoTakenDate(orderedPhotos, targetRelativeDirectoryPath))
					return orderedPhotos;
				var (firstDateTime, lastDateTime) = GetFirstAndLastPhotoTakenDate(orderedPhotos, targetRelativeDirectoryPath);
				var firstDayFormat = firstDateTime.ToString(_options.DateFormatWithDay);
				var lastDayFormat = lastDateTime.ToString(_options.DateFormatWithDay);
				appendValue = $"{firstDayFormat}{_options.DayRangeSeparator}{lastDayFormat}";
				break;
			}
			case FolderAppendType.MatchingMinimumAddress:
			{
				var photosHasReverseGeocode = orderedPhotos.Where(w => w.HasReverseGeocode).ToList();
				if (!photosHasReverseGeocode.Any())
					return orderedPhotos;
				var minReverseGeocodeItemCount = photosHasReverseGeocode.Min(m => m.ReverseGeocodeCount);

				int? allSameIndexStartsWith = null;
				for (var reverseGeocodeIndex = minReverseGeocodeItemCount - 1; reverseGeocodeIndex >= 0; reverseGeocodeIndex--)
				{
					var index = reverseGeocodeIndex;
					var groupedBySpecificReverseGeocodeItemOnIndex = photosHasReverseGeocode.GroupBy(g => g.ReverseGeocodes?.ElementAtOrDefault(index));

					if (groupedBySpecificReverseGeocodeItemOnIndex.Count() == 1)
						allSameIndexStartsWith ??= reverseGeocodeIndex;
					else
						allSameIndexStartsWith = null;
				}

				if (allSameIndexStartsWith == null)
					return orderedPhotos;
				var allMatchingReverseGeocodes = photosHasReverseGeocode.First().ReverseGeocodes!.GetRange(0, allSameIndexStartsWith.Value + 1);
				appendValue = string.Join(_options.AddressSeparator, allMatchingReverseGeocodes);
				break;
			}
			default:
				throw new PhotoCliException($"Not defined {nameof(FolderAppendType)}: {folderAppendType}");
		}

		var appendedTargetRelativeDirectoryPath = PathHelper.AppendToTheBottomDirectory(folderAppendLocationType, targetRelativeDirectoryPath, appendValue, _options.FolderAppendSeparator);
		foreach (var photoInfo in orderedPhotos)
			photoInfo.SetTargetRelativePath(appendedTargetRelativeDirectoryPath);

		return orderedPhotos;
	}

	private bool HasNoPhotoTakenDate(IEnumerable<Photo> orderedPhotos, string targetRelativeDirectoryPath)
	{
		var hasNoPhotoTakenDate = !orderedPhotos.Any(a => a.HasTakenDateTime);
		if (hasNoPhotoTakenDate)
			_logger.LogDebug("No photo taken date will locate on {TargetRelativePath}, skipping folder renaming", targetRelativeDirectoryPath);
		return hasNoPhotoTakenDate;
	}

	private DateTime GetFirstPhotoTakenDate(IReadOnlyCollection<Photo> orderedPhotos, string targetRelativeDirectoryPath)
	{
		var orderedPhotosThatHavePhotoTakenDate = VerifyAndGetOrderedPhotoList(orderedPhotos);
		var firstDateTime = orderedPhotosThatHavePhotoTakenDate.First().TakenDateTime!.Value;
		_logger.LogDebug("First photo taken date as {FirstTakenDate} will locate on {TargetRelativePath}", firstDateTime, targetRelativeDirectoryPath);
		return firstDateTime;
	}

	private (DateTime, DateTime) GetFirstAndLastPhotoTakenDate(IReadOnlyCollection<Photo> orderedPhotos, string targetRelativeDirectoryPath)
	{
		var orderedPhotosThatHavePhotoTakenDate = VerifyAndGetOrderedPhotoList(orderedPhotos);
		var firstDateTime = orderedPhotosThatHavePhotoTakenDate.First().TakenDateTime!.Value;
		var lastDateTime = orderedPhotosThatHavePhotoTakenDate.Last().TakenDateTime!.Value;
		_logger.LogDebug("First photo taken date as {FirstTakenDate}, and last photo taken as {LastTakenDate} will locate on {TargetRelativePath}", firstDateTime, lastDateTime, targetRelativeDirectoryPath);
		return (firstDateTime, lastDateTime);
	}

	private List<Photo> VerifyAndGetOrderedPhotoList(IReadOnlyCollection<Photo> orderedPhotos)
	{
		var orderedPhotosThatHavePhotoTakenDate = orderedPhotos.Where(w => w.HasTakenDateTime).ToList();
		orderedPhotosThatHavePhotoTakenDate.ThrowIfNotOrderedByPhotoTakenDate();
		return orderedPhotosThatHavePhotoTakenDate;
	}
}
