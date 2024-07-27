using System.IO.Abstractions;

namespace PhotoCli.Models;

public record Photo
{
	public Photo(IFileInfo photoFile, IFileInfo[]? companionFiles = null)
	{
		PhotoFile = new PhotoFile(photoFile);
		if (companionFiles != null)
			CompanionFiles = companionFiles.Select(companionFile => new PhotoFile(companionFile)).ToArray();
	}

	#region File

	public PhotoFile PhotoFile { get; init; }
	public IReadOnlyCollection<PhotoFile>? CompanionFiles { get; init; }

	public string? NewName { get; private set; }

	public string? TargetRelativePath { get; private set; }

	#endregion

	#region Exif - Metadata

	public ExifData? ExifData { get; private set; }
	public bool HasExifData => ExifData != null;

	#region Exif - Photo Taken Date

	public DateTime? TakenDateTime => ExifData?.TakenDate;
	public bool HasTakenDateTime => TakenDateTime.HasValue;

	#endregion

	#region Exif - Coordinate - Reverse Geocode - Address

	public Coordinate? Coordinate => ExifData?.Coordinate;
	public bool HasCoordinate => Coordinate != null;
	public List<string>? ReverseGeocodes => ExifData?.ReverseGeocodes?.ToList() ?? null;
	public bool HasReverseGeocode => ExifData?.ReverseGeocodes != null && ExifData.ReverseGeocodes.Any();
	public int ReverseGeocodeCount => ExifData?.ReverseGeocodes?.Count() ?? 0;
	public string? ReverseGeocodeFormatted => ExifData?.ReverseGeocodeFormatted;

	#endregion

	#endregion

	public void SetExifData(ExifData exifData)
	{
		ExifData = exifData;
	}

	public void SetNewName(string newName)
	{
		NewName = newName;
	}

	public void SetTargetRelativePath(string targetRelativePath)
	{
		TargetRelativePath = targetRelativePath;
	}

	public void SetTarget(string outputFolder)
	{
		if (TargetRelativePath == null)
			throw new PhotoCliException($"Can't {nameof(SetTarget)} before setting {nameof(TargetRelativePath)}");

		PhotoFile.SetTarget(TargetRelativePath, outputFolder, NewName);

		if (CompanionFiles != null)
		{
			foreach (var companionFile in CompanionFiles)
				companionFile.SetTarget(TargetRelativePath, outputFolder, NewName);
		}
	}
}
