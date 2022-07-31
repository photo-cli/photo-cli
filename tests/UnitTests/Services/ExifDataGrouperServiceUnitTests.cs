namespace PhotoCli.Tests.UnitTests.Services;

public class ExifDataGrouperServiceUnitTests
{
	private readonly ExifDataGrouperService _sut = new(ToolOptionFakes.Create(), NullLogger<ExifDataGrouperService>.Instance);

	#region Date

	public static TheoryData<List<Photo>, Dictionary<string, List<Photo>>> GroupingByDateShouldBeEquivalentToExpectedData = new()
	{
		{
			// two different day with different counts
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(1), PhotoFakes.WithDay(2) },
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatDay(1),
					new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(1) }
				},
				{
					DateTimeFakes.FormatDay(2),
					new List<Photo> { PhotoFakes.WithDay(2) }
				}
			}
		},
		{
			// no date should be filtered out, 3 different day
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithDay(3), PhotoFakes.WithDay(1), PhotoFakes.WithDay(2), PhotoFakes.WithDay(3), PhotoFakes.WithDay(1) },
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatDay(1),
					new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(1) }
				},
				{
					DateTimeFakes.FormatDay(2),
					new List<Photo> { PhotoFakes.WithDay(2) }
				},
				{
					DateTimeFakes.FormatDay(3),
					new List<Photo> { PhotoFakes.WithDay(3), PhotoFakes.WithDay(3) }
				}
			}
		},
		{
			// empty
			new List<Photo>(),
			new Dictionary<string, List<Photo>>()
		}
	};

	[Theory]
	[MemberData(nameof(GroupingByDateShouldBeEquivalentToExpectedData))]
	public void Grouping_By_Date_Should_Be_Equivalent_To_Expected(List<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos)
	{
		GroupByNamingStyleEquivalentToExpected(photoInfos, expectedGroupedPhotoInfos, NamingStyle.Day);
	}

	#endregion

	#region Minute

	public static TheoryData<List<Photo>, Dictionary<string, List<Photo>>> GroupingByMinuteShouldBeEquivalentToExpectedData = new()
	{
		{
			// two different minute with different counts
			new List<Photo> { PhotoFakes.WithMinute(1), PhotoFakes.WithMinute(1), PhotoFakes.WithMinute(2) },
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatMinute(1),
					new List<Photo> { PhotoFakes.WithMinute(1), PhotoFakes.WithMinute(1) }
				},
				{
					DateTimeFakes.FormatMinute(2),
					new List<Photo> { PhotoFakes.WithMinute(2) }
				}
			}
		},
		{
			// no date should be filtered out, 3 different minute
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithMinute(3), PhotoFakes.WithMinute(1), PhotoFakes.WithMinute(2), PhotoFakes.WithMinute(3), PhotoFakes.WithMinute(1) },
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatMinute(1),
					new List<Photo> { PhotoFakes.WithMinute(1), PhotoFakes.WithMinute(1) }
				},
				{
					DateTimeFakes.FormatMinute(2),
					new List<Photo> { PhotoFakes.WithMinute(2) }
				},
				{
					DateTimeFakes.FormatMinute(3),
					new List<Photo> { PhotoFakes.WithMinute(3), PhotoFakes.WithMinute(3) }
				}
			}
		},
		{
			// empty
			new List<Photo>(),
			new Dictionary<string, List<Photo>>()
		}
	};

	[Theory]
	[MemberData(nameof(GroupingByMinuteShouldBeEquivalentToExpectedData))]
	public void Grouping_By_Minute_Should_Be_Equivalent_To_Expected(List<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos)
	{
		GroupByNamingStyleEquivalentToExpected(photoInfos, expectedGroupedPhotoInfos, NamingStyle.DateTimeWithMinutes);
	}

	#endregion

	#region Second

	public static TheoryData<List<Photo>, Dictionary<string, List<Photo>>> GroupingBySecondShouldBeEquivalentToExpectedData = new()
	{
		{
			// two different second with different counts
			new List<Photo> { PhotoFakes.WithSecond(1), PhotoFakes.WithSecond(1), PhotoFakes.WithSecond(2) },
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatSecond(1),
					new List<Photo> { PhotoFakes.WithSecond(1), PhotoFakes.WithSecond(1) }
				},
				{
					DateTimeFakes.FormatSecond(2),
					new List<Photo> { PhotoFakes.WithSecond(2) }
				}
			}
		},
		{
			// no date should be filtered out, 3 different second
			new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithSecond(3), PhotoFakes.WithSecond(1), PhotoFakes.WithSecond(2), PhotoFakes.WithSecond(3), PhotoFakes.WithSecond(1) },
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatSecond(1),
					new List<Photo> { PhotoFakes.WithSecond(1), PhotoFakes.WithSecond(1) }
				},
				{
					DateTimeFakes.FormatSecond(2),
					new List<Photo> { PhotoFakes.WithSecond(2) }
				},
				{
					DateTimeFakes.FormatSecond(3),
					new List<Photo> { PhotoFakes.WithSecond(3), PhotoFakes.WithSecond(3) }
				}
			}
		},
		{
			// empty
			new List<Photo>(),
			new Dictionary<string, List<Photo>>()
		}
	};

	[Theory]
	[MemberData(nameof(GroupingBySecondShouldBeEquivalentToExpectedData))]
	public void Grouping_By_Second_Should_Be_Equivalent_To_Expected(List<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos)
	{
		GroupByNamingStyleEquivalentToExpected(photoInfos, expectedGroupedPhotoInfos, NamingStyle.DateTimeWithSeconds);
	}

	#endregion

	#region ReverseGeocode

	public static TheoryData<List<Photo>, Dictionary<string, List<Photo>>> GroupingByAddressShouldBeEquivalentToExpectedData = new()
	{
		{
			// two different reverse-geocode with different counts
			new List<Photo> { PhotoFakes.WithReverseGeocodeSampleId(1), PhotoFakes.WithReverseGeocodeSampleId(1), PhotoFakes.WithReverseGeocodeSampleId(2) },
			new Dictionary<string, List<Photo>>
			{
				{
					ReverseGeocodeFakes.FlatFormatSampleId(1),
					new List<Photo> { PhotoFakes.WithReverseGeocodeSampleId(1), PhotoFakes.WithReverseGeocodeSampleId(1) }
				},
				{
					ReverseGeocodeFakes.FlatFormatSampleId(2),
					new List<Photo> { PhotoFakes.WithReverseGeocodeSampleId(2) }
				}
			}
		},
		{
			// no date should be filtered out, 3 different day
			new List<Photo>
			{
				PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithReverseGeocodeSampleId(3), PhotoFakes.WithReverseGeocodeSampleId(1), PhotoFakes.WithReverseGeocodeSampleId(2),
				PhotoFakes.WithReverseGeocodeSampleId(3), PhotoFakes.WithReverseGeocodeSampleId(1)
			},
			new Dictionary<string, List<Photo>>
			{
				{
					ReverseGeocodeFakes.FlatFormatSampleId(1),
					new List<Photo> { PhotoFakes.WithReverseGeocodeSampleId(1), PhotoFakes.WithReverseGeocodeSampleId(1) }
				},
				{
					ReverseGeocodeFakes.FlatFormatSampleId(2),
					new List<Photo> { PhotoFakes.WithReverseGeocodeSampleId(2) }
				},
				{
					ReverseGeocodeFakes.Format(ReverseGeocodeFakes.Sample(3)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeSampleId(3), PhotoFakes.WithReverseGeocodeSampleId(3) }
				}
			}
		},
		{
			// empty
			new List<Photo>(),
			new Dictionary<string, List<Photo>>()
		}
	};

	[Theory]
	[MemberData(nameof(GroupingByAddressShouldBeEquivalentToExpectedData))]
	public void Grouping_By_Address_Should_Be_Equivalent_To_Expected(List<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos)
	{
		GroupByNamingStyleEquivalentToExpected(photoInfos, expectedGroupedPhotoInfos, NamingStyle.Address);
	}

	#endregion

	#region AddressDay

	public static TheoryData<List<Photo>, Dictionary<string, List<Photo>>> GroupingByAddressDayShouldBeEquivalentToExpectedData = new()
	{
		{
			// two different day, all same reverse-geocode with different counts
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 2) },
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatDay(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 1) }
				},
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatDay(2)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 2) }
				}
			}
		},
		{
			// no reverse-geocode, no date should be filtered out, 3 different group key
			new List<Photo>
			{
				PhotoFakes.NoReverseGeocode(), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithReverseGeocodeAndDay(1, 2), PhotoFakes.WithReverseGeocodeAndDay(1, 1),
				PhotoFakes.WithReverseGeocodeAndDay(2, 1),
				PhotoFakes.WithReverseGeocodeAndDay(1, 1)
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatDay(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 1) }
				},
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatDay(2)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 2) }
				},
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(2), DateTimeFakes.FormatDay(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(2, 1) }
				}
			}
		},
		{
			// empty
			new List<Photo>(),
			new Dictionary<string, List<Photo>>()
		}
	};

	[Theory]
	[MemberData(nameof(GroupingByAddressDayShouldBeEquivalentToExpectedData))]
	public void Grouping_By_AddressDay_Should_Be_Equivalent_To_Expected(List<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos)
	{
		GroupByNamingStyleEquivalentToExpected(photoInfos, expectedGroupedPhotoInfos, NamingStyle.AddressDay);
	}

	#endregion

	#region DayAddress

	public static TheoryData<List<Photo>, Dictionary<string, List<Photo>>> GroupingByDayAddressShouldBeEquivalentToExpectedData = new()
	{
		{
			// two different day, all same reverse-geocode with different counts
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 2) },
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatDay(1), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 1) }
				},
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatDay(2), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 2) }
				}
			}
		},
		{
			// no reverse-geocode, no date should be filtered out, 3 different group key
			new List<Photo>
			{
				PhotoFakes.NoReverseGeocode(), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithReverseGeocodeAndDay(1, 2), PhotoFakes.WithReverseGeocodeAndDay(1, 1),
				PhotoFakes.WithReverseGeocodeAndDay(2, 1),
				PhotoFakes.WithReverseGeocodeAndDay(1, 1)
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatDay(1), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 1), PhotoFakes.WithReverseGeocodeAndDay(1, 1) }
				},
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatDay(2), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(1, 2) }
				},
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatDay(1), ReverseGeocodeFakes.Sample(2)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndDay(2, 1) }
				}
			}
		},
		{
			// empty
			new List<Photo>(),
			new Dictionary<string, List<Photo>>()
		}
	};

	[Theory]
	[MemberData(nameof(GroupingByDayAddressShouldBeEquivalentToExpectedData))]
	public void Grouping_By_DayAddress_Should_Be_Equivalent_To_Expected(List<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos)
	{
		GroupByNamingStyleEquivalentToExpected(photoInfos, expectedGroupedPhotoInfos, NamingStyle.DayAddress);
	}

	#endregion

	#region AddressDateTimeWithMinutes

	public static TheoryData<List<Photo>, Dictionary<string, List<Photo>>> GroupingByAddressDateTimeWithMinutesShouldBeEquivalentToExpectedData = new()
	{
		{
			// two different day, all same reverse-geocode with different counts
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(1, 1), PhotoFakes.WithReverseGeocodeAndMinute(1, 1), PhotoFakes.WithReverseGeocodeAndMinute(1, 2) },
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatMinute(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(1, 1), PhotoFakes.WithReverseGeocodeAndMinute(1, 1) }
				},
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatMinute(2)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(1, 2) }
				}
			}
		},
		{
			// no reverse-geocode, no date should be filtered out, 3 different group key
			new List<Photo>
			{
				PhotoFakes.NoReverseGeocode(), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithReverseGeocodeAndMinute(1, 2), PhotoFakes.WithReverseGeocodeAndMinute(1, 1),
				PhotoFakes.WithReverseGeocodeAndMinute(2, 1),
				PhotoFakes.WithReverseGeocodeAndMinute(1, 1)
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatMinute(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(1, 1), PhotoFakes.WithReverseGeocodeAndMinute(1, 1) }
				},
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatMinute(2)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(1, 2) }
				},
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(2), DateTimeFakes.FormatMinute(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(2, 1) }
				}
			}
		},
		{
			// empty
			new List<Photo>(),
			new Dictionary<string, List<Photo>>()
		}
	};

	[Theory]
	[MemberData(nameof(GroupingByAddressDateTimeWithMinutesShouldBeEquivalentToExpectedData))]
	public void Grouping_By_AddressDateTimeWithMinutes_Should_Be_Equivalent_To_Expected(List<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos)
	{
		GroupByNamingStyleEquivalentToExpected(photoInfos, expectedGroupedPhotoInfos, NamingStyle.AddressDateTimeWithMinutes);
	}

	#endregion

	#region DateTimeWithMinutesAddress

	public static TheoryData<List<Photo>, Dictionary<string, List<Photo>>> GroupingByDateTimeWithMinutesAddressShouldBeEquivalentToExpectedData = new()
	{
		{
			// two different day, all same reverse-geocode with different counts
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(1, 1), PhotoFakes.WithReverseGeocodeAndMinute(1, 1), PhotoFakes.WithReverseGeocodeAndMinute(1, 2) },
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatMinute(1), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(1, 1), PhotoFakes.WithReverseGeocodeAndMinute(1, 1) }
				},
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatMinute(2), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(1, 2) }
				}
			}
		},
		{
			// no reverse-geocode, no date should be filtered out, 3 different group key
			new List<Photo>
			{
				PhotoFakes.NoReverseGeocode(), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithReverseGeocodeAndMinute(1, 2), PhotoFakes.WithReverseGeocodeAndMinute(1, 1),
				PhotoFakes.WithReverseGeocodeAndMinute(2, 1),
				PhotoFakes.WithReverseGeocodeAndMinute(1, 1)
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatMinute(1), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(1, 1), PhotoFakes.WithReverseGeocodeAndMinute(1, 1) }
				},
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatMinute(2), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(1, 2) }
				},
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatMinute(1), ReverseGeocodeFakes.Sample(2)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndMinute(2, 1) }
				}
			}
		},
		{
			// empty
			new List<Photo>(),
			new Dictionary<string, List<Photo>>()
		}
	};

	[Theory]
	[MemberData(nameof(GroupingByDateTimeWithMinutesAddressShouldBeEquivalentToExpectedData))]
	public void Grouping_By_DateTimeWithMinutesAddress_Should_Be_Equivalent_To_Expected(List<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos)
	{
		GroupByNamingStyleEquivalentToExpected(photoInfos, expectedGroupedPhotoInfos, NamingStyle.DateTimeWithMinutesAddress);
	}

	#endregion

	#region AddressDateTimeWithSeconds

	public static TheoryData<List<Photo>, Dictionary<string, List<Photo>>> GroupingByAddressDateTimeWithSecondsShouldBeEquivalentToExpectedData = new()
	{
		{
			// two different day, all same reverse-geocode with different counts
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(1, 1), PhotoFakes.WithReverseGeocodeAndSecond(1, 1), PhotoFakes.WithReverseGeocodeAndSecond(1, 2) },
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatSecond(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(1, 1), PhotoFakes.WithReverseGeocodeAndSecond(1, 1) }
				},
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatSecond(2)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(1, 2) }
				}
			}
		},
		{
			// no reverse-geocode, no date should be filtered out, 3 different group key
			new List<Photo>
			{
				PhotoFakes.NoReverseGeocode(), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithReverseGeocodeAndSecond(1, 2), PhotoFakes.WithReverseGeocodeAndSecond(1, 1),
				PhotoFakes.WithReverseGeocodeAndSecond(2, 1),
				PhotoFakes.WithReverseGeocodeAndSecond(1, 1)
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatSecond(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(1, 1), PhotoFakes.WithReverseGeocodeAndSecond(1, 1) }
				},
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(1), DateTimeFakes.FormatSecond(2)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(1, 2) }
				},
				{
					DirectoryGrouperFormat(ReverseGeocodeFakes.Sample(2), DateTimeFakes.FormatSecond(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(2, 1) }
				}
			}
		},
		{
			// empty
			new List<Photo>(),
			new Dictionary<string, List<Photo>>()
		}
	};

	[Theory]
	[MemberData(nameof(GroupingByAddressDateTimeWithSecondsShouldBeEquivalentToExpectedData))]
	public void Grouping_By_AddressDateTimeWithSeconds_Should_Be_Equivalent_To_Expected(List<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos)
	{
		GroupByNamingStyleEquivalentToExpected(photoInfos, expectedGroupedPhotoInfos, NamingStyle.AddressDateTimeWithSeconds);
	}

	#endregion

	#region DateTimeWithSecondsAddress

	public static TheoryData<List<Photo>, Dictionary<string, List<Photo>>> GroupingByDateTimeWithSecondsAddressShouldBeEquivalentToExpectedData = new()
	{
		{
			// two different day, all same reverse-geocode with different counts
			new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(1, 1), PhotoFakes.WithReverseGeocodeAndSecond(1, 1), PhotoFakes.WithReverseGeocodeAndSecond(1, 2) },
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatSecond(1), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(1, 1), PhotoFakes.WithReverseGeocodeAndSecond(1, 1) }
				},
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatSecond(2), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(1, 2) }
				}
			}
		},
		{
			// no reverse-geocode, no date should be filtered out, 3 different group key
			new List<Photo>
			{
				PhotoFakes.NoReverseGeocode(), PhotoFakes.NoPhotoTakenDate(), PhotoFakes.WithReverseGeocodeAndSecond(1, 2), PhotoFakes.WithReverseGeocodeAndSecond(1, 1),
				PhotoFakes.WithReverseGeocodeAndSecond(2, 1),
				PhotoFakes.WithReverseGeocodeAndSecond(1, 1)
			},
			new Dictionary<string, List<Photo>>
			{
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatSecond(1), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(1, 1), PhotoFakes.WithReverseGeocodeAndSecond(1, 1) }
				},
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatSecond(2), ReverseGeocodeFakes.Sample(1)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(1, 2) }
				},
				{
					DirectoryGrouperFormat(DateTimeFakes.FormatSecond(1), ReverseGeocodeFakes.Sample(2)),
					new List<Photo> { PhotoFakes.WithReverseGeocodeAndSecond(2, 1) }
				}
			}
		},
		{
			// empty
			new List<Photo>(),
			new Dictionary<string, List<Photo>>()
		}
	};

	[Theory]
	[MemberData(nameof(GroupingByDateTimeWithSecondsAddressShouldBeEquivalentToExpectedData))]
	public void Grouping_By_DateTimeWithSecondsAddress_Should_Be_Equivalent_To_Expected(List<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos)
	{
		GroupByNamingStyleEquivalentToExpected(photoInfos, expectedGroupedPhotoInfos, NamingStyle.DateTimeWithSecondsAddress);
	}

	#endregion

	private void GroupByNamingStyleEquivalentToExpected(IReadOnlyCollection<Photo> photoInfos, Dictionary<string, List<Photo>> expectedGroupedPhotoInfos, NamingStyle namingStyle)
	{
		var actualGroupedPhotoInfos = _sut.Group(photoInfos, namingStyle);
		actualGroupedPhotoInfos.Should().BeEquivalentTo(expectedGroupedPhotoInfos);
	}

	[Theory]
	[InlineData(NamingStyle.Numeric)]
	public void Invalid_Naming_Style_Should_Throw_PhotoOrganizerToolException(NamingStyle namingStyle)
	{
		Assert.Throws<PhotoCliException>(() => _sut.Group(new List<Photo>(), namingStyle));
	}

	private static string DirectoryGrouperFormat(string dateFormat, List<string> reverseGeocodes)
	{
		return $"{dateFormat}-{ReverseGeocodeFakes.Format(reverseGeocodes)}";
	}

	private static string DirectoryGrouperFormat(List<string> reverseGeocodes, string dateFormat)
	{
		return $"{ReverseGeocodeFakes.Format(reverseGeocodes)}-{dateFormat}";
	}
}
