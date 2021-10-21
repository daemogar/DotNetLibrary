namespace System.Text.Json.Serialization;

/// <summary>
/// Attribute for marking property of type of list of strings to use
/// the jsoncoverter to change to comma list of string.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class JsonListStringConverterAttribute : JsonConverterAttribute
{
	/// <summary>
	/// Constructor for attribute.
	/// </summary>
	public JsonListStringConverterAttribute()
		: base(typeof(JsonListStringConverter))
	{
	}
}
