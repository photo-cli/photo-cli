namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class CopyOptionsFluentValidationTests : BaseFluentValidationTests<CopyOptions, CopyOptionsValidator>
{
	private const string FolderAppendTypeInfo = "FolderAppendType ( --folder-append or -a )";
	private const string FolderAppendLocationTypeInfo = "FolderAppendLocationType ( --folder-append-location or -p )";
	private const string ReverseGeocodeInfo = "ReverseGeocodeProvider ( --reverse-geocode or -e )";
	private const string FolderAppendTypeWithMatchingMinimumAddressInfo = "FolderAppendType ( --folder-append or -a ) with value MatchingMinimumAddress";
	private const string GroupByFolderTypeWithAddressFlatInfo = "GroupByFolderType ( --group-by or -g ) with value AddressFlat";

	#region Valid

	#region PhotoTakenDate

	#region Single

	public static TheoryData<CopyOptions> SingleFolderProcessTypePhotoTakenDateWithoutGrouping = new()
	{
		CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.Single)
	};

	public static TheoryData<CopyOptions> SingleFolderProcessTypePhotoTakenDateWithGrouping = new()
	{
		CopyOptionsFakes.Create(groupByFolderTypeOptional: GroupByFolderTypeFakes.Valid())
	};

	#endregion

	#region FlattenAllSubFolders

	public static TheoryData<CopyOptions> FlattenAllSubFoldersFolderProcessTypePhotoTakenDateWithoutGrouping = new()
	{
		CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.FlattenAllSubFolders)
	};

	public static TheoryData<CopyOptions> FlattenAllSubFoldersFolderProcessTypePhotoTakenDateWithGrouping = new()
	{
		CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.FlattenAllSubFolders, groupByFolderTypeOptional: GroupByFolderTypeFakes.Valid())
	};

	#endregion

	#region SubFoldersPreserveFolderHierarchy

	public static TheoryData<CopyOptions> SubFoldersPreserveFolderHierarchyFolderProcessTypePhotoTakenDateWithoutFolderAppend = new()
	{
		CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy)
	};

	public static TheoryData<CopyOptions> SubFoldersPreserveFolderHierarchyFolderProcessTypePhotoTakenDateWithFolderAppend = new()
	{
		CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy, folderAppendTypeOptional: FolderAppendTypeFakes.Valid(),
			folderAppendLocationTypeOptional: FolderAppendLocationTypeFakes.Valid())
	};

	#endregion

	#endregion

	#region ReverseGeocode

	#region Single

	public static TheoryData<CopyOptions> SingleFolderProcessTypeReverseGeocodeWithoutGrouping = new()
	{
		CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.Single, namingStyleRequired: NamingStyleFakes.WithReverseGeocode(),
			reverseGeoCodeProviderOptional: ReverseGeocodeProviderFakes.Valid(),
			bigDataCloudAdminLevelsOptional: BigDataCloudAdminLevelsFakes.Valid())
	};

	public static TheoryData<CopyOptions> SingleFolderProcessTypeReverseGeocodeWithGrouping = new()
	{
		CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.Single, namingStyleRequired: NamingStyleFakes.WithReverseGeocode(),
			groupByFolderTypeOptional: GroupByFolderTypeFakes.WithReverseGeocode(), reverseGeoCodeProviderOptional: ReverseGeocodeProviderFakes.Valid(),
			bigDataCloudAdminLevelsOptional: BigDataCloudAdminLevelsFakes.Valid()),
	};

	#endregion

	#region FlattenAllSubFolders

	public static TheoryData<CopyOptions> FlattenAllSubFoldersFolderProcessTypeReverseGeocodeWithoutGrouping = new()
	{
		CopyOptionsFakes.Create(namingStyleRequired: NamingStyleFakes.WithReverseGeocode(), folderProcessTypeRequired: FolderProcessType.FlattenAllSubFolders,
			reverseGeoCodeProviderOptional: ReverseGeocodeProviderFakes.Valid(), bigDataCloudAdminLevelsOptional: BigDataCloudAdminLevelsFakes.Valid())
	};

	public static TheoryData<CopyOptions> FlattenAllSubFoldersFolderProcessTypeReverseGeocodeWithGrouping = new()
	{
		CopyOptionsFakes.Create(namingStyleRequired: NamingStyleFakes.WithReverseGeocode(), folderProcessTypeRequired: FolderProcessType.FlattenAllSubFolders,
			groupByFolderTypeOptional: GroupByFolderTypeFakes.WithReverseGeocode(), reverseGeoCodeProviderOptional: ReverseGeocodeProviderFakes.Valid(),
			bigDataCloudAdminLevelsOptional: BigDataCloudAdminLevelsFakes.Valid())
	};

	#endregion

	#region SubFoldersPreserveFolderHierarchy

	public static TheoryData<CopyOptions> SubFoldersPreserveFolderHierarchyFolderProcessTypeReverseGeocodeWithoutFolderAppend = new()
	{
		CopyOptionsFakes.Create(namingStyleRequired: NamingStyleFakes.WithReverseGeocode(), folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy,
			reverseGeoCodeProviderOptional: ReverseGeocodeProviderFakes.Valid(), bigDataCloudAdminLevelsOptional: BigDataCloudAdminLevelsFakes.Valid())
	};

	public static TheoryData<CopyOptions> SubFoldersPreserveFolderHierarchyFolderProcessTypeReverseGeocodeWithFolderAppend = new()
	{
		CopyOptionsFakes.Create(namingStyleRequired: NamingStyleFakes.WithReverseGeocode(), folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy,
			folderAppendTypeOptional: FolderAppendTypeFakes.Valid(), folderAppendLocationTypeOptional: FolderAppendLocationTypeFakes.Valid(),
			reverseGeoCodeProviderOptional: ReverseGeocodeProviderFakes.Valid(),
			bigDataCloudAdminLevelsOptional: BigDataCloudAdminLevelsFakes.Valid())
	};

	#endregion

	#endregion

	[Theory]
	[MemberData(nameof(SingleFolderProcessTypePhotoTakenDateWithoutGrouping))]
	[MemberData(nameof(SingleFolderProcessTypePhotoTakenDateWithGrouping))]
	[MemberData(nameof(FlattenAllSubFoldersFolderProcessTypePhotoTakenDateWithoutGrouping))]
	[MemberData(nameof(FlattenAllSubFoldersFolderProcessTypePhotoTakenDateWithGrouping))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyFolderProcessTypePhotoTakenDateWithoutFolderAppend))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyFolderProcessTypePhotoTakenDateWithFolderAppend))]
	[MemberData(nameof(SingleFolderProcessTypeReverseGeocodeWithoutGrouping))]
	[MemberData(nameof(SingleFolderProcessTypeReverseGeocodeWithGrouping))]
	[MemberData(nameof(FlattenAllSubFoldersFolderProcessTypeReverseGeocodeWithoutGrouping))]
	[MemberData(nameof(FlattenAllSubFoldersFolderProcessTypeReverseGeocodeWithGrouping))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyFolderProcessTypeReverseGeocodeWithoutFolderAppend))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyFolderProcessTypeReverseGeocodeWithFolderAppend))]
	public void Valid_CommandLineOptions_Should_Have_NoError(CopyOptions copyOptions)
	{
		ValidationShouldHaveNoError(copyOptions);
	}

	#endregion

	#region Invalid

	#region Conditional

	[Fact]
	public void When_Using_Single_With_FolderAppendType_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.Single, folderAppendTypeOptional: FolderAppendTypeFakes.Valid());
		CheckPropertyNull(options, nameof(CopyOptions.FolderAppendType), CantUseMessage(nameof(FolderAppendType), nameof(FolderProcessType.Single)));
	}

	[Fact]
	public void When_Using_Single_With_FolderAppendLocationType_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.Single, folderAppendLocationTypeOptional: FolderAppendLocationTypeFakes.Valid());
		CheckPropertyNull(options, nameof(CopyOptions.FolderAppendLocationType), CantUseMessage(nameof(FolderAppendLocationType), nameof(FolderProcessType.Single)));
	}

	[Fact]
	public void When_Using_FlattenAllSubFolders_With_FolderAppendType_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.FlattenAllSubFolders,
			folderAppendTypeOptional: FolderAppendTypeFakes.Valid());
		CheckPropertyNull(options, nameof(CopyOptions.FolderAppendType), CantUseMessage(nameof(FolderAppendType), nameof(FolderProcessType.FlattenAllSubFolders)));
	}

	[Fact]
	public void When_Using_FlattenAllSubFolders_With_FolderAppendLocationType_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.FlattenAllSubFolders,
			folderAppendLocationTypeOptional: FolderAppendLocationTypeFakes.Valid());
		CheckPropertyNull(options, nameof(CopyOptions.FolderAppendLocationType),
			CantUseMessage(nameof(FolderAppendLocationType), nameof(FolderProcessType.FlattenAllSubFolders)));
	}

	[Fact]
	public void When_Using_SubFoldersPreserveFolderHierarchy_With_GroupByFolderType_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy,
			groupByFolderTypeOptional: GroupByFolderTypeFakes.Valid());
		CheckPropertyNull(options, nameof(CopyOptions.GroupByFolderType), CantUseMessage(nameof(GroupByFolderType), nameof(FolderProcessType.SubFoldersPreserveFolderHierarchy)));
	}

	[Fact]
	public void When_Using_FolderAppendType_Not_Using_FolderAppendLocationType_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessTypeFakes.OtherThanSingle(), folderAppendTypeOptional: FolderAppendTypeFakes.Valid());
		CheckPropertyNotNull(options, nameof(CopyOptions.FolderAppendLocationType), MustUseMessage(FolderAppendLocationTypeInfo, FolderAppendTypeInfo));
	}

	[Fact]
	public void When_Using_FolderAppendLocationType_Not_Using_FolderAppendType_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessTypeFakes.OtherThanSingle(), folderAppendLocationTypeOptional: FolderAppendLocationTypeFakes.Valid());
		CheckPropertyNotNull(options, nameof(CopyOptions.FolderAppendType), MustUseMessage(FolderAppendTypeInfo, FolderAppendLocationTypeInfo));
	}

	[Fact]
	public void When_Using_GroupByFolderType_Using_FolderAppendType_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy,
			groupByFolderTypeOptional: GroupByFolderTypeFakes.Valid(), folderAppendTypeOptional: FolderAppendTypeFakes.Valid());
		CheckPropertyNull(options, nameof(CopyOptions.FolderAppendType), CantUseMessage(nameof(FolderAppendType), nameof(GroupByFolderType)));
	}

	[Fact]
	public void When_Using_GroupByFolderType_Using_FolderAppendLocationType_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy,
			groupByFolderTypeOptional: GroupByFolderTypeFakes.Valid(), folderAppendLocationTypeOptional: FolderAppendLocationTypeFakes.Valid());
		CheckPropertyNull(options, nameof(CopyOptions.FolderAppendLocationType), CantUseMessage(nameof(FolderAppendLocationType), nameof(GroupByFolderType)));
	}

	[Fact]
	public void When_Using_GroupByFolderType_Address_NotUsing_ReverseGeocodeProvider_Should_Give_PredicateValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(groupByFolderTypeOptional: GroupByFolderType.AddressFlat);
		CheckPropertyInvalidValue(options, nameof(CopyOptions.ReverseGeocodeProvider), MustUseMessage(ReverseGeocodeInfo, GroupByFolderTypeWithAddressFlatInfo));
	}

	[Theory]
	[InlineData(NamingStyle.Address)]
	[InlineData(NamingStyle.AddressDay)]
	[InlineData(NamingStyle.DayAddress)]
	[InlineData(NamingStyle.AddressDateTimeWithMinutes)]
	[InlineData(NamingStyle.AddressDateTimeWithSeconds)]
	[InlineData(NamingStyle.DateTimeWithMinutesAddress)]
	[InlineData(NamingStyle.DateTimeWithSecondsAddress)]
	public void When_Using_NamingStyle_As_One_Of_Address_Options_NotUsing_ReverseGeocodeProvider_Should_Give_PredicateValidator_And_Verify_Error_Message(NamingStyle namingStyle)
	{
		var options = CopyOptionsFakes.Create(namingStyleRequired: namingStyle);
		var namingStyleAsAddressInfo = "NamingStyle ( --naming-style or -s ) with value Address";
		CheckPropertyInvalidValue(options, nameof(CopyOptions.ReverseGeocodeProvider), MustUseMessage(ReverseGeocodeInfo, namingStyleAsAddressInfo));
	}

	[Fact]
	public void When_Using_FolderAppendType_MatchingMinimumAddress_NotUsing_ReverseGeocodeProvider_Should_Give_PredicateValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderAppendTypeOptional: FolderAppendType.MatchingMinimumAddress);
		CheckPropertyInvalidValue(options, nameof(CopyOptions.ReverseGeocodeProvider), MustUseMessage(ReverseGeocodeInfo, FolderAppendTypeWithMatchingMinimumAddressInfo));
	}

	[Fact]
	public void WhenUsingMissingReverseGeocodeActionOtherThanContinueNotUsingReverseGeocodeProvider_ShouldGivePredicateValidatorAndVerifyErrorMessage()
	{
		var options = CopyOptionsFakes.Create(missingReverseGeocodeAction: MissingReverseGeocodeAction.PreventProcess);
		CheckPropertyInvalidValue(options, nameof(CopyOptions.ReverseGeocodeProvider), MustUseMessage(ReverseGeocodeProviderInfo, MissingReverseGeocodeActionInfo));
	}

	#region ReverseGeocode Providers

	[Fact]
	public void WhenUsingBigDataCloudWithoutBigDataCloudAdminLevels_ShouldGiveNullValidatorAndVerifyErrorMessage()
	{
		var options = CopyOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.BigDataCloud);
		CheckPropertyNotEmpty(options, nameof(CopyOptions.BigDataCloudAdminLevels), MustUseMessage(BigDataCloudAdminLevelInfo, ReverseGeocodeWithBigDataCloudInfo));
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public void WhenUsingOpenStreetMapWithoutOpenStreetMapProperties_ShouldGiveNullValidatorAndVerifyErrorMessage(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		var options = CopyOptionsFakes.WithReverseGeocodeService(reverseGeocodeProvider);
		CheckPropertyNotEmpty(options, nameof(CopyOptions.OpenStreetMapProperties), MustUseMessage(OpenStreetMapPropertiesInfo, ReverseGeocodeProviderInfoWithValue(reverseGeocodeProvider)));
	}

	[Fact]
	public void WhenUsingGoogleMapsWithoutGoogleMapsAddressTypes_ShouldGiveNullValidatorAndVerifyErrorMessage()
	{
		var options = CopyOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.GoogleMaps);
		CheckPropertyNotEmpty(options, nameof(CopyOptions.GoogleMapsAddressTypes), MustUseMessage(GoogleMapsAddressTypeInfo, ReverseGeocodeProviderWithGoogleMapsInfo));
	}

	#endregion

	#endregion

	#region Valid No Error

	public static TheoryData<CopyOptions> SingleFolderProcessTypeValidData = new()
	{
		CopyOptionsFakes.WithFolderProcessTypeSingle(),
		CopyOptionsFakes.WithFolderProcessTypeSingle(GroupByFolderTypeFakes.Valid()),
	};

	public static TheoryData<CopyOptions> FlattenAllSubFoldersFolderProcessTypeValidData = new()
	{
		CopyOptionsFakes.WithFolderProcessTypeFlattenAllSubFolders(),
		CopyOptionsFakes.WithFolderProcessTypeFlattenAllSubFolders(GroupByFolderTypeFakes.Valid()),
	};


	public static TheoryData<CopyOptions> SubFoldersPreserveFolderHierarchyFolderProcessTypeValidData = new()
	{
		CopyOptionsFakes.WithFolderProcessTypeSubFoldersPreserveFolderHierarchy(),
		CopyOptionsFakes.WithFolderProcessTypeSubFoldersPreserveFolderHierarchy(FolderAppendTypeFakes.Valid(), FolderAppendLocationTypeFakes.Valid()),
	};

	[Theory]
	[MemberData(nameof(SingleFolderProcessTypeValidData))]
	[MemberData(nameof(FlattenAllSubFoldersFolderProcessTypeValidData))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyFolderProcessTypeValidData))]
	public void Given_Valid_Options_Should_Have_No_Error(CopyOptions copyOptions)
	{
		ValidationShouldHaveNoError(copyOptions);
	}

	#endregion

	#region Required

	[Fact]
	public void Null_OutputFolderPath_Should_Give_NotNullValidator_Error()
	{
		var commandLineOptions = CopyOptionsFakes.Create(null!);
		CheckPropertyRequiredString(commandLineOptions, nameof(CopyOptions.OutputPath), Required(nameof(CopyOptions.OutputPath), "output", 'o'));
	}

	#endregion

	#region Enum Value Invalid Range

	[Fact]
	public void InvalidRangeForNamingStyle_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(namingStyleRequired: (NamingStyle)byte.MaxValue);
		CheckEnumInvalidRangeValue<NamingStyle>(options, nameof(CopyOptions.NamingStyle), true);
	}

	[Fact]
	public void InvalidRangeForFolderProcessType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: (FolderProcessType)byte.MaxValue);
		CheckEnumInvalidRangeValue<FolderProcessType>(options, nameof(CopyOptions.FolderProcessType), true);
	}

	[Fact]
	public void InvalidRangeForNumberNamingTextStyle_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(numberNamingTextStyleRequired: (NumberNamingTextStyle)byte.MaxValue);
		CheckEnumInvalidRangeValue<NumberNamingTextStyle>(options, nameof(CopyOptions.NumberNamingTextStyle), true);
	}

	[Fact]
	public void InvalidRangeForCopyInvalidFormatAction_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(invalidFormatActionOptional: (CopyInvalidFormatAction)byte.MaxValue);
		CheckEnumInvalidRangeValue<CopyInvalidFormatAction>(options, nameof(CopyOptions.InvalidFileFormatAction), false);
	}

	[Fact]
	public void InvalidRangeForNoPhotoTakenDateAction_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(noPhotoTakenDateActionOptional: (CopyNoPhotoTakenDateAction)byte.MaxValue);
		CheckEnumInvalidRangeValue<CopyNoPhotoTakenDateAction>(options, nameof(CopyOptions.NoPhotoTakenDateAction), false);
	}

	[Fact]
	public void InvalidRangeForNoCoordinateAction_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(noCoordinateActionOptional: (CopyNoCoordinateAction)byte.MaxValue);
		CheckEnumInvalidRangeValue<CopyNoCoordinateAction>(options, nameof(CopyOptions.NoCoordinateAction), false);
	}

	[Fact]
	public void InvalidRangeForGroupByFolderType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessTypeFakes.OtherThanSubFoldersPreserveFolderHierarchy(),
			groupByFolderTypeOptional: (GroupByFolderType)byte.MaxValue);

		CheckEnumInvalidRangeValue<GroupByFolderType>(options, nameof(CopyOptions.GroupByFolderType), true);
	}

	[Fact]
	public void InvalidRangeForFolderAppendType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy,
			folderAppendTypeOptional: (FolderAppendType)byte.MaxValue, folderAppendLocationTypeOptional: FolderAppendLocationTypeFakes.Valid());

		CheckEnumInvalidRangeValue<FolderAppendType>(options, nameof(CopyOptions.FolderAppendType), true);
	}

	[Fact]
	public void InvalidRangeForFolderAppendLocationType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy,
			folderAppendLocationTypeOptional: (FolderAppendLocationType)byte.MaxValue, folderAppendTypeOptional: FolderAppendTypeFakes.Valid());

		CheckEnumInvalidRangeValue<FolderAppendLocationType>(options, nameof(CopyOptions.FolderAppendLocationType), true);
	}

	#endregion

	#region Enum Invalid Default Value

	[Fact]
	public void InvalidDefaultValueForAlbumType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(namingStyleRequired: 0);
		CheckEnumInvalidRangeValue<NamingStyle>(options, nameof(CopyOptions.NamingStyle), true);
	}

	[Fact]
	public void InvalidDefaultValueForFolderProcessType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: 0);
		CheckEnumInvalidRangeValue<FolderProcessType>(options, nameof(CopyOptions.FolderProcessType), true);
	}

	[Fact]
	public void InvalidDefaultValueForNumberNamingTextStyle_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(numberNamingTextStyleRequired: 0);
		CheckEnumInvalidRangeValue<NumberNamingTextStyle>(options, nameof(CopyOptions.NumberNamingTextStyle), true);
	}

	[Fact]
	public void InvalidDefaultValueForGroupByFolderType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(groupByFolderTypeOptional: 0);
		CheckEnumInvalidRangeValue<GroupByFolderType>(options, nameof(CopyOptions.GroupByFolderType), true);
	}

	[Fact]
	public void InvalidDefaultValueForFolderAppendType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(folderAppendTypeOptional: 0);
		CheckEnumInvalidRangeValue<FolderAppendType>(options, nameof(CopyOptions.FolderAppendType), true);
	}

	[Fact]
	public void InvalidDefaultValueForFolderAppendLocationType_ShouldGiveValidEnumValidatorErrorWithExpectedErrorMessageDisplayingValidOptions()
	{
		var options = CopyOptionsFakes.Create(folderAppendLocationTypeOptional: 0);
		CheckEnumInvalidRangeValue<FolderAppendLocationType>(options, nameof(CopyOptions.FolderAppendLocationType), true);
	}

	#endregion

	#endregion

	protected override CopyOptionsValidator CreateValidator()
	{
		return new CopyOptionsValidator();
	}
}
