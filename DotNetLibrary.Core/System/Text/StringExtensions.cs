using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Text;

/// <summary>
/// Generic extension methods for enhancing string functions.
/// </summary>
public static class StringExtensions
{
	/// <summary>
	/// Return the suffix of a number. 1st, 2nd, 3rd, etc.
	/// </summary>
	/// <param name="number">The number to format.</param>
	/// <param name="format">Additional formatting for the number.</param>
	/// <returns>The formatted number with the suffix added at the end.</returns>
	public static string WithSuffix(this int number, string? format = default)
		=> (format is null ? number : number.ToString(format)) + (number % 10, number) switch
		{
			(1, not 11) => $"{number}st",
			(2, not 12) => $"{number}nd",
			(3, not 13) => $"{number}rd",
			_ => "th"
		};

	/// <summary>
	/// Safely returns up to <paramref name="length"/> number of characters from 
	/// <paramref name="text"/>. Unlike the <see cref="String.Substring(int, int)"/> 
	/// with a start index of 0 which throws an error if the requested start and length 
	/// parameters are outside the bounds of the <paramref name="text"/>. This will also 
	/// return the original value if it is null or white space.
	/// </summary>
	/// <param name="text">The text to clip and return the first <paramref name="length"/> number of characters.</param>
	/// <param name="length">The maximum number of characters to return from the start <paramref name="text"/>.</param>
	/// <returns>Returns up to <paramref name="length"/> of characters from start of <paramref name="text"/>.</returns>
	public static string SubstringClip(this string text, int length)
		=> text.SubstringClip(length, 0);

	/// <summary>
	/// Safely returns up to <paramref name="length"/> number of characters starting at the
	/// <paramref name="startIndex"/> index of the <paramref name="text"/>. Unlike the 
	/// <see cref="String.Substring(int, int)"/> which throws an error if the requested start 
	/// or/and length parameters are outside the bounds of the <paramref name="text"/>. This 
	/// will also return the original value if it is null or white space or start is outside 
	/// the bounds of the <paramref name="text"/>.
	/// </summary>
	/// <param name="text">The text to clip and return the <paramref name="length"/> number of characters starting at the <paramref name="startIndex"/>.</param>
	/// <param name="startIndex">The first character position to start clipping.</param>
	/// <param name="length">The maximum number of characters to return from the <paramref name="startIndex"/> of the <paramref name="text"/>.</param>
	/// <returns>Returns up to <paramref name="length"/> of characters from the <paramref name="startIndex"/> of the <paramref name="text"/>.</returns>
	public static string SubstringClip(this string text, int startIndex, int length)
	{
		if (string.IsNullOrWhiteSpace(text) || startIndex >= text.Length)
			return text;

		length = Math.Min(text.Length - startIndex, length);
		return text[startIndex..length];
	}

	/// <summary>
	/// Converts <paramref name="text"/> to title case (except for words that are 
	/// entirely in uppercase, which are considered to be acronyms). By default
	/// the text will first be expanded. This is useful for turning camel or Pascal
	/// case <paramref name="text"/> in prep to title case it.
	/// <example><code>ie: theQuickBrownFox_1Jumped|orRatherRan => the Quick Brown Fox_1 Jumped|or Rather Ran</code></example>
	/// </summary>
	/// <param name="text">The string to convert to title case.</param>
	/// <param name="expandWords">By default spaces are added before capital letters unless false is passed and the text is used as passed in.</param>
	/// <returns>The specified string converted to title case.</returns>
	public static string ToTitleCase(this string text, bool expandWords = true)
		=> text == null
			? null!
			: CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
				expandWords
					? Regex.Replace(text,
						"([^A-Z_|])([A-Z])",
						"$1 $2")
					: text);
}
