namespace PhotoCli.Models.Enums;

public enum NamingStyle : byte
{
	Unset = 0,
	Numeric = 1,
	Day = 2,
	DateTimeWithMinutes = 3,
	DateTimeWithSeconds = 4,
	Address = 5,
	DayAddress = 6,
	DateTimeWithMinutesAddress = 7,
	DateTimeWithSecondsAddress = 8,
	AddressDay = 9,
	AddressDateTimeWithMinutes = 10,
	AddressDateTimeWithSeconds = 11,
}
