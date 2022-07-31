using FluentValidation;

namespace PhotoCli.Options.Validators;

public class CopyOptionsValidator : BaseValidator<CopyOptions>
{
	public CopyOptionsValidator()
	{
		Include(new SharedReverseGeocodeValidator());

		RuleFor(r => r.OutputPath).NotNull().WithMessage(Required(nameof(CopyOptions.OutputPath),
			OptionNames.OutputPathOptionNameLong, OptionNames.OutputPathOptionNameShort));

		RuleFor(r => r.NamingStyle).NotNull().IsInEnum().Must(m => m != NamingStyle.Unset)
			.WithMessage(Required(nameof(CopyOptions.NamingStyle), OptionNames.NamingStyleOptionNameLong, OptionNames.NamingStyleOptionNameShort));

		RuleFor(r => r.FolderProcessType).NotNull().IsInEnum().Must(m => m != FolderProcessType.Unset).WithMessage(Required(nameof(CopyOptions.FolderProcessType),
			OptionNames.FolderProcessTypeOptionNameLong, OptionNames.FolderProcessTypeOptionNameShort));

		RuleFor(r => r.NumberNamingTextStyle).NotNull().IsInEnum().Must(m => m != NumberNamingTextStyle.Unset).WithMessage(Required(nameof(CopyOptions.NumberNamingTextStyle),
			OptionNames.NumberNamingTextStyleOptionNameLong, OptionNames.NumberNamingTextStyleOptionNameShort));

		RuleFor(r => r.NoPhotoTakenDateAction).IsInEnum();
		RuleFor(r => r.NoCoordinateAction).IsInEnum();

		RuleFor(r => r.GroupByFolderType).IsInEnum().Must(m => m != GroupByFolderType.Unset);
		RuleFor(r => r.FolderAppendType).IsInEnum().Must(m => m != FolderAppendType.Unset);
		RuleFor(r => r.FolderAppendLocationType).IsInEnum().Must(m => m != FolderAppendLocationType.Unset);

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

		When(w => w.FolderProcessType is FolderProcessType.SubFoldersPreserveFolderHierarchy,
			() => { RuleFor(r => r.GroupByFolderType).Null().WithMessage(CantUseMessage(nameof(GroupByFolderType), nameof(FolderProcessType.SubFoldersPreserveFolderHierarchy))); });

		When(w => w.FolderAppendType is not null && w.FolderProcessType is not FolderProcessType.Single && w.GroupByFolderType is null,
			() =>
			{
				RuleFor(r => r.FolderAppendLocationType).NotNull().WithMessage(MustUseMessage(nameof(FolderAppendLocationType), nameof(FolderAppendType),
					OptionNames.FolderAppendLocationTypeOptionNameLong, OptionNames.FolderAppendLocationTypeOptionNameShort));
			});

		When(w => w.FolderAppendLocationType is not null && w.FolderProcessType is not FolderProcessType.Single && w.GroupByFolderType is null,
			() =>
			{
				RuleFor(r => r.FolderAppendType).NotNull().WithMessage(MustUseMessage(nameof(FolderAppendType), nameof(FolderAppendLocationType), OptionNames.FolderAppendTypeOptionNameLong,
					OptionNames.FolderAppendTypeOptionNameShort));
			});

		When(w => w.GroupByFolderType is not null, () =>
		{
			RuleFor(r => r.FolderAppendType).Null().WithMessage(CantUseMessage(nameof(FolderAppendType), nameof(GroupByFolderType)));
			RuleFor(r => r.FolderAppendLocationType).Null().WithMessage(CantUseMessage(nameof(FolderAppendLocationType), nameof(GroupByFolderType)));
		});

		When(w => w.GroupByFolderType is GroupByFolderType.AddressFlat, () => ReverseGeocodeProviderAndCoordinateAction(nameof(GroupByFolderType.AddressFlat)));

		When(w => w.NamingStyle is NamingStyle.Address or NamingStyle.DayAddress or NamingStyle.AddressDay or NamingStyle.DateTimeWithMinutesAddress or NamingStyle.DateTimeWithSecondsAddress or
			NamingStyle.AddressDateTimeWithMinutes or NamingStyle.AddressDateTimeWithSeconds, () => ReverseGeocodeProviderAndCoordinateAction(nameof(NamingStyle.Address)));

		When(w => w.FolderAppendType is FolderAppendType.MatchingMinimumAddress, () => ReverseGeocodeProviderAndCoordinateAction(nameof(FolderAppendType.MatchingMinimumAddress)));
	}

	private void ReverseGeocodeProviderAndCoordinateAction(string when)
	{
		RuleFor(r => r.ReverseGeocodeProvider).Must(m => m != ReverseGeocodeProvider.Disabled).WithMessage(MustUseMessage(nameof(ReverseGeocodeProvider), when,
			OptionNames.ReverseGeocodeProvidersOptionNameLong, OptionNames.ReverseGeocodeProvidersOptionNameShort));
		RuleFor(r => r.NoCoordinateAction).NotNull().WithMessage(MustUseMessage(nameof(CopyNoCoordinateAction), when, OptionNames.CopyNoCoordinateActionOptionNameLong,
			OptionNames.CopyNoCoordinateActionOptionNameShort));
	}
}
