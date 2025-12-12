using Spectre.Console;

namespace PhotoCli.Tests.Fakes;

public static class SpectreConsoleFakes
{
	public static readonly IAnsiConsole Actual = AnsiConsole.Console;
}
