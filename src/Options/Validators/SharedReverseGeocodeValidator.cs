using FluentValidation;

namespace PhotoCli.Options.Validators;

public class SharedReverseGeocodeValidator : BaseValidator<IReverseGeocodeOptions>
{
	public SharedReverseGeocodeValidator()
	{
		#region ReverseGeocode Providers

		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.BigDataCloud,
			() =>
			{
				RuleFor(r => r.BigDataCloudAdminLevels).NotEmpty().WithMessage(MustUseMessage(nameof(IReverseGeocodeOptions.BigDataCloudAdminLevels), nameof(ReverseGeocodeProvider.BigDataCloud),
					OptionNames.BigDataCloudAdminLevelsOptionNameLong, OptionNames.BigDataCloudAdminLevelsOptionNameShort));
			});

		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.OpenStreetMapFoundation, () => { RequireOpenStreetMapProperties(ReverseGeocodeProvider.OpenStreetMapFoundation); });
		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.LocationIq, () => { RequireOpenStreetMapProperties(ReverseGeocodeProvider.LocationIq); });

		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.GoogleMaps,
			() =>
			{
				RuleFor(r => r.GoogleMapsAddressTypes).NotEmpty().WithMessage(MustUseMessage(nameof(IReverseGeocodeOptions.GoogleMapsAddressTypes), nameof(ReverseGeocodeProvider.GoogleMaps),
					OptionNames.GoogleMapsAddressTypesOptionNameLong, OptionNames.GoogleMapsAddressTypesOptionNameShort));
			});

		#endregion
	}

	private void RequireOpenStreetMapProperties(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		RuleFor(r => r.OpenStreetMapProperties).NotEmpty().WithMessage(MustUseMessage(nameof(IReverseGeocodeOptions.OpenStreetMapProperties), reverseGeocodeProvider.ToString(),
			OptionNames.OpenStreetMapPropertiesOptionNameLong, OptionNames.OpenStreetMapPropertiesOptionNameShort));
	}
}
