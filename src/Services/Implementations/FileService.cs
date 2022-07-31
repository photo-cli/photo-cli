using System.IO.Abstractions;

namespace PhotoCli.Services.Implementations;

public class FileService : IFileService
{
	private readonly IFileSystem _fileSystem;
	private readonly ILogger<FileService> _logger;
	private readonly Statistics _statistics;
	private List<string>? _directoriesCreatedHistoryForDryRun;

	public FileService(IFileSystem fileSystem, ILogger<FileService> logger, Statistics statistics)
	{
		_fileSystem = fileSystem;
		_logger = logger;
		_statistics = statistics;
	}

	public void Copy(IReadOnlyCollection<Photo> photos, string outputFolder, bool isDryRun)
	{
		foreach (var photo in photos)
		{
			var fileInfo = _fileSystem.FileInfo.FromFileName(photo.FilePath);
			var destinationPath = photo.DestinationPath(outputFolder);
			var newFileInfo = _fileSystem.FileInfo.FromFileName(destinationPath);

			if (isDryRun)
			{
				_directoriesCreatedHistoryForDryRun ??= new List<string> { _fileSystem.DirectoryInfo.FromDirectoryName(outputFolder).FullName };
				if (!_directoriesCreatedHistoryForDryRun.Contains(newFileInfo.Directory.FullName))
				{
					_directoriesCreatedHistoryForDryRun.Add(newFileInfo.Directory.FullName);
					++_statistics.DirectoriesCreated;
				}
			}
			else if (!newFileInfo.Directory.Exists)
			{
				_logger.LogTrace("Directory is creating: {Path}", newFileInfo.Directory.FullName);
				newFileInfo.Directory.Create();
				++_statistics.DirectoriesCreated;
				_logger.LogInformation("Directory created: {Path} ", newFileInfo.Directory.FullName);
			}

			_logger.LogTrace("Photo is copying to: {To} from: {From}", destinationPath, fileInfo.FullName);
			if (!isDryRun)
			{
				try
				{
					fileInfo.CopyTo(destinationPath);
				}
				catch (IOException ioException)
				{
					_statistics.FileIoErrors.Add(ioException.Message);
					_logger.LogCritical(ioException, "Can't copy file");
				}
			}

			_logger.LogInformation("Photo is copied to: {To} from: {From}", destinationPath, fileInfo.FullName);
		}

		_statistics.PhotosCopied += photos.Count;
	}
}
