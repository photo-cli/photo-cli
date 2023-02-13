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
	internal const string NoPhotoTakenDateFolderNameDefault = "no-photo-taken-date";
	internal const string NoAddressFolderNameDefault = "no-address";
	internal const string NoAddressAndPhotoTakenDateFolderNameDefault = "no-address-and-no-photo-taken-date";
	internal const string PhotoOrganizerReportCsvDefault = "photo-cli-report.csv";
	internal const string DryRunCsvReportFileNameDefault = "photo-cli-dry-run.csv";
	internal const int ConnectionLimitDefault = 4;

	public ToolOptions(ToolOptionsRaw options)
	{
		LogLevel = options.LogLevel ?? new LogLevel { Default = Microsoft.Extensions.Logging.LogLevel.Error.ToString() };
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
		NoPhotoTakenDateFolderName = options.NoPhotoTakenDateFolderName ?? NoPhotoTakenDateFolderNameDefault;
		NoAddressFolderName = options.NoAddressFolderName ?? NoAddressFolderNameDefault;
		NoAddressAndPhotoTakenDateFolderName = options.NoAddressAndPhotoTakenDateFolderName ?? NoAddressAndPhotoTakenDateFolderNameDefault;
		CsvReportFileName = options.CsvReportFileName ?? PhotoOrganizerReportCsvDefault;
		DryRunCsvReportFileName = options.DryRunCsvReportFileName ?? DryRunCsvReportFileNameDefault;
		ConnectionLimit = options.ConnectionLimit ?? ConnectionLimitDefault;
		BigDataCloudApiKey = options.BigDataCloudApiKey;
		GoogleMapsApiKey = options.GoogleMapsApiKey;
		LocationIqApiKey = options.LocationIqApiKey;
	}

	public LogLevel LogLevel { get; set; }
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

	public string NoPhotoTakenDateFolderName { get; set; }
	public string NoAddressFolderName { get; set; }
	public string NoAddressAndPhotoTakenDateFolderName { get; set; }

	public string CsvReportFileName { get; set; }
	public string DryRunCsvReportFileName { get; set; }

	public int ConnectionLimit { get; set; }
	public string? BigDataCloudApiKey { get; set; }
	public string? GoogleMapsApiKey { get; set; }
	public string? LocationIqApiKey { get; set; }

	public static ToolOptions Default()
	{
		return new ToolOptions(new ToolOptionsRaw());
	}
}
