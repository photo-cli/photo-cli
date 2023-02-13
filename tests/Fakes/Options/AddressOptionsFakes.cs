namespace PhotoCli.Tests.Fakes.Options;

public static class AddressOptionsFakes
{
	public static AddressOptions WithAddressListType(AddressListType type, ReverseGeocodeProvider? reverseGeocodeService = null)
	{
		return new AddressOptions(FileNameFakes.ValidInputPhotoPath, reverseGeocodeService ?? ReverseGeocodeProviderFakes.Valid(), type);
	}

	public static AddressOptions Valid()
	{
		return new AddressOptions(FileNameFakes.ValidInputPhotoPath, ReverseGeocodeProviderFakes.Valid(), AddressListTypeFakes.Valid());
	}

	public static AddressOptions WithReverseGeocodeServiceAndAddressListType(ReverseGeocodeProvider reverseGeocodeProvider, AddressListType addressListType)
	{
		return new AddressOptions(FileNameFakes.ValidInputPhotoPath, reverseGeocodeProvider, addressListType);
	}
}
