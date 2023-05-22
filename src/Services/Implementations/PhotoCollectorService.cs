using System.IO.Abstractions;

namespace PhotoCli.Services.Implementations;

public class PhotoCollectorService : IPhotoCollectorService
{
	private const string ProgressName = "Searching photos";
	private static readonly string[] SupportedExtensions = { ".jpg" };
	private readonly IConsoleWriter _consoleWriter;
	private readonly Statistics _statistics;
	private readonly ILogger<PhotoCollectorService> _logger;
	private readonly IFileSystem _fileSystem;

	public PhotoCollectorService(IFileSystem fileSystem, IConsoleWriter consoleWriter, Statistics statistics, ILogger<PhotoCollectorService> logger)
	{
		_fileSystem = fileSystem;
		_consoleWriter = consoleWriter;
		_statistics = statistics;
		_logger = logger;
	}

	public string[] Collect(string folderPath, bool allDirectories)
	{
		_logger.LogTrace(allDirectories ? "Getting all photos on all sub directories in {FolderPath} with extensions {Extensions}" : "Getting photos in just {FolderPath} with extensions {Extensions}",
			folderPath, SupportedExtensions);

		_consoleWriter.ProgressStart(ProgressName);
		var searchOption = allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
		string[] filePaths;
		try
		{
			filePaths = _fileSystem.Directory
				.GetFiles(folderPath, "*.*", searchOption)
				.Where(w => SupportedExtensions.Any(a => w.EndsWith(a, StringComparison.InvariantCultureIgnoreCase))).ToArray();
		}
		catch(UnauthorizedAccessException unauthorizedAccessException)
		{
			const string message = "Cannot read files with the current user. Give more specific folder as input or give user a read access for the path listed in error.";
			_logger.LogCritical(unauthorizedAccessException, message);
			throw new PhotoCliException($"{message} -> {unauthorizedAccessException.Message}");
		}
		_consoleWriter.ProgressFinish(ProgressName, $"{filePaths.Length} photo(s) found.");
		_statistics.PhotosFound = filePaths.Length;
		return filePaths;
	}
}
