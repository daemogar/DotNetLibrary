using System.Text.Json.Serialization;

namespace System.Text.Json
{
	public delegate void OptionsConfigurationCallback(JsonSerializerOptions options);
	
	public static class JsonExtensions
	{
		private static JsonSerializerOptions Options { get; } = new()
		{
			IncludeFields = true,
			WriteIndented = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
			PropertyNameCaseInsensitive = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			ReferenceHandler = ReferenceHandler.Preserve
		};

		/// <summary>
		/// Convert target object into a JSON serialized string.
		/// </summary>
		/// <typeparam name="T">Any object type can be converted.</typeparam>
		/// <param name="data">Object that should be serialized to a json string.</param>
		/// <returns>JSON </returns>
		public static string ToJsonString<T>(this T data) => data.ToJsonString(Options);
		public static string ToJsonString<T>(this T data, bool writeIndented = false)
			=> JsonSerializer.Serialize(data, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = writeIndented,
				ReferenceHandler = ReferenceHandler.Preserve
			});
		public static string ToJsonString<T>(this T data, JsonSerializerOptions options)
			=> JsonSerializer.Serialize(data, options);
		public static string ToJsonString<T>(this T data, OptionsConfigurationCallback optionsCallback)
		{
			var options = JsonSerializer.Deserialize<JsonSerializerOptions>(
				JsonSerializer.Serialize(data, Options));
			optionsCallback(options!);
			return data.ToJsonString(options!);
		}

		public static T FromJsonString<T>(this string json)
			=> (T)json.FromJsonStringUsingType(typeof(T));

		public static T FromJsonStringUsingModel<T>(this string json, T model)
			=> (T)json.FromJsonStringUsingType(model?.GetType() ?? typeof(T));

		public static object FromJsonStringUsingType(this string json, Type type)
			 => JsonSerializer.Deserialize(json, type, new JsonSerializerOptions
			 {
				 PropertyNameCaseInsensitive = true,
				 PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				 AllowTrailingCommas = true,
				 ReadCommentHandling = JsonCommentHandling.Skip
			 })!;
	}
}
