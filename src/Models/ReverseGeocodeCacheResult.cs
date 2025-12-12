namespace PhotoCli.Models;

public record ReverseGeocodeCacheResult<T>(bool CacheHit, T? Result);
