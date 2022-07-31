using FluentValidation.Results;

namespace PhotoCli.Tests.Fakes;

public static class ValidationResultFakes
{
	public static ValidationResult Get(bool hasErrors)
	{
		return hasErrors ? HasErrors() : NoError();
	}

	public static ValidationResult NoError()
	{
		return new ValidationResult();
	}

	public static ValidationResult HasErrors(string errorMessage = "error-message")
	{
		return new ValidationResult(new[]
		{
			new ValidationFailure("invalid-property", errorMessage)
		});
	}
}
