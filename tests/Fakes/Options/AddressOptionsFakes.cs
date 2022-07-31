namespace PhotoCli.Tests.Fakes.Options;

public static class AddressOptionsFakes
{
	private const string InputPhotoPath = "photo.jpg";

	public static AddressOptions WithAddressListType(AddressListType type, ReverseGeocodeProvider? reverseGeocodeService = null)
	{
		return new AddressOptions(InputPhotoPath, reverseGeocodeService ?? ReverseGeocodeProviderFakes.Valid(), type);
	}

	public static AddressOptions Valid()
	{
		return new AddressOptions(InputPhotoPath, ReverseGeocodeProviderFakes.Valid(), AddressListTypeFakes.Valid());
	}

	public static AddressOptions WithReverseGeocodeService(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		return new AddressOptions(InputPhotoPath, reverseGeocodeProvider, AddressListTypeFakes.Valid());
	}
}
