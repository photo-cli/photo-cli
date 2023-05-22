namespace PhotoCli.Tests.UnitTests.Services;

public class ExifOrganizerUnitTests
{
	#region NoPhotoDateTimeTakenAction

	#region All Valid Photo

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> PhotoDateTimeTakenActionAllPhotosThatHasDateEnsureListOrderedData = new()
	{
		{
			new List<Photo> { PhotoFakes.WithDay(1) },
			new List<Photo> { PhotoFakes.WithDay(1) }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(2) },
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(2) }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(2), PhotoFakes.WithDay(1) },
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(2) }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(2), PhotoFakes.WithDay(1) },
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(2) }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(2), PhotoFakes.WithDay(3), PhotoFakes.WithDay(1) },
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(2), PhotoFakes.WithDay(3) }
		},
	};

	[Theory]
	[MemberData(nameof(PhotoDateTimeTakenActionAllPhotosThatHasDateEnsureListOrderedData))]
	public void PhotoDateTimeTakenAction_All_Photos_That_Has_Date_Ensure_List_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		var noPhotoDateTimeTakenActions = new[]
		{
			CopyNoPhotoTakenDateAction.InSubFolder, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoPhotoTakenDateAction.AppendToEndOrderByFileName,
			CopyNoPhotoTakenDateAction.Continue, CopyNoPhotoTakenDateAction.InsertToBeginningOrderByFileName
		};
		foreach (var noPhotoDateTimeTakenAction in noPhotoDateTimeTakenActions)
			OrderCheckListEquivalent(sourceList, expectedOrderedList, noPhotoDateTimeTakenAction: noPhotoDateTimeTakenAction);
	}

	#endregion

	#region DontCopyToOutput

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> PhotoDateTimeTakenActionCombinedPhotosWithDateAndNoDateShouldBeFilteredAndEnsureListOrderedData = new()
	{
		{
			new List<Photo> { PhotoFakes.NoPhotoTakenDate() },
			new List<Photo>()
		},
		{
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(2) },
			new List<Photo> { PhotoFakes.WithDay(2) }
		},
		{
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(1) },
			new List<Photo> { PhotoFakes.WithDay(1) }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(3), PhotoFakes.WithDay(2), PhotoFakes.NoPhotoTakenDate() },
			new List<Photo> { PhotoFakes.WithDay(2), PhotoFakes.WithDay(3) }
		},
	};

	[Theory]
	[MemberData(nameof(PhotoDateTimeTakenActionCombinedPhotosWithDateAndNoDateShouldBeFilteredAndEnsureListOrderedData))]
	public void PhotoDateTimeTakenAction_With_DontCopyToOutput_Combined_Photos_With_Date_And_No_Date_Should_Be_Filtered_And_Ensure_List_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, noPhotoDateTimeTakenAction: CopyNoPhotoTakenDateAction.DontCopyToOutput);
	}

	#endregion

	#region AppendToEndOrderByFileName

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> PhotoDateTimeTakenActionAppendToEndOrderByFileNameEnsureListOrderedData = new()
	{
		{
			new List<Photo> { PhotoFakes.NoPhotoTakenDate() },
			new List<Photo> { PhotoFakes.NoPhotoTakenDate() }
		},
		{
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(2) },
			new List<Photo> { PhotoFakes.WithDay(2), PhotoFakes.NoPhotoTakenDate() }
		},
		{
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(1) },
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.NoPhotoTakenDate() }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(3), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(2) },
			new List<Photo> { PhotoFakes.WithDay(2), PhotoFakes.WithDay(3), PhotoFakes.NoPhotoTakenDate() }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(3), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(2), PhotoFakes.NoPhotoTakenDate() },
			new List<Photo> { PhotoFakes.WithDay(2), PhotoFakes.WithDay(3), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.NoPhotoTakenDate() }
		},
	};

	[Theory]
	[MemberData(nameof(PhotoDateTimeTakenActionAppendToEndOrderByFileNameEnsureListOrderedData))]
	public void PhotoDateTimeTakenAction_AppendToEndOrderByFileName_Ensure_List_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, noPhotoDateTimeTakenAction: CopyNoPhotoTakenDateAction.AppendToEndOrderByFileName);
	}

	#endregion

	#region InsertToBeginningOrderByFileName

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> PhotoDateTimeTakenActionInsertToBeginningOrderByFileNameEnsureListOrderedData = new()
	{
		{
			new List<Photo> { PhotoFakes.NoPhotoTakenDate() },
			new List<Photo> { PhotoFakes.NoPhotoTakenDate() }
		},
		{
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(2) },
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(2) }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.NoPhotoTakenDate() },
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(1) }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(3), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(2) },
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(2), PhotoFakes.WithDay(3) }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(3), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(2), PhotoFakes.NoPhotoTakenDate() },
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(2), PhotoFakes.WithDay(3) }
		},
	};

	[Theory]
	[MemberData(nameof(PhotoDateTimeTakenActionInsertToBeginningOrderByFileNameEnsureListOrderedData))]
	public void PhotoDateTimeTakenAction_InsertToBeginningOrderByFileName_Ensure_List_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, noPhotoDateTimeTakenAction: CopyNoPhotoTakenDateAction.InsertToBeginningOrderByFileName);
	}

	#endregion

	#endregion

	#region NoCoordinateAction

	#region All Valid ReverseGeocode

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> NoCoordinateActionAllPhotosThatHasReverseGeocodeEnsureListOrderedData = new()
	{
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1) },
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1) }
		},
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 2), PhotoFakes.WithReverseGeocodeAndDay(1, 1) },
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 2) }
		},
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 2), PhotoFakes.WithReverseGeocodeAndDay(2, 3), PhotoFakes.WithReverseGeocodeAndDay(2, 1) },
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(2, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 2), PhotoFakes.WithReverseGeocodeAndDay(2, 3) }
		},
	};

	[Theory]
	[MemberData(nameof(NoCoordinateActionAllPhotosThatHasReverseGeocodeEnsureListOrderedData))]
	public void NoCoordinateAction_All_Photos_That_Has_ReverseGeocode_Ensure_List_Not_Filtered_And_Ordered_By_Date(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		var noCoordinateActions = new[]
		{
			CopyNoCoordinateAction.Continue, CopyNoCoordinateAction.InSubFolder,
		};
		foreach (var noCoordinateAction in noCoordinateActions)
			OrderCheckListEquivalent(sourceList, expectedOrderedList, noCoordinateAction: noCoordinateAction);
	}

	#endregion

	#region DontCopyToOutput

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> NoCoordinateActionCombinedPhotosThatHasReverseGeocodeOrNoReverseGeocodeEnsureListFilteredData = new()
	{
		{
			new List<Photo> { PhotoFakes.NoReverseGeocode() },
			new List<Photo>()
		},
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.NoReverseGeocode() },
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1) }
		},
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 2), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithReverseGeocodeAndDay(2, 1) },
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(2, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 2) }
		},
	};

	[Theory]
	[MemberData(nameof(NoCoordinateActionCombinedPhotosThatHasReverseGeocodeOrNoReverseGeocodeEnsureListFilteredData))]
	public void NoCoordinateAction_Combined_Photos_That_Has_ReverseGeocode_Or_NoReverseGeocode_Ensure_List_Filtered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, noCoordinateAction: CopyNoCoordinateAction.DontCopyToOutput);
	}

	#endregion

	#endregion

	#region NoPhotoDateTimeTakenAction & NoCoordinateAction

	#region Both DontCopyToOutput

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> NoPhotoDateTimeAndNoCoordinateActionUsingDontCopyToOutputOnPhotoTakenDateActionAndNoCoordinateActionEnsureListFilteredAndOrderedData = new()
	{
		{
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.NoReverseGeocode() },
			new List<Photo>()
		},
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.NoReverseGeocode(), PhotoFakes.NoPhotoTakenDate() },
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1) }
		},
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 2), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.NoReverseGeocode(), PhotoFakes.WithReverseGeocodeAndDay(2, 1) },
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(2, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 2) }
		},
	};

	[Theory]
	[MemberData(nameof(NoPhotoDateTimeAndNoCoordinateActionUsingDontCopyToOutputOnPhotoTakenDateActionAndNoCoordinateActionEnsureListFilteredAndOrderedData))]
	public void NoPhotoDateTimeAndNoCoordinateAction_Using_DontCopyToOutput_On_PhotoTakenDateAction_And_NoCoordinateAction_Ensure_List_Filtered_And_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, noPhotoDateTimeTakenAction: CopyNoPhotoTakenDateAction.DontCopyToOutput, noCoordinateAction: CopyNoCoordinateAction.DontCopyToOutput);
	}

	#endregion

	#region Both CopyToOutputDontChangeFileName

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>, IReadOnlyCollection<Photo>>
		UsingContinueDontChangeFileNameOnPhotoTakenDateActionAndNoCoordinateActionEnsureListFilteredAndOrderedData = new()
		{
			{
				new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.NoReverseGeocode() },
				new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.NoReverseGeocode() },
				new List<Photo>()
			},
			{
				new List<Photo> { PhotoFakes.NoReverseGeocode(), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithReverseGeocodeAndDay(1, 1) },
				new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.NoReverseGeocode(), PhotoFakes.NoPhotoTakenDate() },
				new List<Photo>()
			},
			{
				new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithReverseGeocodeAndDay(1, 2), PhotoFakes.NoReverseGeocode(), PhotoFakes.WithReverseGeocodeAndDay(2, 1), },
				new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(2, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 2), PhotoFakes.NoReverseGeocode(), PhotoFakes.NoPhotoTakenDate(), },
				new List<Photo>()
			},
		};

	[Theory]
	[MemberData(nameof(UsingContinueDontChangeFileNameOnPhotoTakenDateActionAndNoCoordinateActionEnsureListFilteredAndOrderedData))]
	public void Using_CopyToOutputDontChangeFileName_On_PhotoTakenDateAction_And_NoCoordinateAction_Ensure_List_Filtered_And_Ordered(List<Photo> sourceList,
		IReadOnlyCollection<Photo> expectedOrderedList, IReadOnlyCollection<Photo> expectedNotToRenamePhotos)
	{
		OrderCheckListEquivalentWithNotToRenamePhotos(sourceList, expectedOrderedList, expectedNotToRenamePhotos, noPhotoDateTimeTakenAction: CopyNoPhotoTakenDateAction.Continue,
			noCoordinateAction: CopyNoCoordinateAction.Continue);
	}

	#endregion

	#endregion

	#region InvalidFormatAction

	#region All Valid Photo

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> InvalidFormatActionAllPhotosThatIsValidEnsureListOrderedByPhotoTakeDateData = new()
	{
		{
			new List<Photo> { PhotoFakes.ValidFileWithDay(1) },
			new List<Photo> { PhotoFakes.ValidFileWithDay(1) }
		},
		{
			new List<Photo> { PhotoFakes.ValidFileWithDay(1), PhotoFakes.ValidFileWithDay(2) },
			new List<Photo> { PhotoFakes.ValidFileWithDay(1), PhotoFakes.ValidFileWithDay(2) }
		},
		{
			new List<Photo> { PhotoFakes.ValidFileWithDay(2), PhotoFakes.ValidFileWithDay(1) },
			new List<Photo> { PhotoFakes.ValidFileWithDay(1), PhotoFakes.ValidFileWithDay(2) }
		},
		{
			new List<Photo> { PhotoFakes.ValidFileWithDay(2), PhotoFakes.ValidFileWithDay(1) },
			new List<Photo> { PhotoFakes.ValidFileWithDay(1), PhotoFakes.ValidFileWithDay(2) }
		},
		{
			new List<Photo> { PhotoFakes.ValidFileWithDay(2), PhotoFakes.ValidFileWithDay(3), PhotoFakes.ValidFileWithDay(1) },
			new List<Photo> { PhotoFakes.ValidFileWithDay(1), PhotoFakes.ValidFileWithDay(2), PhotoFakes.ValidFileWithDay(3) }
		},
	};

	[Theory]
	[MemberData(nameof(InvalidFormatActionAllPhotosThatIsValidEnsureListOrderedByPhotoTakeDateData))]
	public void InvalidFormatAction_All_Photos_That_Is_Valid_Ensure_List_Ordered_By_Photo_Take_Date(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		var invalidFormatActions = new[]
		{
			CopyInvalidFormatAction.Continue, CopyInvalidFormatAction.PreventProcess, CopyInvalidFormatAction.InSubFolder, CopyInvalidFormatAction.DontCopyToOutput
		};
		foreach (var invalidFormatAction in invalidFormatActions)
			OrderCheckListEquivalent(sourceList, expectedOrderedList, invalidFormatAction: invalidFormatAction);
	}

	#endregion

	#region DontCopyToOutput

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> InvalidFormatActionDontCopyToOutputCombinedPhotosWithValidAndInvalidShouldBeFilteredAndEnsureListOrderedData = new()
	{

		{
			new List<Photo> { PhotoFakes.WithInvalidFileFormat() },
			new List<Photo>()
		},
		{
			new List<Photo> { PhotoFakes.WithInvalidFileFormat(), PhotoFakes.WithDay(2) },
			new List<Photo> { PhotoFakes.WithDay(2) }
		},
		{
			new List<Photo> { PhotoFakes.WithInvalidFileFormat(), PhotoFakes.WithDay(1) },
			new List<Photo> { PhotoFakes.WithDay(1) }
		},
		{
			new List<Photo> { PhotoFakes.WithDay(3), PhotoFakes.WithDay(2), PhotoFakes.WithInvalidFileFormat() },
			new List<Photo> { PhotoFakes.WithDay(2), PhotoFakes.WithDay(3) }
		},
		{
			new List<Photo> { PhotoFakes.WithInvalidFileFormat(), PhotoFakes.NoPhotoTakenDate() },
			new List<Photo> { PhotoFakes.NoPhotoTakenDate() }
		},
		{
			new List<Photo> { PhotoFakes.NoReverseGeocode(), PhotoFakes.WithInvalidFileFormat()},
			new List<Photo> { PhotoFakes.NoReverseGeocode() }
		},
	};

	[Theory]
	[MemberData(nameof(InvalidFormatActionDontCopyToOutputCombinedPhotosWithValidAndInvalidShouldBeFilteredAndEnsureListOrderedData))]
	public void InvalidFormatAction_DontCopyToOutput_Combined_Photos_With_Valid_And_Invalid_Should_Be_Filtered_And_Ensure_List_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, invalidFormatAction: CopyInvalidFormatAction.DontCopyToOutput);
	}

	#endregion

	#endregion

	private void OrderCheckListEquivalent(IReadOnlyCollection<Photo> sourceList, IEnumerable<Photo> expectedOrderedList,
		CopyInvalidFormatAction invalidFormatAction = CopyInvalidFormatAction.Continue, CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction = CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction noCoordinateAction = CopyNoCoordinateAction.Continue)
	{
		var sut = new ExifOrganizerService(NullLogger<ExifOrganizerService>.Instance);
		var (orderedPhotos, _) = sut.FilterAndSortByNoActionTypes(sourceList, invalidFormatAction, noPhotoDateTimeTakenAction, noCoordinateAction, string.Empty);
		orderedPhotos.Should().BeEquivalentTo(expectedOrderedList, options => options.WithStrictOrdering());
	}

	private void OrderCheckListEquivalentWithNotToRenamePhotos(IReadOnlyCollection<Photo> sourceList, IEnumerable<Photo> expectedOrderedList, IEnumerable<Photo> expectedNotToRenamePhotos,
		CopyInvalidFormatAction invalidFormatAction = CopyInvalidFormatAction.Continue, CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction = CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction noCoordinateAction = CopyNoCoordinateAction.Continue)
	{
		var sut = new ExifOrganizerService(NullLogger<ExifOrganizerService>.Instance);
		var (orderedPhotos, notToRenamePhotos) = sut.FilterAndSortByNoActionTypes(sourceList, invalidFormatAction, noPhotoDateTimeTakenAction, noCoordinateAction, string.Empty);
		orderedPhotos.Should().BeEquivalentTo(expectedOrderedList, options => options.WithStrictOrdering());
		notToRenamePhotos.Should().BeEquivalentTo(expectedNotToRenamePhotos, options => options.WithStrictOrdering());
	}
}
