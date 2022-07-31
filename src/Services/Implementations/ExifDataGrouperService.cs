namespace PhotoCli.Services.Implementations;

public class ExifDataGrouperService : IExifDataGrouperService
{
	private readonly ToolOptions _options;
	private readonly ILogger<ExifDataGrouperService> _logger;

	public ExifDataGrouperService(ToolOptions options, ILogger<ExifDataGrouperService> logger)
	{
		_options = options;
		_logger = logger;
	}

	public Dictionary<string, List<Photo>> Group(IEnumerable<Photo> photoInfos, NamingStyle namingStyle)
	{
		if (namingStyle is NamingStyle.Numeric)
			throw new PhotoCliException($"{nameof(NamingStyle)} can't be {namingStyle}");
		Dictionary<string, List<Photo>> photosGrouped;

		switch (namingStyle)
		{
			case NamingStyle.Day:
				photosGrouped = Filter(photoInfos, true, false).GroupBy(g => g.PhotoTakenDateTime!.Value.Date.ToString(_options.DateFormatWithDay))
					.ToDictionary(k => k.Key, grouping => grouping.ToList());
				break;
			case NamingStyle.DateTimeWithMinutes:
				photosGrouped = Filter(photoInfos, true, false).GroupBy(g => new
				{
					g.PhotoTakenDateTime!.Value.Date,
					g.PhotoTakenDateTime!.Value.Hour,
					g.PhotoTakenDateTime!.Value.Minute
				}).ToDictionary(k =>
				{
					var dateTime = new DateTime(k.Key.Date.Year, k.Key.Date.Month, k.Key.Date.Day, k.Key.Hour, k.Key.Minute, 0);
					return dateTime.ToString(_options.DateTimeFormatWithMinutes);
				}, grouping => grouping.ToList());
				break;
			case NamingStyle.DateTimeWithSeconds:
				photosGrouped = Filter(photoInfos, true, false).GroupBy(g => new
				{
					g.PhotoTakenDateTime!.Value.Date,
					g.PhotoTakenDateTime!.Value.Hour,
					g.PhotoTakenDateTime!.Value.Minute,
					g.PhotoTakenDateTime!.Value.Second,
				}).ToDictionary(k =>
				{
					var dateTime = new DateTime(k.Key.Date.Year, k.Key.Date.Month, k.Key.Date.Day, k.Key.Hour, k.Key.Minute, k.Key.Second);
					return dateTime.ToString(_options.DateTimeFormatWithSeconds);
				}, grouping => grouping.ToList());
				break;
			case NamingStyle.Address:
				photosGrouped = Filter(photoInfos, false, true).GroupBy(g => g.ReverseGeocodeFormatted)
					.ToDictionary(k => k.Key!, grouping => grouping.ToList());
				break;
			case NamingStyle.DayAddress or NamingStyle.AddressDay:
				photosGrouped = Filter(photoInfos, true, true).GroupBy(g => new
				{
					ReverseGeocode = g.ReverseGeocodeFormatted,
					g.PhotoTakenDateTime!.Value.Date
				}).ToDictionary(k =>
				{
					var dateTime = new DateTime(k.Key.Date.Year, k.Key.Date.Month, k.Key.Date.Day);
					var dateTimeFormat = dateTime.ToString(_options.DateFormatWithDay);
					return FormatOrderAddressAndDateTime(namingStyle is NamingStyle.DayAddress, k.Key.ReverseGeocode!, dateTimeFormat);
				}, grouping => grouping.ToList());
				break;
			case NamingStyle.DateTimeWithMinutesAddress or NamingStyle.AddressDateTimeWithMinutes:
				photosGrouped = Filter(photoInfos, true, true).GroupBy(g => new
				{
					ReverseGeocode = g.ReverseGeocodeFormatted,
					g.PhotoTakenDateTime!.Value.Date,
					g.PhotoTakenDateTime!.Value.Hour,
					g.PhotoTakenDateTime!.Value.Minute,
				}).ToDictionary(k =>
				{
					var dateTime = new DateTime(k.Key.Date.Year, k.Key.Date.Month, k.Key.Date.Day, k.Key.Hour, k.Key.Minute, 0);
					var dateTimeFormat = dateTime.ToString(_options.DateTimeFormatWithMinutes);
					return FormatOrderAddressAndDateTime(namingStyle is NamingStyle.DateTimeWithMinutesAddress, k.Key.ReverseGeocode!, dateTimeFormat);
				}, grouping => grouping.ToList());
				break;
			case NamingStyle.DateTimeWithSecondsAddress or NamingStyle.AddressDateTimeWithSeconds:
				photosGrouped = Filter(photoInfos, true, true).GroupBy(g => new
				{
					ReverseGeocode = g.ReverseGeocodeFormatted,
					g.PhotoTakenDateTime!.Value.Date,
					g.PhotoTakenDateTime!.Value.Hour,
					g.PhotoTakenDateTime!.Value.Minute,
					g.PhotoTakenDateTime!.Value.Second,
				}).ToDictionary(k =>
				{
					var dateTime = new DateTime(k.Key.Date.Year, k.Key.Date.Month, k.Key.Date.Day, k.Key.Hour, k.Key.Minute, k.Key.Second);
					var dateTimeFormat = dateTime.ToString(_options.DateTimeFormatWithSeconds);
					return FormatOrderAddressAndDateTime(namingStyle is NamingStyle.DateTimeWithSecondsAddress, k.Key.ReverseGeocode!, dateTimeFormat);
				}, grouping => grouping.ToList());
				break;
			default:
				throw new PhotoCliException($"Not implemented {nameof(NamingStyle)}: {namingStyle}");
		}

		_logger.LogInformation("Grouped photo exif data into {ExifGroupCount}", photosGrouped.Count);
		return photosGrouped;
	}

	private string FormatOrderAddressAndDateTime(bool isDateBeforeAddress, string address, string dateTimeFormat)
	{
		return isDateBeforeAddress ? $"{dateTimeFormat}-{address}" : $"{address}-{dateTimeFormat}";
	}

	private static IEnumerable<Photo> Filter(IEnumerable<Photo> photoInfos, bool filterPhotoTakenDate, bool filterReverseGeocode)
	{
		if (filterPhotoTakenDate && filterReverseGeocode)
			return photoInfos.Where(w => w.HasPhotoTakenDateTime && w.HasReverseGeocode).ToList();
		if (filterPhotoTakenDate)
			return photoInfos.Where(w => w.HasPhotoTakenDateTime).ToList();
		if (filterReverseGeocode)
			return photoInfos.Where(w => w.HasReverseGeocode).ToList();
		throw new PhotoCliException($"One of this {nameof(filterPhotoTakenDate)} or {nameof(filterReverseGeocode)} should be true");
	}
}
