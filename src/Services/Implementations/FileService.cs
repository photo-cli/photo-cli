using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace PhotoCli.Services.Implementations;

public class FileService : IFileService
{
	private readonly IFileSystem _fileSystem;
	private readonly ILogger<FileService> _logger;
	private readonly Statistics _statistics;
	private readonly IConsoleWriter _consoleWriter;
	private List<string>? _directoriesCreatedHistoryForDryRun;

	public FileService(IFileSystem fileSystem, ILogger<FileService> logger, Statistics statistics, IConsoleWriter consoleWriter)
	{
		_fileSystem = fileSystem;
		_logger = logger;
		_statistics = statistics;
		_consoleWriter = consoleWriter;
	}

	public IReadOnlyCollection<Photo> Copy(IReadOnlyCollection<Photo> photos, string outputFolder, bool isDryRun)
	{
		return CopyInternal(photos, outputFolder, isDryRun, true);
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
			photo.SetTarget(outputFolder);
			var fileInfo = _fileSystem.FileInfo.New(photo.PhotoFile.SourcePath);
			if (photo.PhotoFile.TargetFullPath == null)
				throw new PhotoCliException($"Couldn't copy main photo file {photo.PhotoFile.SourceFullPath} without a {nameof(PhotoFile.TargetFullPath)}");
			var newPhotoFileInfo = _fileSystem.FileInfo.New(photo.PhotoFile.TargetFullPath);

			if (isDryRun)
			{
				_directoriesCreatedHistoryForDryRun ??= [];
				if (!_directoriesCreatedHistoryForDryRun.Contains(newPhotoFileInfo.Directory!.FullName))
				{
					_directoriesCreatedHistoryForDryRun.Add(newPhotoFileInfo.Directory.FullName);
					++_statistics.DirectoriesCreated;
				}
			}
			else if (!newPhotoFileInfo.Directory!.Exists)
			{
				_logger.LogTrace("Directory is creating: {Path}", newPhotoFileInfo.Directory.FullName);
				newPhotoFileInfo.Directory.Create();
				++_statistics.DirectoriesCreated;
				_logger.LogInformation("Directory created: {Path} ", newPhotoFileInfo.Directory.FullName);
			}
			_logger.LogTrace("Photo is copying to: {To} from: {From}", photo.PhotoFile.TargetFullPath, photo.PhotoFile.SourcePath);
			try
			{
				if (newPhotoFileInfo.Exists)
				{
					if (breakOnDestinationExist)
						throw new FileExistsOnDestinationPathException(photo.PhotoFile);
					++_statistics.PhotosExisted;
					_logger.LogInformation("Photo is existed on to: {Path}, skipping", photo.PhotoFile.TargetFullPath);
					continue;
				}
				if (!isDryRun)
					fileInfo.CopyTo(photo.PhotoFile.TargetFullPath, false);
				++_statistics.PhotosCopied;

				if (photo.CompanionFiles != null)
				{
					foreach (var companionFile in photo.CompanionFiles)
					{
						var companionFileInfo = _fileSystem.FileInfo.New(companionFile.SourcePath);
						if (companionFile.TargetFullPath == null)
							throw new PhotoCliException($"Couldn't copy companion file {companionFile.SourceFullPath} without a {nameof(PhotoFile.TargetFullPath)}");

						var newCompanionFileInfo = _fileSystem.FileInfo.New(companionFile.TargetFullPath);
						if (newCompanionFileInfo.Exists)
						{
							if (breakOnDestinationExist)
								throw new FileExistsOnDestinationPathException(companionFile);
							++_statistics.CompanionFilesExisted;
							_logger.LogInformation("Companion file is existed on to: {Path}, skipping", companionFile.TargetFullPath);
							continue;
						}

						if (!isDryRun)
							companionFileInfo.CopyTo(companionFile.TargetFullPath, false);
						++_statistics.CompanionFilesCopied;
					}
				}

				photosCopied.Add(photo);
			}
			catch (IOException ioException)
			{
				_statistics.FileIoErrors.Add(new FileIoErrorInfo
				{
					FilePath = photo.PhotoFile.SourcePath,
					ExceptionMessage = ioException.Message,
					ExceptionType = ioException.GetType().Name
				});
				_logger.LogCritical(ioException, "Can't copy file");
			}
			_logger.LogInformation("Photo is copied to: {To} from: {From}", photo.PhotoFile.TargetFullPath, fileInfo.FullName);
		}

		return photosCopied;
	}

	public async Task<bool> VerifyFileIntegrity(IEnumerable<Photo> photos)
	{
		using var shaSource = SHA1.Create();
		using var shaDestination = SHA1.Create();
		using var shaCompanionSource = SHA1.Create();
		using var shaCompanionDestination = SHA1.Create();
		foreach (var photo in photos)
		{
			var sourceFileInfo = _fileSystem.FileInfo.New(photo.PhotoFile.SourcePath);
			if (photo.PhotoFile.TargetFullPath == null)
				throw new PhotoCliException($"Couldn't copy a photo don't have a {nameof(PhotoFile.TargetFullPath)} on {photo.PhotoFile.SourceFullPath}");
			var destinationFileInfo = _fileSystem.FileInfo.New(photo.PhotoFile.TargetFullPath);
			if (!destinationFileInfo.Exists)
			{
				_logger.LogCritical("Target photo doesn't exists at path : {DestinationPath} for a source photo path: {SourcePath}", photo.PhotoFile.TargetFullPath, photo.PhotoFile.SourcePath);
				return false;
			}
			string sourceHashFormatted;
			if (photo.PhotoFile.Sha1Hash == null)
			{
				sourceHashFormatted = await GetFileHashFormatted(sourceFileInfo, shaDestination);
				photo.PhotoFile.Sha1Hash = sourceHashFormatted;
			}
			else
			{
				sourceHashFormatted = photo.PhotoFile.Sha1Hash;
			}
			var destinationHashFormatted = await GetFileHashFormatted(destinationFileInfo, shaDestination);
			var mainPhotoIntegrityIsSuccessful = sourceHashFormatted == destinationHashFormatted;
			if (!mainPhotoIntegrityIsSuccessful)
			{
				_logger.LogCritical("Source main photo file content path: {SourcePath} doesn't match with target photo's hash: {DestinationPath}",
					photo.PhotoFile.SourcePath, photo.PhotoFile.TargetFullPath);

				return false;
			}
			photo.PhotoFile.Sha1Hash = sourceHashFormatted;

			if (photo.CompanionFiles != null)
			{
				foreach (var companionFile in photo.CompanionFiles)
				{
					var companionSourceFileInfo = _fileSystem.FileInfo.New(companionFile.SourcePath);
					if (companionFile.TargetFullPath == null)
						throw new PhotoCliException($"Couldn't copy a companion file which don't have a {nameof(PhotoFile.TargetFullPath)} for companion file {companionFile.SourceFullPath}");
					var companionDestinationFileInfo = _fileSystem.FileInfo.New(companionFile.TargetFullPath);

					if (!companionDestinationFileInfo.Exists)
					{
						_logger.LogCritical("Target companion file doesn't exists at path : {DestinationPath} for a source companion file path: {SourcePath}",
							companionFile.TargetFullPath, companionFile.SourcePath);

						return false;
					}

					var companionSourceHashFormatted = await GetFileHashFormatted(companionSourceFileInfo, shaCompanionSource);
					var companionDestinationHashFormatted = await GetFileHashFormatted(companionDestinationFileInfo, shaCompanionDestination);

					var companionIntegrityIsSuccessful = companionSourceHashFormatted == companionDestinationHashFormatted;
					if (!companionIntegrityIsSuccessful)
					{
						_logger.LogCritical("Source companion file content path: {SourcePath} doesn't match with target companion file's hash: {DestinationPath}",
							companionFile.SourcePath, companionFile.TargetFullPath);

						return false;
					}
					companionFile.Sha1Hash = companionDestinationHashFormatted;
				}
			}
		}
		return true;
	}

	public async Task SaveGnuHashFileTree(IEnumerable<Photo> photos, string outputFolder)
	{
		var gnuFormat = GnuHashFileTreeFormat(photos);
		var path = Path.Combine(outputFolder, Constants.VerifyFileHashFileName);
		await _fileSystem.File.WriteAllTextAsync(path, gnuFormat);
	}

	public async Task<IReadOnlyCollection<Photo>> CalculateFileHash(IReadOnlyCollection<Photo> photos)
	{
		const string progressName = "Calculating file hashes";
		_consoleWriter.ProgressStart(progressName);
		using var sha = SHA1.Create();
		foreach (var photo in photos)
		{
			var fileInfo = _fileSystem.FileInfo.New(photo.PhotoFile.SourcePath);
			photo.PhotoFile.Sha1Hash = await GetFileHashFormatted(fileInfo, sha);
		}
		_consoleWriter.ProgressFinish(progressName);
		return photos;
	}

	public void CreateOutputFolderIfNotExists(string outputFolder)
	{
		_fileSystem.Directory.CreateDirectory(outputFolder);
	}

	public void DeletePhotoSources(IReadOnlyCollection<Photo> photos, bool isDryRun = false)
	{
		const string fileProgressName = "Deleting source files";
		_consoleWriter.ProgressStart(fileProgressName);
		var directories = new HashSet<string>();
		foreach (var photo in photos)
		{
			var photoFileInfo = _fileSystem.FileInfo.New(photo.PhotoFile.SourceFullPath);
			if (photoFileInfo.Directory == null)
				throw new PhotoCliException("directory should be available");

			if (photo.CompanionFiles != null)
			{
				foreach (var companionFile in photo.CompanionFiles)
				{
					var companionFileInfo = _fileSystem.FileInfo.New(companionFile.SourceFullPath);
					companionFileInfo.Delete();
					++_statistics.SourceCompanionFileDeleted;
				}
			}
			directories.Add(photoFileInfo.Directory.FullName);
			photoFileInfo.Delete();
			++_statistics.SourcePhotoFileDeleted;
		}

		_consoleWriter.ProgressFinish(fileProgressName);

		const string directoryProgressName = "Deleting empty directories";
		_consoleWriter.ProgressStart(directoryProgressName);
		var sortedDirectoriesByDeepestToTop = directories.OrderByDescending(o => o.Count(c => c == PathHelper.PathSeparator())).ToList();
		foreach (var directory in sortedDirectoriesByDeepestToTop)
		{
			var directoryInfo = _fileSystem.DirectoryInfo.New(directory);
			var anyFile = directoryInfo.EnumerateFiles().Any();
			var anyDirectory = directoryInfo.EnumerateDirectories().Any();
			if (!anyFile && !anyDirectory)
			{
				directoryInfo.Delete();
				++_statistics.SourceEmptyDirectoryDeleted;
			}
		}
		_consoleWriter.ProgressFinish(directoryProgressName);
	}

	public void CreateParentFolderIfNotExist(string filePath)
	{
		var fileInfo = _fileSystem.FileInfo.New(filePath);
		if (fileInfo.Directory is { Exists: false })
		{
			_logger.LogDebug("Directory is creating: {Path}", fileInfo.Directory.FullName);
			fileInfo.Directory.Create();
			_logger.LogDebug("Directory created: {Path} ", fileInfo.Directory.FullName);
		}
	}

	private string GnuHashFileTreeFormat(IEnumerable<Photo> photos)
	{
		var content = new StringBuilder();
		foreach (var photo in photos)
		{
			content.Append($"{photo.PhotoFile.Sha1Hash}  {photo.PhotoFile.TargetRelativePath}{Environment.NewLine}");
			if (photo.CompanionFiles != null)
			{
				foreach (var companionFile in photo.CompanionFiles)
					content.Append($"{companionFile.Sha1Hash}  {companionFile.TargetRelativePath}{Environment.NewLine}");
			}
		}
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
