using FluentValidation;

namespace PhotoCli.Options.Validators;

public class CopyOptionsValidator : BaseValidator<CopyOptions>
{
	public CopyOptionsValidator()
	{
		var optionType = typeof(CopyOptions);
		Include(new SharedReverseGeocodeValidator(optionType));
		Include(new ActionableReverseGeocodeValidator(optionType));

		var outputPathInfo = GetOptionFormat(e => e.OutputPath);
		var folderAppendLocationTypeInfo = GetOptionFormat(e => e.FolderAppendLocationType);
		var folderAppendTypeInfo = GetOptionFormat(e => e.FolderAppendType);
		var groupByFolderTypeWithAddressFlatInfo = GetOptionFormatWithValue(e => e.GroupByFolderType, nameof(GroupByFolderType.AddressFlat));
		var namingStyleWithAddressInfo = GetOptionFormatWithValue(e => e.NamingStyle, nameof(NamingStyle.Address));
		var folderAppendTypeWithMatchingMinimumAddressInfo = GetOptionFormatWithValue(e => e.FolderAppendType, nameof(FolderAppendType.MatchingMinimumAddress));
		var reverseGeocodeProviderInfo = GetOptionFormat(e => e.ReverseGeocodeProvider);
		var noCoordinateActionInfo = GetOptionFormat(e => e.NoCoordinateAction);

		RuleFor(r => r.OutputPath).RequiredString(outputPathInfo);

		RuleFor(r => r.NamingStyle).ValidEnum();
		RuleFor(r => r.FolderProcessType).ValidEnum();
		RuleFor(r => r.NumberNamingTextStyle).ValidEnum();
		RuleFor(r => r.InvalidFileFormatAction).ValidEnum(true);
		RuleFor(r => r.NoPhotoTakenDateAction).ValidEnum(true);
		RuleFor(r => r.NoCoordinateAction).ValidEnum(true);
		RuleFor(r => r.GroupByFolderType).ValidEnum();
		RuleFor(r => r.FolderAppendType).ValidEnum();
		RuleFor(r => r.FolderAppendLocationType).ValidEnum();

		When(w => w.FolderProcessType is FolderProcessType.Single, () =>
		{
			RuleFor(r => r.FolderAppendType).Null().WithMessage(CantUseMessage(nameof(FolderAppendType), nameof(FolderProcessType.Single)));
			RuleFor(r => r.FolderAppendLocationType).Null().WithMessage(CantUseMessage(nameof(FolderAppendLocationType), nameof(FolderProcessType.Single)));
		});

		When(w => w.FolderProcessType is FolderProcessType.FlattenAllSubFolders, () =>
		{
			RuleFor(r => r.FolderAppendType).Null().WithMessage(CantUseMessage(nameof(FolderAppendType), nameof(FolderProcessType.FlattenAllSubFolders)));
			RuleFor(r => r.FolderAppendLocationType).Null().WithMessage(CantUseMessage(nameof(FolderAppendLocationType), nameof(FolderProcessType.FlattenAllSubFolders)));
		});

		When(w => w.FolderProcessType is FolderProcessType.SubFoldersPreserveFolderHierarchy, () =>
		{
			RuleFor(r => r.GroupByFolderType).Null().WithMessage(CantUseMessage(nameof(GroupByFolderType), nameof(FolderProcessType.SubFoldersPreserveFolderHierarchy)));
		});

		When(w => w.FolderAppendType is not null && w.FolderProcessType is not FolderProcessType.Single && w.GroupByFolderType is null, () =>
		{
			RuleFor(r => r.FolderAppendLocationType).NotNull().WithMessage(MustUseMessage(folderAppendLocationTypeInfo, folderAppendTypeInfo));
		});

		When(w => w.FolderAppendLocationType is not null && w.FolderProcessType is not FolderProcessType.Single && w.GroupByFolderType is null, () =>
		{
			RuleFor(r => r.FolderAppendType).NotNull().WithMessage(MustUseMessage(folderAppendTypeInfo, folderAppendLocationTypeInfo));
		});

		When(w => w.GroupByFolderType is not null, () =>
		{
			RuleFor(r => r.FolderAppendType).Null().WithMessage(CantUseMessage(nameof(FolderAppendType), nameof(GroupByFolderType)));
			RuleFor(r => r.FolderAppendLocationType).Null().WithMessage(CantUseMessage(nameof(FolderAppendLocationType), nameof(GroupByFolderType)));
		});

		When(w => w.GroupByFolderType is GroupByFolderType.AddressFlat, () =>
			{
				ReverseGeocodeProviderAndCoordinateAction(groupByFolderTypeWithAddressFlatInfo, reverseGeocodeProviderInfo, noCoordinateActionInfo);
			});

		When(w => w.NamingStyle is NamingStyle.Address or NamingStyle.DayAddress or NamingStyle.AddressDay or NamingStyle.DateTimeWithMinutesAddress or NamingStyle.DateTimeWithSecondsAddress or
			NamingStyle.AddressDateTimeWithMinutes or NamingStyle.AddressDateTimeWithSeconds, () =>
		{
			ReverseGeocodeProviderAndCoordinateAction(namingStyleWithAddressInfo, reverseGeocodeProviderInfo, noCoordinateActionInfo);
		});

		When(w => w.FolderAppendType is FolderAppendType.MatchingMinimumAddress, () =>
		{
			ReverseGeocodeProviderAndCoordinateAction(folderAppendTypeWithMatchingMinimumAddressInfo, reverseGeocodeProviderInfo, noCoordinateActionInfo);
		});
	}

	private void ReverseGeocodeProviderAndCoordinateAction(string when, string reverseGeocodeInfo, string noCoordinateActionInfo)
	{
		RuleFor(r => r.ReverseGeocodeProvider).Must(m => m != ReverseGeocodeProvider.Disabled).WithMessage(MustUseMessage(reverseGeocodeInfo, when));
		RuleFor(r => r.NoCoordinateAction).NotNull().WithMessage(MustUseMessage(noCoordinateActionInfo, when));
	}
}
