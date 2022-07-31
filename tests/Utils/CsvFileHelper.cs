using System.IO.Abstractions;
using CsvHelper;

namespace PhotoCli.Tests.Utils;

internal static class CsvFileHelper
{
	internal static IEnumerable<PhotoCsv> ReadRecords(IFileInfo csvFile)
	{
		var fileStreamRead = csvFile.OpenRead();
		return ReadRecordsFromStream(fileStreamRead);
	}

	internal static List<PhotoCsv> ReadRecords(FileInfo csvFile)
	{
		var fileStreamRead = csvFile.OpenRead();
		return ReadRecordsFromStream(fileStreamRead);
	}

	private static List<PhotoCsv> ReadRecordsFromStream(Stream fileStreamRead)
	{
		using var reader = new StreamReader(fileStreamRead);
		using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
		var records = csv.GetRecords<PhotoCsv>().ToList();
		return records;
	}
}
