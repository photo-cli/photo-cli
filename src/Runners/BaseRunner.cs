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
			_consoleWriter.Write($"- {_statistics.PhotosCopied} photo(s) copied.");
		if (_statistics.DirectoriesCreated > 0)
			_consoleWriter.Write($"- {_statistics.DirectoriesCreated} directory/directories created.");

		if (_statistics.PhotoThatHasTakenDateAndCoordinate > 0)
			_consoleWriter.Write($"- {_statistics.PhotoThatHasTakenDateAndCoordinate} photo(s) has taken date and coordinate.");
		if (_statistics.PhotoThatHasTakenDateButNoCoordinate > 0)
			_consoleWriter.Write($"- {_statistics.PhotoThatHasTakenDateButNoCoordinate} photo(s) has taken date but no coordinate.");
		if (_statistics.PhotoThatHasCoordinateButNoTakenDate > 0)
			_consoleWriter.Write($"- {_statistics.PhotoThatHasCoordinateButNoTakenDate} photo(s) has coordinate but no taken date.");
		if (_statistics.PhotoThatNoCoordinateAndNoTakenDate > 0)
			_consoleWriter.Write($"- {_statistics.PhotoThatNoCoordinateAndNoTakenDate} photo(s) has no taken date and coordinate.");
		if (_statistics.InvalidFormatError > 0)
			_consoleWriter.Write($"- {_statistics.InvalidFormatError} photo(s) has unknown/invalid format..");
		if (_statistics.InternalError > 0)
			_consoleWriter.Write($"- {_statistics.InternalError} photo(s) caused unexpected error internally.");
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

	protected bool NoExifDataPreventActions(out ExitCode exitCode, bool allPhotosAreValid, bool allPhotosHasPhotoTaken, bool allPhotosHasCoordinate,
		bool isInvalidFileFormatPreventProcessOptionSelected, bool isNoPhotoTakenDatePreventProcessOptionSelected, bool isNoCoordinatePreventProcessOptionSelected,
		Dictionary<string, ExifData?> exifDataByPath)
	{
		var invalidFileFormatPreventProcess = InvalidFileFormatActionPreventProcess(allPhotosAreValid, isInvalidFileFormatPreventProcessOptionSelected, exifDataByPath);
		if (invalidFileFormatPreventProcess)
		{
			exitCode = ExitCode.PhotosWithInvalidFileFormatPreventedProcess;
			return false;
		}

		var noPhotoDateTimeTakenActionPreventProcess = NoPhotoTakenDateActionPreventProcess(allPhotosHasPhotoTaken, isNoPhotoTakenDatePreventProcessOptionSelected, exifDataByPath);
		var noCoordinateActionPreventProcess = NoCoordinateActionPreventProcess(allPhotosHasCoordinate, isNoCoordinatePreventProcessOptionSelected, exifDataByPath);

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

	private bool InvalidFileFormatActionPreventProcess(bool allPhotosAreValid, bool isPreventProcessOptionSelected, Dictionary<string, ExifData?> photoTakenDateTimesByPath)
	{
		if (allPhotosAreValid || !isPreventProcessOptionSelected)
			return false;
		_logger.LogDebug("Prevented process because invalid file format action set to prevent process");
		var photosWithInvalidFileFormat = photoTakenDateTimesByPath.Where(w => w.Value == null);
		foreach (var (photoPath, _) in photosWithInvalidFileFormat)
			_logger.LogError("Photo is in invalid file format: {Path}", photoPath);
		return true;
	}

	private bool NoPhotoTakenDateActionPreventProcess(bool allPhotosHasPhotoTaken, bool isPreventProcessOptionSelected, Dictionary<string, ExifData?> photoTakenDateTimesByPath)
	{
		if (allPhotosHasPhotoTaken || !isPreventProcessOptionSelected)
			return false;
		_logger.LogDebug("Prevented process because no photo taken date action set to prevent process");
		var photosWithNoPhotoTakenDate = photoTakenDateTimesByPath.Where(w => w.Value?.TakenDate == null);
		foreach (var (photoPath, _) in photosWithNoPhotoTakenDate)
			_logger.LogError("No photo taken date: {Path}", photoPath);
		return true;
	}

	private bool NoCoordinateActionPreventProcess(bool allPhotosHasCoordinate, bool isPreventProcessOptionSelected, Dictionary<string, ExifData?> photoTakenDateTimesByPath)
	{
		if (allPhotosHasCoordinate || !isPreventProcessOptionSelected)
			return false;
		_logger.LogDebug("Prevented process because no coordinate action set to prevent process");
		var photosWithNoCoordinate = photoTakenDateTimesByPath.Where(w => w.Value?.Coordinate == null);
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
