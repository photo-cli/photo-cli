namespace PhotoCli.Tests.UnitTests.Services;

public class DirectoryGrouperServiceUnitTests
{
	#region Shared

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> AllValidFilesLocatedInSourceRootShouldGroupedInRootPath = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1.jpg"), ExifDataFakes.WithDay(1) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), RootTargetRelativePath),
					}
				}
			}
		},
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceRootPath("f2.jpg"), ExifDataFakes.WithDay(2) }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), RootTargetRelativePath),
						SourceRootPhotoInfo("f2.jpg", ExifDataFakes.WithDay(2), RootTargetRelativePath),
					}
				}
			}
		}
	};

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> SomeValidFilesSomeInvalidFilesShouldGroupedInRootPath = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1) },
				{ SourceRootPath("f2-root-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
				{ SourceRootPath("f3-root-no-reverse-geocode.jpg"), ExifDataFakes.WithNoReverseGeocode() },
				{ SourceRootPath("f4-root-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
				{ SourceRootPath("f5-root-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), RootTargetRelativePath),
						SourceRootPhotoInfo("f2-root-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(), RootTargetRelativePath),
						SourceRootPhotoInfo("f3-root-no-reverse-geocode.jpg", ExifDataFakes.WithNoReverseGeocode(), RootTargetRelativePath),
						SourceRootPhotoInfo("f4-root-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), RootTargetRelativePath),
						SourceRootPhotoInfoInvalidFileFormat("f5-root-invalid-file-format.jpg", RootTargetRelativePath),
					}
				},
			}
		},
	};

	#endregion

	#region SubFoldersPreserveFolderHierarchy

	#region Without Move Action

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> AllFilesLocatedInSubFolderShouldGroupedInTheirRelativePath = new()
	{
		{
			new Dictionary<string, ExifData?>
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
			new Dictionary<string, ExifData?>
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

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> FilesLocatedInRootAndSubFoldersShouldGroupedInTheirRelativePath = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithDay(2) }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDay(1), RootTargetRelativePath),
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

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> FilesWithNoExifDataOrInvalidFormatStaysInTheirSourceFolders = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1) },
				{ SourceRootPath("f2-root-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
				{ SourceRootPath("f3-root-no-reverse-geocode.jpg"), ExifDataFakes.WithNoReverseGeocode() },
				{ SourceRootPath("f4-root-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
				{ SourceRootPath("f5-root-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() },
				{ SourceSubPath("f6-sub.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2) },
				{ SourceSubPath("f7-sub-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
				{ SourceSubPath("f8-sub-no-reverse-geocode.jpg"), ExifDataFakes.WithNoReverseGeocode() },
				{ SourceSubPath("f9-sub-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
				{ SourceSubPath("f10-sub-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), RootTargetRelativePath),
						SourceRootPhotoInfo("f2-root-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(), RootTargetRelativePath),
						SourceRootPhotoInfo("f3-root-no-reverse-geocode.jpg", ExifDataFakes.WithNoReverseGeocode(), RootTargetRelativePath),
						SourceRootPhotoInfo("f4-root-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), RootTargetRelativePath),
						SourceRootPhotoInfoInvalidFileFormat("f5-root-invalid-file-format.jpg", RootTargetRelativePath),
					}
				},
				{
					SubPath,
					new List<Photo>
					{
						SourceSubPhotoInfo("f6-sub.jpg", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2), SubPath),
						SourceSubPhotoInfo("f7-sub-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(), SubPath),
						SourceSubPhotoInfo("f8-sub-no-reverse-geocode.jpg", ExifDataFakes.WithNoReverseGeocode(), SubPath),
						SourceSubPhotoInfo("f9-sub-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), SubPath),
						SourceSubPhotoInfoInvalidFileFormat("f10-sub-invalid-file-format.jpg", SubPath),
					}
				},
			}
		},
	};

	[Theory]
	[MemberData(nameof(AllValidFilesLocatedInSourceRootShouldGroupedInRootPath))]
	[MemberData(nameof(SomeValidFilesSomeInvalidFilesShouldGroupedInRootPath))]
	[MemberData(nameof(AllFilesLocatedInSubFolderShouldGroupedInTheirRelativePath))]
	[MemberData(nameof(FilesLocatedInRootAndSubFoldersShouldGroupedInTheirRelativePath))]
	[MemberData(nameof(FilesWithNoExifDataOrInvalidFormatStaysInTheirSourceFolders))]

	public void SubFoldersPreserveFolderHierarchy_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(Dictionary<string, ExifData?> photoExifDataByFilePath, Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.SubFoldersPreserveFolderHierarchy, null, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region With Move Action

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>, bool, bool, bool> FilesNotHavePhotoTakenDateShouldGroupedInSubFolderIfNoPhotoDateTimeTakenGroupedInSubFolderIsTrue = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceRootPath("f1-root-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
				{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithDay(2) },
				{ SourceSubPath("f2-sub-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDay(1), RootTargetRelativePath),
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
			false,
			true,
			false
		}
	};

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>, bool, bool, bool> FilesNotHaveGeocodeShouldGroupedInSubFolderIfNoReverseGeocodeGroupedInSubFolderIsTrue = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithReverseGeocodeSampleId(1) },
				{ SourceRootPath("f1-root-no-reverse-geocode.jpg"), ExifDataFakes.WithNoReverseGeocode() },
				{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithReverseGeocodeSampleId(2) },
				{ SourceSubPath("f2-sub-no-reverse-geocode.jpg"), ExifDataFakes.WithNoReverseGeocode() }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithReverseGeocodeSampleId(1), RootTargetRelativePath),
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
			false,
			true
		}
	};

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>, bool, bool, bool> FilesNotHaveGeocodeAndPhotoTakenDateShouldGroupedInSubFolderIfNoReverseGeocodeGroupedInSubFolderAndNoPhotoDateTimeTakenGroupedInSubFolderIsTrue = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1) },
				{ SourceRootPath("f1-root-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
				{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2) },
				{ SourceSubPath("f2-sub-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), RootTargetRelativePath),
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
			false,
			true,
			true
		}
	};

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>, bool, bool, bool> FilesHasInvalidFormatShouldGroupedInSubFolderIfInvalidFileFormatGroupedInSubFolderIsTrue = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1) },
				{ SourceRootPath("f1-root-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() },
				{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2) },
				{ SourceSubPath("f2-sub-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), RootTargetRelativePath),
					}
				},
				{
					ToolOptionFakes.PhotoFormatInvalidFolderName,
					new List<Photo>
					{
						SourceRootPhotoInfoInvalidFileFormat("f1-root-invalid-file-format.jpg", ToolOptionFakes.PhotoFormatInvalidFolderName),
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
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.PhotoFormatInvalidFolderName),
					new List<Photo>
					{
						SourceSubPhotoInfoInvalidFileFormat("f2-sub-invalid-file-format.jpg", MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.PhotoFormatInvalidFolderName)),
					}
				},
			},
			true,
			false,
			false
		}
	};

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>, bool, bool, bool> FilesWithNoExifDataOrInvalidFormatGoesIntoItsRelativeSubFolder = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1) },
				{ SourceRootPath("f2-root-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
				{ SourceRootPath("f3-root-no-reverse-geocode.jpg"), ExifDataFakes.WithNoReverseGeocode() },
				{ SourceRootPath("f4-root-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
				{ SourceRootPath("f5-root-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() },
				{ SourceSubPath("f6-sub.jpg"), ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2) },
				{ SourceSubPath("f7-sub-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
				{ SourceSubPath("f8-sub-no-reverse-geocode.jpg"), ExifDataFakes.WithNoReverseGeocode() },
				{ SourceSubPath("f9-sub-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
				{ SourceSubPath("f10-sub-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), RootTargetRelativePath),
					}
				},
				{
					ToolOptionFakes.NoPhotoTakenDateFolderName,
					new List<Photo>
					{
						SourceRootPhotoInfo("f2-root-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(), ToolOptionFakes.NoPhotoTakenDateFolderName),
					}
				},
				{
					ToolOptionFakes.NoAddressFolderName,
					new List<Photo>
					{
						SourceRootPhotoInfo("f3-root-no-reverse-geocode.jpg", ExifDataFakes.WithNoReverseGeocode(), ToolOptionFakes.NoAddressFolderName),
					}
				},
				{
					ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName,
					new List<Photo>
					{
						SourceRootPhotoInfo("f4-root-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(),
							ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName),
					}
				},
				{
					ToolOptionFakes.PhotoFormatInvalidFolderName,
					new List<Photo>
					{
						SourceRootPhotoInfoInvalidFileFormat("f5-root-invalid-file-format.jpg", ToolOptionFakes.PhotoFormatInvalidFolderName),
					}
				},
				{
					SubPath,
					new List<Photo>
					{
						SourceSubPhotoInfo("f6-sub.jpg", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2), SubPath),
					}
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoPhotoTakenDateFolderName),
					new List<Photo>
					{
						SourceSubPhotoInfo("f7-sub-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(),
							MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoPhotoTakenDateFolderName)),
					}
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressFolderName),
					new List<Photo>
					{
						SourceSubPhotoInfo("f8-sub-no-reverse-geocode.jpg", ExifDataFakes.WithNoReverseGeocode(),
							MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressFolderName)),
					}
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName),
					new List<Photo>
					{
						SourceSubPhotoInfo("f9-sub-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(),
							MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName)),
					}
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.PhotoFormatInvalidFolderName),
					new List<Photo>
					{
						SourceSubPhotoInfoInvalidFileFormat("f10-sub-invalid-file-format.jpg", MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.PhotoFormatInvalidFolderName)),
					}
				},
			},
			true,
			true,
			true
		}
	};

	[Theory]
	[MemberData(nameof(FilesNotHavePhotoTakenDateShouldGroupedInSubFolderIfNoPhotoDateTimeTakenGroupedInSubFolderIsTrue))]
	[MemberData(nameof(FilesNotHaveGeocodeShouldGroupedInSubFolderIfNoReverseGeocodeGroupedInSubFolderIsTrue))]
	[MemberData(nameof(FilesNotHaveGeocodeAndPhotoTakenDateShouldGroupedInSubFolderIfNoReverseGeocodeGroupedInSubFolderAndNoPhotoDateTimeTakenGroupedInSubFolderIsTrue))]
	[MemberData(nameof(FilesHasInvalidFormatShouldGroupedInSubFolderIfInvalidFileFormatGroupedInSubFolderIsTrue))]
	[MemberData(nameof(FilesWithNoExifDataOrInvalidFormatGoesIntoItsRelativeSubFolder))]
	public void SubFoldersPreserveFolderHierarchy_SelectedMoveActionForSubFolders_Should_Move_To_Its_Relative_Folder(Dictionary<string, ExifData?> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory, bool invalidFileFormatGroupedInSubFolder, bool noPhotoDateTimeTakenGroupedInSubFolder, bool noReverseGeocodeGroupedInSubFolder)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.SubFoldersPreserveFolderHierarchy, null, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory,
			invalidFileFormatGroupedInSubFolder, noPhotoDateTimeTakenGroupedInSubFolder, noReverseGeocodeGroupedInSubFolder);
	}

	#endregion

	#endregion

	#region Single

	[Theory]
	[MemberData(nameof(AllValidFilesLocatedInSourceRootShouldGroupedInRootPath))]
	[MemberData(nameof(SomeValidFilesSomeInvalidFilesShouldGroupedInRootPath))]
	public void Single_GroupFilesByRelativeDirectory_Be_EquivalentTo(Dictionary<string, ExifData?> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.Single, null, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	public static TheoryData<Dictionary<string, ExifData?>> SendingFilesLocatedOnSubFoldersOnSingleFolderProcessTypeShouldThrowPhotoOrganizerToolExceptionData = new()
	{
		new Dictionary<string, ExifData?>
		{
			{ SourceSubPath("f-sub.jpg"), ExifDataFakes.WithDay(2) }
		},
		new Dictionary<string, ExifData?>
		{
			{ SourceRootPath("f-root.jpg"), ExifDataFakes.WithDay(1) },
			{ SourceSubPath("f-sub.jpg"), ExifDataFakes.WithDay(2) }
		}
	};

	[Theory]
	[MemberData(nameof(SendingFilesLocatedOnSubFoldersOnSingleFolderProcessTypeShouldThrowPhotoOrganizerToolExceptionData))]
	public void Sending_Files_Located_On_Sub_Folders_On_Single_FolderProcessType_Should_Throw_PhotoOrganizerToolException(Dictionary<string, ExifData?> photoExifDataByFilePath)
	{
		Assert.Throws<PhotoCliException>(() =>
			GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.Single, null, photoExifDataByFilePath, new Dictionary<string, List<Photo>>())
		);
	}

	#endregion

	#region FlattenAllSubFolders

	#region No Group By Folder Type

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> AllFilesLocatedInSubFolderShouldFlattenedIntoRootPath = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceSubPath("f1.jpg"), ExifDataFakes.WithDay(1) },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceSubPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), RootTargetRelativePath),
					}
				}
			}
		},
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceSubPath("f1.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceSubPath("f2.jpg"), ExifDataFakes.WithDay(2) }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceSubPhotoInfo("f1.jpg", ExifDataFakes.WithDay(1), RootTargetRelativePath),
						SourceSubPhotoInfo("f2.jpg", ExifDataFakes.WithDay(2), RootTargetRelativePath),
					}
				}
			}
		}
	};

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> FilesLocatedInRootAndSubFoldersShouldFlattenedIntoRootPath = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1-root.jpg"), ExifDataFakes.WithDay(1) },
				{ SourceSubPath("f2-sub.jpg"), ExifDataFakes.WithDay(2) }
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f1-root.jpg", ExifDataFakes.WithDay(1), RootTargetRelativePath),
						SourceSubPhotoInfo("f2-sub.jpg", ExifDataFakes.WithDay(2), RootTargetRelativePath),
					}
				},
			}
		}
	};

	[Theory]
	[MemberData(nameof(AllValidFilesLocatedInSourceRootShouldGroupedInRootPath))]
	[MemberData(nameof(SomeValidFilesSomeInvalidFilesShouldGroupedInRootPath))]
	[MemberData(nameof(AllFilesLocatedInSubFolderShouldFlattenedIntoRootPath))]
	[MemberData(nameof(FilesLocatedInRootAndSubFoldersShouldFlattenedIntoRootPath))]
	public void FlattenAllSubFolders_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(Dictionary<string, ExifData?> photoExifDataByFilePath, Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, null, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region Year

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> AllFilesThatHasPhotoTakenDateFlattenedAndGroupIntoYearFolders = new()
	{
		{
			new Dictionary<string, ExifData?>
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
			new Dictionary<string, ExifData?>
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
	[MemberData(nameof(AllFilesThatHasPhotoTakenDateFlattenedAndGroupIntoYearFolders))]
	public void FlattenAllSubFolders_GroupBy_Year_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(Dictionary<string, ExifData?> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.Year, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region YearMonth

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoYearMonth = new()
	{
		{
			new Dictionary<string, ExifData?>
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
			new Dictionary<string, ExifData?>
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
	public void FlattenAllSubFolders_GroupBy_YearMonth_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(Dictionary<string, ExifData?> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.YearMonth, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region YearMonthDay

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoYearMonthDay = new()
	{
		{
			new Dictionary<string, ExifData?>
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
			new Dictionary<string, ExifData?>
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
	public void FlattenAllSubFolders_GroupBy_YearMonthDay_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(Dictionary<string, ExifData?> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.YearMonthDay, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region AddressFlat

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoAddressFlat = new()
	{
		{
			new Dictionary<string, ExifData?>
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
	public void FlattenAllSubFolders_GroupBy_AddressFlat_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(Dictionary<string, ExifData?> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.AddressFlat, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region AddressHierarchy

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoAddressHierarchy = new()
	{
		{
			new Dictionary<string, ExifData?>
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
	public void FlattenAllSubFolders_GroupBy_AddressHierarchy_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(Dictionary<string, ExifData?> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.AddressHierarchy, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region No Exif Data, Invalid File Format - Move Action For Sub Folders

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>, bool, bool, bool> FlattenedAndGroupIntoNoMoveActionForSubfoldersRootFolder = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1.jpg"), ExifDataFakes.WithYear(2001) },
				{ SourceRootPath("f2-root-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
				{ SourceRootPath("f3-root-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
				{ SourceRootPath("f4-root-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					new List<Photo>
					{
						SourceRootPhotoInfo("f1.jpg", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001)),
					}
				},
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f2-root-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(), RootTargetRelativePath),
						SourceRootPhotoInfo("f3-root-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), RootTargetRelativePath),
						SourceRootPhotoInfoInvalidFileFormat("f4-root-invalid-file-format.jpg", RootTargetRelativePath),
					}
				},
			},
			false,
			false,
			false
		},
	};

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>, bool, bool, bool> FlattenedAndGroupIntoMoveActionForInvalidFilesInSubfolders = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1.jpg"), ExifDataFakes.WithYear(2001) },
				{ SourceRootPath("f2-root-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
				{ SourceRootPath("f3-root-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
				{ SourceRootPath("f4-root-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					new List<Photo>
					{
						SourceRootPhotoInfo("f1.jpg", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001)),
					}
				},
				{
					ToolOptionFakes.PhotoFormatInvalidFolderName,
					new List<Photo>
					{
						SourceRootPhotoInfoInvalidFileFormat("f4-root-invalid-file-format.jpg", ToolOptionFakes.PhotoFormatInvalidFolderName),
					}
				},
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f2-root-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(), RootTargetRelativePath),
						SourceRootPhotoInfo("f3-root-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), RootTargetRelativePath),
					}
				},
			},
			true,
			false,
			false
		},
	};

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>, bool, bool, bool> FlattenedAndGroupIntoMoveActionForNoPhotoDateTimeTakenInSubFolders = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1.jpg"), ExifDataFakes.WithYear(2001) },
				{ SourceRootPath("f2-root-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
				{ SourceRootPath("f3-root-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
				{ SourceRootPath("f4-root-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					new List<Photo>
					{
						SourceRootPhotoInfo("f1.jpg", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001)),
					}
				},
				{
					ToolOptionFakes.NoPhotoTakenDateFolderName,
					new List<Photo>
					{
						SourceRootPhotoInfo("f2-root-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(), ToolOptionFakes.NoPhotoTakenDateFolderName),
						SourceRootPhotoInfo("f3-root-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), ToolOptionFakes.NoPhotoTakenDateFolderName),
						SourceRootPhotoInfoInvalidFileFormat("f4-root-invalid-file-format.jpg", ToolOptionFakes.NoPhotoTakenDateFolderName),
					}
				},
			},
			false,
			true,
			false
		},
	};

	public static TheoryData<Dictionary<string, ExifData?>, Dictionary<string, List<Photo>>, bool, bool, bool> FlattenedAndGroupIntoMoveActionForNoReverseGeocodeGroupedInSubfolders = new()
	{
		{
			new Dictionary<string, ExifData?>
			{
				{ SourceRootPath("f1.jpg"), ExifDataFakes.WithYearAndReverseGeocode(2001) },
				{ SourceRootPath("f2-root-no-taken-date.jpg"), ExifDataFakes.WithNoPhotoTakenDate() },
				{ SourceRootPath("f3-root-no-reverse-geocode-no-taken-date.jpg"), ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate() },
				{ SourceRootPath("f4-root-invalid-file-format.jpg"), ExifDataFakes.WithInvalidFileFormat() },
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					new List<Photo>
					{
						SourceRootPhotoInfo("f1.jpg", ExifDataFakes.WithYearAndReverseGeocode(2001), DateTimeFakes.FormatYear(2001)),
					}
				},
				{
					ToolOptionFakes.NoAddressFolderName,
					new List<Photo>
					{
						SourceRootPhotoInfo("f3-root-no-reverse-geocode-no-taken-date.jpg", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), ToolOptionFakes.NoAddressFolderName),
						SourceRootPhotoInfoInvalidFileFormat("f4-root-invalid-file-format.jpg", ToolOptionFakes.NoAddressFolderName),
					}
				},
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						SourceRootPhotoInfo("f2-root-no-taken-date.jpg", ExifDataFakes.WithNoPhotoTakenDate(), RootTargetRelativePath),
					}
				},
			},
			false,
			false,
			true
		},
	};

	[Theory]
	[MemberData(nameof(FlattenedAndGroupIntoNoMoveActionForSubfoldersRootFolder))]
	[MemberData(nameof(FlattenedAndGroupIntoMoveActionForInvalidFilesInSubfolders))]
	[MemberData(nameof(FlattenedAndGroupIntoMoveActionForNoPhotoDateTimeTakenInSubFolders))]
	[MemberData(nameof(FlattenedAndGroupIntoMoveActionForNoReverseGeocodeGroupedInSubfolders))]
	public void FlattenAllSubFolders_GroupBy_MoveActionForSubFolders_Should_Be_EquivalentTo_Expected(Dictionary<string, ExifData?> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory, bool invalidFileFormatGroupedInSubFolder, bool noPhotoDateTimeTakenGroupedInSubFolder, bool noReverseGeocodeGroupedInSubFolder)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.Year, photoExifDataByFilePath, expectedGroupedPhotoInfosByRelativeDirectory,
			invalidFileFormatGroupedInSubFolder, noPhotoDateTimeTakenGroupedInSubFolder, noReverseGeocodeGroupedInSubFolder);
	}

	#endregion

	#endregion

	#region Helpers

	private static void GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType folderProcessType, GroupByFolderType? groupByFolderType, Dictionary<string, ExifData?> photoExifDataByFilePath,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory, bool invalidFileFormatGroupedInSubFolder = false, bool noPhotoDateTimeTakenGroupedInSubFolder = false, bool noReverseGeocodeGroupedInSubFolder = false)
	{
		var fileSystem = new MockFileSystem();
		var sut = new DirectoryGrouperService(fileSystem, ToolOptionFakes.Create(), NullLogger<DirectoryGrouperService>.Instance, ConsoleWriterFakes.Valid());

		var actualGroupedPhotoPathsByRelativeDirectory = sut.GroupFiles(photoExifDataByFilePath, SourcePath, folderProcessType, groupByFolderType,
			invalidFileFormatGroupedInSubFolder, noPhotoDateTimeTakenGroupedInSubFolder, noReverseGeocodeGroupedInSubFolder);

		actualGroupedPhotoPathsByRelativeDirectory.Should().BeEquivalentTo(expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region Fakes

	private static Photo SourceRootPhotoInfo(string fileName, ExifData? exifData, string targetRelativeDirectoryPath)
	{
		return PhotoFakes.WithValidFilePathAndExifData(SourcePath, fileName, exifData, targetRelativeDirectoryPath);
	}

	private static Photo SourceSubPhotoInfo(string fileName, ExifData? exifData, string targetRelativeDirectoryPath)
	{
		return PhotoFakes.WithValidFilePathAndExifData($"{SourcePath}/{SubPath}", fileName, exifData, targetRelativeDirectoryPath);
	}

	private static Photo SourceRootPhotoInfoInvalidFileFormat(string fileName, string targetRelativeDirectoryPath)
	{
		return PhotoFakes.WithValidFilePathInvalidFileFormat(SourcePath, fileName, targetRelativeDirectoryPath);
	}

	private static Photo SourceSubPhotoInfoInvalidFileFormat(string fileName, string targetRelativeDirectoryPath)
	{
		return PhotoFakes.WithValidFilePathInvalidFileFormat($"{SourcePath}/{SubPath}", fileName, targetRelativeDirectoryPath);
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
	private const string RootTargetRelativePath = "";

	#endregion
}
