using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;

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

	public async Task<bool> VerifyFileIntegrity(IEnumerable<Photo> photos, string outputFolder)
	{
		using var shaSource = SHA1.Create();
		using var shaDestination = SHA1.Create();
		foreach (var photo in photos)
		{
			var sourceFileInfo = _fileSystem.FileInfo.FromFileName(photo.FilePath);
			var destinationPath = photo.DestinationPath(outputFolder);
			var destinationFileInfo = _fileSystem.FileInfo.FromFileName(destinationPath);

			if (!destinationFileInfo.Exists)
			{
				_logger.LogCritical("Target photo didn't exists at path : {DestinationPath} for a source photo path: {PhotoFilePath}", destinationPath, photo.FilePath);
				return false;
			}

			await using var sourceStream = sourceFileInfo.OpenRead();
			await using var destinationStream = destinationFileInfo.OpenRead();

			var sourceHashTask = shaSource.ComputeHashAsync(sourceStream);
			var destinationHashTask = shaDestination.ComputeHashAsync(destinationStream);

			await Task.WhenAll(sourceHashTask, destinationHashTask);

			var sourceHash = sourceHashTask.Result;
			var destinationHash = destinationHashTask.Result;

			var fileIntegrityIsSuccessful = sourceHash.SequenceEqual(destinationHash);
			if (!fileIntegrityIsSuccessful)
			{
				_logger.LogCritical("Source photo file content path: {SourcePath} doesn't match with target photo aTarget photo\'s content hash doesn\'t exists at path : {DestinationPath} for a source photo path: {SourceFileInfo}", photo.FilePath, destinationPath, sourceFileInfo);
				return false;
			}

			var hex = Convert.ToHexString(sourceHash);
			var hexLowered = hex.ToLower();
			photo.Sha1Hash = hexLowered;
		}
		return true;
	}

	public async Task SaveGnuHashFileTree(IEnumerable<Photo> photos, string outputFolder)
	{
		var gnuFormat = GnuHashFileTreeFormat(photos);
		var path = Path.Combine(outputFolder, Constants.VerifyFileHashFileName);
		await _fileSystem.File.WriteAllTextAsync(path, gnuFormat);
	}

	private string GnuHashFileTreeFormat(IEnumerable<Photo> photos)
	{
		var content = new StringBuilder();
		foreach (var photo in photos)
			content.Append($"{photo.Sha1Hash}  {photo.RelativePath()}{Environment.NewLine}");
		return content.ToString();
	}
}
