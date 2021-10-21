using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json;

/// <summary>
/// Json serialization and deserialization helper extension methods.
/// </summary>
public static class JsonExtensions
{
	private static JsonSerializerOptions Options { get; } = new()
	{
		IncludeFields = true,
		WriteIndented = true,
		AllowTrailingCommas = true,
		PropertyNameCaseInsensitive = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		ReadCommentHandling = JsonCommentHandling.Skip,
		ReferenceHandler = ReferenceHandler.Preserve
	};

	/// <summary>
	/// Convert target object into a JSON serialized string.
	/// </summary>
	/// <typeparam name="T">Any object type can be converted.</typeparam>
	/// <param name="data">Object that should be serialized to a json string.</param>
	/// <returns>JSON string of the object.</returns>
	public static string ToJsonString<T>(this T data)
		=> data.ToJsonString(Options);

	/// <summary>
	/// Convert target object into a JSON serialized string.
	/// </summary>
	/// <typeparam name="T">Any object type can be converted.</typeparam>
	/// <param name="data">Object that should be serialized to a json string.</param>
	/// <param name="writeIndented">Should it be formatted with tabs and returns.</param>
	/// <returns>JSON string of the object.</returns>
	public static string ToJsonString<T>(this T data, bool writeIndented = false)
		=> JsonSerializer.Serialize(data, new JsonSerializerOptions(Options)
		{
			WriteIndented = writeIndented,
		});

	/// <summary>
	/// Convert target object into a JSON serialized string.
	/// </summary>
	/// <typeparam name="T">Any object type can be converted.</typeparam>
	/// <param name="data">Object that should be serialized to a json string.</param>
	/// <param name="options">JsonSerializerOptions configuration for serializing the object into a string.</param>
	/// <returns>JSON string of the object.</returns>
	public static string ToJsonString<T>(this T data, JsonSerializerOptions options)
		=> JsonSerializer.Serialize(data, options);

	/// <summary>
	/// Convert a json string into an object.
	/// </summary>
	/// <typeparam name="T">Any object type can be converted.</typeparam>
	/// <param name="json">A string json representation to be converted into an object.</param>
	/// <returns>The object that is returned from deserializing the json string.</returns>
	public static T FromJsonString<T>(this string json)
		=> (T)json.FromJsonStringUsingType(typeof(T));

	/// <summary>
	/// Convert a json string into an object.
	/// </summary>
	/// <typeparam name="T">Any object type can be converted.</typeparam>
	/// <param name="json">A string json representation to be converted into an object.</param>
	/// <param name="model">A target type to convert the json string into.</param>
	/// <returns>The object that is returned from deserializing the json string.</returns>
	public static T FromJsonStringUsingModel<T>(this string json, T model)
		=> (T)json.FromJsonStringUsingType(model?.GetType() ?? typeof(T));

	/// <summary>
	/// Convert a json string into an object.
	/// </summary>
	/// <typeparam name="T">Any object type can be converted.</typeparam>
	/// <param name="json">A string json representation to be converted into an object.</param>
	/// <param name="type">A target type to convert the json string into.</param>
	/// <returns>The object that is returned from deserializing the json string.</returns>
	public static object FromJsonStringUsingType(this string json, Type type)
	{
		try
		{
			return JsonSerializer.Deserialize(json, type, Options)!;
		}
		catch (JsonException e)
		{
			Console.Error.WriteLine(Error(1, e, json).ToJsonString(true));
			throw new InvalidDataException(
				$"Object is not of type {type.Name}.");
		}

		static List<object> Error(int step, Exception e, string input)
		{
			List<object> list = new();

			list.Add(new
			{
				Step = step,
				e.Message,
				Input = input,
				e.Source,
				e.Data,
				e.StackTrace
			});

			if (e.InnerException is not null)
				list.AddRange(Error(step + 1, e.InnerException, ""));

			return list;
		}
	}

	/// <summary>
	/// Convert a json string into an object.
	/// </summary>
	/// <typeparam name="T">Any object type can be converted.</typeparam>
	/// <param name="stream">A stream of data that represents a json string of an object.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The object that is returned from deserializing the json string.</returns>
	/// <exception cref="NullReferenceException">If allowNull is false and the json string deserializes into a null object, this exception is thrown.</exception>
	public static async Task<T?> FromJsonStreamAsync<T>(
		this Task<Stream> stream, CancellationToken cancellationToken)
		=> await stream.FromJsonStreamAsync<T>(true, cancellationToken);

	/// <summary>
	/// Convert a json string into an object.
	/// </summary>
	/// <typeparam name="T">Any object type can be converted.</typeparam>
	/// <param name="stream">A stream of data that represents a json string of an object.</param>
	/// <param name="allowNull">Allow a null value or object to be returned. True to allow nulls, and false to throw null exception error.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The object that is returned from deserializing the json string.</returns>
	/// <exception cref="NullReferenceException">If allowNull is false and the json string deserializes into a null object, this exception is thrown.</exception>
	public static async Task<T> FromJsonStreamAsync<T>(
		this Task<Stream> stream, bool allowNull, CancellationToken cancellationToken)
	{
		var data = await JsonSerializer.DeserializeAsync<T>(
						 await stream, Options, cancellationToken);

		if (data != null || allowNull)
			return data!;

		throw new NullReferenceException(
			$"Stream was not able to be deserialize.");
	}
}
