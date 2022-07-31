namespace PhotoCli.Tests.UnitTests.Services;

public class PhotoOrganizerUnitTests
{
	#region NoPhotoDateTimeTakenAction

	#region All Valid Photo

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> AllPhotosThatHasDateEnsureListOrderedData = new()
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
	[MemberData(nameof(AllPhotosThatHasDateEnsureListOrderedData))]
	public void All_Photos_That_Has_Date_Ensure_List_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		var noPhotoDateTimeTakenActions = new[]
		{
			CopyNoPhotoTakenDateAction.InSubFolder, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoPhotoTakenDateAction.AppendToEndOrderByFileName,
			CopyNoPhotoTakenDateAction.Continue, CopyNoPhotoTakenDateAction.InsertToBeginningOrderByFileName
		};
		foreach (var noPhotoDateTimeTakenAction in noPhotoDateTimeTakenActions)
			OrderCheckListEquivalent(sourceList, expectedOrderedList, noPhotoDateTimeTakenAction);
	}

	#endregion

	#region DontCopyToOutput

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> CombinedPhotosWithDateAndNoDateShouldBeFilteredAndEnsureListOrderedData = new()
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
	[MemberData(nameof(CombinedPhotosWithDateAndNoDateShouldBeFilteredAndEnsureListOrderedData))]
	public void With_DontCopyToOutput_Combined_Photos_With_Date_And_No_Date_Should_Be_Filtered_And_Ensure_List_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, CopyNoPhotoTakenDateAction.DontCopyToOutput);
	}

	#endregion

	#region AppendToEndOrderByFileName

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> AppendToEndOrderByFileNameEnsureListOrderedData = new()
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
	[MemberData(nameof(AppendToEndOrderByFileNameEnsureListOrderedData))]
	public void AppendToEndOrderByFileName_Ensure_List_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, CopyNoPhotoTakenDateAction.AppendToEndOrderByFileName);
	}

	#endregion

	#region InsertToBeginningOrderByFileName

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> InsertToBeginningOrderByFileNameEnsureListOrderedData = new()
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
	[MemberData(nameof(InsertToBeginningOrderByFileNameEnsureListOrderedData))]
	public void InsertToBeginningOrderByFileName_Ensure_List_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, CopyNoPhotoTakenDateAction.InsertToBeginningOrderByFileName);
	}

	#endregion

	#endregion

	#region NoCoordinateAction

	#region All Valid ReverseGeocode

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> AllPhotosThatHasReverseGeocodeEnsureListOrderedData = new()
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
	[MemberData(nameof(AllPhotosThatHasReverseGeocodeEnsureListOrderedData))]
	public void All_Photos_That_Has_ReverseGeocode_Ensure_List_Not_Filtered_And_Ordered_By_Date(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
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

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> CombinedPhotosThatHasReverseGeocodeOrNoReverseGeocodeEnsureListFilteredData = new()
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
	[MemberData(nameof(CombinedPhotosThatHasReverseGeocodeOrNoReverseGeocodeEnsureListFilteredData))]
	public void Combined_Photos_That_Has_ReverseGeocode_Or_NoReverseGeocode_Ensure_List_Filtered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, noCoordinateAction: CopyNoCoordinateAction.DontCopyToOutput);
	}

	#endregion

	#endregion

	#region NoPhotoDateTimeTakenAction & NoCoordinateAction

	#region Both DontCopyToOutput

	public static TheoryData<List<Photo>, IReadOnlyCollection<Photo>> UsingDontCopyToOutputOnPhotoTakenDateActionAndNoCoordinateActionEnsureListFilteredAndOrderedData = new()
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
	[MemberData(nameof(UsingDontCopyToOutputOnPhotoTakenDateActionAndNoCoordinateActionEnsureListFilteredAndOrderedData))]
	public void Using_DontCopyToOutput_On_PhotoTakenDateAction_And_NoCoordinateAction_Ensure_List_Filtered_And_Ordered(List<Photo> sourceList, IReadOnlyCollection<Photo> expectedOrderedList)
	{
		OrderCheckListEquivalent(sourceList, expectedOrderedList, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput);
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
		OrderCheckListEquivalentWithNotToRenamePhotos(sourceList, expectedOrderedList, expectedNotToRenamePhotos, CopyNoPhotoTakenDateAction.Continue,
			CopyNoCoordinateAction.Continue);
	}

	#endregion

	#endregion

	private void OrderCheckListEquivalent(IReadOnlyCollection<Photo> sourceList, IEnumerable<Photo> expectedOrderedList,
		CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction = CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction noCoordinateAction = CopyNoCoordinateAction.Continue)
	{
		var sut = new ExifOrganizerService(NullLogger<ExifOrganizerService>.Instance);
		var (orderedPhotos, _) = sut.FilterAndSortByNoActionTypes(sourceList, noPhotoDateTimeTakenAction, noCoordinateAction);
		orderedPhotos.Should().BeEquivalentTo(expectedOrderedList, options => options.WithStrictOrdering());
	}

	private void OrderCheckListEquivalentWithNotToRenamePhotos(IReadOnlyCollection<Photo> sourceList, IEnumerable<Photo> expectedOrderedList, IEnumerable<Photo> expectedNotToRenamePhotos,
		CopyNoPhotoTakenDateAction noPhotoDateTimeTakenAction, CopyNoCoordinateAction noCoordinateAction)
	{
		var sut = new ExifOrganizerService(NullLogger<ExifOrganizerService>.Instance);
		var (orderedPhotos, notToRenamePhotos) = sut.FilterAndSortByNoActionTypes(sourceList, noPhotoDateTimeTakenAction, noCoordinateAction);
		orderedPhotos.Should().BeEquivalentTo(expectedOrderedList, options => options.WithStrictOrdering());
		notToRenamePhotos.Should().BeEquivalentTo(expectedNotToRenamePhotos, options => options.WithStrictOrdering());
	}
}
