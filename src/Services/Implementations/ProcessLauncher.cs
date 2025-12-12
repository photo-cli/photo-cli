using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PhotoCli.Services.Implementations;

public class ProcessLauncher : IProcessLauncher
{
	private readonly ToolOptions _toolOptions;
	private readonly ILogger<ProcessLauncher> _logger;

	public ProcessLauncher(ToolOptions toolOptions, ILogger<ProcessLauncher> logger)
	{
		_toolOptions = toolOptions;
		_logger = logger;
	}

	public async Task Launch(IEnumerable<string> filePaths)
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			_logger.LogWarning("Process launch is only supported on macOS");
			return;
		}

		var filePathsList = filePaths.ToList();
		if (!filePathsList.Any())
		{
			_logger.LogWarning("No file paths provided for preview");
			return;
		}

		var arguments = $"{_toolOptions.MacOsArgumentPrefix} " + string.Join(" ", filePathsList);
		var processStartInfo = new ProcessStartInfo
		{
			FileName = _toolOptions.MacOsCommand,
			Arguments = arguments,
		};

		var process = Process.Start(processStartInfo);
		if (process == null)
			throw new PhotoCliException("Failed to start preview process");

		await process.WaitForExitAsync();
	}
}
