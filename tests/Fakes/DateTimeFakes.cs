namespace PhotoCli.Tests.Fakes;

public static class DateTimeFakes
{
	private const int YearDefault = 2000;
	private const int MonthDefault = 1;
	private const int DayDefault = 1;
	private const int MinuteDefault = 0;
	private const int HourDefault = 0;
	private const int SecondDefault = 0;

	public static DateTime WithDayHour(int day, int hour)
	{
		return new DateTime(YearDefault, MonthDefault, day, hour, MinuteDefault, SecondDefault);
	}

	public static DateTime WithMinuteSecond(int minute, int second)
	{
		return new DateTime(YearDefault, MonthDefault, DayDefault, HourDefault, minute, second);
	}

	public static DateTime WithDay(int day)
	{
		return new DateTime(YearDefault, MonthDefault, day);
	}

	public static DateTime WithMonth(int month)
	{
		return new DateTime(YearDefault, month, DayDefault);
	}

	public static DateTime WithYear(int year)
	{
		return new DateTime(year, MonthDefault, DayDefault);
	}

	public static string FormatDay(int day)
	{
		var dateTime = WithDay(day);
		return dateTime.ToString(ToolOptionFakes.DateFormatWithDay);
	}

	public static string DirectoryFormatDay(int day)
	{
		var dateTime = WithDay(day);
		return Path.Combine(dateTime.ToString(ToolOptionFakes.YearFormat), dateTime.ToString(ToolOptionFakes.MonthFormat), dateTime.ToString(ToolOptionFakes.DayFormat));
	}

	public static string FormatMonth(int month)
	{
		var dateTime = WithMonth(month);
		return dateTime.ToString(ToolOptionFakes.DateFormatWithMonth);
	}

	public static string DirectoryFormatMonth(int month)
	{
		var dateTime = WithMonth(month);
		return Path.Combine(dateTime.ToString(ToolOptionFakes.YearFormat), dateTime.ToString(ToolOptionFakes.MonthFormat));
	}

	public static string FormatYear(int year)
	{
		var dateTime = WithYear(year);
		return dateTime.ToString(ToolOptionFakes.YearFormat);
	}

	public static DateTime WithMinute(int minute)
	{
		return new DateTime(YearDefault, MonthDefault, DayDefault, HourDefault, minute, SecondDefault);
	}

	public static string FormatMinute(int minute)
	{
		var dateTime = WithMinute(minute);
		return dateTime.ToString(ToolOptionFakes.DateTimeFormatWithMinutes);
	}

	public static DateTime WithSecond(int second)
	{
		return new DateTime(YearDefault, MonthDefault, DayDefault, HourDefault, MinuteDefault, second);
	}

	public static DateTime Valid()
	{
		return DateTime.MinValue;
	}

	public static string FormatSecond(int second)
	{
		var dateTime = WithSecond(second);
		return dateTime.ToString(ToolOptionFakes.DateTimeFormatWithSecondsDefault);
	}
}
