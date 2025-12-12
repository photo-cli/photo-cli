using Spectre.Console;

namespace PhotoCli.Utils.Extensions;

public class AnsiConsoleExtended
{
	private readonly IAnsiConsole _console;

	public AnsiConsoleExtended(IAnsiConsole console)
	{
		_console = console;
	}

	public void WriteLineWithTime(string value)
	{
		WriteLineWithTime(value, Color.Default.ToMarkup());
	}

	public void WriteLineWithTime(string value, Color color)
	{
		WriteLineWithTime(value, color.ToMarkup());
	}

	public void WriteLineWithTime(string value, string spectreFormat)
	{
		var withTime = $"[[{DateTime.Now:HH:mm:ss}]] {value}";
		WriteLine(withTime, spectreFormat);
	}

	public void WriteLine(string value)
	{
		WriteLine(value, Color.Default.ToMarkup());
	}

	public void WriteLine(string value, Color color)
	{
		WriteLine(value, color.ToMarkup());
	}

	public void WriteLine(string value, string spectreFormat)
	{
		_console.Write(new Markup($"[{spectreFormat}]{value}[/]\n").Overflow(Overflow.Crop));
	}

	public string OutputTextByFormat(string text, Color color)
	{
		return OutputTextByFormat(text, color.ToMarkup());
	}

	public string OutputTextByFormat(string text, string format)
	{
		return $"[{format}]{text}[/]";
	}

	public string OutputBetweenNewLines(string text)
	{
		return $"\n{text}\n";
	}

	public string EscapeMarkup(string text)
	{
		return text.Replace("[", "[[").Replace("]", "]]");
	}

	public void EmptyLine()
	{
		_console.WriteLine("");
	}

	public void WriteTable(Table table)
	{
		_console.Write(table);
	}

	public void RawWriteLine(string value)
	{
		_console.WriteLine(value);
	}
}
