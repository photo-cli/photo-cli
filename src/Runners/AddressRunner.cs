using System.IO.Abstractions;

namespace PhotoCli.Runners;

public class AddressRunner : IConsoleRunner
{
	private readonly IConsoleWriter _consoleWriter;
	private readonly IFileSystem _fileSystem;
	private readonly IExifParserService _exifParserService;
	private readonly AddressOptions _options;
	private readonly IReverseGeocodeService _reverseGeocodeService;

	public AddressRunner(IExifParserService exifParserService, IReverseGeocodeService reverseGeocodeService, AddressOptions options, IConsoleWriter consoleWriter, IFileSystem fileSystem)
	{
		_exifParserService = exifParserService;
		_reverseGeocodeService = reverseGeocodeService;
		_options = options;
		_consoleWriter = consoleWriter;
		_fileSystem = fileSystem;
	}

	public async Task<ExitCode> Execute()
	{
		var outputFile = _fileSystem.FileInfo.New(_options.InputPath);
		if (!outputFile.Exists)
			return ExitCode.InputFileNotExists;

		var photoExifData = _exifParserService.Parse(_options.InputPath, false, true);
		if (photoExifData?.Coordinate == null)
			return ExitCode.PhotosWithNoCoordinatePreventedProcess;

		switch (_options.AddressListType)
		{
			case AddressListType.AllAvailableProperties:
				var allAvailableReverseGeocodes = await _reverseGeocodeService.AllAvailableReverseGeocodes(photoExifData.Coordinate);
				foreach (var (propertyName, propertyValue) in allAvailableReverseGeocodes)
					_consoleWriter.Write($"{propertyName}: {propertyValue}");
				break;
			case AddressListType.SelectedProperties:
				var reverseGeocodes = await _reverseGeocodeService.Get(photoExifData.Coordinate);
				var formattedReverseGeocodes = string.Join(Environment.NewLine, reverseGeocodes);
				_consoleWriter.Write(formattedReverseGeocodes);
				break;
			case AddressListType.FullResponse:
				var rawResponse = await _reverseGeocodeService.RawResponse(photoExifData.Coordinate!);
				_consoleWriter.Write(rawResponse);
				break;
			default:
				throw new PhotoCliException($"Not implemented {nameof(AddressListType)}: {_options.AddressListType}");
		}

		return ExitCode.Success;
	}
}
