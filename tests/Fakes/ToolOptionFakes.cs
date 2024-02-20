namespace PhotoCli.Tests.Fakes;

public static class ToolOptionFakes
{
	public const string CsvReportFileName = "photo-cli-report.csv";
	public const string DryRunCsvReportFileName = "photo-cli-dry-run.csv";

	public const string YearFormat = "yyyy";
	public const string MonthFormat = "MM";
	public const string DayFormat = "dd";

	public const string DateFormatWithMonth = "yyyy.MM";
	public const string DateFormatWithDay = "yyyy.MM.dd";
	public const string DateTimeFormatWithMinutes = "yyyy.MM.dd_HH.mm";
	public const string DateTimeFormatWithSecondsDefault = "yyyy.MM.dd_HH.mm.ss";

	public const string AddressSeparator = "-";
	public const string FolderAppendSeparator = "-";
	public const string DayRangeSeparator = "-";
	public const string SameNameNumberSeparator = "-";

	public const string PhotoFormatInvalidFolderName = "invalid-photo-format";
	public const string NoPhotoTakenDateFolderName = "no-photo-taken-date";
	public const string NoAddressFolderName = "no-address";
	public const string NoAddressAndPhotoTakenDateFolderName = "no-address-and-no-photo-taken-date";
	public const string ArchivePhotoTakenDateHashSeparator = "-";

	public static ToolOptions Create()
	{
		return new ToolOptions(new ToolOptionsRaw
		{
			YearFormat = YearFormat,
			MonthFormat = MonthFormat,
			DayFormat = DayFormat,
			DateFormatWithMonth = DateFormatWithMonth,
			DateFormatWithDay = DateFormatWithDay,
			DateTimeFormatWithMinutes = DateTimeFormatWithMinutes,
			DateTimeFormatWithSeconds = DateTimeFormatWithSecondsDefault,

			CsvReportFileName = CsvReportFileName,
			DryRunCsvReportFileName = DryRunCsvReportFileName,

			AddressSeparator = AddressSeparator,
			FolderAppendSeparator = FolderAppendSeparator,
			DayRangeSeparator = DayRangeSeparator,
			SameNameNumberSeparator = SameNameNumberSeparator,

			PhotoFormatInvalidFolderName = PhotoFormatInvalidFolderName,
			NoPhotoTakenDateFolderName = NoPhotoTakenDateFolderName,
			NoAddressFolderName = NoAddressFolderName,
			NoAddressAndPhotoTakenDateFolderName = NoAddressAndPhotoTakenDateFolderName,

			ArchivePhotoTakenDateHashSeparator = ArchivePhotoTakenDateHashSeparator
		});
	}
}
