using System.IO.Abstractions;

namespace PhotoCli.Services.Implementations;

public class PhotoCollectorService : IPhotoCollectorService
{
	private const string ProgressPhotoMainFilesName = "Searching photo main files";
	private const string ProgressPhotoCompanionFilesName = "Searching photo companion files";
	private readonly IConsoleWriter _consoleWriter;
	private readonly Statistics _statistics;
	private readonly ToolOptions _toolOptions;
	private readonly ILogger<PhotoCollectorService> _logger;
	private readonly IFileSystem _fileSystem;

	public PhotoCollectorService(IFileSystem fileSystem, IConsoleWriter consoleWriter, Statistics statistics, ToolOptions toolOptions, ILogger<PhotoCollectorService> logger)
	{
		_fileSystem = fileSystem;
		_consoleWriter = consoleWriter;
		_statistics = statistics;
		_toolOptions = toolOptions;
		_logger = logger;
	}

	public IReadOnlyCollection<Photo> Collect(string folderPath, bool allDirectories, bool searchCompanionFiles)
	{
		var supportedExtensions = AppendDotToRawExtensionValues(_toolOptions.SupportedExtensions);

		if (allDirectories)
			_logger.LogTrace("Getting all photos on all sub directories in {FolderPath} with extensions {Extensions}", folderPath, supportedExtensions);
		else
			_logger.LogTrace("Getting photos in just {FolderPath} with extensions {Extensions}", folderPath, supportedExtensions);

		_consoleWriter.ProgressStart(ProgressPhotoMainFilesName);
		var searchOption = allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
		string[] filePaths;
		try
		{
			filePaths = _fileSystem.Directory
				.EnumerateFiles(folderPath, "*.*", searchOption)
				.Where(w => supportedExtensions.Any(a => w.EndsWith(a, StringComparison.InvariantCultureIgnoreCase))).ToArray();
		}
		catch(DirectoryNotFoundException directoryNotFoundException)
		{
			const string message = "Directory not found, do not change the file system after start processing.";
			_logger.LogCritical(directoryNotFoundException, message);
			throw new PhotoCliException($"{message} -> {directoryNotFoundException.Message}");
		}
		catch(UnauthorizedAccessException unauthorizedAccessException)
		{
			const string message = "Cannot read files with the current user. Give more specific folder as input or give user a read access for the path listed in error.";
			_logger.LogCritical(unauthorizedAccessException, message);
			throw new PhotoCliException($"{message} -> {unauthorizedAccessException.Message}");
		}
		_consoleWriter.ProgressFinish(ProgressPhotoMainFilesName, $"{filePaths.Length} photo(s) found.");
		_statistics.PhotosFound = filePaths.Length;

		var photosInternal = new List<Photo>();
		if (searchCompanionFiles && _toolOptions.CompanionExtensions.Length > 0)
		{
			_consoleWriter.ProgressStart(ProgressPhotoCompanionFilesName);
			var companionExtensions = AppendDotToRawExtensionValues(_toolOptions.CompanionExtensions);

			var companionFilePaths = _fileSystem.Directory
				.EnumerateFiles(folderPath, "*.*", searchOption)
				.Where(w => companionExtensions.Any(a => w.EndsWith(a, StringComparison.InvariantCultureIgnoreCase))).ToArray();

			var companionFileCount = 0;
			foreach (var filePath in filePaths)
			{
				var filePathWithoutExtension = PathHelper.FilePathWithoutExtension(filePath);
				var possibleCompanionFiles = companionExtensions.Select(s => filePathWithoutExtension + s).ToArray();

				var photoCompanionFiles = companionFilePaths.Where(w =>
					w.StartsWith(filePathWithoutExtension, StringComparison.InvariantCultureIgnoreCase)
					&&
					possibleCompanionFiles.Any(a => a.Equals(w, StringComparison.InvariantCultureIgnoreCase))
				).ToList();

				var photoFile = _fileSystem.FileInfo.New(filePath);
				var photoCompanionFileInfo = photoCompanionFiles.Select(photoCompanionFile => _fileSystem.FileInfo.New(photoCompanionFile)).ToList();
				var photo = new Photo(photoFile, photoCompanionFileInfo.Count > 0 ? photoCompanionFileInfo.ToArray() : null);
				companionFileCount += photoCompanionFileInfo.Count;
				photosInternal.Add(photo);
			}

			_consoleWriter.ProgressFinish(ProgressPhotoCompanionFilesName, $"{companionFileCount} companion file(s) found.");
		}
		else
		{
			foreach (var filePath in filePaths)
			{
				var photo = new Photo(_fileSystem.FileInfo.New(filePath));
				photosInternal.Add(photo);
			}
		}

		var readOnlyPhotos = photosInternal.AsReadOnly();
		return readOnlyPhotos;
	}

	private static List<string> AppendDotToRawExtensionValues(IEnumerable<string> rawExtensionValues)
	{
		return rawExtensionValues.Select(s => "." + s).ToList();
	}
}
