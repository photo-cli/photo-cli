using Spectre.Console;

namespace PhotoCli.Services.Contracts;

public interface IConsoleWriter
{
	void Write(string message);
	void WriteJson(string message);
	void WriteSuccess(string message);
	void WriteError(string message);
	void ProgressStart(string name, int? totalCount = null);
	void InProgressItemComplete(string name, string? additionalInformation = null);
	void ProgressFinish(string name, string? additionalInformation = "");
	void InitializeSpectreContext(StatusContext specterStatusContext);
	void WriteTable(Table table);
	void RawWriteLine(string value);
}
