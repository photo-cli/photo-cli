namespace PhotoCli.Services.Contracts;

public interface IDbService
{
	Task<ArchiveResult> Archive(IEnumerable<Photo> photos, bool isDryRun = false);
	Task<AlbumResult> NewIndividualAlbum(string name, List<PhotoEntity> photos, bool isDryRun = false);
	Task<AlbumResult> UpdateIndividualAlbum(int albumId, List<PhotoEntity> photos, bool isDryRun = false);
	Task<AlbumResult> NewDateRangeAlbum(string name, AlbumDateRange dateRangeSession, List<PhotoEntity> photos, bool isDryRun = false);
	Task<AlbumResult> UpdateDateRangeAlbum(int albumId, AlbumDateRange dateRangeSession, List<PhotoEntity> photos, bool isDryRun = false);
	Task<AlbumResult> SaveReverseGeocodeAlbums(List<PhotoEntity> photos, bool isDryRun = false);
	Task<TResponse?> GetReverseGeocodeCache<TResponse>(ReverseGeocodeRequest request, ReverseGeocodeProvider provider);
	Task SaveReverseGeocodeCache<TResponse>(ReverseGeocodeRequest request, TResponse response, ReverseGeocodeProvider provider);
	Task<AlbumPhotoResult> GetAlbumPhotosById(int albumId);
	Task<List<PhotoEntity>> GetPhotosByDate(int? year, byte? month, byte? day);
	Task<AlbumEntity?> GetAlbumByName(string name);
	Task<AlbumEntity?> GetAlbumById(int albumId);
	Task<List<AlbumEntity>> GetAllAlbums();
	Task<int> TotalAlbumCount();
	Task<long> TotalPhotoCount();
	Task<long> TotalReverseGeocodeCacheCount();
	Task<List<PhotoEntity>> SearchPhotos(DateTime? start, DateTime? end, string? location, int limit);
	Task<PhotoEntity?> GetPhotoByPath(string filePath);
	Task<List<PhotoStatisticsRow>> GetPhotoStatistics(string groupBy);
	Task<List<PhotoNearLocationResult>> FindPhotosNearLocation(double latitude, double longitude, double radiusKm, int limit);
}
