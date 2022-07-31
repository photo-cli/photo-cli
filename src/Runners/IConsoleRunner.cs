namespace PhotoCli.Runners;

public interface IConsoleRunner
{
	Task<ExitCode> Execute();
}
