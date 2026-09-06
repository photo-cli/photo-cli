namespace PhotoCli.Options;

public class ToolOptions
{
	internal const string YearFormatDefault = "yyyy";
	internal const string MonthFormatDefault = "MM";
	internal const string DayFormatDefault = "dd";
	internal const string DateFormatWithMonthDefault = "yyyy.MM";
	internal const string DateFormatWithDayDefault = "yyyy.MM.dd";
	internal const string DateTimeFormatWithMinutesDefault = "yyyy.MM.dd_HH.mm";
	internal const string DateTimeFormatWithSecondsDefault = "yyyy.MM.dd_HH.mm.ss";
	internal const string AddressSeparatorDefault = "-";
	internal const string FolderAppendSeparatorDefault = "-";
	internal const string DayRangeSeparatorDefault = "-";
	internal const string SameNameNumberSeparatorDefault = "-";
	internal const string PhotoFormatInvalidFolderNameDefault = "invalid-photo-format";
	internal const string NoPhotoTakenDateFolderNameDefault = "no-photo-taken-date";
	internal const string NoAddressFolderNameDefault = "no-address";
	internal const string NoAddressAndPhotoTakenDateFolderNameDefault = "no-address-and-no-photo-taken-date";
	internal const string PhotoOrganizerReportCsvDefault = "photo-cli-report.csv";
	internal const string DryRunCsvReportFileNameDefault = "photo-cli-dry-run.csv";
	internal const string ArchivePhotoTakenDateHashSeparatorDefault = "-";
	internal const byte ConnectionLimitDefault = 4;
	internal const byte CoordinatePrecisionDefault = 4;
	internal static readonly string[] SupportedExtensionsDefault = ["jpg", "jpeg", "heic", "png"];
	public static readonly string[] CompanionExtensionsDefault = ["mov"];
	internal const bool LogCategoryNameOutputDefault = false;
	internal const string MacOsCommandDefault = "open";
	internal const string MacOsArgumentPrefixDefault = "-a Preview";
	internal static readonly Dictionary<string, string> LogLevelDefault = new()
	{
		{ "Default", nameof(Microsoft.Extensions.Logging.LogLevel.Error) },
		{ "PhotoCli", nameof(Microsoft.Extensions.Logging.LogLevel.Warning) },
		{ "PhotoCli.Services.Implementations.ReverseGeocodes", nameof(Microsoft.Extensions.Logging.LogLevel.Warning) },
		{ "Polly", nameof(Microsoft.Extensions.Logging.LogLevel.Warning) },
		{ "Microsoft", nameof(Microsoft.Extensions.Logging.LogLevel.Warning) },
		{ "System.Net.Http.HttpClient", nameof(Microsoft.Extensions.Logging.LogLevel.Warning) },
	};

	public ToolOptions(ToolOptionsRaw options)
	{
		YearFormat = options.YearFormat ?? YearFormatDefault;
		MonthFormat = options.MonthFormat ?? MonthFormatDefault;
		DayFormat = options.DayFormat ?? DayFormatDefault;
		DateFormatWithMonth = options.DateFormatWithMonth ?? DateFormatWithMonthDefault;
		DateFormatWithDay = options.DateFormatWithDay ?? DateFormatWithDayDefault;
		DateTimeFormatWithMinutes = options.DateTimeFormatWithMinutes ?? DateTimeFormatWithMinutesDefault;
		DateTimeFormatWithSeconds = options.DateTimeFormatWithSeconds ?? DateTimeFormatWithSecondsDefault;
		AddressSeparator = options.AddressSeparator ?? AddressSeparatorDefault;
		FolderAppendSeparator = options.FolderAppendSeparator ?? FolderAppendSeparatorDefault;
		DayRangeSeparator = options.DayRangeSeparator ?? DayRangeSeparatorDefault;
		SameNameNumberSeparator = options.SameNameNumberSeparator ?? SameNameNumberSeparatorDefault;
		PhotoFormatInvalidFolderName = options.PhotoFormatInvalidFolderName ?? PhotoFormatInvalidFolderNameDefault;
		NoPhotoTakenDateFolderName = options.NoPhotoTakenDateFolderName ?? NoPhotoTakenDateFolderNameDefault;
		NoAddressFolderName = options.NoAddressFolderName ?? NoAddressFolderNameDefault;
		NoAddressAndPhotoTakenDateFolderName = options.NoAddressAndPhotoTakenDateFolderName ?? NoAddressAndPhotoTakenDateFolderNameDefault;
		CsvReportFileName = options.CsvReportFileName ?? PhotoOrganizerReportCsvDefault;
		DryRunCsvReportFileName = options.DryRunCsvReportFileName ?? DryRunCsvReportFileNameDefault;
		ConnectionLimit = options.ConnectionLimit ?? ConnectionLimitDefault;
		BigDataCloudApiKey = options.BigDataCloudApiKey;
		GoogleMapsApiKey = options.GoogleMapsApiKey;
		LocationIqApiKey = options.LocationIqApiKey;
		ArchivePath = options.ArchivePath;
		ArchiveDatabasePath = options.ArchiveDatabasePath;
		ArchivePhotoTakenDateHashSeparator = options.ArchivePhotoTakenDateHashSeparator ?? ArchivePhotoTakenDateHashSeparatorDefault;
		CoordinatePrecision = options.CoordinatePrecision ?? CoordinatePrecisionDefault;
		SupportedExtensions = options.SupportedExtensions ?? SupportedExtensionsDefault;
		CompanionExtensions = options.CompanionExtensions ?? CompanionExtensionsDefault;
		LogCategoryNameOutput = options.LogCategoryNameOutput ?? LogCategoryNameOutputDefault;
		MacOsCommand = options.MacOsCommand ?? MacOsCommandDefault;
		MacOsArgumentPrefix = options.MacOsArgumentPrefix ?? MacOsArgumentPrefixDefault;
		LogLevel = options.LogLevel ?? LogLevelDefault;
	}

	public Dictionary<string, string>? LogLevel { get; set; }
	public string YearFormat { get; set; }
	public string MonthFormat { get; set; }
	public string DayFormat { get; set; }

	public string DateFormatWithMonth { get; set; }
	public string DateFormatWithDay { get; set; }
	public string DateTimeFormatWithMinutes { get; set; }
	public string DateTimeFormatWithSeconds { get; set; }
	public string AddressSeparator { get; set; }
	public string FolderAppendSeparator { get; set; }
	public string DayRangeSeparator { get; set; }
	public string SameNameNumberSeparator { get; set; }
	public string PhotoFormatInvalidFolderName { get; set; }
	public string NoPhotoTakenDateFolderName { get; set; }
	public string NoAddressFolderName { get; set; }
	public string NoAddressAndPhotoTakenDateFolderName { get; set; }

	public string CsvReportFileName { get; set; }
	public string DryRunCsvReportFileName { get; set; }

	public byte ConnectionLimit { get; set; }
	public string? BigDataCloudApiKey { get; set; }
	public string? GoogleMapsApiKey { get; set; }
	public string? LocationIqApiKey { get; set; }
	public string? ArchivePath { get; set; }
	public string? ArchiveDatabasePath { get; set; }
	public byte CoordinatePrecision { get; set; }
	public string ArchivePhotoTakenDateHashSeparator { get; set; }
	public string[] SupportedExtensions { get; set; }
	public string[] CompanionExtensions { get; set; }
	public bool LogCategoryNameOutput { get; set; }
	public string MacOsCommand { get; set; }
	public string MacOsArgumentPrefix { get; set; }

	public static ToolOptions Default()
	{
		return new ToolOptions(new ToolOptionsRaw());
	}
}
