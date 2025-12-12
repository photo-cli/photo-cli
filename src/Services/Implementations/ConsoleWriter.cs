using Spectre.Console;

namespace PhotoCli.Services.Implementations;

public class ConsoleWriter : IConsoleWriter
{
	private readonly AnsiConsoleExtended _ansiConsoleExtended;
	private static readonly object ProgressLock = new();
	private int _progressCompletedCount;
	private int _progressTotalCount;
	private StatusContext? _spectreStatusContext;

	public ConsoleWriter(AnsiConsoleExtended ansiConsoleExtended)
	{
		_ansiConsoleExtended = ansiConsoleExtended;
	}

	public void Write(string message)
	{
		_ansiConsoleExtended.WriteLineWithTime(message);
	}

	public void WriteJson(string message)
	{
		var escaped = _ansiConsoleExtended.EscapeMarkup(message);
		Write(escaped);
	}

	public void WriteSuccess(string message)
	{
		Write(message, Color.Green);
	}

	public void WriteError(string message)
	{
		Write(message, Color.Red);
	}

	public void ProgressStart(string name, int? totalCount = null)
	{
		var value = $"{name}: started";
		_ansiConsoleExtended.WriteLineWithTime(value);
		if (totalCount != null)
			_progressTotalCount = totalCount.Value;
	}

	public void InProgressItemComplete(string name, string? additionalInformation = null)
	{
		if (_spectreStatusContext == null)
			throw new InvalidOperationException("Spectre status context is not initialized. Call InitializeSpectreContext first.");

		Interlocked.Increment(ref _progressCompletedCount);
		lock (ProgressLock)
		{
			var percentage = (float)_progressCompletedCount / _progressTotalCount;
			var progressInfo = $"{_progressCompletedCount}/{_progressTotalCount}";
			var ending = additionalInformation != null ? $" - {additionalInformation}" : string.Empty;
			_spectreStatusContext.Status($"{name}: {percentage:0%} - ({progressInfo}){ending}");
		}
	}

	public void ProgressFinish(string name, string? additionalInformation = "")
	{
		_progressCompletedCount = 0;
		var content = $"{name}: finished. {additionalInformation}";
		_ansiConsoleExtended.WriteLineWithTime(content);
	}

	public void InitializeSpectreContext(StatusContext specterStatusContext)
	{
		_spectreStatusContext = specterStatusContext;
	}

	public void WriteTable(Table table)
	{
		_ansiConsoleExtended.WriteTable(table);
	}

	public void RawWriteLine(string value)
	{
		_ansiConsoleExtended.RawWriteLine(value);
	}

	private void Write(string value, Color color)
	{
		_ansiConsoleExtended.WriteLineWithTime(value, color);
	}
}
