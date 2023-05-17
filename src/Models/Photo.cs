using System.IO.Abstractions;

namespace PhotoCli.Models;

public class Photo
{
	public Photo(IFileSystemInfo fileInfo, ExifData photoExifData, string targetRelativeDirectoryPath)
	{
		FilePath = fileInfo.FullName;
		if (!fileInfo.Extension.StartsWith("."))
			throw new PhotoCliException("File extension should start with `.`");
		FileNameWithoutExtension = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
		Extension = fileInfo.Extension.Remove(0, 1).ToLowerInvariant();
		PhotoExifData = photoExifData;
		TargetRelativeDirectoryPath = targetRelativeDirectoryPath;
	}

	public string FileNameWithoutExtension { get; }
	public string Extension { get; }

	public string FilePath { get; }

	public ExifData PhotoExifData { get; }
	public DateTime? PhotoTakenDateTime => PhotoExifData.TakenDate;
	public string? ReverseGeocodeFormatted => PhotoExifData.ReverseGeocodeFormatted;

	public string TargetRelativeDirectoryPath { get; set; }

	public string? NewName { get; set; }

	public bool HasPhotoTakenDateTime => PhotoTakenDateTime.HasValue;
	public bool HasCoordinate => PhotoExifData.Coordinate != null;
	public bool HasReverseGeocode => PhotoExifData.ReverseGeocodes != null && PhotoExifData.ReverseGeocodes.Any();
	public List<string>? ReverseGeocodes => PhotoExifData.ReverseGeocodes?.ToList() ?? null;

	public int ReverseGeocodeCount => PhotoExifData.ReverseGeocodes?.Count() ?? 0;
	private string GetFileNameForOutput => NewName ?? FileNameWithoutExtension;

	public string Sha1Hash { get; set; }

	public string DestinationPath(string outputFolder)
	{
		return Path.Combine(outputFolder, RelativePath());
	}

	public string RelativePath()
	{
		return Path.Combine(TargetRelativeDirectoryPath, $"{GetFileNameForOutput}.{Extension}");
	}
}
