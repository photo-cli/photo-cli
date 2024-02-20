namespace PhotoCli.Models.ReverseGeocode;

public record ReverseGeocodeRequest(Coordinate Coordinate, string? Language = null);
