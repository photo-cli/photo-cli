using System.IO.Abstractions;

namespace PhotoCli.Runners;

public class InfoRunner : BaseRunner, IConsoleRunner
{
	private readonly ICsvService _csvService;
	private readonly IExifDataAppenderService _exifDataAppenderService;
	private readonly IFileSystem _fileSystem;
	private readonly ILogger<InfoRunner> _logger;
	private readonly InfoOptions _options;
	private readonly IPhotoCollectorService _photoCollectorService;
	private readonly IReverseGeocodeFetcherService _reverseGeocodeFetcherService;

	public InfoRunner(ILogger<InfoRunner> logger, InfoOptions options, IPhotoCollectorService photoCollectorService, IExifDataAppenderService exifDataAppenderService, IFileSystem fileSystem,
		IReverseGeocodeFetcherService reverseGeocodeFetcherService, ICsvService csvService, Statistics statistics, IConsoleWriter consoleWriter) : base(logger, fileSystem,
		statistics, consoleWriter)
	{
		_logger = logger;
		_options = options;
		_photoCollectorService = photoCollectorService;
		_exifDataAppenderService = exifDataAppenderService;
		_fileSystem = fileSystem;
		_reverseGeocodeFetcherService = reverseGeocodeFetcherService;
		_csvService = csvService;
	}

	public async Task<ExitCode> Execute()
	{
		var sourceFolderPath = _options.InputPath ?? Environment.CurrentDirectory;

		if (!CheckInputFolderExists(sourceFolderPath, out var exitCodeInputFolder))
			return exitCodeInputFolder;

		if (!CheckOutputPath(out var exitCodeOutputPath))
			return exitCodeOutputPath;

		var photoPaths = _photoCollectorService.Collect(sourceFolderPath, _options.AllFolders);

		if (!ValidatePhotoPaths(out var exitCodePhotoPaths, photoPaths, sourceFolderPath))
			return exitCodePhotoPaths;

		var isPreventProcessOptionSelectedNoPhotoTakenDate = _options.NoPhotoTakenDateAction == InfoNoPhotoTakenDateAction.PreventProcess;
		var isPreventProcessOptionSelectedNoCoordinate = _options.NoCoordinateAction == InfoNoCoordinateAction.PreventProcess;
		var photoExifDataByPath = _exifDataAppenderService.ExifDataByPath(photoPaths, out var allPhotosHasPhotoTaken, out var allPhotosHasCoordinate);
		if (!NoExifDataPreventActions(out var exitCodeNoExif, allPhotosHasPhotoTaken, allPhotosHasCoordinate, isPreventProcessOptionSelectedNoPhotoTakenDate,
			    isPreventProcessOptionSelectedNoCoordinate, photoExifDataByPath))
		{
			return exitCodeNoExif;
		}

		if (_options.ReverseGeocodeProvider != ReverseGeocodeProvider.Disabled)
			photoExifDataByPath = await _reverseGeocodeFetcherService.Fetch(photoExifDataByPath);

		await _csvService.WriteExifDataToCsvOutput(photoExifDataByPath, _options.OutputPath);

		WriteStatistics();

		return ExitCode.Success;
	}

	private bool CheckOutputPath(out ExitCode exitCode)
	{
		var outputFile = _fileSystem.FileInfo.FromFileName(_options.OutputPath);
		if (outputFile.Exists)
		{
			_logger.LogCritical("Output file: {Path} is exists", _options.OutputPath);
			exitCode = ExitCode.OutputPathIsExists;
			return false;
		}

		if (!outputFile.Directory.Exists && !HasCreatedDirectory(outputFile.Directory))
		{
			exitCode = ExitCode.OutputPathDontHaveCreateDirectoryPermission;
			return false;
		}

		if (!HasPermissionToWriteFile(outputFile))
		{
			exitCode = ExitCode.OutputPathDontHaveWriteFilePermission;
			return false;
		}

		exitCode = ExitCode.Unset;
		return true;
	}
}
