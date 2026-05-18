using FluentValidation;

namespace PhotoCli.Options.Validators;

public class ActionableReverseGeocodeValidator : BaseValidator<IActionableReverseGeocodeOptions>
{
	public ActionableReverseGeocodeValidator(Type optionType)
	{
		When(w => w.MissingReverseGeocodeAction is not MissingReverseGeocodeAction.Continue, () =>
		{
			var reverseGeocodeProviderInfo = GetOptionFormatByType(optionType, nameof(IActionableReverseGeocodeOptions.ReverseGeocodeProvider));
			var missingReverseGeocodeActionInfo = GetOptionFormatByType(optionType, nameof(IActionableReverseGeocodeOptions.MissingReverseGeocodeAction));

			RuleFor(r => r.ReverseGeocodeProvider)
				.Must(m => m != ReverseGeocodeProvider.Disabled)
				.WithMessage(MustUseMessage(reverseGeocodeProviderInfo, missingReverseGeocodeActionInfo));
		});
	}
}
