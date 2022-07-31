namespace PhotoCli.Tests.UnitTests.Services;

public class DirectoryGrouperServiceUnitTests
{
	#region SubFoldersPreserveFolderHierarchy

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>> AllFilesLocatedInSourceRootShouldGroupedInRootPath = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ SourceRootPath("f1.jpg"), ExifDataFakes.WithDay(1) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					string.Empty,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), string.Empty),
					}
				}
			}
		},
		{
			new Dictionary<string, ExifData>
			{
				{ SourceRootPath("f1.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceRootPath("f2.jpg"), ExifDataFakes.WithDay(2) }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					string.Empty,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), string.Empty),
						SourceRootPhotoInfo("f2.jpg", ExifDataFakes.WithDay(2), string.Empty),
					}
				}
			}
		}
	};

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>> AllFilesLocatedInSubFolderShouldGroupedInTheirRelativePath = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ SourceSubPath("f1.jpg"), ExifDataFakes.WithDay(1) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					SubPath,
					new List<Photo>
					{
						SourceSubPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), SubPath),
					}
				}
			}
		},
		{
			new Dictionary<string, ExifData>
			{
				{ SourceSubPath("f1.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceSubPath("f2.jpg"), ExifDataFakes.WithDay(2) }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					SubPath,
					new List<Photo>
					{
						SourceSubPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), SubPath),
						SourceSubPhotoInfo("f2.jpg", ExifDataFakes.WithDay(2), SubPath),
					}
				}
			}
		}
	};

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>> FilesLocatedInRootAndSubFoldersShouldGroupedInTheirRelativePath = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithDay(2) }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					string.Empty,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDay(1), string.Empty),
					}
				},
				{
					SubPath,
					new List<Photo>
					{
						SourceSubPhotoInfo("f2-sub.jpg", ExifDataFakes.WithDay(2), SubPath),
					}
				}
			}
		}
	};

	[Theory]
	[MemberData(nameof(AllFilesLocatedInSourceRootShouldGroupedInRootPath))]
	[MemberData(nameof(AllFilesLocatedInSubFolderShouldGroupedInTheirRelativePath))]
	[MemberData(nameof(FilesLocatedInRootAndSubFoldersShouldGroupedInTheirRelativePath))]
	public void SubFoldersPreserveFolderHierarchy_GroupFilesByRelativeDirectory_Be_EquivalentTo(Dictionary<string, ExifData> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.SubFoldersPreserveFolderHierarchy, null, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>, bool, bool>
		FilesNotHavePhotoTakenDateShouldGroupedInSubFolderIfNoPhotoDateTimeTakenGroupedInSubFolderIsTrue = new()
		{
			{
				new Dictionary<string, ExifData>
				{
					{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDay(1) },
					{ SourceRootPath("f1-root-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
					{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithDay(2) },
					{ SourceSubPath("f2-sub-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() }
				},
				new Dictionary<string, List<Photo>>
				{
					{
						string.Empty,
						new List<Photo>
						{
							SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDay(1), string.Empty),
						}
					},
					{
						ToolOptionFakes.NoPhotoTakenDateFolderName,
						new List<Photo>
						{
							SourceRootPhotoInfo("f1-root-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(), ToolOptionFakes.NoPhotoTakenDateFolderName),
						}
					},
					{
						SubPath,
						new List<Photo>
						{
							SourceSubPhotoInfo("f2-sub.jpg", ExifDataFakes.WithDay(2), SubPath),
						}
					},
					{
						MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoPhotoTakenDateFolderName),
						new List<Photo>
						{
							SourceSubPhotoInfo("f2-sub-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(),
								MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoPhotoTakenDateFolderName)),
						}
					},
				},
				true,
				false
			}
		};

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>, bool, bool> FilesNotHaveGeocodeShouldGroupedInSubFolderIfNoReverseGeocodeGroupedInSubFolderIsTrue =
		new()
		{
			{
				new Dictionary<string, ExifData>
				{
					{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithReverseGeocodeSampleId(1) },
					{ SourceRootPath("f1-root-no-reverse-geocode.jpg"), ExifDataFakes.WithNoReverseGeocode() },
					{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithReverseGeocodeSampleId(2) },
					{ SourceSubPath("f2-sub-no-reverse-geocode.jpg"), ExifDataFakes.WithNoReverseGeocode() }
				},
				new Dictionary<string, List<Photo>>
				{
					{
						string.Empty,
						new List<Photo>
						{
							SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithReverseGeocodeSampleId(1), string.Empty),
						}
					},
					{
						ToolOptionFakes.NoAddressFolderName,
						new List<Photo>
						{
							SourceRootPhotoInfo("f1-root-no-reverse-geocode.jpg", ExifDataFakes.WithNoReverseGeocode(), ToolOptionFakes.NoAddressFolderName),
						}
					},
					{
						SubPath,
						new List<Photo>
						{
							SourceSubPhotoInfo("f2-sub.jpg", ExifDataFakes.WithReverseGeocodeSampleId(2), SubPath),
						}
					},
					{
						MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressFolderName),
						new List<Photo>
						{
							SourceSubPhotoInfo("f2-sub-no-reverse-geocode.jpg", ExifDataFakes.WithNoReverseGeocode(),
								MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressFolderName)),
						}
					},
				},
				false,
				true
			}
		};

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>, bool, bool>
		FilesNotHaveGeocodeAndPhotoTakenDateShouldGroupedInSubFolderIfNoReverseGeocodeGroupedInSubFolderAndNoPhotoDateTimeTakenGroupedInSubFolderIsTrue = new()
		{
			{
				new Dictionary<string, ExifData>
				{
					{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1) },
					{ SourceRootPath("f1-root-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
					{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2) },
					{ SourceSubPath("f2-sub-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() }
				},
				new Dictionary<string, List<Photo>>
				{
					{
						string.Empty,
						new List<Photo>
						{
							SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), string.Empty),
						}
					},
					{
						ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName,
						new List<Photo>
						{
							SourceRootPhotoInfo("f1-root-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(),
								ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName),
						}
					},
					{
						SubPath,
						new List<Photo>
						{
							SourceSubPhotoInfo("f2-sub.jpg", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2), SubPath),
						}
					},
					{
						MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName),
						new List<Photo>
						{
							SourceSubPhotoInfo("f2-sub-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(),
								MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName)),
						}
					},
				},
				true,
				true
			}
		};

	[Theory]
	[MemberData(nameof(FilesNotHavePhotoTakenDateShouldGroupedInSubFolderIfNoPhotoDateTimeTakenGroupedInSubFolderIsTrue))]
	[MemberData(nameof(FilesNotHaveGeocodeShouldGroupedInSubFolderIfNoReverseGeocodeGroupedInSubFolderIsTrue))]
	[MemberData(nameof(FilesNotHaveGeocodeAndPhotoTakenDateShouldGroupedInSubFolderIfNoReverseGeocodeGroupedInSubFolderAndNoPhotoDateTimeTakenGroupedInSubFolderIsTrue))]
	public void SubFoldersPreserveFolderHierarchy_GroupFilesByRelativeDirectory_Be_EquivalentTo_With_No_Action_Parameter(Dictionary<string, ExifData> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory, bool noPhotoDateTimeTakenGroupedInSubFolder, bool noReverseGeocodeGroupedInSubFolder)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.SubFoldersPreserveFolderHierarchy, null, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory,
			noPhotoDateTimeTakenGroupedInSubFolder, noReverseGeocodeGroupedInSubFolder);
	}

	#endregion

	#region Single

	[Theory]
	[MemberData(nameof(AllFilesLocatedInSourceRootShouldGroupedInRootPath))]
	public void Single_GroupFilesByRelativeDirectory_Be_EquivalentTo(Dictionary<string, ExifData> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.Single, null, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	public static TheoryData<Dictionary<string, ExifData>> SendingFilesLocatedOnSubFoldersOnSingleFolderProcessTypeShouldThrowPhotoOrganizerToolExceptionData = new()
	{
		new Dictionary<string, ExifData>
		{
			{ SourceSubPath("f-sub.jpg"), ExifDataFakes.WithDay(2) }
		},
		new Dictionary<string, ExifData>
		{
			{ SourceRootPath("f-root.jpg"), ExifDataFakes.WithDay(1) },
			{ SourceSubPath("f-sub.jpg"), ExifDataFakes.WithDay(2) }
		}
	};

	[Theory]
	[MemberData(nameof(SendingFilesLocatedOnSubFoldersOnSingleFolderProcessTypeShouldThrowPhotoOrganizerToolExceptionData))]
	public void Sending_Files_Located_On_Sub_Folders_On_Single_FolderProcessType_Should_Throw_PhotoOrganizerToolException(Dictionary<string, ExifData> photoExifDataByFilePath)
	{
		Assert.Throws<PhotoCliException>(() =>
			GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.Single, null, photoExifDataByFilePath, new Dictionary<string, List<Photo>>())
		);
	}

	#endregion

	#region FlattenAllSubFolders

	#region No FlattenType

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>> AllFilesLocatedInSubFolderShouldFlattenedIntoRootPath = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ SourceSubPath("f1.jpg"), ExifDataFakes.WithDay(1) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					string.Empty,
					new List<Photo>
					{
						SourceSubPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), string.Empty),
					}
				}
			}
		},
		{
			new Dictionary<string, ExifData>
			{
				{ SourceSubPath("f1.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceSubPath("f2.jpg"), ExifDataFakes.WithDay(2) }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					string.Empty,
					new List<Photo>
					{
						SourceSubPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), string.Empty),
						SourceSubPhotoInfo("f2.jpg", ExifDataFakes.WithDay(2), string.Empty),
					}
				}
			}
		}
	};

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>> FilesLocatedInRootAndSubFoldersShouldFlattenedIntoRootPath = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithDay(2) }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					string.Empty,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDay(1), string.Empty),
						SourceSubPhotoInfo("f2-sub.jpg", ExifDataFakes.WithDay(2), string.Empty),
					}
				},
			}
		}
	};

	[Theory]
	[MemberData(nameof(AllFilesLocatedInSourceRootShouldGroupedInRootPath))]
	[MemberData(nameof(AllFilesLocatedInSubFolderShouldFlattenedIntoRootPath))]
	[MemberData(nameof(FilesLocatedInRootAndSubFoldersShouldFlattenedIntoRootPath))]
	public void FlattenAllSubFolders_GroupFilesByRelativeDirectory_Be_EquivalentTo(Dictionary<string, ExifData> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, null, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region Year

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoYear = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ SourceRootPath("f1.jpg"), ExifDataFakes.WithYear(2001) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					new List<Photo>
					{
						SourceRootPhotoInfo("f1.jpg", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001)),
					}
				}
			}
		},
		{
			new Dictionary<string, ExifData>
			{
				{ SourceRootPath("f1-1.jpg"), ExifDataFakes.WithYear(2001) },
				{ SourceSubPath("f2.jpg"), ExifDataFakes.WithYear(2002) },
				{ SourceSubPath("f1-2.jpg"), ExifDataFakes.WithYear(2001) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-1.jpg", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001)),
						SourceSubPhotoInfo("f1-2.jpg", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001)),
					}
				},
				{
					DateTimeFakes.FormatYear(2002),
					new List<Photo>
					{
						SourceSubPhotoInfo("f2.jpg", ExifDataFakes.WithYear(2002), DateTimeFakes.FormatYear(2002)),
					}
				}
			}
		}
	};

	[Theory]
	[MemberData(nameof(AllFilesFlattenedAndGroupIntoYear))]
	public void FlattenAllSubFoldersThenGroupByYear_GroupFilesByRelativeDirectory_Be_EquivalentTo(Dictionary<string, ExifData> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.Year, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region YearMonth

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoYearMonth = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ SourceRootPath("f1.jpg"), ExifDataFakes.WithMonth(1) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.DirectoryFormatMonth(1),
					new List<Photo>
					{
						SourceRootPhotoInfo("f1.jpg", ExifDataFakes.WithMonth(1), DateTimeFakes.DirectoryFormatMonth(1)),
					}
				}
			}
		},
		{
			new Dictionary<string, ExifData>
			{
				{ SourceSubPath("f1-1.jpg"), ExifDataFakes.WithMonth(1) },
				{ SourceRootPath("f2.jpg"), ExifDataFakes.WithMonth(2) },
				{ SourceRootPath("f1-2.jpg"), ExifDataFakes.WithMonth(1) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.DirectoryFormatMonth(1),
					new List<Photo>
					{
						SourceSubPhotoInfo("f1-1.jpg", ExifDataFakes.WithMonth(1), DateTimeFakes.DirectoryFormatMonth(1)),
						SourceRootPhotoInfo("f1-2.jpg", ExifDataFakes.WithMonth(1), DateTimeFakes.DirectoryFormatMonth(1)),
					}
				},
				{
					DateTimeFakes.DirectoryFormatMonth(2),
					new List<Photo>
					{
						SourceRootPhotoInfo("f2.jpg", ExifDataFakes.WithMonth(2), DateTimeFakes.DirectoryFormatMonth(2)),
					}
				}
			}
		}
	};

	[Theory]
	[MemberData(nameof(AllFilesFlattenedAndGroupIntoYearMonth))]
	public void FlattenAllSubFoldersThenGroupByYearMonth_GroupFilesByRelativeDirectory_Be_EquivalentTo(Dictionary<string, ExifData> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.YearMonth, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region YearMonthDay

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoYearMonthDay = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ SourceSubPath("f1.jpg"), ExifDataFakes.WithDay(1) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.DirectoryFormatDay(1),
					new List<Photo>
					{
						SourceSubPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), DateTimeFakes.DirectoryFormatDay(1)),
					}
				}
			}
		},
		{
			new Dictionary<string, ExifData>
			{
				{ SourceRootPath("f1-1.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceSubPath("f2.jpg"), ExifDataFakes.WithDay(2) },
				{ SourceRootPath("f1-2.jpg"), ExifDataFakes.WithDay(1) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.DirectoryFormatDay(1),
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-1.jpg", ExifDataFakes.WithDay(1), DateTimeFakes.DirectoryFormatDay(1)),
						SourceRootPhotoInfo("f1-2.jpg", ExifDataFakes.WithDay(1), DateTimeFakes.DirectoryFormatDay(1)),
					}
				},
				{
					DateTimeFakes.DirectoryFormatDay(2),
					new List<Photo>
					{
						SourceSubPhotoInfo("f2.jpg", ExifDataFakes.WithDay(2), DateTimeFakes.DirectoryFormatDay(2)),
					}
				}
			}
		}
	};

	[Theory]
	[MemberData(nameof(AllFilesFlattenedAndGroupIntoYearMonthDay))]
	public void FlattenAllSubFoldersThenGroupByYearMonthDay_GroupFilesByRelativeDirectory_Be_EquivalentTo(Dictionary<string, ExifData> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.YearMonthDay, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region AddressFlat

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoAddressFlat = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ SourceSubPath("f2-1.jpg"), ExifDataFakes.WithReverseGeocodeSampleId(2) },
				{ SourceSubPath("f1.jpg"), ExifDataFakes.WithReverseGeocodeSampleId(1) },
				{ SourceSubPath("f2-2.jpg"), ExifDataFakes.WithReverseGeocodeSampleId(2) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					ReverseGeocodeFakes.FlatFormatSampleId(1),
					new List<Photo>
					{
						SourceSubPhotoInfo("f1.jpg", ExifDataFakes.WithReverseGeocodeSampleId(1), ReverseGeocodeFakes.FlatFormatSampleId(1)),
					}
				},
				{
					ReverseGeocodeFakes.FlatFormatSampleId(2),
					new List<Photo>
					{
						SourceSubPhotoInfo("f2-1.jpg", ExifDataFakes.WithReverseGeocodeSampleId(2), ReverseGeocodeFakes.FlatFormatSampleId(2)),
						SourceSubPhotoInfo("f2-2.jpg", ExifDataFakes.WithReverseGeocodeSampleId(2), ReverseGeocodeFakes.FlatFormatSampleId(2)),
					}
				},
			}
		},
	};

	[Theory]
	[MemberData(nameof(AllFilesFlattenedAndGroupIntoAddressFlat))]
	public void FlattenAllSubFolders_Then_GroupByFolder_Type_AddressFlat_Be_EquivalentTo(Dictionary<string, ExifData> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.AddressFlat, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region AddressHierarchy

	public static TheoryData<Dictionary<string, ExifData>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoAddressHierarchy = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ SourceSubPath("f2-1.jpg"), ExifDataFakes.WithReverseGeocodeSampleId(2) },
				{ SourceSubPath("f1.jpg"), ExifDataFakes.WithReverseGeocodeSampleId(1) },
				{ SourceSubPath("f2-2.jpg"), ExifDataFakes.WithReverseGeocodeSampleId(2) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					ReverseGeocodeFakes.HierarchyFormatSampleId(1),
					new List<Photo>
					{
						SourceSubPhotoInfo("f1.jpg", ExifDataFakes.WithReverseGeocodeSampleId(1), ReverseGeocodeFakes.HierarchyFormatSampleId(1)),
					}
				},
				{
					ReverseGeocodeFakes.HierarchyFormatSampleId(2),
					new List<Photo>
					{
						SourceSubPhotoInfo("f2-1.jpg", ExifDataFakes.WithReverseGeocodeSampleId(2), ReverseGeocodeFakes.HierarchyFormatSampleId(2)),
						SourceSubPhotoInfo("f2-2.jpg", ExifDataFakes.WithReverseGeocodeSampleId(2), ReverseGeocodeFakes.HierarchyFormatSampleId(2)),
					}
				},
			}
		},
	};

	[Theory]
	[MemberData(nameof(AllFilesFlattenedAndGroupIntoAddressHierarchy))]
	public void FlattenAllSubFolders_Then_GroupByFolder_Type_AddressHierarchy_Be_EquivalentTo(Dictionary<string, ExifData> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.AddressHierarchy, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#endregion

	#region Helpers

	private static void GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType folderProcessType, GroupByFolderType? groupByFolderType, Dictionary<string, ExifData> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory, bool noPhotoDateTimeTakenGroupedInSubFolder = false, bool noReverseGeocodeGroupedInSubFolder = false)
	{
		var fileSystem = new MockFileSystem();
		var sut = new DirectoryGrouperService(fileSystem, ToolOptionFakes.Create(), NullLogger<DirectoryGrouperService>.Instance, ConsoleWriterFakes.Valid());
		var groupedPhotoPathsByRelativeDirectory = sut.GroupFiles(photoExifDataByFilePath, SourcePath, folderProcessType, groupByFolderType,
			noPhotoDateTimeTakenGroupedInSubFolder, noReverseGeocodeGroupedInSubFolder);
		groupedPhotoPathsByRelativeDirectory.Should().BeEquivalentTo(expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region Fakes

	private static Photo SourceRootPhotoInfo(string fileName, ExifData exifData, string targetRelativeDirectoryPath)
	{
		return PhotoFakes.WithValidFilePath(SourcePath, fileName, exifData, targetRelativeDirectoryPath);
	}

	private static Photo SourceSubPhotoInfo(string fileName, ExifData exifData, string targetRelativeDirectoryPath)
	{
		return PhotoFakes.WithValidFilePath($"{SourcePath}/{SubPath}", fileName, exifData, targetRelativeDirectoryPath);
	}

	private static string SourceRootPath(string fileName)
	{
		return MockFileSystemHelper.Path($"/{SourcePath}/{fileName}");
	}

	private static string SourceSubPath(string fileName)
	{
		return MockFileSystemHelper.Path($"/{SourcePath}/{SubPath}/{fileName}");
	}

	private const string SourcePath = "source-path";
	private const string SubPath = "sub-path";

	#endregion
}
