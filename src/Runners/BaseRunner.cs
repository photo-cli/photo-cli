using System.IO.Abstractions;

namespace PhotoCli.Runners;

public abstract class BaseRunner
{
	private readonly IConsoleWriter _consoleWriter;
	private readonly IFileSystem _fileSystem;
	private readonly ILogger<BaseRunner> _logger;
	private readonly Statistics _statistics;

	protected BaseRunner(ILogger<BaseRunner> logger, IFileSystem fileSystem, Statistics statistics, IConsoleWriter consoleWriter)
	{
		_logger = logger;
		_fileSystem = fileSystem;
		_statistics = statistics;
		_consoleWriter = consoleWriter;
	}

	protected void WriteStatistics()
	{
		if (_statistics.FileIoErrors.Count > 0)
		{
			_consoleWriter.Write("- File IO Errors");
			var fileErrorsJoined = string.Join(Environment.NewLine, _statistics.FileIoErrors.Select(s => $"- {s}"));
			_consoleWriter.Write(fileErrorsJoined);
		}

		if (_statistics.PhotosCopied > 0)
			_consoleWriter.Write($"- {_statistics.PhotosCopied} photos copied.");
		if (_statistics.DirectoriesCreated > 0)
			_consoleWriter.Write($"- {_statistics.DirectoriesCreated} directories created.");

		if (_statistics.PhotoThatHasTakenDateAndCoordinate > 0)
			_consoleWriter.Write($"- {_statistics.PhotoThatHasTakenDateAndCoordinate} photos has taken date and coordinate.");
		if (_statistics.PhotoThatHasTakenDateButNoCoordinate > 0)
			_consoleWriter.Write($"- {_statistics.PhotoThatHasTakenDateButNoCoordinate} photos has taken date but no coordinate.");
		if (_statistics.PhotoThatHasCoordinateButNoTakenDate > 0)
			_consoleWriter.Write($"- {_statistics.PhotoThatHasCoordinateButNoTakenDate} photos has coordinate but no taken date.");
		if (_statistics.PhotoThatNoCoordinateAndNoTakenDate > 0)
			_consoleWriter.Write($"- {_statistics.PhotoThatNoCoordinateAndNoTakenDate} photos has no taken date and coordinate.");
	}

	protected bool ValidatePhotoPaths(out ExitCode exitCode, string[] photoPaths, string path)
	{
		if (photoPaths.Length == 0)
		{
			Console.WriteLine($"No photo found on folder: {path}");
			exitCode = ExitCode.NoPhotoFoundOnDirectory;
			return false;
		}

		exitCode = ExitCode.Unset;
		return true;
	}

	protected bool NoExifDataPreventActions(out ExitCode exitCode, bool allPhotosHasPhotoTaken, bool allPhotosHasCoordinate, bool isPreventProcessOptionSelectedNoPhotoTakenDate,
		bool isPreventProcessOptionSelectedNoCoordinate, Dictionary<string, ExifData> photoTakenDateTimesByPath)
	{
		var noPhotoDateTimeTakenActionPreventProcess = NoPhotoTakenDateActionPreventProcess(allPhotosHasPhotoTaken, isPreventProcessOptionSelectedNoPhotoTakenDate,
			photoTakenDateTimesByPath);
		var noCoordinateActionPreventProcess = NoCoordinateActionPreventProcess(allPhotosHasCoordinate, isPreventProcessOptionSelectedNoCoordinate, photoTakenDateTimesByPath);
		if (noPhotoDateTimeTakenActionPreventProcess && noCoordinateActionPreventProcess)
		{
			exitCode = ExitCode.PhotosWithNoCoordinateAndNoDatePreventedProcess;
			return false;
		}

		if (noPhotoDateTimeTakenActionPreventProcess)
		{
			exitCode = ExitCode.PhotosWithNoDatePreventedProcess;
			return false;
		}

		if (noCoordinateActionPreventProcess)
		{
			exitCode = ExitCode.PhotosWithNoCoordinatePreventedProcess;
			return false;
		}

		exitCode = ExitCode.Unset;
		return true;
	}

	private bool NoPhotoTakenDateActionPreventProcess(bool allPhotosHasPhotoTaken, bool isPreventProcessOptionSelected, Dictionary<string, ExifData> photoTakenDateTimesByPath)
	{
		if (allPhotosHasPhotoTaken || !isPreventProcessOptionSelected)
			return false;
		_logger.LogDebug("Prevented process because no photo taken date action set to prevent process");
		var photosWithNoPhotoTakenDate = photoTakenDateTimesByPath.Where(w => !w.Value.TakenDate.HasValue);
		foreach (var (photoPath, _) in photosWithNoPhotoTakenDate)
			_logger.LogError("No photo taken date: {Path}", photoPath);
		return true;
	}

	private bool NoCoordinateActionPreventProcess(bool allPhotosHasCoordinate, bool isPreventProcessOptionSelected, Dictionary<string, ExifData> photoTakenDateTimesByPath)
	{
		if (allPhotosHasCoordinate || !isPreventProcessOptionSelected)
			return false;
		_logger.LogDebug("Prevented process because no coordinate action set to prevent process");
		var photosWithNoCoordinate = photoTakenDateTimesByPath.Where(w => w.Value.Coordinate == null);
		foreach (var (photoPath, _) in photosWithNoCoordinate)
			_logger.LogError("No coordinate: {Path}", photoPath);
		return true;
	}

	protected bool CheckInputFolderExists(string sourceFolderPath, out ExitCode exitCode)
	{
		if (!_fileSystem.Directory.Exists(sourceFolderPath))
		{
			_logger.LogCritical("Input folder path not exists");
			exitCode = ExitCode.InputFolderNotExists;
			return false;
		}

		exitCode = ExitCode.Unset;
		return true;
	}

	protected bool HasPermissionToWriteFile(IFileInfo file)
	{
		try
		{
			using (var streamWriter = file.CreateText())
				streamWriter.Write(1);
			file.Delete();
			return true;
		}
		catch (UnauthorizedAccessException ex)
		{
			_logger.LogCritical(ex, "Don't have permission to write on {FilePath}", file.FullName);
			return false;
		}
	}

	protected bool HasCreatedDirectory(IDirectoryInfo directory)
	{
		try
		{
			directory.Create();
			return true;
		}
		catch (UnauthorizedAccessException ex)
		{
			_logger.LogCritical(ex, "Don't have permission to create directory on {FilePath}", directory.FullName);
			return false;
		}
	}
}
