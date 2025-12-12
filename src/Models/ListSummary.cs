using System.ComponentModel.DataAnnotations;

namespace PhotoCli.Models;

public record ListSummary : DisplayableRecord
{
	[Display(Name = "Album(s)")]
	public int AlbumCount { get; set; }

	[Display(Name = "Photo(s)")]
	public long PhotoCount { get; set; }

	[Display(Name = "Reverse Geocode Cache(s)")]
	public long ReverseGeocodeCacheCount { get; set; }
}
