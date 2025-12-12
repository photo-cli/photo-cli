using FluentValidation;

namespace PhotoCli.Options.Validators;

public class SharedReverseGeocodeValidator : BaseValidator<IReverseGeocodeOptions>
{
	public SharedReverseGeocodeValidator(Type optionType)
	{
		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.BigDataCloud, () =>
		{
			var bigDataCloudAdminLevels = GetOptionFormatByType(optionType, nameof(IReverseGeocodeOptions.BigDataCloudAdminLevels));
			var bigDataCloudProviderInfo = GetOptionFormatByTypeWithValue(optionType, nameof(IReverseGeocodeOptions.ReverseGeocodeProvider), nameof(ReverseGeocodeProvider.BigDataCloud));
			RuleFor(r => r.BigDataCloudAdminLevels).NotEmpty().WithMessage(MustUseMessage(bigDataCloudAdminLevels, bigDataCloudProviderInfo));
		});

		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.OpenStreetMapFoundation, () =>
		{
			RequireOpenStreetMapProperties(ReverseGeocodeProvider.OpenStreetMapFoundation, optionType);
		});

		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.LocationIq, () =>
		{
			RequireOpenStreetMapProperties(ReverseGeocodeProvider.LocationIq, optionType);
		});

		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.GoogleMaps, () =>
		{
			var googleMapsAddressTypes = GetOptionFormatByType(optionType, nameof(IReverseGeocodeOptions.GoogleMapsAddressTypes));
			var googleMapsProviderInfo = GetOptionFormatByTypeWithValue(optionType, nameof(IReverseGeocodeOptions.ReverseGeocodeProvider), nameof(ReverseGeocodeProvider.GoogleMaps));
			RuleFor(r => r.GoogleMapsAddressTypes).NotEmpty().WithMessage(MustUseMessage(googleMapsAddressTypes, googleMapsProviderInfo));
		});
	}

	private void RequireOpenStreetMapProperties(ReverseGeocodeProvider reverseGeocodeProvider, Type optionType)
	{
		var openStreetProperties = GetOptionFormatByType(optionType, nameof(IReverseGeocodeOptions.OpenStreetMapProperties));
		var reverseGeocodeProviderInfo = GetOptionFormatByTypeWithValue(optionType, nameof(IReverseGeocodeOptions.ReverseGeocodeProvider), reverseGeocodeProvider.ToString());
		RuleFor(r => r.OpenStreetMapProperties).NotEmpty().WithMessage(MustUseMessage(openStreetProperties, reverseGeocodeProviderInfo));
	}
}
