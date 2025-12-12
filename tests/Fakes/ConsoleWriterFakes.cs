using Spectre.Console;

namespace PhotoCli.Tests.Fakes;

public static class ConsoleWriterFakes
{
	public static ConsoleWriter Valid()
	{
		var ansiConsoleExtended = new AnsiConsoleExtended(AnsiConsole.Console);
		var consoleWriter = new ConsoleWriter(ansiConsoleExtended);
		return consoleWriter;
	}
}
