namespace PhotoCli;

public record PhotoNearLocationResult(string Path, DateTime? DateTaken, string? ReverseGeocodeFormatted, double DistanceKm);
