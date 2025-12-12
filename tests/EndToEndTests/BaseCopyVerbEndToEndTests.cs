namespace PhotoCli.Tests.EndToEndTests;

public class BaseCopyVerbEndToEndTests : BaseEndToEndTests
{
	protected async Task<(Statistics, List<PhotoCsv>)> ExecuteCopy(ICollection<string> args, FileInfo fileInfo)
	{
		var actualStatistics = await RunMainOutputAsStatistics(args);
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(fileInfo);
		return (actualStatistics, actualPhotoCsvModels);
	}

	protected static void VerifyCsvModelsNewPathExists(IEnumerable<PhotoCsv> actualPhotoCsvModels, string outputFolder)
	{
		var fileInfos = actualPhotoCsvModels.Select(actualPhotoCsvModel => new FileInfo(Path.Combine(outputFolder, actualPhotoCsvModel.PhotoNewPath!)));
		foreach (var fileInfo in fileInfos)
			fileInfo.Exists.Should().Be(true);
	}
}
