using System.Collections.Generic;

namespace System.Text.Json.Serialization;

/// <summary>
/// Converter for lists of strings to and from json strings.
/// </summary>
public class JsonListStringConverter : JsonConverter<List<string>>
{
	/// <summary>
	/// Read a comma seperated string array from reader.
	/// </summary>
	/// <param name="reader">Utf8JsonReader</param>
	/// <param name="typeToConvert">typeof(List&lt;string&gt;)</param>
	/// <param name="options">JsonSerializerOptions</param>
	/// <returns>A list of strings from a comma seperated string array.</returns>
	public override List<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		List<string> list = new();
		var value = reader.GetString();
		if (value is not null)
			list.Add(value);
		return list;
	}

	/// <summary>
	/// Write string output of list of strings into a comma seperated string array.
	/// </summary>
	/// <param name="writer">Utf8JsonWriter</param>
	/// <param name="list">List of string to turn into comma seperated string array.</param>
	/// <param name="options">JsonSerializerOptions</param>
	public override void Write(Utf8JsonWriter writer, List<string> list, JsonSerializerOptions options)
	{
		var comma = "";
		writer.WriteStartArray();
		foreach (var value in list)
		{
			var output = $"{comma}{value}";
			writer.WriteStringValue(output);
			comma = ",";
		}
		writer.WriteEndArray();
	}
}