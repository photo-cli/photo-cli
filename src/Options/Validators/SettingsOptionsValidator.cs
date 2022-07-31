using FluentValidation;

namespace PhotoCli.Options.Validators;

public class SettingsOptionsValidator : BaseValidator<SettingsOptions>
{
	public SettingsOptionsValidator()
	{
		When(p => p.Value.IsPresent(), () => { RuleFor(r => r.Key).RequiredString(Required(nameof(SettingsOptions.Key), OptionNames.KeyOptionNameLong, OptionNames.KeyOptionNameShort)); });

		When(p => p.Reset, () =>
		{
			RuleFor(r => r.Key).Null();
			RuleFor(r => r.Value).Null();
		});
	}
}
