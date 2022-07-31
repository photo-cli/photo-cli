namespace PhotoCli.Services.Contracts;

public interface IConsoleWriter
{
	void Write(string value);
	void ProgressStart(string name, int? totalCount = null);
	void InProgressItemComplete(string name);
	void ProgressFinish(string name, string? additionalInformation = "");
}
