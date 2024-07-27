using System.IO.Abstractions;

namespace PhotoCli.Services.Implementations;

public class DirectoryGrouperService : IDirectoryGrouperService
{
	private const string ProgressName = "Directory grouping";
	private readonly IFileSystem _fileSystem;
	private readonly ToolOptions _options;
	private readonly ILogger<DirectoryGrouperService> _logger;
	private readonly IConsoleWriter _consoleWriter;

	public DirectoryGrouperService(IFileSystem fileSystem, ToolOptions options, ILogger<DirectoryGrouperService> logger, IConsoleWriter consoleWriter)
	{
		_fileSystem = fileSystem;
		_options = options;
		_logger = logger;
		_consoleWriter = consoleWriter;
	}

	public Dictionary<string, IReadOnlyCollection<Photo>> GroupFiles(IReadOnlyCollection<Photo> photos, string sourceRootPath, FolderProcessType folderProcessType,
		GroupByFolderType? groupByFolderType, bool invalidFileFormatGroupedInSubFolder, bool noPhotoDateTimeTakenGroupedInSubFolder, bool noReverseGeocodeGroupedInSubFolder)
	{
		_consoleWriter.ProgressStart(ProgressName);
		var groupedPhotosByRelativeDirectoryInternal = new Dictionary<string, List<Photo>>();
		var sourceRootDirectoryPath = _fileSystem.DirectoryInfo.New(sourceRootPath).FullName;
		var sourcePathTrimmed = PathHelper.TrimFolderSeparators(sourceRootDirectoryPath);
		foreach (var photo in photos)
		{
			var fileInfo = _fileSystem.FileInfo.New(photo.PhotoFile.SourcePath);
			var directory = fileInfo.Directory!.FullName.Trim('/');
			string targetRelativeDirectoryPath;

			var exifData = photo.ExifData;
			if (exifData != null && groupByFolderType is GroupByFolderType.AddressFlat)
			{
				targetRelativeDirectoryPath = exifData.ReverseGeocodeFormatted ?? string.Empty;
			}
			else if (exifData?.ReverseGeocodes != null && groupByFolderType is GroupByFolderType.AddressHierarchy)
			{
				targetRelativeDirectoryPath = string.Join(Path.DirectorySeparatorChar, exifData.ReverseGeocodes);
			}
			else if (exifData?.TakenDate != null && groupByFolderType is GroupByFolderType.Year)
			{
				targetRelativeDirectoryPath = exifData.TakenDate.Value.ToString(_options.YearFormat);
			}
			else if (exifData?.TakenDate != null && groupByFolderType is GroupByFolderType.YearMonth)
			{
				targetRelativeDirectoryPath = $"{exifData.TakenDate.Value.ToString(_options.YearFormat)}{Path.DirectorySeparatorChar}{exifData.TakenDate.Value.ToString(_options.MonthFormat)}";
			}
			else if (exifData?.TakenDate != null && groupByFolderType is GroupByFolderType.YearMonthDay)
			{
				targetRelativeDirectoryPath = $"{exifData.TakenDate.Value.ToString(_options.YearFormat)}{Path.DirectorySeparatorChar}{exifData.TakenDate.Value.ToString(_options.MonthFormat)}{Path.DirectorySeparatorChar}{exifData.TakenDate.Value.ToString(_options.DayFormat)}";
			}
			else if (folderProcessType is FolderProcessType.Single)
			{
				if (sourcePathTrimmed != directory)
					throw new PhotoCliException($"All files should be located in source path in {nameof(FolderProcessType.Single)}");
				targetRelativeDirectoryPath = string.Empty;
			}
			else if (folderProcessType is FolderProcessType.FlattenAllSubFolders)
			{
				targetRelativeDirectoryPath = string.Empty;
			}
			else if (sourcePathTrimmed == directory)
			{
				targetRelativeDirectoryPath = string.Empty;
			}
			else
			{
				var relativeDirectoryPath = PathHelper.TrimFolderSeparators(directory.RemoveFirst(sourcePathTrimmed));
				targetRelativeDirectoryPath = relativeDirectoryPath;
			}

			if (exifData == null && invalidFileFormatGroupedInSubFolder)
			{
				targetRelativeDirectoryPath = Path.Combine(targetRelativeDirectoryPath, _options.PhotoFormatInvalidFolderName);
			}
			else
			{
				var noPhotoTakenShouldBeInSubFolder = exifData?.TakenDate == null && noPhotoDateTimeTakenGroupedInSubFolder;
				var noReverseGeocodeShouldBeInSubFolder = exifData?.ReverseGeocodes == null && noReverseGeocodeGroupedInSubFolder;
				if (noPhotoTakenShouldBeInSubFolder && noReverseGeocodeShouldBeInSubFolder)
					targetRelativeDirectoryPath = Path.Combine(targetRelativeDirectoryPath, _options.NoAddressAndPhotoTakenDateFolderName);
				else if (noPhotoTakenShouldBeInSubFolder)
					targetRelativeDirectoryPath = Path.Combine(targetRelativeDirectoryPath, _options.NoPhotoTakenDateFolderName);
				else if (noReverseGeocodeShouldBeInSubFolder)
					targetRelativeDirectoryPath = Path.Combine(targetRelativeDirectoryPath, _options.NoAddressFolderName);
			}

			_logger.LogTrace("File ({FilePath}) target directory: {TargetRelativeDirectoryPath} ", photo.PhotoFile.SourcePath, targetRelativeDirectoryPath);
			photo.SetTargetRelativePath(targetRelativeDirectoryPath);
			if (groupedPhotosByRelativeDirectoryInternal.TryGetValue(targetRelativeDirectoryPath, out var groupPhotos))
				groupPhotos.Add(photo);
			else
				groupedPhotosByRelativeDirectoryInternal.Add(targetRelativeDirectoryPath, [photo]);
		}

		var groupedPhotosByRelativeDirectoryReadOnly = groupedPhotosByRelativeDirectoryInternal.ToDictionary(k => k.Key, e => (IReadOnlyCollection<Photo>)e.Value.AsReadOnly());
		_consoleWriter.ProgressFinish(ProgressName);
		return groupedPhotosByRelativeDirectoryReadOnly;
	}
}
