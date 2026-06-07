using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Spectre.Console;

namespace PhotoCli.Runners;

public class ListRunner : BaseRunner, IConsoleRunner
{
	private readonly ILogger<ListRunner> _logger;
	private readonly ListOptions _options;
	private readonly IDbService _dbService;
	private readonly IFileSystem _fileSystem;
	private readonly IConsoleWriter _consoleWriter;
	private readonly AnsiConsoleExtended _ansiConsoleExtended;
	private readonly IProcessLauncher _processLauncher;

	public ListRunner(ListOptions options, IDbService dbService, IFileSystem fileSystem, Statistics statistics, IConsoleWriter consoleWriter,
		AnsiConsoleExtended ansiConsoleExtended, IProcessLauncher processLauncher, ILogger<ListRunner> logger) : base(logger, fileSystem, statistics, consoleWriter)
	{
		_options = options;
		_dbService = dbService;
		_fileSystem = fileSystem;
		_consoleWriter = consoleWriter;
		_ansiConsoleExtended = ansiConsoleExtended;
		_processLauncher = processLauncher;
		_logger = logger;
	}

	public async Task<ExitCode> Execute()
	{
		var archivePath = _options.ArchivePath;
		if (!CheckArchiveDatabaseExists(archivePath, out var exitCodeInputFolder))
			return exitCodeInputFolder;

		switch (_options.ListType)
		{
			case ListType.Summary:
				return await Summary();
			case ListType.Albums:
				return await Albums();
			case ListType.PhotosByAlbumId:
				return await PhotoByAlbumId(archivePath);
			case ListType.PhotosByAlbumName:
				return await PhotoByAlbumName(archivePath);
			case ListType.PhotosByExactDate:
				return await PhotoByExactDate(archivePath);
			case ListType.PhotosByDateRange:
				return await PhotoByDateRange(archivePath);
			default:
				throw new PhotoCliException();
		}
	}

	private async Task<ExitCode> Summary()
	{
		var albumCountTask = _dbService.TotalAlbumCount();
		var photoCountTask = _dbService.TotalPhotoCount();
		var reverseGeocodeCacheCountTask = _dbService.TotalReverseGeocodeCacheCount();
		await Task.WhenAll(albumCountTask, photoCountTask, reverseGeocodeCacheCountTask);
		var albumCount = await albumCountTask;
		var photoCount = await photoCountTask;
		var reverseGeocodeCacheCount = await reverseGeocodeCacheCountTask;

		var table = new Table();
		table.AddColumn(new TableColumn("Table"));
		table.AddColumn(new TableColumn("Count"));

		var listSummary = new ListSummary
		{
			AlbumCount = albumCount,
			PhotoCount = photoCount,
			ReverseGeocodeCacheCount = reverseGeocodeCacheCount,
		};

		AddKeyValueRowToTable(s => s.AlbumCount, listSummary, table);
		AddKeyValueRowToTable(s => s.PhotoCount, listSummary, table);
		AddKeyValueRowToTable(s => s.ReverseGeocodeCacheCount, listSummary, table);

		_consoleWriter.WriteTable(table);
		return ExitCode.Success;
	}

	private async Task<ExitCode> Albums()
	{
		var table = new Table();
		table.AddColumn(new TableColumn("Id"));
		table.AddColumn(new TableColumn("Name"));
		table.AddColumn(new TableColumn("Type"));
		table.AddColumn(new TableColumn("Created At"));
		table.AddColumn(new TableColumn("Configuration"));

		var albums = await _dbService.GetAllAlbums();
		foreach (var album in albums)
			table.AddRow(album.Id.ToString(), album.Name, album.Type.ToString(), album.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), _ansiConsoleExtended.EscapeMarkup(album.Configuration));
		_consoleWriter.WriteTable(table);
		return ExitCode.Success;
	}

	private async Task<ExitCode> PhotoByAlbumId(string inputArchivePath)
	{
		if (!_options.AlbumId.HasValue)
			throw new PhotoCliException("Album id is required to list photos by albums");

		var albumPhotoResult = await _dbService.GetAlbumPhotosById(_options.AlbumId.Value);
		if (albumPhotoResult.Status == AlbumPhotoResultStatus.Successful)
			return await ListPhotos(albumPhotoResult.Photos, inputArchivePath);
		return albumPhotoResult.Status switch
		{
			AlbumPhotoResultStatus.AlbumNotFound => ExitCode.AlbumNotFoundById,
			AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat => ExitCode.ExistingAlbumConfigurationNotValid,
			_ => throw new PhotoCliException($"Not defined album photo result on {nameof(PhotoByAlbumId)} for {nameof(AlbumPhotoResultStatus)}: {albumPhotoResult.Status}")
		};
	}

	private async Task<ExitCode> PhotoByAlbumName(string inputArchivePath)
	{
		if (_options.AlbumName == null)
			throw new PhotoCliException("Album id or album name is required to list photos by albums");

		var albumPhotoResult = await _dbService.GetAlbumPhotosByName(_options.AlbumName);
		if (albumPhotoResult.Status == AlbumPhotoResultStatus.Successful)
			return await ListPhotos(albumPhotoResult.Photos, inputArchivePath);
		return albumPhotoResult.Status switch
		{
			AlbumPhotoResultStatus.AlbumNotFound => ExitCode.AlbumNotFoundByName,
			AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat => ExitCode.ExistingAlbumConfigurationNotValid,
			_ => throw new PhotoCliException($"Not defined album photo result on {nameof(PhotoByAlbumId)} for {nameof(AlbumPhotoResultStatus)}: {albumPhotoResult.Status}")
		};

	}

	private async Task<ExitCode> PhotoByExactDate(string inputArchivePath)
	{
		var photos = await _dbService.GetPhotosByDate(_options.Year, _options.Month, _options.Day);
		return await ListPhotos(photos, inputArchivePath);
	}

	private async Task<ExitCode> PhotoByDateRange(string inputArchivePath)
	{
		var photos = await _dbService.GetPhotosByDateRange(_options.StartDate, _options.EndDate);
		return await ListPhotos(photos, inputArchivePath);
	}

	private async Task<ExitCode> ListPhotos(List<PhotoEntity> photos, string inputArchivePath)
	{
		if (photos.Count == 0)
		{
			_consoleWriter.Write("No photo found to list, try to use another date range or album id");
			return ExitCode.NoPhotoFoundToList;
		}

		var runMacPreview = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && !_options.RawOutput;
		var photoPaths = new List<string>();
		foreach (var photo in photos)
		{
			var fullPhotoPath = Path.Combine(inputArchivePath, photo.Path);
			if (runMacPreview)
				photoPaths.Add(fullPhotoPath);
			else
				_consoleWriter.RawWriteLine(fullPhotoPath);
		}
		if (runMacPreview)
		{
			await _processLauncher.Launch(photoPaths);
		}

		return ExitCode.Success;
	}

	private bool CheckArchiveDatabaseExists(string archivePath, out ExitCode exitCode)
	{
		var archiveDatabaseInputPathToCheck = _options.CustomDatabasePath ?? Path.Combine(archivePath, Constants.ArchiveSQLiteDatabaseFileName);
		if (!_fileSystem.File.Exists(archiveDatabaseInputPathToCheck))
		{
			_logger.LogCritical("Archive database not found at: {Path}", archiveDatabaseInputPathToCheck);
			exitCode = ExitCode.NoArchiveDatabaseFound;
			return false;
		}

		exitCode = ExitCode.Unset;
		return true;
	}
}
