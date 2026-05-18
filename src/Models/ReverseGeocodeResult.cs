namespace PhotoCli.Models;

public record ReverseGeocodeResult(IReadOnlyCollection<Photo> Photos, bool AllPhotosHasReverseGeocodedAsRequested);
