using Spectre.Console;

namespace PhotoCli.Services.Implementations;

public class NullConsoleWriter : IConsoleWriter
{
	public void Write(string message) { }
	public void WriteJson(string message) { }
	public void WriteSuccess(string message) { }
	public void WriteError(string message) { }
	public void ProgressStart(string name, int? totalCount = null) { }
	public void InProgressItemComplete(string name, string? additionalInformation = null) { }
	public void ProgressFinish(string name, string? additionalInformation = "") { }
	public void InitializeSpectreContext(StatusContext specterStatusContext) { }
	public void WriteTable(Table table) { }
	public void RawWriteLine(string value) { }
}
