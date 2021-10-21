using System.Text.Json;

namespace System.Text;

/// <summary>
/// Generic extension methods for simplifing byte encoding and
/// decoding to and from strings.
/// </summary>
public static class ByteEncodingExtensions
{
	/// <summary>
	/// Convert <paramref name="data"/> into a json string and 
	/// byte encode into a UTF32 encoded string.
	/// </summary>
	/// <typeparam name="T">The type of an object that should be byte encoded into a UTF32 encoded string.</typeparam>
	/// <param name="data">An object that should be byte encoded into a UTF32 encoded string.</param>
	/// <returns>String UTF32 byte encoded string of an object.</returns>
	/// <exception cref="NullReferenceException">Throws null if the object is null.</exception>
	public static string ToBase64ByteEncodedString<T>(this T data)
	{
		if (data is null)
			throw new ArgumentNullException(nameof(data),
				$"Can not base 32 encode a null object.");

		var json = data.ToJsonString();
		var bytes = Encoding.UTF32.GetBytes(json);
		return Convert.ToBase64String(bytes);
	}

	/// <summary>
	/// Extract object from a UTF32 encoded string.
	/// </summary>
	/// <typeparam name="T">The type of an object that should be return from the byte encoded UTF32 encoded string.</typeparam>
	/// <param name="json">UTF32 byte encoded string to be decoded.</param>
	/// <returns>An object decoded from the UTF32 byte encoded string.</returns>
	public static T FromBase64ByteEncodedString<T>(this string json)
	{
		if (string.IsNullOrWhiteSpace(json))
			return default!;

		var bytes = Convert.FromBase64String(json);
		var data = Encoding.UTF32.GetString(bytes);

		return data.FromJsonString<T>();
	}
}
