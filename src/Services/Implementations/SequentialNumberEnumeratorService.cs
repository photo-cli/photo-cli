namespace PhotoCli.Services.Implementations;

public class SequentialNumberEnumeratorService : ISequentialNumberEnumeratorService
{
	private readonly ILogger<SequentialNumberEnumeratorService> _logger;

	public SequentialNumberEnumeratorService(ILogger<SequentialNumberEnumeratorService> logger)
	{
		_logger = logger;
	}

	public IEnumerable<string> NumberIterator(int toNumerateCount, NumberNamingTextStyle numberNamingTextStyle)
	{
		if (toNumerateCount < 0)
			throw new PhotoCliException($"{nameof(toNumerateCount)} should be zero or positive");

		_logger.LogTrace("Numerating {ToNumerateCount} numbers", toNumerateCount);
		if (numberNamingTextStyle == NumberNamingTextStyle.OnlySequentialNumbers)
		{
			for (var i = 1; i <= toNumerateCount; i++)
				yield return i.ToString();
			yield break;
		}

		var digitLength = GetMaximumDigitLength(toNumerateCount);
		switch (numberNamingTextStyle)
		{
			case NumberNamingTextStyle.PaddingZeroCharacter:
			{
				for (var i = 1; i <= toNumerateCount; i++)
					yield return i.ToString().PadLeft(digitLength, '0');
				yield break;
			}
			case NumberNamingTextStyle.AllNamesAreSameLength:
			{
				var startNumber = GetMinimumValueWithADigitLength(digitLength);
				var endNumber = GetMaximumValueWithADigitLength(digitLength);
				_logger.LogTrace("For digit length {DigitLength}; start number: {StartNumber}, end number: {EndNumber}", digitLength, startNumber, endNumber);
				var availableNumber = endNumber - startNumber + 1;
				if (availableNumber < toNumerateCount)
				{
					startNumber = GetMinimumValueWithADigitLength(++digitLength);
					_logger.LogDebug("Increased digit length by one ({DigitLength}) because available number ({AvailableNumber}) not enough for numbers to be numerated({ToNumerate})", digitLength,
						availableNumber, toNumerateCount);
				}

				for (int nextNumber = startNumber, numberGiven = 1; numberGiven != toNumerateCount + 1; nextNumber++, numberGiven++)
					yield return nextNumber.ToString();
				yield break;
			}
			default:
				throw new PhotoCliException($"Not implemented {nameof(NumberNamingTextStyle)}: {numberNamingTextStyle}");
		}
	}

	private int GetMaximumDigitLength(int toNameCount)
	{
		var digitLenght = toNameCount.ToString().Length;
		_logger.LogTrace("Digit lenght: {DigitLength}", digitLenght);
		return digitLenght;
	}

	private static int GetMinimumValueWithADigitLength(int digitLength)
	{
		var minimumValue = (int)Math.Pow(10, digitLength - 1);
		return minimumValue;
	}

	private static int GetMaximumValueWithADigitLength(int digitLength)
	{
		var maximumValue = (int)Math.Pow(10, digitLength) - 1;
		return maximumValue;
	}
}
