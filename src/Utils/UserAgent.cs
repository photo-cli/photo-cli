using System.Net.Http.Headers;

namespace PhotoCli.Utils;

public static class UserAgent
{
	public static ProductInfoHeaderValue Instance()
	{
		var agentName = Assembly.GetCallingAssembly().GetName().Name!;
		var version = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
		if (version is null || version.StartsWith("1.0.0"))
			version = "dev";
		var agent = new ProductInfoHeaderValue(new ProductHeaderValue(agentName, version));
		return agent;
	}
}
