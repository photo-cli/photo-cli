using FluentValidation;

namespace PhotoCli.Options.Validators;

public class SettingsOptionsValidator : BaseValidator<SettingsOptions>
{
	public SettingsOptionsValidator()
	{
		var keyInfo = GetOptionFormat(e => e.Key);

		When(p => p.Value.IsPresent(), () =>
		{
			RuleFor(r => r.Key).RequiredString(keyInfo);
		});

		When(p => p.Reset, () =>
		{
			RuleFor(r => r.Key).Null();
			RuleFor(r => r.Value).Null();
		});
	}
}
