using System.ComponentModel.DataAnnotations;

namespace PhotoCli.Models;

public record Statistics : DisplayableRecord
{
	[Display(Name = "Photo(s) found")]
	public int PhotosFound { get; set; }

	[Display(Name = "Photo(s) copied")]
	public int PhotosCopied { get; set; }

	[Display(Name = "Directory/directories created")]
	public int DirectoriesCreated { get; set; }

	[Display(Name = "Photo(s) has taken date and coordinate")]
	public int PhotoThatHasTakenDateAndCoordinate { get; set; }

	[Display(Name = "Photo(s) has taken date but no coordinate")]
	public int PhotoThatHasTakenDateButNoCoordinate { get; set; }

	[Display(Name = "Photo(s) has coordinate but no taken date")]
	public int PhotoThatHasCoordinateButNoTakenDate { get; set; }

	[Display(Name = "Photo(s) has no taken date and coordinate")]
	public int PhotoThatNoCoordinateAndNoTakenDate { get; set; }

	public int HasCoordinateCount => PhotoThatHasTakenDateAndCoordinate + PhotoThatHasCoordinateButNoTakenDate;

	public List<FileIoErrorInfo> FileIoErrors { get; } = new();

	[Display(Name = "Photo(s) has unknown/invalid format")]
	public int InvalidFormatError { get; set; }

	[Display(Name = "Photo(s) caused unexpected error internally")]
	public int InternalError { get; set; }

	[Display(Name = "Photo(s) existed on the output")]
	public int PhotosExisted { get; set; }

	[Display(Name = "Photo(s) are skipped, they have the same photo")]
	public int PhotosSame { get; set; }

	[Display(Name = "Companion file(s) found")]
	public int CompanionFilesFound { get; set; }

	[Display(Name = "Companion file(s) copied")]
	public int CompanionFilesCopied { get; set; }

	[Display(Name = "Companion file(s) existed on the output")]
	public int CompanionFilesExisted { get; set; }

	[Display(Name = "Reverse geocode request sent")]
	public int ReserveGeocodeRequestSent { get; set; }

	[Display(Name = "Reverse geocode evaluated from memory")]
	public int ReserveGeocodeFromMemory { get; set; }

	[Display(Name = "Reverse geocode evaluated from database")]
	public int ReserveGeocodeFromDatabase { get; set; }

	[Display(Name = "User defined album created")]
	public int UserDefinedAlbumCreated { get; set; }

	[Display(Name = "User defined album updated")]
	public int UserDefinedAlbumUpdated { get; set; }

	[Display(Name = "Auto address album created")]
	public int AutoAddressAlbumCreated { get; set; }

	[Display(Name = "Source photo file(s) deleted")]
	public int SourcePhotoFileDeleted { get; set; }

	[Display(Name = "Source companion file(s) deleted")]
	public int SourceCompanionFileDeleted { get; set; }

	[Display(Name = "Source empty directory(ies) deleted")]
	public int SourceEmptyDirectoryDeleted { get; set; }

}
