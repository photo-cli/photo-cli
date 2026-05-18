using System.ComponentModel;
using ModelContextProtocol.Server;

namespace PhotoCli.McpTools;

[McpServerToolType]
public class ArchiveMcpTools(IDbService dbService, IProcessLauncher processLauncher, McpOptions mcpOptions)
{
	[McpServerTool(Name = "search_photos")]
	[Description("Search photos in the archive by date range, location text, or limit results. Returns photo paths, dates, and location info.")]
	public async Task<string> SearchPhotos(
		[Description("Start date filter in ISO 8601 format (e.g. 2023-01-01). Optional.")] string? startDate = null,
		[Description("End date filter in ISO 8601 format (e.g. 2023-12-31). Optional.")] string? endDate = null,
		[Description("Text to search in the reverse geocode address (e.g. 'Paris', 'France'). Case-insensitive. Optional.")] string? location = null,
		[Description("Maximum number of results to return. Default is 50.")] int limit = 50)
	{
		DateTime? start = !string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out var s) ? s : null;
		DateTime? end = !string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out var e) ? e : null;

		var photos = await dbService.SearchPhotos(start, end, location, limit);
		var result = photos.Select(p => new { p.Path, p.DateTaken, p.ReverseGeocodeFormatted, p.Latitude, p.Longitude });
		return JsonSerializer.Serialize(result, StaticOptions.McpToolOptions);
	}

	[McpServerTool(Name = "get_photo")]
	[Description("Get full metadata for a specific photo by its file path.")]
	public async Task<string> GetPhoto(
		[Description("The relative file path of the photo as stored in the archive (e.g. '2023/06/15/abc123.jpg').")] string filePath)
	{
		var photo = await dbService.GetPhotoByPath(filePath);

		if (photo == null)
			return $"No photo found with path: {filePath}";

		var result = new
		{
			photo.Id,
			photo.Path,
			photo.DateTaken,
			photo.Year,
			photo.Month,
			photo.Day,
			photo.Hour,
			photo.Minute,
			photo.Seconds,
			photo.Latitude,
			photo.Longitude,
			photo.ReverseGeocodeFormatted,
			photo.Address1,
			photo.Address2,
			photo.Address3,
			photo.Address4,
			photo.Address5,
			photo.Address6,
			photo.Address7,
			photo.Address8,
			photo.Sha1Hash,
			photo.CreatedAt,
		};
		return JsonSerializer.Serialize(result, StaticOptions.McpToolOptions);
	}

	[McpServerTool(Name = "list_albums")]
	[Description("List all albums in the archive with their id, name, type, creation date, and configuration.")]
	public async Task<string> ListAlbums()
	{
		var albums = await dbService.GetAllAlbums();
		var result = albums.Select(a => new { a.Id, a.Name, Type = a.Type.ToString(), a.CreatedAt, a.Configuration });
		return JsonSerializer.Serialize(result, StaticOptions.McpToolOptions);
	}

	[McpServerTool(Name = "get_statistics")]
	[Description("Get photo count statistics grouped by year, month, country (address1), city (address2), or camera model (address3).")]
	public async Task<string> GetStatistics(
		[Description("How to group the statistics. One of: year, month, location, address1, address2, address3, address4. Default is year.")] string groupBy = "year")
	{
		var rows = await dbService.GetPhotoStatistics(groupBy);
		if (rows.Count == 0 && !IsKnownGroupBy(groupBy))
			return $"Unknown groupBy value '{groupBy}'. Use: year, month, location, address1, address2, address3, address4";
		return JsonSerializer.Serialize(rows, StaticOptions.McpToolOptions);
	}

	[McpServerTool(Name = "find_near_location")]
	[Description("Find photos taken near a GPS coordinate within a given radius. Uses Haversine distance formula.")]
	public async Task<string> FindNearLocation(
		[Description("Latitude of the center point (decimal degrees, e.g. 48.8566).")] double latitude,
		[Description("Longitude of the center point (decimal degrees, e.g. 2.3522).")] double longitude,
		[Description("Search radius in kilometers. Default is 10.")] double radiusKm = 10,
		[Description("Maximum number of results to return. Default is 20.")] int limit = 20)
	{
		var results = await dbService.FindPhotosNearLocation(latitude, longitude, radiusKm, limit);
		return JsonSerializer.Serialize(results, StaticOptions.McpToolOptions);
	}

	[McpServerTool(Name = "list_photos_by_album_id")]
	[Description("List photos belonging to an album by its numeric ID. Returns photo paths, dates, and location info.")]
	public async Task<string> ListPhotosByAlbumId(
		[Description("The numeric ID of the album.")] int albumId)
	{
		var albumPhotoResult = await dbService.GetAlbumPhotosById(albumId);
		return albumPhotoResult.Status switch
		{
			AlbumPhotoResultStatus.Successful => SerializePhotos(albumPhotoResult.Photos),
			AlbumPhotoResultStatus.AlbumNotFound => $"Album with id {albumId} not found.",
			AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat => $"Album with id {albumId} has invalid configuration.",
			_ => $"Unexpected error for album id {albumId}."
		};
	}

	[McpServerTool(Name = "list_photos_by_album_name")]
	[Description("List photos belonging to an album by its name. Returns photo paths, dates, and location info.")]
	public async Task<string> ListPhotosByAlbumName(
		[Description("The name of the album.")] string albumName)
	{
		var albumPhotoResult = await dbService.GetAlbumPhotosByName(albumName);
		return albumPhotoResult.Status switch
		{
			AlbumPhotoResultStatus.Successful => SerializePhotos(albumPhotoResult.Photos),
			AlbumPhotoResultStatus.AlbumNotFound => $"Album with name '{albumName}' not found.",
			AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat => $"Album with name '{albumName}' has invalid configuration.",
			_ => $"Unexpected error for album name '{albumName}'."
		};
	}

	[McpServerTool(Name = "list_photos_by_exact_date")]
	[Description("List photos matching an exact date (year, month, day). All parameters are optional for broader matching.")]
	public async Task<string> ListPhotosByExactDate(
		[Description("The year to filter by (e.g. 2023). Optional.")] int? year = null,
		[Description("The month to filter by (1-12). Optional.")] byte? month = null,
		[Description("The day to filter by (1-31). Optional.")] byte? day = null)
	{
		var photos = await dbService.GetPhotosByDate(year, month, day);
		return SerializePhotos(photos);
	}

	[McpServerTool(Name = "list_photos_by_date_range")]
	[Description("List photo0s within a date range. Both start and end dates are inclusive.")]
	public async Task<string> ListPhotosByDateRange(
		[Description("Start date in ISO 8601 format (e.g. 2023-01-01). Optional.")] string? startDate = null,
		[Description("End date in ISO 8601 format (e.g. 2023-12-31). Optional.")] string? endDate = null)
	{
		DateTime? start = !string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out var s) ? s : null;
		DateTime? end = !string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out var e) ? e : null;

		var photos = await dbService.GetPhotosByDateRange(start, end);
		return SerializePhotos(photos);
	}

	[McpServerTool(Name = "open_photos_by_album_id")]
	[Description("Open photos belonging to an album by its numeric ID in the default viewer (macOS Preview).")]
	public async Task<string> OpenPhotosByAlbumId(
		[Description("The numeric ID of the album.")] int albumId)
	{
		var albumPhotoResult = await dbService.GetAlbumPhotosById(albumId);
		return albumPhotoResult.Status switch
		{
			AlbumPhotoResultStatus.Successful => await OpenPhotos(albumPhotoResult.Photos),
			AlbumPhotoResultStatus.AlbumNotFound => $"Album with id {albumId} not found.",
			AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat => $"Album with id {albumId} has invalid configuration.",
			_ => $"Unexpected error for album id {albumId}."
		};
	}

	[McpServerTool(Name = "open_photos_by_album_name")]
	[Description("Open photos belonging to an album by its name in the default viewer (macOS Preview).")]
	public async Task<string> OpenPhotosByAlbumName(
		[Description("The name of the album.")] string albumName)
	{
		var albumPhotoResult = await dbService.GetAlbumPhotosByName(albumName);
		return albumPhotoResult.Status switch
		{
			AlbumPhotoResultStatus.Successful => await OpenPhotos(albumPhotoResult.Photos),
			AlbumPhotoResultStatus.AlbumNotFound => $"Album with name '{albumName}' not found.",
			AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat => $"Album with name '{albumName}' has invalid configuration.",
			_ => $"Unexpected error for album name '{albumName}'."
		};
	}

	[McpServerTool(Name = "open_photos_by_exact_date")]
	[Description("Open photos matching an exact date (year, month, day) in the default viewer (macOS Preview). All parameters are optional for broader matching.")]
	public async Task<string> OpenPhotosByExactDate(
		[Description("The year to filter by (e.g. 2023). Optional.")] int? year = null,
		[Description("The month to filter by (1-12). Optional.")] byte? month = null,
		[Description("The day to filter by (1-31). Optional.")] byte? day = null)
	{
		var photos = await dbService.GetPhotosByDate(year, month, day);
		return await OpenPhotos(photos);
	}

	[McpServerTool(Name = "open_photos_by_date_range")]
	[Description("Open photos within a date range in the default viewer (macOS Preview). Both start and end dates are inclusive.")]
	public async Task<string> OpenPhotosByDateRange(
		[Description("Start date in ISO 8601 format (e.g. 2023-01-01). Optional.")] string? startDate = null,
		[Description("End date in ISO 8601 format (e.g. 2023-12-31). Optional.")] string? endDate = null)
	{
		DateTime? start = !string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out var s) ? s : null;
		DateTime? end = !string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out var e) ? e : null;

		var photos = await dbService.GetPhotosByDateRange(start, end);
		return await OpenPhotos(photos);
	}

	private async Task<string> OpenPhotos(List<PhotoEntity> photos)
	{
		if (photos.Count == 0)
			return "No photos found to open.";

		var photoPaths = photos.Select(p => Path.Combine(mcpOptions.ArchivePath, p.Path)).ToList();
		await processLauncher.Launch(photoPaths);
		return $"Opened {photos.Count} photo(s) in the default viewer.";
	}

	private static string SerializePhotos(List<PhotoEntity> photos)
	{
		var result = photos.Select(p => new { p.Path, p.DateTaken, p.ReverseGeocodeFormatted, p.Latitude, p.Longitude });
		return JsonSerializer.Serialize(result, StaticOptions.McpToolOptions);
	}

	private static bool IsKnownGroupBy(string groupBy) =>
		groupBy.Trim().ToLowerInvariant() is "year" or "month" or "location" or "address1" or "address2" or "address3" or "address4";
}
