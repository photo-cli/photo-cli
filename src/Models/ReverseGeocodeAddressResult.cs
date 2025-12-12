namespace PhotoCli.Models;

public record ReverseGeocodeAddressResult(IEnumerable<string> AddressList, bool AllPhotosHasReverseGeocodedAsRequested);
