using FluentValidation;

namespace PhotoCli.Options.Validators;

public class AddressOptionsValidator : BaseValidator<AddressOptions>
{
	public AddressOptionsValidator()
	{
		RuleFor(r => r.InputPath).NotNull().WithMessage(Required(nameof(CopyOptions.InputPath), OptionNames.InputPathOptionNameLong, OptionNames.InputPathOptionNameShort))
			.Matches(Constants.PhotoExtensionRegex).WithMessage($"{nameof(CopyOptions.InputPath)} should have .jpg, .jpeg, or .heic extension");

		RuleFor(r => r.ReverseGeocodeProvider).Must(m => m != ReverseGeocodeProvider.Disabled)
			.WithMessage(Required(nameof(ReverseGeocodeProvider), OptionNames.ReverseGeocodeProvidersOptionNameLong, OptionNames.ReverseGeocodeProvidersOptionNameShort));

		RuleFor(r => r.AddressListType).IsInEnum();
		When(w => w.AddressListType == AddressListType.SelectedProperties, () => { Include(new SharedReverseGeocodeValidator()); });
	}
}
