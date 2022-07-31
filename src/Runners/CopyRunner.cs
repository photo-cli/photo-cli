using System.IO.Abstractions;

namespace PhotoCli.Runners;

public class CopyRunner : BaseRunner, IConsoleRunner
{
	private const string TargetRelativeFolderProgressName = "Processing target folder";

	private readonly ICsvService _csvService;
	private readonly IDirectoryGrouperService _directoryGrouperService;
	private readonly IExifDataAppenderService _exifDataAppenderService;
	private readonly IExifOrganizerService _exifOrganizerService;
	private readonly IFileNamerService _fileNamerService;
	private readonly IFileService _fileService;
	private readonly IFileSystem _fileSystem;
	private readonly IFolderRenamerService _folderRenamer;
	private readonly ILogger<CopyRunner> _logger;
	private readonly CopyOptions _options;
	private readonly IPhotoCollectorService _photoCollectorService;
	private readonly IReverseGeocodeFetcherService _reverseGeocodeFetcherService;
	private readonly ToolOptions _toolOptions;
	private readonly IConsoleWriter _consoleWriter;

	public CopyRunner(ILogger<CopyRunner> logger, CopyOptions options, IPhotoCollectorService photoCollectorService, IExifDataAppenderService exifDataAppenderService,
		IDirectoryGrouperService directoryGrouperService, IFileNamerService fileNamerService, IFileService fileService, IFileSystem fileSystem, IExifOrganizerService exifOrganizerService,
		IFolderRenamerService folderRenamer, IReverseGeocodeFetcherService reverseGeocodeFetcherService, ICsvService csvService, ToolOptions toolOptions, Statistics statistics,
		IConsoleWriter consoleWriter) : base(logger, fileSystem, statistics, consoleWriter)
	{
		_options = options;
		_logger = logger;
		_photoCollectorService = photoCollectorService;
		_exifDataAppenderService = exifDataAppenderService;
		_directoryGrouperService = directoryGrouperService;
		_fileNamerService = fileNamerService;
		_fileService = fileService;
		_fileSystem = fileSystem;
		_exifOrganizerService = exifOrganizerService;
		_folderRenamer = folderRenamer;
		_reverseGeocodeFetcherService = reverseGeocodeFetcherService;
		_csvService = csvService;
		_toolOptions = toolOptions;
		_consoleWriter = consoleWriter;
	}

	public async Task<ExitCode> Execute()
	{
		var sourceFolderPath = _options.InputPath ?? Environment.CurrentDirectory;

		if (!CheckInputFolderExists(sourceFolderPath, out var exitCodeInputFolder))
			return exitCodeInputFolder;

		if (!CheckOutputPathIsUsing(_options.IsDryRun, out var exitCodeOutputPath))
			return exitCodeOutputPath;

		var processAllSubFolders = _options.FolderProcessType != FolderProcessType.Single;
		var photoPaths = _photoCollectorService.Collect(sourceFolderPath, processAllSubFolders);
		if (photoPaths.Length == 0)
		{
			Console.WriteLine($"No photo found on folder: {sourceFolderPath}");
			return ExitCode.NoPhotoFoundOnDirectory;
		}

		var isPreventProcessOptionSelectedNoPhotoTakenDate = _options.NoPhotoTakenDateAction == CopyNoPhotoTakenDateAction.PreventProcess;
		var isPreventProcessOptionSelectedNoCoordinate = _options.NoCoordinateAction == CopyNoCoordinateAction.PreventProcess;
		var photoExifDataByPath = _exifDataAppenderService.ExifDataByPath(photoPaths, out var allPhotosHasPhotoTaken, out var allPhotosHasCoordinate);
		if (!NoExifDataPreventActions(out var exitCodeNoExif, allPhotosHasPhotoTaken, allPhotosHasCoordinate, isPreventProcessOptionSelectedNoPhotoTakenDate,
			    isPreventProcessOptionSelectedNoCoordinate, photoExifDataByPath))
		{
			return exitCodeNoExif;
		}

		if (_options.ReverseGeocodeProvider != ReverseGeocodeProvider.Disabled)
		{
			_reverseGeocodeFetcherService.RateLimitWarning();
			photoExifDataByPath = await _reverseGeocodeFetcherService.Fetch(photoExifDataByPath);
		}

		var groupedPhotosByRelativeDirectory = _directoryGrouperService.GroupFiles(photoExifDataByPath, sourceFolderPath, _options.FolderProcessType,
			_options.GroupByFolderType, _options.NoPhotoTakenDateAction == CopyNoPhotoTakenDateAction.InSubFolder, _options.NoCoordinateAction == CopyNoCoordinateAction.InSubFolder);

		var filteredPhotosByRelativeDirectory = new Dictionary<string, IReadOnlyCollection<Photo>>();

		_consoleWriter.ProgressStart(TargetRelativeFolderProgressName, groupedPhotosByRelativeDirectory.Count);
		foreach (var (targetRelativeDirectoryPath, photoInfos) in groupedPhotosByRelativeDirectory)
		{
			var (orderedPhotos, notToRenamePhotos) = _exifOrganizerService.FilterAndSortByNoActionTypes(photoInfos, _options.NoPhotoTakenDateAction, _options.NoCoordinateAction);
			_fileNamerService.SetFileName(orderedPhotos, _options.NamingStyle, _options.NumberNamingTextStyle);
			if (_options.FolderProcessType is FolderProcessType.SubFoldersPreserveFolderHierarchy && _options.FolderAppendType.HasValue && _options.FolderAppendLocationType.HasValue)
				_folderRenamer.RenameByFolderAppendType(orderedPhotos, _options.FolderAppendType.Value, _options.FolderAppendLocationType.Value, targetRelativeDirectoryPath);

			var allPhotos = new List<Photo>(orderedPhotos);
			allPhotos.AddRange(notToRenamePhotos);
			_fileService.Copy(allPhotos, _options.OutputPath, _options.IsDryRun);
			filteredPhotosByRelativeDirectory.Add(targetRelativeDirectoryPath, allPhotos);
			_consoleWriter.InProgressItemComplete(TargetRelativeFolderProgressName);
		}

		_consoleWriter.ProgressFinish(TargetRelativeFolderProgressName);

		await _csvService.Report(filteredPhotosByRelativeDirectory, _options.OutputPath, _options.IsDryRun);

		WriteStatistics();

		return ExitCode.Success;
	}

	private bool CheckOutputPathIsUsing(bool isDryRun, out ExitCode exitCode)
	{
		if (isDryRun)
		{
			var outputFile = _fileSystem.FileInfo.FromFileName(_toolOptions.DryRunCsvReportFileName);
			if (outputFile.Exists)
			{
				_logger.LogCritical("Output file: {Path} is exists", _toolOptions.DryRunCsvReportFileName);
				exitCode = ExitCode.OutputPathIsExists;
				return false;
			}

			exitCode = ExitCode.Unset;
			return true;
		}

		var outputDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(_options.OutputPath);
		if (!outputDirectory.Exists)
		{
			if (!HasCreatedDirectory(outputDirectory))
			{
				exitCode = ExitCode.OutputPathDontHaveCreateDirectoryPermission;
				return false;
			}
		}
		else if (outputDirectory.GetDirectories().Length > 0 || outputDirectory.GetFiles().Length > 0)
		{
			_logger.LogCritical("Output folder: {Path} is not empty. It has directories or files in it", _options.OutputPath);
			exitCode = ExitCode.OutputFolderIsNotEmpty;
			return false;
		}

		exitCode = ExitCode.Unset;
		return true;
	}
}
