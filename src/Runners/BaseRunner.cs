using System.ComponentModel.DataAnnotations;
using System.IO.Abstractions;
using System.Linq.Expressions;
using System.Reflection;
using Spectre.Console;

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
		var table = new Table().Title("Statistics");
		table.AddColumn(new TableColumn("Statistic"));
		table.AddColumn(new TableColumn("Count"));

		table.AddRow("File System Error(s)", _statistics.FileIoErrors.Count.ToString());
		AddStatisticRow(s => s.PhotosFound, table);
		AddStatisticRow(s => s.PhotosCopied, table);
		AddStatisticRow(s => s.PhotosExisted, table);
		AddStatisticRow(s => s.PhotosSame, table);
		AddStatisticRow(s => s.DirectoriesCreated, table);

		table.AddEmptyRow();

		AddStatisticRow(s => s.CompanionFilesFound, table);
		AddStatisticRow(s => s.CompanionFilesCopied, table);
		AddStatisticRow(s => s.CompanionFilesExisted, table);

		table.AddEmptyRow();

		AddStatisticRow(s => s.SourcePhotoFileDeleted, table);
		AddStatisticRow(s => s.SourceCompanionFileDeleted, table);
		AddStatisticRow(s => s.SourceEmptyDirectoryDeleted, table);

		table.AddEmptyRow();

		AddStatisticRow(s => s.UserDefinedAlbumCreated, table);
		AddStatisticRow(s => s.UserDefinedAlbumUpdated, table);
		AddStatisticRow(s => s.AutoAddressAlbumCreated, table);

		table.AddEmptyRow();

		AddStatisticRow(s => s.ReserveGeocodeRequestSent, table);
		AddStatisticRow(s => s.ReserveGeocodeFromMemory, table);
		AddStatisticRow(s => s.ReserveGeocodeFromDatabase, table);
		AddStatisticRow(s => s.PhotoThatHasTakenDateAndCoordinate, table);
		AddStatisticRow(s => s.PhotoThatHasTakenDateButNoCoordinate, table);
		AddStatisticRow(s => s.PhotoThatHasCoordinateButNoTakenDate, table);
		AddStatisticRow(s => s.PhotoThatNoCoordinateAndNoTakenDate, table);

		table.AddEmptyRow();

		AddStatisticRow(s => s.InvalidFormatError, table);
		AddStatisticRow(s => s.InternalError, table);

		_consoleWriter.WriteTable(table);

		WriteFileIoTable();
	}

	private void WriteFileIoTable()
	{
		if (_statistics.FileIoErrors.Count == 0)
			return;
		var table = new Table().Title("File IO Errors").BorderColor(Color.Red);
		table.AddColumn(new TableColumn("File Path"));
		table.AddColumn(new TableColumn("Exception Message"));
		table.AddColumn(new TableColumn("Exception Type"));
		foreach (var fileIoError in _statistics.FileIoErrors)
			table.AddRow(fileIoError.FilePath, fileIoError.ExceptionMessage, fileIoError.ExceptionType);
		_consoleWriter.WriteTable(table);
	}

	private void AddStatisticRow<TValue>(Expression<Func<Statistics, TValue>> expression, Table table) where TValue : struct
	{
		AddKeyValueRowToTable(expression, _statistics, table);
	}

	protected void AddKeyValueRowToTable<T, TValue>(Expression<Func<T, TValue>> expression, T row, Table table) where T : DisplayableRecord where TValue : struct
	{
		var (name, value) = row.GetDisplayNameAndValue(expression);
		table.AddRow(name, value.ToString() ?? string.Empty);
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

	protected bool ExifDataPreventActions(out ExitCode exitCode, ExifDataResult exifDataResult, bool isInvalidFileFormatPreventProcessOptionSelected,
		bool isNoPhotoTakenDatePreventProcessOptionSelected, bool isNoCoordinatePreventProcessOptionSelected, short? expectedDayRange)
	{
		var invalidFileFormatPreventProcess = InvalidFileFormatActionPreventProcess(exifDataResult.AllPhotosAreValid, isInvalidFileFormatPreventProcessOptionSelected, exifDataResult.Photos);
		if (invalidFileFormatPreventProcess)
		{
			exitCode = ExitCode.PhotosWithInvalidFileFormatPreventedProcess;
			return false;
		}

		var noPhotoDateTimeTakenActionPreventProcess = NoPhotoTakenDateActionPreventProcess(exifDataResult.AllPhotosHasPhotoTaken, isNoPhotoTakenDatePreventProcessOptionSelected, exifDataResult.Photos);
		var noCoordinateActionPreventProcess = NoCoordinateActionPreventProcess(exifDataResult.AllPhotosHasCoordinate, isNoCoordinatePreventProcessOptionSelected, exifDataResult.Photos);

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

		if (expectedDayRange != null && exifDataResult.DateRange != null)
		{
			var range = exifDataResult.DateRange.End - exifDataResult.DateRange.Start;
			var unexpectedRange = range.Days > expectedDayRange;
			if (unexpectedRange)
			{
				_logger.LogCritical("{ExpectedDayRange} is exceeded with a {DateRange} of days", expectedDayRange, range.Days);
				exitCode = ExitCode.PhotosWithUnexpectedDateRangePreventedProcess;
				return false;
			}
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
