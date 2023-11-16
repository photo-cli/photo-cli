namespace PhotoCli.Tests.Utils;

internal class InMemorySQLiteConnectionStringProvider : ISQLiteConnectionStringProvider
{
	public string Value => $"DataSource=file:{Guid.NewGuid()}?mode=memory";
}
