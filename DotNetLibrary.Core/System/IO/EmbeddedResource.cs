using System.Reflection;
using System.Text.Json;

namespace System.IO;

/// <summary>
/// Helper to get Embedded resources by filename.
/// </summary>
public static class EmbeddedResource
{
	/// <inheritdoc cref="ReadAsync{T}(string, JsonSerializerOptions, Assembly, CancellationToken)"/>
	public static async Task<T?> ReadAsync<T>(string filename, CancellationToken cancellationToken)
		=> await ReadAsync<T>(filename, null!, ToArray(), cancellationToken);

	/// <inheritdoc cref="ReadAsync{T}(string, JsonSerializerOptions, Assembly, CancellationToken)"/>
	public static async Task<T?> ReadAsync<T>(string filename, Assembly assembly, CancellationToken cancellationToken)
		=> await ReadAsync<T>(filename, null!, ToArray(assembly), cancellationToken);

	/// <inheritdoc cref="ReadAsync{T}(string, JsonSerializerOptions, Assembly, CancellationToken)"/>
	public static async Task<T?> ReadAsync<T>(string filename, JsonSerializerOptions options, CancellationToken cancellationToken)
		=> await ReadAsync<T>(filename, options, ToArray(), cancellationToken);

	/// <inheritdoc cref = "ReadAsync{T}(string, JsonSerializerOptions, IEnumerable{Assembly}, CancellationToken)" />
	/// <param name="filename"><inheritdoc cref="ReadAsync{T}(string, JsonSerializerOptions, IEnumerable{Assembly}, CancellationToken)"/></param>
	/// <param name="options"><inheritdoc cref="ReadAsync{T}(string, JsonSerializerOptions, IEnumerable{Assembly}, CancellationToken)"/></param>
	/// <param name="assembly"><inheritdoc cref="ReadAsync{T}(string, JsonSerializerOptions, IEnumerable{Assembly}, CancellationToken)"/></param>
	/// <param name="cancellationToken"><inheritdoc cref="ReadAsync{T}(string, JsonSerializerOptions, IEnumerable{Assembly}, CancellationToken)"/></param>
	public static async Task<T?> ReadAsync<T>(string filename, JsonSerializerOptions options, Assembly assembly, CancellationToken cancellationToken)
			=> await ReadAsync<T>(filename, options, ToArray(assembly), cancellationToken);

	/// <inheritdoc cref="ReadAsync{T}(string, JsonSerializerOptions, IEnumerable{Assembly}, CancellationToken)"/>
	public static async Task<T?> ReadAsync<T>(string filename, IEnumerable<Assembly> assemblies, CancellationToken cancellationToken)
				=> await ReadAsync<T>(filename, null!, assemblies, cancellationToken);

	/// <summary>
	/// Read content from a file into a json object and deserialize it into an object of type
	/// <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of object to be returned.</typeparam>
	/// <param name="filename"><inheritdoc cref="ReadAllTextAsync(string, Assembly?)"/></param>
	/// <param name="options">Options for the json deserialization process.</param>
	/// <param name="assemblies">Additional assemblies to search first before the default locations.</param>
	/// <param name="cancellationToken">A token for cancelling the async process early.</param>
	/// <returns>The embedded resource as a C# object.</returns>
	public static async Task<T?> ReadAsync<T>(string filename, JsonSerializerOptions options, IEnumerable<Assembly> assemblies, CancellationToken cancellationToken)
	{
		var stream = await UseStreamAsync(filename, assemblies, p => p, false);

		if (stream is null)
			return default;

		try
		{
			return await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
		}
		finally
		{
			await stream.DisposeAsync();
		}
	}

	/// <summary>
	/// Read all the text from an embedded resource.
	/// </summary>
	/// <param name="filename">The name of the file as it exists on disk. This does not need to be namespaced unless there are duplicate files in seperate assemblies.</param>
	/// <param name="assembly">An assembly to search first before the default locations.</param>
	/// <returns>The embedded resource contents as a string or null if no content was found.</returns>
	public static async Task<string?> ReadAllTextAsync(string filename, Assembly? assembly = default)
		=> await ReadAllTextAsync(filename, ToArray(assembly));

	/// <summary>
	/// <inheritdoc cref="ReadAllTextAsync(string, Assembly?)"/>
	/// </summary>
	/// <param name="filename"><inheritdoc cref="ReadAllTextAsync(string, Assembly?)"/></param>
	/// <param name="assemblies">Additional assemblies to search first before the default locations.</param>
	/// <returns><inheritdoc cref="ReadAllTextAsync(string, Assembly?)"/></returns>
	public static async Task<string?> ReadAllTextAsync(string filename, IEnumerable<Assembly> assemblies)
	{
		using var stream = await ReadStreamAsync(filename, assemblies);
		return stream is null ? null : await stream.ReadToEndAsync();
	}

	/// <summary>
	/// Get a stream reader for the given embedded resource filename.
	/// </summary>
	/// <param name="filename"><inheritdoc cref="ReadAllTextAsync(string, Assembly?)"/></param>
	/// <param name="assembly"><inheritdoc cref="ReadAllTextAsync(string, Assembly?)"/></param>
	/// <returns><inheritdoc cref="ReadAllTextAsync(string, Assembly?)"/></returns>
	public static async Task<StreamReader?> ReadStreamAsync(string filename, Assembly? assembly)
		=> await ReadStreamAsync(filename, ToArray(assembly));

	/// <summary>
	/// <inheritdoc cref="ReadStreamAsync(string, Assembly?)"/>
	/// </summary>
	/// <param name="filename"><inheritdoc cref="ReadStreamAsync(string, Assembly?)"/></param>
	/// <param name="assemblies">Additional assemblies to search first before the default locations.</param>
	/// <returns><inheritdoc cref="ReadStreamAsync(string, Assembly?)"/></returns>
	public static async Task<StreamReader?> ReadStreamAsync(string filename, IEnumerable<Assembly> assemblies)
		=> await UseStreamAsync<StreamReader>(filename, assemblies, stream => new(stream), false);

	/// <summary>
	/// Get all assembly manifest resource names. This is using
	/// <seealso cref="Assembly.GetManifestResourceNames"/> to retrieve
	/// all the names.
	/// </summary>
	/// <param name="assembly">An assembly to get manifest resource names from.</param>
	/// <returns>A list of manifest resource names.</returns>
	public static string[] GetAllFilenames(Assembly? assembly = default)
		=> GetAllFilenames(
			assembly is not null
				? new[] { assembly }
				: Array.Empty<Assembly>());

	/// <summary>
	/// <inheritdoc cref="GetAllFilenames(Assembly?)"/>
	/// </summary>
	/// <param name="assemblies">List of assemblies to get manifest resource names from.</param>
	/// <returns><inheritdoc cref="GetAllFilenames(Assembly?)"/></returns>
	public static string[] GetAllFilenames(
		IEnumerable<Assembly> assemblies)
		=> GetManifestResourceNames(assemblies, (_, p) => p)
			.SelectMany(p => p.Search)
			.ToArray();

	private static async Task<T?> UseStreamAsync<T>(
		string filename,
		IEnumerable<Assembly> assemblies,
		Func<Stream, T> callback,
		bool disposeStream = false)
	{
		var stream = Find(ref filename, assemblies)?.GetManifestResourceStream(filename);
		if (stream is null)
			return default;

		try
		{
			return callback(stream);
		}
		finally
		{
			if (disposeStream)
				await stream.DisposeAsync();
		}
	}

	private static Assembly? Find(ref string filename, IEnumerable<Assembly> assemblies)
	{
		var name = filename;
		var resource = GetManifestResourceNames(assemblies,
			(_, p) => p?.FirstOrDefault(q => q.Contains(name)))
			.FirstOrDefault(p => p.Search is not null);

		if (resource.Search is null)
			return null;

		filename = resource.Search;
		return resource.Assembly;
	}

	private static IEnumerable<(Assembly Assembly, T Search)>
		GetManifestResourceNames<T>(IEnumerable<Assembly> assemblies,
		Func<Assembly, string[], T> callback)
		=> assemblies
			.Union(new[]
			{
				Assembly.GetEntryAssembly(),
				Assembly.GetCallingAssembly(),
				Assembly.GetExecutingAssembly()
			})
			.Where(p => p is not null)
			.Select(p => (p!,
				callback(p, p?.GetManifestResourceNames()
					?? Array.Empty<string>())
			));

	private static IEnumerable<Assembly> ToArray(Assembly? assembly = null)
		=> assembly is null ? Array.Empty<Assembly>() : new[] { assembly };
}
