using System.IO.Abstractions;

namespace PhotoCli.Runners;

public class ArchiveRunner : BaseRunner, IConsoleRunner
{
	private const string TargetRelativeFolderProgressName = "Processing target folder";

	private readonly IDirectoryGrouperService _directoryGrouperService;
	private readonly IExifDataAppenderService _exifDataAppenderService;
	private readonly IFileNamerService _fileNamerService;
	private readonly IFileService _fileService;
	private readonly ILogger<ArchiveRunner> _logger;
	private readonly ArchiveOptions _options;
	private readonly IPhotoCollectorService _photoCollectorService;
	private readonly IReverseGeocodeFetcherService _reverseGeocodeFetcherService;
	private readonly IConsoleWriter _consoleWriter;
	private readonly IDuplicatePhotoRemoveService _duplicatePhotoRemoveService;
	private readonly IDbService _dbService;
	private readonly ArchiveDatabaseOptions _archiveDatabaseOptions;

	public ArchiveRunner(ILogger<ArchiveRunner> logger, ArchiveOptions options, IPhotoCollectorService photoCollectorService, IExifDataAppenderService exifDataAppenderService,
		IDirectoryGrouperService directoryGrouperService, IFileNamerService fileNamerService, IFileService fileService, IFileSystem fileSystem, Statistics statistics,
		IReverseGeocodeFetcherService reverseGeocodeFetcherService, IConsoleWriter consoleWriter,
		IDuplicatePhotoRemoveService duplicatePhotoRemoveService, ArchiveDatabaseOptions archiveDatabaseOptions, IDbService dbService) : base(logger, fileSystem, statistics, consoleWriter)
	{
		_options = options;
		_logger = logger;
		_photoCollectorService = photoCollectorService;
		_exifDataAppenderService = exifDataAppenderService;
		_directoryGrouperService = directoryGrouperService;
		_fileNamerService = fileNamerService;
		_fileService = fileService;
		_reverseGeocodeFetcherService = reverseGeocodeFetcherService;
		_consoleWriter = consoleWriter;
		_duplicatePhotoRemoveService = duplicatePhotoRemoveService;
		_archiveDatabaseOptions = archiveDatabaseOptions;
		_dbService = dbService;
	}

	public async Task<ExitCode> Execute()
	{
		var sourceFolderPath = _options.InputPath ?? Environment.CurrentDirectory;

		if (!CheckInputFolderExists(sourceFolderPath, out var exitCodeInputFolder))
			return exitCodeInputFolder;

		var photosFound = _photoCollectorService.Collect(sourceFolderPath, true, true);
		if (photosFound.Count == 0)
		{
			Console.WriteLine($"No photo found on folder: {sourceFolderPath}");
			return ExitCode.NoPhotoFoundOnDirectory;
		}

		var isInvalidFileFormatPreventProcessOptionSelected = _options.InvalidFileFormatAction == ArchiveInvalidFormatAction.PreventProcess;
		var isNoPhotoTakenDatePreventProcessOptionSelected = _options.NoPhotoTakenDateAction == ArchiveNoPhotoTakenDateAction.PreventProcess;
		var isNoCoordinatePreventProcessOptionSelected = _options.NoCoordinateAction == ArchiveNoCoordinateAction.PreventProcess;

		var exifDataResult = _exifDataAppenderService.ExtractExifData(photosFound);

		var photosWithExif = exifDataResult.Photos;

		if (!ExifDataPreventActions(out var exitCodeNoExif, exifDataResult, isInvalidFileFormatPreventProcessOptionSelected,
				isNoPhotoTakenDatePreventProcessOptionSelected, isNoCoordinatePreventProcessOptionSelected, _options.ExpectedDayRange))
		{
			return exitCodeNoExif;
		}

		var preventAlbumExitCode = await PreventProcessOnAlbumConfiguration(exifDataResult);
		if (preventAlbumExitCode != null)
			return preventAlbumExitCode.Value;

		var photosHashed = await _fileService.CalculateFileHash(photosWithExif);

		if (_options.ReverseGeocodeProvider != ReverseGeocodeProvider.Disabled)
		{
			_reverseGeocodeFetcherService.RateLimitWarning();
			(photosHashed, var allPhotosHasReverseGeocodedAsRequested) = await _reverseGeocodeFetcherService.Fetch(photosHashed, false);
			if (_options.MissingReverseGeocodeAction == MissingReverseGeocodeAction.PreventProcess && !allPhotosHasReverseGeocodedAsRequested)
				return ExitCode.PhotosWithMissingReverseGeocodeInfoAsRequested;
		}

		var groupedPhotosByRelativeDirectory = _directoryGrouperService.GroupFiles(photosHashed, sourceFolderPath, FolderProcessType.FlattenAllSubFolders,
			GroupByFolderType.YearMonthDay, true, true, false);

		_consoleWriter.ProgressStart(TargetRelativeFolderProgressName, groupedPhotosByRelativeDirectory.Count);
		var isNotDryRun = !_options.IsDryRun;

		var newCopiedPhotosByRelativeDirectory = new Dictionary<string, IReadOnlyCollection<Photo>>();
		foreach (var (targetRelativeDirectoryPath, photosInRelativeDirectory) in groupedPhotosByRelativeDirectory)
		{
			_logger.LogTrace("Processing {TargetRelativeDirectory}", targetRelativeDirectoryPath);

			var uniquePhotos = _duplicatePhotoRemoveService.GroupAndFilterByPhotoHash(photosInRelativeDirectory);
			var renamedPhotos = _fileNamerService.SetArchiveFileName(uniquePhotos);
			var newCopiedFiles = _fileService.CopyIfNotExists(renamedPhotos, _archiveDatabaseOptions.Path, _options.IsDryRun);
			newCopiedPhotosByRelativeDirectory.Add(targetRelativeDirectoryPath, newCopiedFiles);
			if (isNotDryRun)
			{
				var allFilesVerified = await _fileService.VerifyFileIntegrity(uniquePhotos);
				if (!allFilesVerified)
					return ExitCode.FileVerifyErrors;
			}
			_consoleWriter.InProgressItemComplete(TargetRelativeFolderProgressName);
			_logger.LogTrace("Processed {TargetRelativeDirectory}", targetRelativeDirectoryPath);
		}
		_consoleWriter.ProgressFinish(TargetRelativeFolderProgressName);

		if (isNotDryRun)
			_consoleWriter.Write("Verified all photo files copied successfully by comparing file hashes from original photo files.");

		var newCopiedAllPhotos = newCopiedPhotosByRelativeDirectory.SelectMany(s => s.Value).ToList();

		var (archived, photoDb) = await _dbService.Archive(newCopiedAllPhotos, _options.IsDryRun);
		if (!archived)
			return ExitCode.InconsistencyOnSavingPhotosToDatabase;

		var exitCode = await SaveAlbums(photoDb, exifDataResult.DateRange);
		if (exitCode != ExitCode.Unset)
			return exitCode;

		if (_options.DeleteSource)
			_fileService.DeletePhotoSources(newCopiedAllPhotos, _options.IsDryRun);

		WriteStatistics();
		_consoleWriter.WriteSuccess("Archive process completed successfully");
		return ExitCode.Success;
	}

	private async Task<ExitCode?> PreventProcessOnAlbumConfiguration(ExifDataResult exifDataResult)
	{
		if (_options.AlbumType == null)
			return null;

		if (_options.AlbumType == ArchiveAlbumType.DateRange && exifDataResult.DateRange == null)
			return ExitCode.NoDataRangeFoundOnPhotos;

		if (_options.AlbumNameNew.IsPresent())
		{
			var existingAlbum = await _dbService.GetAlbumByName(_options.AlbumNameNew);
			if (existingAlbum != null)
				return ExitCode.AlbumNameMustBeUniqueWhileAddingOrUseUpdate;
		}
		else if (_options.AlbumIdUpdate != null)
		{
			var existingAlbum = await _dbService.GetAlbumById(_options.AlbumIdUpdate.Value);
			if (existingAlbum == null)
				return ExitCode.AlbumNotFoundById;
		}
		else
		{
			throw new PhotoCliException($"Not defined album flow on {nameof(PreventProcessOnAlbumConfiguration)} for album type: ${_options.AlbumType}");
		}
		return null;
	}

	private async Task<ExitCode> SaveAlbums(List<PhotoEntity> photos, AlbumDateRange? albumDateRange)
	{
		if (_options.AlbumType != null)
		{
			var albumResult = await SaveUserDefinedAlbums(photos, albumDateRange);
			if (!IsAlbumSuccessfullySaved(albumResult, out var exitCode))
				return exitCode;
		}

		if (_options.AutoReverseGeocodeAlbum)
		{
			var autoAddressAlbumResult = await _dbService.SaveReverseGeocodeAlbums(photos, _options.IsDryRun);
			if (!IsAlbumSuccessfullySaved(autoAddressAlbumResult, out var exitCode))
				return exitCode;
		}

		return ExitCode.Unset;
	}

	private async Task<AlbumResult> SaveUserDefinedAlbums(List<PhotoEntity> photos, AlbumDateRange? albumDateRange)
	{
		AlbumResult albumResult;

		if (_options.AlbumNameNew.IsPresent())
		{
			switch (_options.AlbumType)
			{
				case ArchiveAlbumType.Individual:
					albumResult = await _dbService.NewIndividualAlbum(_options.AlbumNameNew, photos, _options.IsDryRun);
					break;
				case ArchiveAlbumType.DateRange:
					if (albumDateRange == null)
						throw new PhotoCliException($"Creating DateRange album without date range, must be prevented early in the process validated on ${nameof(PreventProcessOnAlbumConfiguration)}");
					albumResult = await _dbService.NewDateRangeAlbum(_options.AlbumNameNew, albumDateRange, photos, _options.IsDryRun);
					break;
				default:
					throw new PhotoCliException($"Not defined album type: {_options.AlbumType} for new album flow on ${nameof(SaveUserDefinedAlbums)}");
			}
		}
		else if (_options.AlbumIdUpdate != null)
		{
			var albumId = _options.AlbumIdUpdate.Value;
			switch (_options.AlbumType)
			{
				case ArchiveAlbumType.Individual:
					albumResult = await _dbService.UpdateIndividualAlbum(albumId, photos, _options.IsDryRun);
					break;
				case ArchiveAlbumType.DateRange:
					if (albumDateRange == null)
						throw new PhotoCliException($"Updating DateRange album without date range, must be prevented early in the process validated on ${nameof(PreventProcessOnAlbumConfiguration)}");
					albumResult = await _dbService.UpdateDateRangeAlbum(albumId, albumDateRange, photos, _options.IsDryRun);
					break;
				default:
					throw new PhotoCliException($"Not defined album type: {_options.AlbumType} for album update flow on ${nameof(SaveUserDefinedAlbums)}");
			}
		}
		else
		{
			throw new PhotoCliException($"Not defined album flow on {nameof(SaveUserDefinedAlbums)}");
		}

		return albumResult;
	}

	private static bool IsAlbumSuccessfullySaved(AlbumResult albumResult, out ExitCode exitCode)
	{
		if (albumResult == AlbumResult.Successful)
		{
			exitCode = ExitCode.Unset;
			return true;
		}
		exitCode = albumResult switch
		{
			AlbumResult.AlbumExists => ExitCode.AlbumExist,
			AlbumResult.NoPhotosToAddInAlbum => ExitCode.NoPhotosToAddInAlbum,
			AlbumResult.DataInconsistency => ExitCode.InconsistencyOnSavingUserDefinedAlbumToDatabase,
			AlbumResult.AlbumNotFound => ExitCode.AlbumNotFoundById,
			AlbumResult.ExistingConfigurationNotInCorrectFormat => ExitCode.ExistingAlbumConfigurationNotValid,
			_ => throw new PhotoCliException($"Not defined album result on {nameof(IsAlbumSuccessfullySaved)} for {nameof(AlbumResult)}: {albumResult}")
		};
		return false;
	}
}
