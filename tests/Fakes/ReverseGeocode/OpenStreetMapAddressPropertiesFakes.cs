using System.Reflection;

namespace PhotoCli.Tests.Fakes.ReverseGeocode;

public static class OpenStreetMapAddressPropertiesFakes
{
	public static List<PropertyInfo> ValidPropertyInfos()
	{
		var properties = new List<string> { nameof(OpenStreetMapAddress.Country), nameof(OpenStreetMapAddress.City) };
		var requestedAddressProperties = typeof(OpenStreetMapAddress).GetProperties().Where(w => properties.Contains(w.Name)).ToList();
		return requestedAddressProperties;
	}

	public static List<string> Valid()
	{
		var validPropertyInfos = ValidPropertyInfos();
		return validPropertyInfos.Select(s => s.Name.ToString()).ToList();
	}
}
