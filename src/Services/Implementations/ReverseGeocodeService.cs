namespace PhotoCli.Services.Implementations;

public class ReverseGeocodeService : IReverseGeocodeService
{
	private readonly IBigDataCloudReverseGeocodeService _bigDataCloud;
	private readonly IGoogleMapsReverseGeocodeService _googleMaps;
	private readonly ILocationIqReverseGeocodeService _locationIq;
	private readonly ILogger<ReverseGeocodeService> _logger;
	private readonly IOpenStreetMapFoundationReverseGeocodeService _openStreetMapFoundation;
	private readonly IReverseGeocodeOptions _options;

	private List<PropertyInfo>? _openStreetMapSelectedPropertyInfosOnAddressObject;

	public ReverseGeocodeService(IReverseGeocodeOptions options, IBigDataCloudReverseGeocodeService bigDataCloud, IOpenStreetMapFoundationReverseGeocodeService openStreetMapFoundation,
		IGoogleMapsReverseGeocodeService googleMaps, ILocationIqReverseGeocodeService locationIq, ILogger<ReverseGeocodeService> logger)
	{
		_options = options;
		_bigDataCloud = bigDataCloud;
		_openStreetMapFoundation = openStreetMapFoundation;
		_googleMaps = googleMaps;
		_locationIq = locationIq;
		_logger = logger;
	}

	public Task<IEnumerable<string>> Get(Coordinate coordinate)
	{
		_logger.LogTrace("Getting reverse geocode list of coordinate {Coordinate} from provider: {ReverseGeocodeProvider}", coordinate, _options.ReverseGeocodeProvider);
		return _options.ReverseGeocodeProvider switch
		{
			ReverseGeocodeProvider.BigDataCloud => _bigDataCloud.Get(coordinate, _options.Language, _options.BigDataCloudAdminLevels),
			ReverseGeocodeProvider.OpenStreetMapFoundation => _openStreetMapFoundation.Get(coordinate, BuildPrepareOpenStreetMapProperties(_options)),
			ReverseGeocodeProvider.GoogleMaps => _googleMaps.Get(coordinate, _options.Language, _options.GoogleMapsAddressTypes),
			ReverseGeocodeProvider.LocationIq => _locationIq.Get(coordinate, BuildPrepareOpenStreetMapProperties(_options)),
			_ => throw new PhotoCliException($"{nameof(_options.ReverseGeocodeProvider)}, should have a valid value")
		};
	}

	public async Task<string> RawResponse(Coordinate coordinate)
	{
		_logger.LogTrace("Getting raw serialized response of coordinate {Coordinate} from provider: {ReverseGeocodeProvider}", coordinate, _options.ReverseGeocodeProvider);
		object? response = _options.ReverseGeocodeProvider switch
		{
			ReverseGeocodeProvider.BigDataCloud => await _bigDataCloud.SerializeFullResponse(coordinate, _options.Language),
			ReverseGeocodeProvider.OpenStreetMapFoundation => await _openStreetMapFoundation.SerializeFullResponse(coordinate),
			ReverseGeocodeProvider.GoogleMaps => await _googleMaps.SerializeFullResponse(coordinate, _options.Language),
			ReverseGeocodeProvider.LocationIq => await _locationIq.SerializeFullResponse(coordinate),
			_ => throw new PhotoCliException($"{nameof(_options.ReverseGeocodeProvider)}, should have a valid value")
		};
		if (response == null)
			throw new PhotoCliException("Can't get raw response");
		var serializedResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
		return serializedResponse;
	}

	public Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate)
	{
		_logger.LogTrace("Getting all available reverse geocodes of coordinate {Coordinate} from provider: {ReverseGeocodeProvider}", coordinate, _options.ReverseGeocodeProvider);
		var response = _options.ReverseGeocodeProvider switch
		{
			ReverseGeocodeProvider.BigDataCloud => _bigDataCloud.AllAvailableReverseGeocodes(coordinate, _options.Language),
			ReverseGeocodeProvider.OpenStreetMapFoundation => _openStreetMapFoundation.AllAvailableReverseGeocodes(coordinate),
			ReverseGeocodeProvider.GoogleMaps => _googleMaps.AllAvailableReverseGeocodes(coordinate, _options.Language),
			ReverseGeocodeProvider.LocationIq => _locationIq.AllAvailableReverseGeocodes(coordinate),
			_ => throw new PhotoCliException($"{nameof(_options.ReverseGeocodeProvider)}, should have a valid value")
		};
		if (response == null)
			throw new PhotoCliException("Can't get all available reverse-geocodes");
		return response;
	}

	private List<PropertyInfo> BuildPrepareOpenStreetMapProperties(IReverseGeocodeOptions options)
	{
		if (_openStreetMapSelectedPropertyInfosOnAddressObject == null)
		{
			_logger.LogDebug("Initializing OpenStreetMapProperties by using reflection");
			var openStreetMapAllLowerCaseProperties = options.OpenStreetMapProperties.Select(s => s.ToLowerInvariant());
			_openStreetMapSelectedPropertyInfosOnAddressObject = typeof(OpenStreetMapAddress).GetProperties()
				.Where(w => openStreetMapAllLowerCaseProperties.Contains(w.Name.ToLowerInvariant())).ToList();
		}

		return _openStreetMapSelectedPropertyInfosOnAddressObject;
	}
}
