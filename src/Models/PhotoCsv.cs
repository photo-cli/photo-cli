namespace PhotoCli.Models;

public record PhotoCsv(string PhotoPath, string? PhotoNewPath = "", DateTime? PhotoDateTaken = null, string? ReverseGeocodeFormatted = "", double? Latitude = null, double? Longitude = null,
	int? PhotoTakenYear = null, int? PhotoTakenMonth = null, int? PhotoTakenDay = null, int? PhotoTakenHour = null, int? PhotoTakenMinute = null, int? PhotoTakenSeconds = null,
	string? Address1 = "", string? Address2 = "", string? Address3 = "", string? Address4 = "", string? Address5 = "", string? Address6 = "", string? Address7 = "", string? Address8 = "",
	string? Sha1Hash = "");
