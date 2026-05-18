using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace PhotoCli.Models;

public abstract record DisplayableRecord
{
	public (string, TValue) GetDisplayNameAndValue<T, TValue>(Expression<Func<T, TValue>> expression) where T : DisplayableRecord where TValue : struct
	{
		MemberExpression? memberExpression;
		if (expression.Body is MemberExpression directMemberExpression)
			memberExpression = directMemberExpression;
		else
			throw new ArgumentException("Expression is not a member expression");

		var property = memberExpression?.Member as PropertyInfo;
		var displayAttribute = property?.GetCustomAttribute<DisplayAttribute>();
		if (displayAttribute?.Name == null)
			throw new ArgumentException("Display name is null");
		var propertyValue = (TValue)(property?.GetValue(this) ?? default(TValue));
		return (displayAttribute.Name, propertyValue);
	}
}
