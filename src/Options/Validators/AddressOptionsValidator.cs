using FluentValidation;

namespace PhotoCli.Options.Validators;

public class AddressOptionsValidator : BaseValidator<AddressOptions>
{
	public AddressOptionsValidator()
	{
		var inputPathInfo = GetOptionFormat(e => e.InputPath);

		RuleFor(r => r.InputPath)
			.RequiredString(inputPathInfo)
			.Matches(Constants.PhotoExtensionRegex).WithMessage($"{nameof(CopyOptions.InputPath)} should have .jpg, .jpeg, .heic or .hif extension");

		RuleFor(r => r.ReverseGeocodeProvider).ValidEnum();

		RuleFor(r => r.AddressListType).ValidEnum(true);

		When(w => w.AddressListType == AddressListType.SelectedProperties, () =>
		{
			Include(new SharedReverseGeocodeValidator(typeof(AddressOptions)));
		});
	}
}
