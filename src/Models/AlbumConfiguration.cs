namespace PhotoCli.Models;

public record AlbumConfiguration(List<long>? PhotoIds = null, AlbumDateRange? DateRange = null, AlbumReverseGeocode? ReverseGeocode = null);
