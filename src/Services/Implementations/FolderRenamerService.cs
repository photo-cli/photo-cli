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

	public void RenameByFolderAppendType(IReadOnlyCollection<Photo> orderedPhotoInfos, FolderAppendType folderAppendType, FolderAppendLocationType folderAppendLocationType,
		string targetRelativeDirectoryPath)
	{
		if (_ignoredFolders.Any(targetRelativeDirectoryPath.EndsWith) || targetRelativeDirectoryPath == string.Empty)
		{
			_logger.LogDebug("Ignored folder: {Path} found, skipping folder renaming", targetRelativeDirectoryPath);
			return;
		}


		string? appendValue;
		switch (folderAppendType)
		{
			case FolderAppendType.FirstYearMonthDay:
			{
				if (HasNoPhotoTakenDate(orderedPhotoInfos, targetRelativeDirectoryPath))
					return;
				var firstDateTime = GetFirstPhotoTakenDate(orderedPhotoInfos, targetRelativeDirectoryPath);
				appendValue = firstDateTime.ToString(_options.DateFormatWithDay);
				break;
			}
			case FolderAppendType.FirstYearMonth:
			{
				if (HasNoPhotoTakenDate(orderedPhotoInfos, targetRelativeDirectoryPath))
					return;
				var firstDateTime = GetFirstPhotoTakenDate(orderedPhotoInfos, targetRelativeDirectoryPath);
				appendValue = firstDateTime.ToString(_options.DateFormatWithMonth);
				break;
			}
			case FolderAppendType.FirstYear:
			{
				if (HasNoPhotoTakenDate(orderedPhotoInfos, targetRelativeDirectoryPath))
					return;
				var firstDateTime = GetFirstPhotoTakenDate(orderedPhotoInfos, targetRelativeDirectoryPath);
				appendValue = firstDateTime.ToString(_options.YearFormat);
				break;
			}
			case FolderAppendType.DayRange:
			{
				if (HasNoPhotoTakenDate(orderedPhotoInfos, targetRelativeDirectoryPath))
					return;
				var (firstDateTime, lastDateTime) = GetFirstAndLastPhotoTakenDate(orderedPhotoInfos, targetRelativeDirectoryPath);
				var firstDayFormat = firstDateTime.ToString(_options.DateFormatWithDay);
				var lastDayFormat = lastDateTime.ToString(_options.DateFormatWithDay);
				appendValue = $"{firstDayFormat}{_options.DayRangeSeparator}{lastDayFormat}";
				break;
			}
			case FolderAppendType.MatchingMinimumAddress:
			{
				var photosHasReverseGeocode = orderedPhotoInfos.Where(w => w.HasReverseGeocode).ToList();
				if (!photosHasReverseGeocode.Any())
					return;
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
					return;
				var allMatchingReverseGeocodes = photosHasReverseGeocode.First().ReverseGeocodes!.GetRange(0, allSameIndexStartsWith.Value + 1);
				appendValue = string.Join(_options.AddressSeparator, allMatchingReverseGeocodes);
				break;
			}
			default:
				throw new PhotoCliException($"Not defined {nameof(FolderAppendType)}: {folderAppendType}");
		}

		var appendedTargetRelativeDirectoryPath = PathHelper.AppendToTheBottomDirectory(folderAppendLocationType, targetRelativeDirectoryPath, appendValue, _options.FolderAppendSeparator);
		foreach (var photoInfo in orderedPhotoInfos)
			photoInfo.TargetRelativeDirectoryPath = appendedTargetRelativeDirectoryPath;
	}

	private bool HasNoPhotoTakenDate(IEnumerable<Photo> orderedPhotoInfos, string targetRelativeDirectoryPath)
	{
		var hasNoPhotoTakenDate = !orderedPhotoInfos.Any(a => a.HasPhotoTakenDateTime);
		if (hasNoPhotoTakenDate)
			_logger.LogDebug("No photo taken date will locate on {TargetRelativePath}, skipping folder renaming", targetRelativeDirectoryPath);
		return hasNoPhotoTakenDate;
	}

	private DateTime GetFirstPhotoTakenDate(IReadOnlyCollection<Photo> orderedPhotoInfos, string targetRelativeDirectoryPath)
	{
		var orderedPhotoInfosThatHavePhotoTakenDate = VerifyAndGetOrderedPhotoList(orderedPhotoInfos);
		var firstDateTime = orderedPhotoInfosThatHavePhotoTakenDate.First(f => f.HasPhotoTakenDateTime).PhotoTakenDateTime!.Value;
		_logger.LogDebug("First photo taken date as {FirstTakenDate} will locate on {TargetRelativePath}", firstDateTime, targetRelativeDirectoryPath);
		return firstDateTime;
	}

	private (DateTime, DateTime) GetFirstAndLastPhotoTakenDate(IReadOnlyCollection<Photo> orderedPhotoInfos, string targetRelativeDirectoryPath)
	{
		var orderedPhotoInfosThatHavePhotoTakenDate = VerifyAndGetOrderedPhotoList(orderedPhotoInfos);
		var firstDateTime = orderedPhotoInfosThatHavePhotoTakenDate.First(f => f.HasPhotoTakenDateTime).PhotoTakenDateTime!.Value;
		var lastDateTime = orderedPhotoInfosThatHavePhotoTakenDate.Last().PhotoTakenDateTime!.Value;
		_logger.LogDebug("First photo taken date as {FirstTakenDate}, and last photo taken as {LastTakenDate} will locate on {TargetRelativePath}", firstDateTime, lastDateTime,
			targetRelativeDirectoryPath);
		return (firstDateTime, lastDateTime);
	}

	private List<Photo> VerifyAndGetOrderedPhotoList(IReadOnlyCollection<Photo> orderedPhotoInfos)
	{
		var orderedPhotoInfosThatHavePhotoTakenDate = orderedPhotoInfos.Where(w => w.HasPhotoTakenDateTime).ToList();
		orderedPhotoInfosThatHavePhotoTakenDate.ThrowIfNotOrderedByPhotoTakenDate();
		return orderedPhotoInfosThatHavePhotoTakenDate;
	}
}
