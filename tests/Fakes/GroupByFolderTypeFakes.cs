namespace PhotoCli.Tests.Fakes;

public static class GroupByFolderTypeFakes
{
	public static GroupByFolderType Valid()
	{
		return GroupByFolderType.YearMonthDay;
	}

	public static GroupByFolderType WithReverseGeocode()
	{
		return GroupByFolderType.AddressFlat;
	}
}
