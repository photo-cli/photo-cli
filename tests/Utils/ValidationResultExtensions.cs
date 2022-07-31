using FluentValidation.Results;

namespace PhotoCli.Tests.Utils;

public static class ValidationResultExtensions
{
	public static string FlattenToSingleMessage(this ValidationResult validationResult)
	{
		return string.Join(", ", validationResult.Errors.Select(s => s.ErrorMessage));
	}
}
