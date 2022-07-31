namespace PhotoCli.Tests.Fakes;

public static class NamingStyleFakes
{
	public static NamingStyle Valid()
	{
		return NamingStyle.Numeric;
	}

	public static NamingStyle WithReverseGeocode()
	{
		return NamingStyle.Address;
	}
}
