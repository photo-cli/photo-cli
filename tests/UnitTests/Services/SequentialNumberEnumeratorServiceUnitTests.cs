namespace PhotoCli.Tests.UnitTests.Services;

public class SequentialNumberEnumeratorServiceUnitTests
{
	private readonly SequentialNumberEnumeratorService _sut = new(NullLogger<SequentialNumberEnumeratorService>.Instance);

	public static TheoryData<int, List<string>> OnlySequentialNumbersData = new()
	{
		{ 1, new List<string> { "1" } },
		{ 9, new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9" } },
		{ 10, new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" } },
		{ 11, new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" } },
	};

	[Theory]
	[MemberData(nameof(OnlySequentialNumbersData))]
	public void OnlySequentialNumbers_Equals_Expected_Name(int toNameCount, List<string> expectedNames)
	{
		NameListExceptedEquals(toNameCount, expectedNames, NumberNamingTextStyle.OnlySequentialNumbers);
	}

	public static TheoryData<int, List<string>> PaddingZeroCharacterData = new()
	{
		{ 1, new List<string> { "1" } },
		{ 9, new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9" } },
		{ 10, new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" } },
		{ 11, new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11" } },
	};

	[Theory]
	[MemberData(nameof(PaddingZeroCharacterData))]
	public void PaddingZeroCharacter_Equals_Expected_Name(int toNameCount, List<string> expectedNames)
	{
		NameListExceptedEquals(toNameCount, expectedNames, NumberNamingTextStyle.PaddingZeroCharacter);
	}

	public static TheoryData<int, List<string>> AllNamesAreSameLengthData = new()
	{
		{ 1, new List<string> { "1" } },
		{ 9, new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9" } },
		{ 10, new List<string> { "10", "11", "12", "13", "14", "15", "16", "17", "18", "19" } },
		{ 11, new List<string> { "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" } },
	};

	[Theory]
	[MemberData(nameof(AllNamesAreSameLengthData))]
	public void AllNamesAreSameLength_Equals_Expected_Name(int toNameCount, List<string> expectedNames)
	{
		NameListExceptedEquals(toNameCount, expectedNames, NumberNamingTextStyle.AllNamesAreSameLength);
	}

	[Theory]
	[InlineData(9, 1)]
	[InlineData(10, 2)]
	[InlineData(90, 2)]
	[InlineData(91, 3)]
	[InlineData(900, 3)]
	[InlineData(901, 4)]
	[InlineData(9001, 5)]
	public void AllNamesAreSameLength_AvailableNumber_Range_Fits_Digit_Length_List_Equal(int toNameCount, int digit)
	{
		var startNumber = Math.Pow(10, digit - 1);
		var expectedNames = new List<string>();
		for (var i = startNumber; i < toNameCount + startNumber; i++)
			expectedNames.Add(i.ToString());
		NameListExceptedEquals(toNameCount, expectedNames, NumberNamingTextStyle.AllNamesAreSameLength);
	}

	[Fact]
	public void Not_Defined_NumberNamingTextStyle_Should_Throw_PhotoOrganizerToolException()
	{
		Assert.Throws<PhotoCliException>(() =>
			_sut.NumberIterator(1, (NumberNamingTextStyle)byte.MaxValue).GetEnumerator().MoveNext()
		);
	}

	[Fact]
	public void ToNameCount_Smaller_Than_Zero_Should_Throw_PhotoOrganizerToolException()
	{
		Assert.Throws<PhotoCliException>(() =>
			_sut.NumberIterator(-1, NumberNamingTextStyle.OnlySequentialNumbers).GetEnumerator().MoveNext()
		);
	}

	private void NameListExceptedEquals(int toNameCount, List<string> expectedNames, NumberNamingTextStyle numberNamingTextStyle)
	{
		var nameList = _sut.NumberIterator(toNameCount, numberNamingTextStyle).ToList();
		nameList.Should().Equal(expectedNames);
	}
}
