namespace PhotoCli.Models;

public record ExifDataResult(IReadOnlyCollection<Photo> Photos, bool AllPhotosAreValid, bool AllPhotosHasPhotoTaken, bool AllPhotosHasCoordinate, AlbumDateRange? DateRange);
