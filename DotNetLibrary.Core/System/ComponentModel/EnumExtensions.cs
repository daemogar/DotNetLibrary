using Microsoft.Extensions.Hosting;

namespace System.ComponentModel;

/// <summary>
/// Simplified attribute getter for enums.
/// </summary>
public static class EnumExtensions
{
	internal record UseDayTime(bool UseDay, bool UseTime, bool AllowMultipleDays, bool UseHoursMinutes)
	{
		public UseDayTime() : this(false, false, false, false) { }
	}

	internal static UseDayTime GetDayTime<TEnum>(this TEnum enumerationValue)
			where TEnum : struct
		=> enumerationValue.UseDay(enumerationValue.UseTime(new()));

	private static UseDayTime UseTime<TEnum>(this TEnum enumerationValue, UseDayTime model)
			where TEnum : struct
		=> GetAttribute(enumerationValue,
			(string p, UseTimeAttribute? q)
				=> model with {
					UseTime = q is not null,
					UseHoursMinutes = q?.UseHoursMinutes ?? false
				});

	private static UseDayTime UseDay<TEnum>(this TEnum enumerationValue, UseDayTime model)
			where TEnum : struct
		=> GetAttribute(enumerationValue,
			(string p, UseDayAttribute? q)
				=> model with
				{
					UseDay = q is not null,
					AllowMultipleDays = q?.AllowMultipleDays ?? false
				});

	/// <summary>
	/// Get the description value of the attribute if it exists. Otherwise
	/// return the attribute string representation.
	/// </summary>
	/// <typeparam name="TEnum">The type of the target enum to search for an attribute on.</typeparam>
	/// <param name="enumerationValue">The target enum to search for an attribute on.</param>
	/// <returns>The value of the description attribute or the enum string value.</returns>
	public static string GetDescription<TEnum>(this TEnum enumerationValue)
			where TEnum : struct
		=> GetAttribute(enumerationValue , 
			(string p, DescriptionAttribute? q) => q?.Description ?? p);

	/// <summary>
	/// Get a specific attribute from an enum.
	/// </summary>
	/// <typeparam name="TEnum">The type of the target enum to search for an attribute on.</typeparam>
	/// <typeparam name="TAttribute">The attribute type to search for.</typeparam>
	/// <typeparam name="TResult">The type that should be returned.</typeparam>
	/// <param name="enumerationValue">The target enum to search for an attribute on.</param>
	/// <param name="callback">A method which takes in the stringified version of the <paramref name="enumerationValue"/> and the attribute attached to the enum value and returns a <typeparamref name="TResult"/>.</param>
	/// <returns>The results of the <paramref name="callback"/>.</returns>
	/// <exception cref="ArgumentException">If <paramref name="enumerationValue"/> is not an enum.</exception>
	/// <exception cref="InvalidOperationException">If <paramref name="enumerationValue"/> does not return a string value.</exception>
	public static TResult GetAttribute<TEnum, TAttribute, TResult>(
		this TEnum enumerationValue, Func<string, TAttribute?, TResult> callback)
		where TEnum : struct
		where TAttribute : Attribute
	{
		var type = enumerationValue.GetType();
		if (!type.IsEnum)
			throw new ArgumentException(
				$"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));

		var name = enumerationValue.ToString();
		if (name is null)
			throw new InvalidOperationException(
				$"{nameof(enumerationValue)} must return a string name.");

		var memberInfo = type.GetMember(name);
		if (memberInfo.Length > 0)
		{
			var attrs = memberInfo[0].GetCustomAttributes(typeof(TAttribute), false);

			if (attrs.Length > 0)
				return callback(name, (TAttribute)attrs[0]);
		}

		return callback(name, null);
	}
}
