namespace PhotoCli.Services.Implementations;

public class ReverseGeocodeFetcherService : IReverseGeocodeFetcherService
{
	private const string ProgressName = "Reverse Geocoding";

	private static readonly Dictionary<ReverseGeocodeProvider, TimeSpan?> WaitTimeBetweenEachRequestByFreemiumProviders = new()
	{
		{ ReverseGeocodeProvider.LocationIq, TimeSpan.FromSeconds(1) },
	};

	private static readonly Dictionary<ReverseGeocodeProvider, TimeSpan?> WaitTimeBetweenEachRequestAlways = new()
	{
		{ ReverseGeocodeProvider.OpenStreetMapFoundation, TimeSpan.FromSeconds(1) },
	};

	private readonly IConsoleWriter _consoleWriter;
	private readonly IReverseGeocodeOptions _reverseGeocodeOptions;
	private readonly IReverseGeocodeService _reverseGeocodeService;
	private readonly Statistics _statistics;
	private readonly ToolOptions _toolOptions;
	private readonly ILogger<ReverseGeocodeFetcherService> _logger;

	public ReverseGeocodeFetcherService(IReverseGeocodeService reverseGeocodeService, IReverseGeocodeOptions reverseGeocodeOptions, IConsoleWriter consoleWriter, Statistics statistics,
		ToolOptions toolOptions, ILogger<ReverseGeocodeFetcherService> logger)
	{
		_reverseGeocodeService = reverseGeocodeService;
		_reverseGeocodeOptions = reverseGeocodeOptions;
		_consoleWriter = consoleWriter;
		_statistics = statistics;
		_toolOptions = toolOptions;
		_logger = logger;
	}

	public async Task<IReadOnlyCollection<Photo>> Fetch(IReadOnlyCollection<Photo> photos)
	{
		_consoleWriter.ProgressStart(ProgressName, _statistics.HasCoordinateCount);

		var waitTimeBetweenEachRequest = RateLimit();
		var semaphore = new SemaphoreSlim(waitTimeBetweenEachRequest != null ? 1 : _toolOptions.ConnectionLimit);

		var fileBasedReverseGeocodeRequests = new List<Tuple<Photo, Task<IEnumerable<string>>>>();

		foreach (var photo in photos)
		{
			if (!photo.HasCoordinate)
			{
				_logger.LogTrace("No coordinate found, skipping {FilePath}", photo.PhotoFile.SourcePath);
				continue;
			}

			await semaphore.WaitAsync();
			var reverseGeocodeRequest = _reverseGeocodeService.Get(photo.Coordinate!);
#pragma warning disable CS4014
			reverseGeocodeRequest.ContinueWith(_ =>
#pragma warning restore CS4014
			{
				semaphore.Release();
				_logger.LogDebug("Semaphore count: {SemaphoreCurrentCount}", semaphore.CurrentCount);
				_logger.LogTrace("Completed reverse geocode request for {FilePath}", photo.PhotoFile.SourcePath);
				_consoleWriter.InProgressItemComplete(ProgressName);
			});
			fileBasedReverseGeocodeRequests.Add(new Tuple<Photo, Task<IEnumerable<string>>>(photo, reverseGeocodeRequest));
			_logger.LogTrace("Queued reverse geocode request for {FilePath}", photo.PhotoFile.SourcePath);
			if (waitTimeBetweenEachRequest != null)
			{
				_logger.LogDebug("Rate limit found, will wait for: {RateLimit}", waitTimeBetweenEachRequest.Value);
				await Task.Delay(waitTimeBetweenEachRequest.Value);
			}
		}

		_logger.LogDebug("Waiting for all queued reverse geocode requests to be finished");
		var allRequestsTasks = fileBasedReverseGeocodeRequests.Select(s => s.Item2).ToArray();
		await Task.WhenAll(allRequestsTasks);
		_logger.LogDebug("All queued reverse geocode requests have been finished");

		foreach (var (photo, reverseGeocodeRequest) in fileBasedReverseGeocodeRequests)
		{
			if(photo.ExifData != null)
				photo.ExifData.ReverseGeocodes = reverseGeocodeRequest.Result;
		}

		_consoleWriter.ProgressFinish(ProgressName);
		return photos;
	}

	/// <summary>
	///     Apply rate limits of Reverse Geocode Providers
	///     Important note: Modifying this logic to bypass rate limit will end up with your public IP banned
	/// </summary>
	/// <returns>Duration needed to wait between each request</returns>
	public TimeSpan? RateLimit()
	{
		var waitTimeSpanBetweenEachRequest = WaitTimeBetweenEachRequestAlways.GetValueOrDefault(_reverseGeocodeOptions.ReverseGeocodeProvider);
		if (waitTimeSpanBetweenEachRequest != null)
		{
			_logger.LogTrace("This {ReverseGeocodeProvider} provider is using rate limit of {RateLimit} second(s) between each request", _reverseGeocodeOptions.ReverseGeocodeProvider,
				waitTimeSpanBetweenEachRequest.Value.TotalSeconds);
			return waitTimeSpanBetweenEachRequest;
		}

		if (_reverseGeocodeOptions.HasPaidLicense.GetValueOrDefault())
		{
			_logger.LogTrace("Has paid license, bypassing rate limit");
			return null;
		}

		waitTimeSpanBetweenEachRequest = WaitTimeBetweenEachRequestByFreemiumProviders.GetValueOrDefault(_reverseGeocodeOptions.ReverseGeocodeProvider);

		if (waitTimeSpanBetweenEachRequest.HasValue)
			_logger.LogTrace("Freemium provider, rate limit of {RateLimit}", waitTimeSpanBetweenEachRequest);

		return waitTimeSpanBetweenEachRequest;
	}

	public void RateLimitWarning()
	{
		var rateLimitForAllUsers = WaitTimeBetweenEachRequestAlways.GetValueOrDefault(_reverseGeocodeOptions.ReverseGeocodeProvider);
		if (rateLimitForAllUsers != null)
		{
			_consoleWriter.Write($"This {_reverseGeocodeOptions.ReverseGeocodeProvider} provider is using rate limit of {rateLimitForAllUsers.Value.TotalSeconds} second(s) between each request");
			return;
		}

		if (_reverseGeocodeOptions.HasPaidLicense.GetValueOrDefault())
		{
			_consoleWriter.Write(
				$"As option `HasPaidLicense` selected, we are not using rate limit for freemium users. Be sure about your API Key is authorized as paid user or else your public IP will be banned from {_reverseGeocodeOptions.ReverseGeocodeProvider}");
			return;
		}

		var rateLimitForFreemiumUsers = WaitTimeBetweenEachRequestByFreemiumProviders.GetValueOrDefault(_reverseGeocodeOptions.ReverseGeocodeProvider);
		if (rateLimitForFreemiumUsers == null)
			return;

		_consoleWriter.Write(
			$"As no option 'HasPaidLicence' selected, this {_reverseGeocodeOptions.ReverseGeocodeProvider} provider is using rate limit of {rateLimitForAllUsers} between each request for freemium users. (Only registering and getting API key not means that you have paid license)");
	}
}
