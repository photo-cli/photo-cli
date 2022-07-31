namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class CopyOptionsFluentValidationTests : BaseFluentValidationTests<CopyOptions, CopyOptionsValidator>
{
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
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessTypeFakes.OtherThanSingle(),
			folderAppendTypeOptional: FolderAppendTypeFakes.Valid());
		CheckPropertyNotNull(options, nameof(CopyOptions.FolderAppendLocationType), MustUseMessage(nameof(FolderAppendLocationType), nameof(FolderAppendType), "folder-append-location", 'p'));
	}

	[Fact]
	public void When_Using_FolderAppendLocationType_Not_Using_FolderAppendType_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessTypeFakes.OtherThanSingle(),
			folderAppendLocationTypeOptional: FolderAppendLocationTypeFakes.Valid());
		CheckPropertyNotNull(options, nameof(CopyOptions.FolderAppendType), MustUseMessage(nameof(FolderAppendType), nameof(FolderAppendLocationType), "folder-append", 'a'));
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
		CheckPropertyInvalidValue(options, nameof(CopyOptions.ReverseGeocodeProvider), MustUseMessage(nameof(ReverseGeocodeProvider), nameof(GroupByFolderType.AddressFlat), "reverse-geocode", 'e'));
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
		CheckPropertyInvalidValue(options, nameof(CopyOptions.ReverseGeocodeProvider), MustUseMessage(nameof(ReverseGeocodeProvider), nameof(NamingStyle.Address), "reverse-geocode", 'e'));
	}

	[Fact]
	public void When_Using_FolderAppendType_MatchingMinimumAddress_NotUsing_ReverseGeocodeProvider_Should_Give_PredicateValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.Create(folderAppendTypeOptional: FolderAppendType.MatchingMinimumAddress);
		CheckPropertyInvalidValue(options, nameof(CopyOptions.ReverseGeocodeProvider), MustUseMessage(nameof(ReverseGeocodeProvider), nameof(FolderAppendType.MatchingMinimumAddress),
			"reverse-geocode", 'e'));
	}

	#region ReverseGeocode Providers

	[Fact]
	public void When_Using_BigDataCloud_Not_Using_BigDataCloudAdminLevels_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var options = CopyOptionsFakes.WithReverseGeocodeService();
		CheckPropertyNotEmpty(options, nameof(CopyOptions.BigDataCloudAdminLevels), MustUseMessage(nameof(CopyOptions.BigDataCloudAdminLevels), nameof(ReverseGeocodeProvider.BigDataCloud),
			"bigdatacloud-levels", 'v'));
	}

	[Theory]
	[InlineData(ReverseGeocodeProvider.OpenStreetMapFoundation)]
	[InlineData(ReverseGeocodeProvider.MapQuest)]
	[InlineData(ReverseGeocodeProvider.LocationIq)]
	public void When_Using_OpenStreetMap_Not_Using_OpenStreetMapProperties_Should_Give_NullValidator_And_Verify_Error_Message(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		var commandLineOptions = CopyOptionsFakes.WithReverseGeocodeService(reverseGeocodeProvider);
		CheckPropertyNotEmpty(commandLineOptions, nameof(CopyOptions.OpenStreetMapProperties), MustUseMessage(nameof(CopyOptions.OpenStreetMapProperties), reverseGeocodeProvider.ToString(),
			"openstreetmap-properties", 'r'));
	}

	[Fact]
	public void When_Using_GoogleMaps_Not_Using_GoogleMapsAddressTypes_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var commandLineOptions = CopyOptionsFakes.WithReverseGeocodeService(ReverseGeocodeProvider.GoogleMaps);
		CheckPropertyNotEmpty(commandLineOptions, nameof(CopyOptions.GoogleMapsAddressTypes),
			MustUseMessage(nameof(CopyOptions.GoogleMapsAddressTypes), nameof(ReverseGeocodeProvider.GoogleMaps), "googlemaps-types", 'm'));
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
		CheckPropertyNotNull(commandLineOptions, nameof(CopyOptions.OutputPath),Required(nameof(CopyOptions.OutputPath), "output", 'o'));
	}

	[Fact]
	public void Unset_NamingStyle_Should_Give_PredicateValidator_And_Verify_Error_Message()
	{
		var commandLineOptions = CopyOptionsFakes.Create(namingStyleRequired: NamingStyle.Unset);
		CheckPropertyInvalidValue(commandLineOptions, nameof(CopyOptions.NamingStyle), Required(nameof(CopyOptions.NamingStyle), "naming-style", 's'));
	}

	[Fact]
	public void Unset_FolderProcessType_Should_Give_PredicateValidator_And_Verify_Error_Message()
	{
		var commandLineOptions = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.Unset);
		CheckPropertyInvalidValue(commandLineOptions, nameof(CopyOptions.FolderProcessType), Required(nameof(CopyOptions.FolderProcessType), "process-type", 'f'));
	}

	[Fact]
	public void Unset_NumberNaming_Should_Give_PredicateValidator_And_Verify_Error_Message()
	{
		var commandLineOptions = CopyOptionsFakes.Create(numberNamingTextStyleRequired: NumberNamingTextStyle.Unset);
		CheckPropertyInvalidValue(commandLineOptions, nameof(CopyOptions.NumberNamingTextStyle), Required(nameof(CopyOptions.NumberNamingTextStyle), "number-style", 'n'));
	}

	#endregion

	#region Enum Value Invalid Range

	[Fact]
	public void NamingStyle_Invalid_Range_Should_Give_EnumValidator_Error()
	{
		var commandLineOptions = CopyOptionsFakes.Create(namingStyleRequired: (NamingStyle)byte.MaxValue);
		CheckPropertyInvalidEnumValue(commandLineOptions, nameof(CopyOptions.NamingStyle));
	}

	[Fact]
	public void FolderProcessType_Invalid_Range_Should_Give_EnumValidator_Error()
	{
		var commandLineOptions = CopyOptionsFakes.Create(folderProcessTypeRequired: (FolderProcessType)byte.MaxValue);
		CheckPropertyInvalidEnumValue(commandLineOptions, nameof(CopyOptions.FolderProcessType));
	}

	[Fact]
	public void NoPhotoDateTimeTakenAction_Invalid_Range_Should_Give_EnumValidator_Error()
	{
		var commandLineOptions = CopyOptionsFakes.Create(noPhotoTakenDateActionOptional: (CopyNoPhotoTakenDateAction)byte.MaxValue);
		CheckPropertyInvalidEnumValue(commandLineOptions, nameof(CopyOptions.NoPhotoTakenDateAction));
	}

	[Fact]
	public void NumberNamingTextStyle_Invalid_Range_Should_Give_EnumValidator_Error()
	{
		var commandLineOptions = CopyOptionsFakes.Create(numberNamingTextStyleRequired: (NumberNamingTextStyle)byte.MaxValue);
		CheckPropertyInvalidEnumValue(commandLineOptions, nameof(CopyOptions.NumberNamingTextStyle));
	}

	[Fact]
	public void GroupByFolderType_Invalid_Range_Should_Give_EnumValidator_Error()
	{
		var commandLineOptions = CopyOptionsFakes.Create(
			folderProcessTypeRequired: FolderProcessTypeFakes.OtherThanSubFoldersPreserveFolderHierarchy(), groupByFolderTypeOptional: (GroupByFolderType)byte.MaxValue);
		CheckPropertyInvalidEnumValue(commandLineOptions, nameof(CopyOptions.GroupByFolderType));
	}

	[Fact]
	public void FolderAppendType_Invalid_Range_Should_Give_EnumValidator_Error()
	{
		var commandLineOptions = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy,
			folderAppendTypeOptional: (FolderAppendType)byte.MaxValue, folderAppendLocationTypeOptional: FolderAppendLocationTypeFakes.Valid());
		CheckPropertyInvalidEnumValue(commandLineOptions, nameof(CopyOptions.FolderAppendType));
	}

	[Fact]
	public void FolderAppendLocationType_Invalid_Range_Should_Give_EnumValidator_Error()
	{
		var commandLineOptions = CopyOptionsFakes.Create(folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy,
			folderAppendLocationTypeOptional: (FolderAppendLocationType)byte.MaxValue, folderAppendTypeOptional: FolderAppendTypeFakes.Valid());
		CheckPropertyInvalidEnumValue(commandLineOptions, nameof(CopyOptions.FolderAppendLocationType));
	}

	#endregion

	#endregion
}
