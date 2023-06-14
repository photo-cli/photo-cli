namespace PhotoCli.Tests.Fakes.Options;

public static class AddressOptionsFakes
{
	public static AddressOptions WithAddressListType(AddressListType type, ReverseGeocodeProvider? reverseGeocodeService = null)
	{
		return new AddressOptions(FileNameFakes.ValidJpgInputPhotoPath, reverseGeocodeService ?? ReverseGeocodeProviderFakes.Valid(), type);
	}

	public static AddressOptions Valid()
	{
		return new AddressOptions(FileNameFakes.ValidJpgInputPhotoPath, ReverseGeocodeProviderFakes.Valid(), AddressListTypeFakes.Valid());
	}

	public static AddressOptions WithReverseGeocodeServiceAndAddressListType(ReverseGeocodeProvider reverseGeocodeProvider, AddressListType addressListType)
	{
		return new AddressOptions(FileNameFakes.ValidJpgInputPhotoPath, reverseGeocodeProvider, addressListType);
	}
}
