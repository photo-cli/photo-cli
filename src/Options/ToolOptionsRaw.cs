namespace PhotoCli.Options;

public class ToolOptionsRaw
{
	public LogLevel? LogLevel { get; set; }
	public string? YearFormat { get; set; }
	public string? MonthFormat { get; set; }
	public string? DayFormat { get; set; }

	public string? DateFormatWithMonth { get; set; }
	public string? DateFormatWithDay { get; set; }
	public string? DateTimeFormatWithMinutes { get; set; }
	public string? DateTimeFormatWithSeconds { get; set; }
	public string? AddressSeparator { get; set; }
	public string? FolderAppendSeparator { get; set; }
	public string? DayRangeSeparator { get; set; }
	public string? SameNameNumberSeparator { get; set; }

	public string? PhotoFormatInvalidFolderName { get; set; }
	public string? NoPhotoTakenDateFolderName { get; set; }
	public string? NoAddressFolderName { get; set; }
	public string? NoAddressAndPhotoTakenDateFolderName { get; set; }

	public string? CsvReportFileName { get; set; }
	public string? DryRunCsvReportFileName { get; set; }

	public int? ConnectionLimit { get; set; }

	public string? BigDataCloudApiKey { get; set; }
	public string? GoogleMapsApiKey { get; set; }
	public string? LocationIqApiKey { get; set; }
	public string? ArchivePhotoTakenDateHashSeparator { get; set; }
	public int? CoordinatePrecision { get; set; }
	public string[]? SupportedExtensions { get; set; }
	public string[]? CompanionExtensions { get; set; }
}
