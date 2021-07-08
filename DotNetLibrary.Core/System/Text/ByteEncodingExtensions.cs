using System.Text.Json;

namespace System.Text
{
	public static class ByteEncodingExtensions

	{
		public static string ToBase64ByteEncodedString<T>(this T data, bool writeIndented = false)
		{
			if (data == null)
				throw new NullReferenceException(
					$"Trying to base 64 encode a null object.");

			var json = data.ToJsonString(writeIndented);
			var bytes = Encoding.UTF32.GetBytes(json);
			return Convert.ToBase64String(bytes);
		}

		public static T FromBase64ByteEncodedString<T>(this string json)
		{
			if (string.IsNullOrWhiteSpace(json))
				return default!;

			var bytes = Convert.FromBase64String(json);
			var data = Encoding.UTF32.GetString(bytes);

			return data.FromJsonString<T>();
		}
	}
}
