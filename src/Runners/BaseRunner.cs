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
		if (_statistics.PhotosExisted > 0)
			_consoleWriter.Write($"- {_statistics.PhotosExisted} photo(s) existed on the output.");
		if (_statistics.PhotosSame > 0)
			_consoleWriter.Write($"- {_statistics.PhotosSame} photo(s) are skipped, they have the same photo.");

		if (_statistics.DirectoriesCreated > 0)
			_consoleWriter.Write($"- {_statistics.DirectoriesCreated} directory/directories created.");

		if (_statistics.CompanionFilesCopied > 0)
			_consoleWriter.Write($"- {_statistics.CompanionFilesCopied} companion file(s) copied.");
		if (_statistics.CompanionFilesExisted > 0)
			_consoleWriter.Write($"- {_statistics.CompanionFilesExisted} companion file(s) existed on the output.");

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

	protected bool ValidatePhotoPaths(out ExitCode exitCode, IReadOnlyCollection<Photo> photoPaths, string path)
	{
		if (photoPaths.Count == 0)
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
		IReadOnlyCollection<Photo> exifDataByPhotoBundle)
	{
		var invalidFileFormatPreventProcess = InvalidFileFormatActionPreventProcess(allPhotosAreValid, isInvalidFileFormatPreventProcessOptionSelected, exifDataByPhotoBundle);
		if (invalidFileFormatPreventProcess)
		{
			exitCode = ExitCode.PhotosWithInvalidFileFormatPreventedProcess;
			return false;
		}

		var noPhotoDateTimeTakenActionPreventProcess = NoPhotoTakenDateActionPreventProcess(allPhotosHasPhotoTaken, isNoPhotoTakenDatePreventProcessOptionSelected, exifDataByPhotoBundle);
		var noCoordinateActionPreventProcess = NoCoordinateActionPreventProcess(allPhotosHasCoordinate, isNoCoordinatePreventProcessOptionSelected, exifDataByPhotoBundle);

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

	private bool InvalidFileFormatActionPreventProcess(bool allPhotosAreValid, bool isPreventProcessOptionSelected, IReadOnlyCollection<Photo> photos)
	{
		if (allPhotosAreValid || !isPreventProcessOptionSelected)
			return false;
		_logger.LogDebug("Prevented process because invalid file format action set to prevent process");
		var photosWithInvalidFileFormat = photos.Where(w => !w.HasExifData);
		foreach (var photo in photosWithInvalidFileFormat)
			_logger.LogError("Photo is in invalid file format: {Path}", photo.PhotoFile.SourcePath);
		return true;
	}

	private bool NoPhotoTakenDateActionPreventProcess(bool allPhotosHasPhotoTaken, bool isPreventProcessOptionSelected, IReadOnlyCollection<Photo> photos)
	{
		if (allPhotosHasPhotoTaken || !isPreventProcessOptionSelected)
			return false;
		_logger.LogDebug("Prevented process because no photo taken date action set to prevent process");
		var photosWithNoPhotoTakenDate = photos.Where(w => !w.HasTakenDateTime);
		foreach (var photo in photosWithNoPhotoTakenDate)
			_logger.LogError("No photo taken date: {Path}", photo.PhotoFile.SourcePath);
		return true;
	}

	private bool NoCoordinateActionPreventProcess(bool allPhotosHasCoordinate, bool isPreventProcessOptionSelected, IReadOnlyCollection<Photo> photos)
	{
		if (allPhotosHasCoordinate || !isPreventProcessOptionSelected)
			return false;
		_logger.LogDebug("Prevented process because no coordinate action set to prevent process");
		var photosWithNoCoordinate = photos.Where(w => !w.HasCoordinate);
		foreach (var photo in photosWithNoCoordinate)
			_logger.LogError("No coordinate: {Path}", photo.PhotoFile.SourcePath);
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
