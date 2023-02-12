using FluentValidation;

namespace PhotoCli.Options.Validators;

public class ApiKeyStoreValidator : BaseValidator<ApiKeyStore>
{
	public ApiKeyStoreValidator()
	{
		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.BigDataCloud,
			() =>
			{
				RuleFor(r => r.BigDataCloud).NotNull().WithMessage(CantFindMessage(ReverseGeocodeProvider.BigDataCloud, ApiKeyStore.BigDataCloudApiKeyEnvironmentKey,
					OptionNames.BigDataCloudApiKeyOptionNameLong,
					OptionNames.BigDataCloudApiKeyOptionNameShort));
			});

		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.GoogleMaps,
			() =>
			{
				RuleFor(r => r.GoogleMaps).NotNull().WithMessage(CantFindMessage(ReverseGeocodeProvider.GoogleMaps, ApiKeyStore.GoogleMapsApiKeyEnvironmentKey,
					OptionNames.GoogleMapsApiKeyOptionNameLong,
					OptionNames.GoogleMapsApiKeyOptionNameShort));
			});

		When(w => w.ReverseGeocodeProvider is ReverseGeocodeProvider.LocationIq,
			() =>
			{
				RuleFor(r => r.LocationIq).NotNull().WithMessage(CantFindMessage(ReverseGeocodeProvider.LocationIq, ApiKeyStore.LocationIqApiKeyEnvironmentKey,
					OptionNames.LocationIqApiKeyOptionNameLong, OptionNames.LocationIqApiKeyOptionNameShort));
			});
	}
}
