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
		CopyInternal(photos, outputFolder, isDryRun, true);
	}

	public IReadOnlyCollection<Photo> CopyIfNotExists(IReadOnlyCollection<Photo> photos, string outputFolder, bool isDryRun)
	{
		return CopyInternal(photos, outputFolder, isDryRun, false);
	}

	private IReadOnlyCollection<Photo> CopyInternal(IReadOnlyCollection<Photo> photos, string outputFolder, bool isDryRun, bool breakOnDestinationExist)
	{
		var photosCopied = new List<Photo>();
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
			try
			{
				if (newFileInfo.Exists)
				{
					if (breakOnDestinationExist)
						throw new FileExistsOnDestinationPathException(destinationPath);
					++_statistics.PhotosExisted;
					_logger.LogInformation("Photo is existed on to: {Path}, skipping", destinationPath);
					continue;
				}
				if (!isDryRun)
					fileInfo.CopyTo(destinationPath, false);
				++_statistics.PhotosCopied;
				photosCopied.Add(photo);
			}
			catch (IOException ioException)
			{
				_statistics.FileIoErrors.Add(ioException.Message);
				_logger.LogCritical(ioException, "Can't copy file");
			}
			_logger.LogInformation("Photo is copied to: {To} from: {From}", destinationPath, fileInfo.FullName);
		}

		return photosCopied;
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
			string sourceHashFormatted;
			if (photo.Sha1Hash == null)
				sourceHashFormatted = await GetFileHashFormatted(sourceFileInfo, shaDestination);
			else
				sourceHashFormatted = photo.Sha1Hash;
			var destinationHashFormatted = await GetFileHashFormatted(destinationFileInfo, shaDestination);
			var fileIntegrityIsSuccessful = sourceHashFormatted == destinationHashFormatted;
			if (!fileIntegrityIsSuccessful)
			{
				_logger.LogCritical("Source photo file content path: {SourcePath} doesn't match with target photo aTarget photo\'s content hash doesn\'t exists at path : {DestinationPath} for a source photo path: {SourceFileInfo}", photo.FilePath, destinationPath, sourceFileInfo);
				return false;
			}
			photo.Sha1Hash ??= destinationHashFormatted;
		}
		return true;
	}

	public async Task SaveGnuHashFileTree(IEnumerable<Photo> photos, string outputFolder)
	{
		var gnuFormat = GnuHashFileTreeFormat(photos);
		var path = Path.Combine(outputFolder, Constants.VerifyFileHashFileName);
		await _fileSystem.File.WriteAllTextAsync(path, gnuFormat);
	}

	public async Task CalculateFileHash(IEnumerable<Photo> photos)
	{
		using var sha = SHA1.Create();
		foreach (var photo in photos)
		{
			var fileInfo = _fileSystem.FileInfo.FromFileName(photo.FilePath);
			photo.Sha1Hash = await GetFileHashFormatted(fileInfo, sha);
		}
	}

	private string GnuHashFileTreeFormat(IEnumerable<Photo> photos)
	{
		var content = new StringBuilder();
		foreach (var photo in photos)
			content.Append($"{photo.Sha1Hash}  {photo.RelativePath()}{Environment.NewLine}");
		return content.ToString();
	}

	private async Task<string> GetFileHashFormatted(IFileInfo fileInfo, HashAlgorithm hashAlgorithm)
	{
		await using var stream = fileInfo.OpenRead();
		var hashByte = await hashAlgorithm.ComputeHashAsync(stream);
		var hex = Convert.ToHexString(hashByte);
		var hexLowered = hex.ToLower();
		return hexLowered;
	}
}
