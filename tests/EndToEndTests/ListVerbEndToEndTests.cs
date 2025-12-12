using System.Runtime.InteropServices;

namespace PhotoCli.Tests.EndToEndTests;

public class ListVerbEndToEndTests : BaseEndToEndTests
{
	#region Summary

	public static TheoryData<ICollection<string>, ListSummary> ListSummaryTestData = new()
	{
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.Summary, TestImagesPathHelper.ArchiveFolder()),
			new ListSummary { AlbumCount = 17, PhotoCount = 16, ReverseGeocodeCacheCount = 13 }
		}
	};

	[Theory]
	[MemberData(nameof(ListSummaryTestData))]
	public async Task Run_WithListSummaryCommand_ReturnsExpectedListSummary(ICollection<string> args, ListSummary expectedListSummary)
	{
		var rawOutput = await RunMainRaw(args);
		var actualListSummary = ParseKeyValueTableFromOutput<ListSummary>(rawOutput);
		actualListSummary.Should().BeEquivalentTo(expectedListSummary);
	}

	#endregion

	#region Albums

	public static TheoryData<ICollection<string>, List<ListAlbumTableRow>> ListAlbumTestData = new()
	{
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.Albums, TestImagesPathHelper.ArchiveFolder()),
			[
				new ListAlbumTableRow(1, "Italy Album with Date Range", "UserDefined", "2025-09-16 21:34:27", "{\"DateRange\":{\"Start\":\"2008-10-22T16:28:39\",\"End\":\"2008-10-22T17:00:07\"}}"),
				new ListAlbumTableRow(2, "Italy-Arezzo", "ReverseGeocode", "2025-09-16 21:34:27", "{\"ReverseGeocode\":{\"ReverseGeocodeFormatted\":\"Italy-Arezzo\"}}"),
				new ListAlbumTableRow(3, "Italy", "ReverseGeocode", "2025-09-16 21:34:27", "{\"ReverseGeocode\":{\"Address1\":\"Italy\"}}"),
				new ListAlbumTableRow(4, "Arezzo", "ReverseGeocode", "2025-09-16 21:34:27", "{\"ReverseGeocode\":{\"Address2\":\"Arezzo\"}}"),
				new ListAlbumTableRow(5, "Spain Album as individual photos", "UserDefined", "2025-09-16 21:35:06", "{\"PhotoIds\":[10,11,12]}"),
				new ListAlbumTableRow(6, "Spain-Madrid", "ReverseGeocode", "2025-09-16 21:35:06", "{\"ReverseGeocode\":{\"ReverseGeocodeFormatted\":\"Spain-Madrid\"}}"),
				new ListAlbumTableRow(7, "Spain", "ReverseGeocode", "2025-09-16 21:35:06", "{\"ReverseGeocode\":{\"Address1\":\"Spain\"}}"),
				new ListAlbumTableRow(8, "Madrid", "ReverseGeocode", "2025-09-16 21:35:06", "{\"ReverseGeocode\":{\"Address2\":\"Madrid\"}}"),
				new ListAlbumTableRow(9, "Others as individual photos", "UserDefined", "2025-09-16 21:35:23", "{\"PhotoIds\":[13,14,15,16]}"),
				new ListAlbumTableRow(10, "Kenya-Nakuru", "ReverseGeocode", "2025-09-16 21:35:23", "{\"ReverseGeocode\":{\"ReverseGeocodeFormatted\":\"Kenya-Nakuru\"}}"),
				new ListAlbumTableRow(11, "Italy-Firenze", "ReverseGeocode", "2025-09-16 21:35:23", "{\"ReverseGeocode\":{\"ReverseGeocodeFormatted\":\"Italy-Firenze\"}}"),
				new ListAlbumTableRow(12, "United Kingdom-England", "ReverseGeocode", "2025-09-16 21:35:23", "{\"ReverseGeocode\":{\"ReverseGeocodeFormatted\":\"United Kingdom-England\"}}"),
				new ListAlbumTableRow(13, "Kenya", "ReverseGeocode", "2025-09-16 21:35:23", "{\"ReverseGeocode\":{\"Address1\":\"Kenya\"}}"),
				new ListAlbumTableRow(14, "United Kingdom", "ReverseGeocode", "2025-09-16 21:35:23", "{\"ReverseGeocode\":{\"Address1\":\"United Kingdom\"}}"),
				new ListAlbumTableRow(15, "Nakuru", "ReverseGeocode", "2025-09-16 21:35:23", "{\"ReverseGeocode\":{\"Address2\":\"Nakuru\"}}"),
				new ListAlbumTableRow(16, "Firenze", "ReverseGeocode", "2025-09-16 21:35:23", "{\"ReverseGeocode\":{\"Address2\":\"Firenze\"}}"),
				new ListAlbumTableRow(17, "England", "ReverseGeocode", "2025-09-16 21:35:23", "{\"ReverseGeocode\":{\"Address2\":\"England\"}}"),
			]
		}
	};

	[Theory]
	[MemberData(nameof(ListAlbumTestData))]
	public async Task Run_WithListAlbumCommand_ReturnsExpectedListSummary(ICollection<string> args, List<ListAlbumTableRow> expectedListAlbums)
	{
		var rawOutput = await RunMainRaw(args);
		var actualListAlbumTableRow = ParseTableText(rawOutput);
		actualListAlbumTableRow.Should().BeEquivalentTo(expectedListAlbums);
	}

	private static List<ListAlbumTableRow> ParseTableText(string tableText)
	{
		var albums = new List<ListAlbumTableRow>();
		var lines = tableText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

		// Skip header lines (first 3 lines: top border, header, separator)
		var dataLines = lines.Skip(3).Take(lines.Length - 4); // Also skip bottom border

		foreach (var line in dataLines)
		{
			// Parse each row using regex to handle the table format
			var match = Regex.Match(line, @"│\s*(\d+)\s*│\s*([^│]+?)\s*│\s*([^│]+?)\s*│\s*([^│]+?)\s*│\s*([^│]+?)\s*│");

			if (match.Success)
			{
				var album = new ListAlbumTableRow
				(
					int.Parse(match.Groups[1].Value.Trim()),
					match.Groups[2].Value.Trim(),
					match.Groups[3].Value.Trim(),
					match.Groups[4].Value.Trim(),
					match.Groups[5].Value.Trim()
				);
				albums.Add(album);
			}
		}

		return albums;
	}

	#endregion

	#region PhotosByAlbum

	private static readonly string[] ItalyArezzoPhotos = [
		TestImagesFolder("2008/10/22/2008.10.22_16.28.39-5d66eec547469a1817bda4abe35c801359b2bb55.jpg"),
		TestImagesFolder("2008/10/22/2008.10.22_16.29.49-629b0b141634d6c0906e49af448bec8d755ba32c.jpg"),
		TestImagesFolder("2008/10/22/2008.10.22_16.38.20-620d23336a12ab54f9f0190fe93960a4dba2df59.jpg"),
		TestImagesFolder("2008/10/22/2008.10.22_16.43.21-3b0a3215b4f66d7ff4804dd223f192c21aee71bc.jpg"),
		TestImagesFolder("2008/10/22/2008.10.22_16.44.01-d470205a1d331a9d3765b3762b7c954bb8efc6ea.jpg"),
		TestImagesFolder("2008/10/22/2008.10.22_16.46.53-f670f2bb6c54898894b06b083185b05086bd4e6e.jpg"),
		TestImagesFolder("2008/10/22/2008.10.22_16.52.15-6b89a245809031ecc47789cdeaa332545330fc39.jpg"),
		TestImagesFolder("2008/10/22/2008.10.22_16.55.37-dd42edcde2433a7df4a3d67bf61944a20884da89.jpg"),
		TestImagesFolder("2008/10/22/2008.10.22_17.00.07-a0ab699f5f99fce8ff49163e87c7590c2c9a66eb.jpg"),
	];

	private static readonly string[] ItalyFirenzePhotos = [
		TestImagesFolder("2005/12/14/2005.12.14_14.39.47-03cb14d5c68beed97cbe73164de9771d537fcd96.jpg"),
	];

	private static readonly string[] SpainPhotosWithPhotoTakenDate = [
		TestImagesFolder("2015/04/10/2015.04.10_20.12.23-3907fc960f2873f40c8f35643dd444e0468be131.jpg"),
		TestImagesFolder("2015/04/10/2015.04.10_20.12.23-9f4e6d352ec172e1059571250655e376769080fe.jpg"),
	];

	private static readonly string[] SpainPhotosWitNoPhotoTakenDate = [
		TestImagesFolder("no-photo-taken-date/cf756397cc3ca81b2650c8801fd64e172504015a.jpg"),
	];

	private static readonly string[] KenyaPhotos = [
		TestImagesFolder("2005/08/13/2005.08.13_09.47.23-5842c73cfdc5f347551bb6016e00c71bb1393169.jpg"),
	];

	public static TheoryData<ICollection<string>, string[]> DateRangeAlbumPhotosWithExpectedFilePaths = new()
	{
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByAlbum, TestImagesPathHelper.ArchiveFolder(), 1, rawOutput: true),
			ItalyArezzoPhotos
		}
	};

	public static TheoryData<ICollection<string>, string[]> ReverseGeocodeAlbumPhotosWithExpectedFilePaths = new()
	{
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByAlbum, TestImagesPathHelper.ArchiveFolder(), 2, rawOutput: true),
			ItalyArezzoPhotos
		},
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByAlbum, TestImagesPathHelper.ArchiveFolder(), 3, rawOutput: true),
			MergeList(ItalyArezzoPhotos, ItalyFirenzePhotos)
		},
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByAlbum, TestImagesPathHelper.ArchiveFolder(), 4, rawOutput: true),
			ItalyArezzoPhotos
		},
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByAlbum, TestImagesPathHelper.ArchiveFolder(), 16, rawOutput: true),
			ItalyFirenzePhotos
		},
	};

	public static TheoryData<ICollection<string>, string[]> PhotoIdsAlbumPhotosWithExpectedFilePaths = new()
	{
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByAlbum, TestImagesPathHelper.ArchiveFolder(), 5, rawOutput: true),
			MergeList(SpainPhotosWithPhotoTakenDate, SpainPhotosWitNoPhotoTakenDate)
		},
	};

	[Theory]
	[MemberData(nameof(DateRangeAlbumPhotosWithExpectedFilePaths))]
	[MemberData(nameof(ReverseGeocodeAlbumPhotosWithExpectedFilePaths))]
	[MemberData(nameof(ReverseGeocodeAlbumPhotosWithExpectedFilePaths))]
	[MemberData(nameof(PhotoIdsAlbumPhotosWithExpectedFilePaths))]
	public async Task Run_WithPhotosByAlbumCommand_ReturnsExpectedPhotoPaths(ICollection<string> args, string[] expectedPhotoPaths)
	{
		var rawOutputLines = await RunMainLines(args);
		rawOutputLines.Should().BeEquivalentTo(expectedPhotoPaths);
	}

	#endregion

	#region PhotosByDate

	public static TheoryData<ICollection<string>, string[]> YearPhotosWithExpectedFilePaths = new()
	{
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByDate, TestImagesPathHelper.ArchiveFolder(), year: 2015, rawOutput: true),
			SpainPhotosWithPhotoTakenDate
		},
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByDate, TestImagesPathHelper.ArchiveFolder(), year: 2005, rawOutput: true),
			MergeList(KenyaPhotos, ItalyFirenzePhotos)
		}
	};

	public static TheoryData<ICollection<string>, string[]> YearMonthPhotosWithExpectedFilePaths = new()
	{
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByDate, TestImagesPathHelper.ArchiveFolder(), year: 2008, month: 10, rawOutput: true),
			ItalyArezzoPhotos
		},
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByDate, TestImagesPathHelper.ArchiveFolder(), year: 2005, month: 8, rawOutput: true),
			KenyaPhotos
		}
	};

	public static TheoryData<ICollection<string>, string[]> YearMonthDayPhotosWithExpectedFilePaths = new()
	{
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByDate, TestImagesPathHelper.ArchiveFolder(), year: 2008, month: 10, day: 22, rawOutput: true),
			ItalyArezzoPhotos
		},
		{
			CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.PhotosByDate, TestImagesPathHelper.ArchiveFolder(), year: 2015, month: 4, day: 10, rawOutput: true),
			SpainPhotosWithPhotoTakenDate
		}
	};

	[Theory]
	[MemberData(nameof(YearPhotosWithExpectedFilePaths))]
	[MemberData(nameof(YearMonthPhotosWithExpectedFilePaths))]
	[MemberData(nameof(YearMonthDayPhotosWithExpectedFilePaths))]
	public async Task Run_WithPhotosByDateCommand_ReturnsExpectedPhotoPaths(ICollection<string> args, string[] expectedPhotoPaths)
	{
		await RunAndVerifyOutputPaths(args, expectedPhotoPaths);
	}

	#endregion

	[Fact]
	public async Task Run_WithNonExistentArchiveFolder_ReturnsNoArchiveDatabaseFoundExitCode()
	{
		var argsWith = CommandLineArgumentsFakes.ListBuildCommandLineOptions(ListType.Summary, TestImagesPathHelper.NotExistingFolder());
		await RunMainRaw(argsWith, expectedExitCode: ExitCode.NoArchiveDatabaseFound);
	}

	#region Helpers

	private async Task RunAndVerifyOutputPaths(ICollection<string> args, string[] expectedPhotoPaths)
	{
		var rawOutputLines = await RunMainLines(args);
		rawOutputLines.Should().BeEquivalentTo(expectedPhotoPaths);
	}

	private static string[] MergeList(params string[][] lists)
	{
		return lists.SelectMany(list => list).ToArray();
	}

	private static string TestImagesFolder(string path)
	{
		var archiveFolderByOs = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ArchiveWindows" : "ArchiveUnix";
		return MockFileSystemHelper.CombineRelativePath("TestImages", archiveFolderByOs, path);
	}

	#endregion
}
