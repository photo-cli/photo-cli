namespace PhotoCli.Services.Implementations;

public class ConsoleWriter : IConsoleWriter
{
	private static readonly object PhotoInprogressLock = new();
	private readonly TextWriter _textWriter;
	private readonly ILogger<ConsoleWriter> _logger;
	private string? _previousProgressName;
	private int _progressCompletedCount;
	private int _progressTotalCount;

	public ConsoleWriter(TextWriter textWriter, ILogger<ConsoleWriter> logger)
	{
		_textWriter = textWriter;
		_logger = logger;
		_logger.LogInformation("User interactive console: {IsUserInteractive}", UserInteractive());
	}

	public void Write(string value)
	{
		_textWriter.WriteLine(value);
	}

	public void ProgressStart(string name, int? totalCount = null)
	{
		_textWriter.WriteLine($"{name}: started.");
		_logger.LogInformation("Progress {ProgressName} started", name);
		_previousProgressName = name;
		if (totalCount != null)
			_progressTotalCount = totalCount.Value;
	}

	public void InProgressItemComplete(string name)
	{
		Interlocked.Increment(ref _progressCompletedCount);
		_logger.LogTrace("Progress name {ProgressName} count: {Current}/{Total}", name, _progressCompletedCount, _progressTotalCount);
		lock (PhotoInprogressLock)
		{
			TryToClearConsoleLastLine(name);
			_textWriter.WriteLine($"{name}: {(float)_progressCompletedCount / _progressTotalCount:0%} - ({_progressCompletedCount}/{_progressTotalCount})");
			_previousProgressName = name;
		}
	}

	public void ProgressFinish(string name, string? additionalInformation = "")
	{
		_progressCompletedCount = 0;
		TryToClearConsoleLastLine(name);
		CoverAllLine($"{name}: finished. {additionalInformation}");
		_logger.LogInformation("Progress {ProgressName} finished", name);
		_previousProgressName = name;
	}

	private void CoverAllLine(string toWrite)
	{
		if (!UserInteractive())
		{
			_logger.LogTrace("Console is not user interactive, directly writing to console");
			_textWriter.WriteLine(toWrite);
			return;
		}

		if (Console.WindowWidth > 0)
			_textWriter.WriteLine(toWrite + new string(' ', Console.WindowWidth - toWrite.Length));
		else
			_textWriter.WriteLine(toWrite);
	}

	private void TryToClearConsoleLastLine(string name)
	{
		if (!UserInteractive())
		{
			_logger.LogTrace("Console is not user interactive, skip clearing console");
			return;
		}

		if (_previousProgressName != name)
			return;
		if (Console.CursorTop > 0)
			Console.SetCursorPosition(0, Console.CursorTop);
		if (Console.WindowWidth > 0)
			_textWriter.Write(new string(' ', Console.WindowWidth));
		if (Console.CursorTop > 0)
			Console.SetCursorPosition(0, Console.CursorTop - 1);
	}

	private bool UserInteractive()
	{
		return Environment.UserInteractive && !Environment.CurrentDirectory.Contains("tests");
	}
}
