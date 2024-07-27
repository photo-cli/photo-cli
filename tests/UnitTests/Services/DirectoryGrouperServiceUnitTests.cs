namespace PhotoCli.Tests.UnitTests.Services;

public class DirectoryGrouperServiceUnitTests
{
	#region Shared

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> AllValidFilesLocatedInSourceRootShouldGroupedInRootPath = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1", ExifDataFakes.WithDay(1))
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("1", ExifDataFakes.WithDay(1), RootTargetRelativePath)
					]
				}
			}
		},
		{
			new []
			{
				PhotoInputRootFolder("1", ExifDataFakes.WithDay(1)),
				PhotoInputRootFolder("2", ExifDataFakes.WithDay(2)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("1", ExifDataFakes.WithDay(1), RootTargetRelativePath),
						GroupedPhotoRootFolder("2", ExifDataFakes.WithDay(2), RootTargetRelativePath)
					]
				}
			}
		}
	};

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> SomeValidFilesSomeInvalidFilesShouldGroupedInRootPath = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1-root", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1)),
				PhotoInputRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
				PhotoInputRootFolder("3-root-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode()),
				PhotoInputRootFolder("4-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
				PhotoInputRootFolder("5-root-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("1-root", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), RootTargetRelativePath),
						GroupedPhotoRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(), RootTargetRelativePath),
						GroupedPhotoRootFolder("3-root-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode(), RootTargetRelativePath),
						GroupedPhotoRootFolder("4-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), RootTargetRelativePath),
						GroupedInvalidPhotoRootFolder("5-root-invalid-file-format", RootTargetRelativePath)
					]
				},
			}
		},
	};

	#endregion

	#region SubFoldersPreserveFolderHierarchy

	#region Without Move Action

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> AllFilesLocatedInSubFolderShouldGroupedInTheirRelativePath = new()
	{
		{
			new []
			{
				PhotoInputSubFolder("1", ExifDataFakes.WithDay(1))
			},
			new Dictionary<string, List<Photo>>
			{
				{
					SubPath,
					[
						GroupedPhotoSubFolder("1", ExifDataFakes.WithDay(1), SubPath)
					]
				}
			}
		},
		{
			new []
			{
				PhotoInputSubFolder("1", ExifDataFakes.WithDay(1)),
				PhotoInputSubFolder("2", ExifDataFakes.WithDay(2)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					SubPath,
					[
						GroupedPhotoSubFolder("1", ExifDataFakes.WithDay(1), SubPath),
						GroupedPhotoSubFolder("2", ExifDataFakes.WithDay(2), SubPath)
					]
				}
			}
		}
	};

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> FilesLocatedInRootAndSubFoldersShouldGroupedInTheirRelativePath = new()
	{
		{
			new []
			{
				PhotoFakes.CreateWithExifData(ExifDataFakes.WithDay(1), SourceRootPath("1-root")),
				PhotoFakes.CreateWithExifData(ExifDataFakes.WithDay(2), SourceSubPath("2-sub")),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					new List<Photo>
					{
						GroupedPhotoRootFolder("1-root", ExifDataFakes.WithDay(1), RootTargetRelativePath),
					}
				},
				{
					SubPath,
					new List<Photo>
					{
						GroupedPhotoSubFolder("2-sub", ExifDataFakes.WithDay(2), SubPath),
					}
				}
			}
		}
	};

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> FilesWithNoExifDataOrInvalidFormatStaysInTheirSourceFolders = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1-root", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1)),
				PhotoInputRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
				PhotoInputRootFolder("3-root-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode()),
				PhotoInputRootFolder("4-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
				PhotoInputRootFolder("5-root-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),
				PhotoInputSubFolder("6-sub", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2)),
				PhotoInputSubFolder("7-sub-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
				PhotoInputSubFolder("8-sub-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode()),
				PhotoInputSubFolder("9-sub-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
				PhotoInputSubFolder("10-sub-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("1-root", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), RootTargetRelativePath),
						GroupedPhotoRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(), RootTargetRelativePath),
						GroupedPhotoRootFolder("3-root-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode(), RootTargetRelativePath),
						GroupedPhotoRootFolder("4-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), RootTargetRelativePath),
						GroupedInvalidPhotoRootFolder("5-root-invalid-file-format", RootTargetRelativePath)
					]
				},
				{
					SubPath,
					[
						GroupedPhotoSubFolder("6-sub", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2), SubPath),
						GroupedPhotoSubFolder("7-sub-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(), SubPath),
						GroupedPhotoSubFolder("8-sub-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode(), SubPath),
						GroupedPhotoSubFolder("9-sub-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), SubPath),
						GroupedInvalidPhotoSubFolder("10-sub-invalid-file-format", SubPath)
					]
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

	public void SubFoldersPreserveFolderHierarchy_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(IReadOnlyList<Photo> photos, Dictionary<string, List<Photo>>
			expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.SubFoldersPreserveFolderHierarchy, null, photos, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region With Move Action

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>, bool, bool, bool> FilesNotHavePhotoTakenDateShouldGroupedInSubFolderIfNoPhotoDateTimeTakenGroupedInSubFolderIsTrue = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1-root", ExifDataFakes.WithDay(1)),
				PhotoInputRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
				PhotoInputSubFolder("3-sub", ExifDataFakes.WithDay(2)),
				PhotoInputSubFolder("4-sub-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("1-root", ExifDataFakes.WithDay(1), RootTargetRelativePath)
					]
				},
				{
					ToolOptionFakes.NoPhotoTakenDateFolderName,
					[
						GroupedPhotoRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(), ToolOptionFakes.NoPhotoTakenDateFolderName)
					]
				},
				{
					SubPath,
					[
						GroupedPhotoSubFolder("3-sub", ExifDataFakes.WithDay(2), SubPath)
					]
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoPhotoTakenDateFolderName),
					[
						GroupedPhotoSubFolder("4-sub-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(),
							MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoPhotoTakenDateFolderName))

					]
				},
			},
			false,
			true,
			false
		}
	};

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>, bool, bool, bool> FilesNotHaveGeocodeShouldGroupedInSubFolderIfNoReverseGeocodeGroupedInSubFolderIsTrue = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1-root", ExifDataFakes.WithReverseGeocodeSampleId(1)),
				PhotoInputRootFolder("2-root-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode()),
				PhotoInputSubFolder("3-sub", ExifDataFakes.WithReverseGeocodeSampleId(2)),
				PhotoInputSubFolder("4-sub-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("1-root", ExifDataFakes.WithReverseGeocodeSampleId(1), RootTargetRelativePath)
					]
				},
				{
					ToolOptionFakes.NoAddressFolderName,
					[
						GroupedPhotoRootFolder("2-root-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode(), ToolOptionFakes.NoAddressFolderName)
					]
				},
				{
					SubPath,
					[
						GroupedPhotoSubFolder("3-sub", ExifDataFakes.WithReverseGeocodeSampleId(2), SubPath)
					]
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressFolderName),
					[
						GroupedPhotoSubFolder("4-sub-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode(),
							MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressFolderName))

					]
				},
			},
			false,
			false,
			true
		}
	};

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>, bool, bool, bool> FilesHasInvalidFormatShouldGroupedInSubFolderIfInvalidFileFormatGroupedInSubFolderIsTrue = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1-root", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1)),
				PhotoInputRootFolder("2-root-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),
				PhotoInputSubFolder("3-sub", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2)),
				PhotoInputSubFolder("4-sub-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("1-root", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), RootTargetRelativePath)
					]
				},
				{
					ToolOptionFakes.PhotoFormatInvalidFolderName,
					[
						GroupedInvalidPhotoRootFolder("2-root-invalid-file-format", ToolOptionFakes.PhotoFormatInvalidFolderName)
					]
				},
				{
					SubPath,
					[
						GroupedPhotoSubFolder("3-sub", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2), SubPath)
					]
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.PhotoFormatInvalidFolderName),
					[
						GroupedInvalidPhotoSubFolder("4-sub-invalid-file-format", MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.PhotoFormatInvalidFolderName))
					]
				},
			},
			true,
			false,
			false
		}
	};

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>, bool, bool, bool> FilesWithNoExifDataOrInvalidFormatGoesIntoItsRelativeSubFolder = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1-root", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1)),
				PhotoInputRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
				PhotoInputRootFolder("3-root-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode()),
				PhotoInputRootFolder("4-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
				PhotoInputRootFolder("5-root-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),

				PhotoInputSubFolder("6-sub", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2)),
				PhotoInputSubFolder("7-sub-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
				PhotoInputSubFolder("8-sub-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode()),
				PhotoInputSubFolder("9-sub-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
				PhotoInputSubFolder("10-sub-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("1-root", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), RootTargetRelativePath)
					]
				},
				{
					ToolOptionFakes.NoPhotoTakenDateFolderName,
					[
						GroupedPhotoRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(), ToolOptionFakes.NoPhotoTakenDateFolderName)
					]
				},
				{
					ToolOptionFakes.NoAddressFolderName,
					[
						GroupedPhotoRootFolder("3-root-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode(), ToolOptionFakes.NoAddressFolderName)
					]
				},
				{
					ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName,
					[
						GroupedPhotoRootFolder("4-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(),
							ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName)

					]
				},
				{
					ToolOptionFakes.PhotoFormatInvalidFolderName,
					[
						GroupedInvalidPhotoRootFolder("5-root-invalid-file-format", ToolOptionFakes.PhotoFormatInvalidFolderName)
					]
				},
				{
					SubPath,
					[
						GroupedPhotoSubFolder("6-sub", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2), SubPath)
					]
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoPhotoTakenDateFolderName),
					[
						GroupedPhotoSubFolder("7-sub-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(),
							MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoPhotoTakenDateFolderName))

					]
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressFolderName),
					[
						GroupedPhotoSubFolder("8-sub-no-reverse-geocode", ExifDataFakes.WithNoReverseGeocode(),
							MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressFolderName))

					]
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName),
					[
						GroupedPhotoSubFolder("9-sub-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(),
							MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName))

					]
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.PhotoFormatInvalidFolderName),
				[
						GroupedInvalidPhotoSubFolder("10-sub-invalid-file-format",
							MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.PhotoFormatInvalidFolderName))
					]
				},
			},
			true,
			true,
			true
		}
	};

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>, bool, bool, bool> FilesNotHaveGeocodeAndPhotoTakenDateShouldGroupedInSubFolderIfNoReverseGeocodeGroupedInSubFolderAndNoPhotoDateTimeTakenGroupedInSubFolderIsTrue = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1-root", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1)),
				PhotoInputRootFolder("2-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
				PhotoInputSubFolder("3-sub", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2)),
				PhotoInputSubFolder("4-sub-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("1-root", ExifDataFakes.WithDayAndReverseGeocodeSampleId(1, 1), RootTargetRelativePath)
					]
				},
				{
					ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName,
					[
						GroupedPhotoRootFolder("2-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(),
							ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName)

					]
				},
				{
					SubPath,
					[
						GroupedPhotoSubFolder("3-sub", ExifDataFakes.WithDayAndReverseGeocodeSampleId(2, 2), SubPath)
					]
				},
				{
					MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName),
					[
						GroupedPhotoSubFolder("4-sub-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(),
							MockFileSystemHelper.RelativePath(SubPath, ToolOptionFakes.NoAddressAndPhotoTakenDateFolderName))

					]
				},
			},
			false,
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
	public void SubFoldersPreserveFolderHierarchy_SelectedMoveActionForSubFolders_Should_Move_To_Its_Relative_Folder(IReadOnlyList<Photo> photos,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory, bool invalidFileFormatGroupedInSubFolder, bool noPhotoDateTimeTakenGroupedInSubFolder, bool noReverseGeocodeGroupedInSubFolder)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.SubFoldersPreserveFolderHierarchy, null, photos, expectedGroupedPhotoInfosByRelativeDirectory,
			invalidFileFormatGroupedInSubFolder, noPhotoDateTimeTakenGroupedInSubFolder, noReverseGeocodeGroupedInSubFolder);
	}

	#endregion

	#endregion

	#region Single

	[Theory]
	[MemberData(nameof(AllValidFilesLocatedInSourceRootShouldGroupedInRootPath))]
	[MemberData(nameof(SomeValidFilesSomeInvalidFilesShouldGroupedInRootPath))]
	public void Single_GroupFilesByRelativeDirectory_Be_EquivalentTo(IReadOnlyList<Photo> photos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.Single, null, photos, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	public static TheoryData<IReadOnlyList<Photo>> SendingFilesLocatedOnSubFoldersOnSingleFolderProcessTypeShouldThrowPhotoOrganizerToolExceptionData = new()
	{
		new List<Photo>
		{
			PhotoFakes.CreateWithExifData(ExifDataFakes.WithDay(1), SourceSubPath("-sub")),
		},

		new List<Photo>
		{
			PhotoFakes.CreateWithExifData(ExifDataFakes.WithDay(2), SourceRootPath("-root")),
			PhotoFakes.CreateWithExifData(ExifDataFakes.WithDay(3), SourceSubPath("-su")),
		},
	};

	[Theory]
	[MemberData(nameof(SendingFilesLocatedOnSubFoldersOnSingleFolderProcessTypeShouldThrowPhotoOrganizerToolExceptionData))]
	public void Sending_Files_Located_On_Sub_Folders_On_Single_FolderProcessType_Should_Throw_PhotoOrganizerToolException(IReadOnlyList<Photo> photos)
	{
		Assert.Throws<PhotoCliException>(() =>
			GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.Single, null, photos, new Dictionary<string, List<Photo>>())
		);
	}

	#endregion

	#region FlattenAllSubFolders

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> AllFilesLocatedInSubFolderShouldFlattenedIntoRootPath = new()
	{
		{
			new []
			{
				PhotoInputSubFolder("1", ExifDataFakes.WithDay(1)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoSubFolder("1", ExifDataFakes.WithDay(1), RootTargetRelativePath)
					]
				}
			}
		},
		{
			new []
			{
				PhotoInputSubFolder("1", ExifDataFakes.WithDay(1)),
				PhotoInputSubFolder("2", ExifDataFakes.WithDay(2)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoSubFolder("1", ExifDataFakes.WithDay(1), RootTargetRelativePath),
						GroupedPhotoSubFolder("2", ExifDataFakes.WithDay(2), RootTargetRelativePath)
					]
				}
			}
		}
	};

	#region No Group By Folder Type

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> FilesLocatedInRootAndSubFoldersShouldFlattenedIntoRootPath = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1-root", ExifDataFakes.WithDay(1)),
				PhotoInputSubFolder("2-sub", ExifDataFakes.WithDay(2)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("1-root", ExifDataFakes.WithDay(1), RootTargetRelativePath),
						GroupedPhotoSubFolder("2-sub", ExifDataFakes.WithDay(2), RootTargetRelativePath)
					]
				},
			}
		}
	};

	[Theory]
	[MemberData(nameof(AllValidFilesLocatedInSourceRootShouldGroupedInRootPath))]
	[MemberData(nameof(SomeValidFilesSomeInvalidFilesShouldGroupedInRootPath))]
	[MemberData(nameof(AllFilesLocatedInSubFolderShouldFlattenedIntoRootPath))]
	[MemberData(nameof(FilesLocatedInRootAndSubFoldersShouldFlattenedIntoRootPath))]
	public void FlattenAllSubFolders_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(IReadOnlyList<Photo> photos, Dictionary<string, List<Photo>>
			expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, null, photos, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region Year

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> AllFilesThatHasPhotoTakenDateFlattenedAndGroupIntoYearFolders = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1", ExifDataFakes.WithYear(2001)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					new List<Photo>
					{
						GroupedPhotoRootFolder("1", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001)),
					}
				}
			}
		},
		{
			new []
			{
				PhotoInputRootFolder("1-1", ExifDataFakes.WithYear(2001)),
				PhotoInputSubFolder("2", ExifDataFakes.WithYear(2002)),
				PhotoInputSubFolder("1-2", ExifDataFakes.WithYear(2001)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					[
						GroupedPhotoRootFolder("1-1", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001)),
						GroupedPhotoSubFolder("1-2", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001)),
					]
				},
				{
					DateTimeFakes.FormatYear(2002),
					[
						GroupedPhotoSubFolder("2", ExifDataFakes.WithYear(2002), DateTimeFakes.FormatYear(2002)),
					]
				}
			}
		}
	};

	[Theory]
	[MemberData(nameof(AllFilesThatHasPhotoTakenDateFlattenedAndGroupIntoYearFolders))]
	public void FlattenAllSubFolders_GroupBy_Year_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(IReadOnlyList<Photo> photos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.Year, photos, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region YearMonth

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoYearMonth = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1", ExifDataFakes.WithMonth(1)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.DirectoryFormatMonth(1),
					[
						GroupedPhotoRootFolder("1", ExifDataFakes.WithMonth(1), DateTimeFakes.DirectoryFormatMonth(1))
					]
				}
			}
		},
		{
			new []
			{
				PhotoInputSubFolder("1-1", ExifDataFakes.WithMonth(1)),
				PhotoInputRootFolder("2", ExifDataFakes.WithMonth(2)),
				PhotoInputRootFolder("1-2", ExifDataFakes.WithMonth(1)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.DirectoryFormatMonth(1),
					[
						GroupedPhotoSubFolder("1-1", ExifDataFakes.WithMonth(1), DateTimeFakes.DirectoryFormatMonth(1)),
						GroupedPhotoRootFolder("1-2", ExifDataFakes.WithMonth(1), DateTimeFakes.DirectoryFormatMonth(1))
					]
				},
				{
					DateTimeFakes.DirectoryFormatMonth(2),
					[
						GroupedPhotoRootFolder("2", ExifDataFakes.WithMonth(2), DateTimeFakes.DirectoryFormatMonth(2))
					]
				}
			}
		}
	};

	[Theory]
	[MemberData(nameof(AllFilesFlattenedAndGroupIntoYearMonth))]
	public void FlattenAllSubFolders_GroupBy_YearMonth_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(IReadOnlyList<Photo> photos,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.YearMonth, photos, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region YearMonthDay

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoYearMonthDay = new()
	{
		{
			new []
			{
				PhotoInputSubFolder("1", ExifDataFakes.WithDay(1)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.DirectoryFormatDay(1),
					[
						GroupedPhotoSubFolder("1", ExifDataFakes.WithDay(1), DateTimeFakes.DirectoryFormatDay(1))
					]
				}
			}
		},
		{
			new []
			{
				PhotoInputRootFolder("1-1", ExifDataFakes.WithDay(1)),
				PhotoInputSubFolder("2", ExifDataFakes.WithDay(2)),
				PhotoInputRootFolder("1-2", ExifDataFakes.WithDay(1)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.DirectoryFormatDay(1),
					[
						GroupedPhotoRootFolder("1-1", ExifDataFakes.WithDay(1), DateTimeFakes.DirectoryFormatDay(1)),
						GroupedPhotoRootFolder("1-2", ExifDataFakes.WithDay(1), DateTimeFakes.DirectoryFormatDay(1))
					]
				},
				{
					DateTimeFakes.DirectoryFormatDay(2),
					[
						GroupedPhotoSubFolder("2", ExifDataFakes.WithDay(2), DateTimeFakes.DirectoryFormatDay(2))
					]
				}
			}
		}
	};

	[Theory]
	[MemberData(nameof(AllFilesFlattenedAndGroupIntoYearMonthDay))]
	public void FlattenAllSubFolders_GroupBy_YearMonthDay_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(IReadOnlyList<Photo> photos,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.YearMonthDay, photos, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region AddressFlat

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoAddressFlat = new()
	{
		{
			new []
			{
				PhotoInputSubFolder("2-1", ExifDataFakes.WithReverseGeocodeSampleId(2)),
				PhotoInputSubFolder("1", ExifDataFakes.WithReverseGeocodeSampleId(1)),
				PhotoInputSubFolder("2-2", ExifDataFakes.WithReverseGeocodeSampleId(2)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					ReverseGeocodeFakes.FlatFormatSampleId(1),
					[
						GroupedPhotoSubFolder("1", ExifDataFakes.WithReverseGeocodeSampleId(1), ReverseGeocodeFakes.FlatFormatSampleId(1))
					]
				},
				{
					ReverseGeocodeFakes.FlatFormatSampleId(2),
					[

						GroupedPhotoSubFolder("2-1", ExifDataFakes.WithReverseGeocodeSampleId(2), ReverseGeocodeFakes.FlatFormatSampleId(2)),
						GroupedPhotoSubFolder("2-2", ExifDataFakes.WithReverseGeocodeSampleId(2), ReverseGeocodeFakes.FlatFormatSampleId(2))
					]
				},
			}
		},
	};

	[Theory]
	[MemberData(nameof(AllFilesFlattenedAndGroupIntoAddressFlat))]
	public void FlattenAllSubFolders_GroupBy_AddressFlat_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(IReadOnlyList<Photo> photos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.AddressFlat, photos, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region AddressHierarchy

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>> AllFilesFlattenedAndGroupIntoAddressHierarchy = new()
	{
		{
			new []
			{
				PhotoInputSubFolder("2-1", ExifDataFakes.WithReverseGeocodeSampleId(2)),
				PhotoInputSubFolder("1", ExifDataFakes.WithReverseGeocodeSampleId(1)),
				PhotoInputSubFolder("2-2", ExifDataFakes.WithReverseGeocodeSampleId(2)),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					ReverseGeocodeFakes.HierarchyFormatSampleId(1),
					[
						GroupedPhotoSubFolder("1", ExifDataFakes.WithReverseGeocodeSampleId(1), ReverseGeocodeFakes.HierarchyFormatSampleId(1))
					]
				},
				{
					ReverseGeocodeFakes.HierarchyFormatSampleId(2),
					[
						GroupedPhotoSubFolder("2-1", ExifDataFakes.WithReverseGeocodeSampleId(2), ReverseGeocodeFakes.HierarchyFormatSampleId(2)),
						GroupedPhotoSubFolder("2-2", ExifDataFakes.WithReverseGeocodeSampleId(2), ReverseGeocodeFakes.HierarchyFormatSampleId(2))
					]
				},
			}
		},
	};

	[Theory]
	[MemberData(nameof(AllFilesFlattenedAndGroupIntoAddressHierarchy))]
	public void FlattenAllSubFolders_GroupBy_AddressHierarchy_NoMoveActionForSubFolders_Should_Be_EquivalentTo_Expected(IReadOnlyList<Photo> photos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.AddressHierarchy, photos, expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region No Exif Data, Invalid File Format - Move Action For Sub Folders

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>, bool, bool, bool> FlattenedAndGroupIntoNoMoveActionForSubfoldersRootFolder = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1", ExifDataFakes.WithYear(2001)),
				PhotoInputRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
				PhotoInputRootFolder("3-root-no-reverse-geocode-no-taken-date",  ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
				PhotoInputRootFolder("4-root-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					[
						GroupedPhotoRootFolder("1", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001))
					]
				},
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(), RootTargetRelativePath),
						GroupedPhotoRootFolder("3-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), RootTargetRelativePath),
						GroupedInvalidPhotoRootFolder("4-root-invalid-file-format", RootTargetRelativePath)
					]
				},
			},
			false,
			false,
			false
		},
	};

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>, bool, bool, bool> FlattenedAndGroupIntoMoveActionForInvalidFilesInSubfolders = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1", ExifDataFakes.WithYear(2001)),
				PhotoInputRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
				PhotoInputRootFolder("3-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
				PhotoInputRootFolder("4-root-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					[
						GroupedPhotoRootFolder("1", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001))
					]
				},
				{
					ToolOptionFakes.PhotoFormatInvalidFolderName,
					[
						GroupedInvalidPhotoRootFolder("4-root-invalid-file-format", ToolOptionFakes.PhotoFormatInvalidFolderName)
					]
				},
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(), RootTargetRelativePath),
						GroupedPhotoRootFolder("3-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), RootTargetRelativePath)
					]
				},
			},
			true,
			false,
			false
		},
	};

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>, bool, bool, bool> FlattenedAndGroupIntoMoveActionForNoPhotoDateTimeTakenInSubFolders = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1", ExifDataFakes.WithYear(2001)),
				PhotoInputRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
				PhotoInputRootFolder("3-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
				PhotoInputRootFolder("4-root-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					[
						GroupedPhotoRootFolder("1", ExifDataFakes.WithYear(2001), DateTimeFakes.FormatYear(2001))
					]
				},
				{
					ToolOptionFakes.NoPhotoTakenDateFolderName,
					[
						GroupedPhotoRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(), ToolOptionFakes.NoPhotoTakenDateFolderName),
						GroupedPhotoRootFolder("3-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), ToolOptionFakes.NoPhotoTakenDateFolderName),
						GroupedInvalidPhotoRootFolder("4-root-invalid-file-format", ToolOptionFakes.NoPhotoTakenDateFolderName)
					]
				},
			},
			false,
			true,
			false
		},
	};

	public static TheoryData<IReadOnlyList<Photo>, Dictionary<string, List<Photo>>, bool, bool, bool> FlattenedAndGroupIntoMoveActionForNoReverseGeocodeGroupedInSubfolders = new()
	{
		{
			new []
			{
				PhotoInputRootFolder("1", ExifDataFakes.WithYearAndReverseGeocode(2001)),
				PhotoInputRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate()),
				PhotoInputRootFolder("3-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate()),
				PhotoInputRootFolder("4-root-invalid-file-format", ExifDataFakes.WithInvalidFileFormat()),
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatYear(2001),
					[
						GroupedPhotoRootFolder("1", ExifDataFakes.WithYearAndReverseGeocode(2001), DateTimeFakes.FormatYear(2001))
					]
				},
				{
					ToolOptionFakes.NoAddressFolderName,
					[
						GroupedPhotoRootFolder("3-root-no-reverse-geocode-no-taken-date", ExifDataFakes.WithNoReverseGeocodeAndNoTakenDate(), ToolOptionFakes.NoAddressFolderName),
						GroupedInvalidPhotoRootFolder("4-root-invalid-file-format", ToolOptionFakes.NoAddressFolderName)
					]
				},
				{
					RootTargetRelativePath,
					[
						GroupedPhotoRootFolder("2-root-no-taken-date", ExifDataFakes.WithNoPhotoTakenDate(), RootTargetRelativePath)
					]
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
	public void FlattenAllSubFolders_GroupBy_MoveActionForSubFolders_Should_Be_EquivalentTo_Expected(IReadOnlyList<Photo> photos,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory, bool invalidFileFormatGroupedInSubFolder, bool noPhotoDateTimeTakenGroupedInSubFolder, bool noReverseGeocodeGroupedInSubFolder)
	{
		GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType.FlattenAllSubFolders, GroupByFolderType.Year, photos, expectedGroupedPhotoInfosByRelativeDirectory,
			invalidFileFormatGroupedInSubFolder, noPhotoDateTimeTakenGroupedInSubFolder, noReverseGeocodeGroupedInSubFolder);
	}

	#endregion

	#endregion

	#region Helpers

	private static void GroupFilesByRelativeDirectoryBeEquivalentTo(FolderProcessType folderProcessType, GroupByFolderType? groupByFolderType, IReadOnlyList<Photo> photos,
		Dictionary<string, List<Photo>> expectedGroupedPhotoInfosByRelativeDirectory, bool invalidFileFormatGroupedInSubFolder = false, bool noPhotoDateTimeTakenGroupedInSubFolder = false, bool noReverseGeocodeGroupedInSubFolder = false)
	{
		var fileSystem = new MockFileSystem();
		var sut = new DirectoryGrouperService(fileSystem, ToolOptionFakes.Create(), NullLogger<DirectoryGrouperService>.Instance, ConsoleWriterFakes.Valid());

		var actualGroupedPhotoPathsByRelativeDirectory = sut.GroupFiles(photos, SourcePath, folderProcessType, groupByFolderType,
			invalidFileFormatGroupedInSubFolder, noPhotoDateTimeTakenGroupedInSubFolder, noReverseGeocodeGroupedInSubFolder);

		actualGroupedPhotoPathsByRelativeDirectory.Should().BeEquivalentTo(expectedGroupedPhotoInfosByRelativeDirectory);
	}

	#endregion

	#region Fakes

	#region Root Folder

	private static Photo PhotoInputRootFolder(string fileName, ExifData? exifData)
	{
		return PhotoFakes.WithSourcePathAndExifData(SourceRootPath(fileName), exifData);
	}

	private static Photo GroupedPhotoRootFolder(string fileName, ExifData? exifData, string targetRelativeDirectoryPath)
	{
		return PhotoFakes.WithValidFilePathAndExifData($"/{SourcePath}", $"{fileName}.{PhotoExtension}", exifData, targetRelativeDirectoryPath);
	}

	private static Photo GroupedInvalidPhotoRootFolder(string fileName, string targetRelativeDirectoryPath)
	{
		return PhotoFakes.WithValidFilePathInvalidFileFormat($"/{SourcePath}", $"{fileName}.{PhotoExtension}", targetRelativeDirectoryPath);
	}

	private static string SourceRootPath(string fileName)
	{
		return MockFileSystemHelper.Path($"/{SourcePath}/{fileName}.{PhotoExtension}");
	}

	#endregion

	#region Sub Folder

	private static Photo PhotoInputSubFolder(string fileName, ExifData? exifData)
	{
		return PhotoFakes.WithSourcePathAndExifData(SourceSubPath(fileName), exifData);
	}

	private static Photo GroupedPhotoSubFolder(string fileName, ExifData? exifData, string targetRelativeDirectoryPath)
	{
		return PhotoFakes.WithValidFilePathAndExifData($"/{SourcePath}/{SubPath}", $"{fileName}.{PhotoExtension}", exifData, targetRelativeDirectoryPath);
	}

	private static Photo GroupedInvalidPhotoSubFolder(string fileName, string targetRelativeDirectoryPath)
	{
		return PhotoFakes.WithValidFilePathInvalidFileFormat($"/{SourcePath}/{SubPath}", $"{fileName}.{PhotoExtension}", targetRelativeDirectoryPath);
	}

	private static string SourceSubPath(string fileName)
	{
		return MockFileSystemHelper.Path($"/{SourcePath}/{SubPath}/{fileName}.{PhotoExtension}");
	}

	#endregion

	private const string SourcePath = "source-path";
	private const string SubPath = "sub-path";
	private const string RootTargetRelativePath = "";
	private const string PhotoExtension = "jpg";

	#endregion
}
