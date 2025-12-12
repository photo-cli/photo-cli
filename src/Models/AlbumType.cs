namespace PhotoCli.Models;

public enum AlbumType : byte
{
	Unset = 0,

	/// <summary>
	/// Individual photo ids and-or DateRange
	/// </summary>
	UserDefined = 1,

	ReverseGeocode = 2,
}
