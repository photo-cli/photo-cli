namespace PhotoCli.Models;

public record PhotoEntity(string Path, DateTime CreatedAt, DateTime? DateTaken = null,
	string? ReverseGeocodeFormatted = "", double? Latitude = null, double? Longitude = null,
	int? Year = null, int? Month = null, int? Day = null, int? Hour = null, int? Minute = null, int? Seconds = null,
	string? Address1 = "", string? Address2 = "", string? Address3 = "", string? Address4 = "", string? Address5 = "", string? Address6 = "", string? Address7 = "", string? Address8 = "",
	string? Sha1Hash = "")
{
	public long Id { get; }
}
