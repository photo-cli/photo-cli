namespace PhotoCli.Utils;

public static class DateTimeHelper
{
	public static DateTime GetEarliestDateTime(params DateTime[] dateTimes)
	{
		return dateTimes.Min();
	}

	public static DateTime GetLatestDateTime(params DateTime[] dateTimes)
	{
		return dateTimes.Max();
	}
}
