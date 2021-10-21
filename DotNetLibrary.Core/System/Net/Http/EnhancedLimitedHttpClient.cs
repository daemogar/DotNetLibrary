using Serilog;

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>
/// Class for enhancing the base HttpClient.
/// </summary>
public class EnhancedLimitedHttpClient
{
	private ILogger Logger { get; }

	private HttpClient Client { get; set; }

	/// <summary>
	/// The base address of all calls that the endpoints will use as a reference.
	/// </summary>
	public string BaseAddress => Client.BaseAddress?.AbsoluteUri!;

	/// <summary>
	/// Converts an HttpClient into a restricted version that has enhanced
	/// methods for getting data.
	/// </summary>
	/// <param name="logger">Serilog Logger</param>
	/// <param name="client">HttpClient to use for making calls.</param>
	public EnhancedLimitedHttpClient(ILogger logger, HttpClient client)
	{
		Logger = logger;
		Client = client;
	}

	/// <summary>
	/// Get object from <paramref name="url"/> and throws a 
	/// <seealso cref="NullReferenceException"/> if object is null or un able 
	/// to be converted to <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the object that shoudld be returned.</typeparam>
	/// <param name="url">The end point to get data back from using a GET method using json.</param>
	/// <param name="cancellationToken">Cancelation token.</param>
	/// <returns></returns>
	/// <exception cref="NullReferenceException">Throw if the <paramref name="allowNulls"/> is false and the object returned is null.</exception>
	public async Task<T> GetJsonAsync<T>(
		string url,
		CancellationToken cancellationToken)
			=> await GetJsonAsync<T>(url, false, cancellationToken);

	/// <summary>
	/// Get object from <paramref name="url"/> and allow null result
	/// if <paramref name="allowNulls"/> is true. Throws a 
	/// <seealso cref="NullReferenceException"/> if object is null or un able 
	/// to be converted to <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the object that shoudld be returned.</typeparam>
	/// <param name="url">The end point to get data back from using a GET method using json.</param>
	/// <param name="allowNulls">If true, allow for nulls to be returned else throw null reference exception.</param>
	/// <param name="cancellationToken">Cancelation token.</param>
	/// <returns></returns>
	/// <exception cref="NullReferenceException">Throw if the <paramref name="allowNulls"/> is false and the object returned is null.</exception>
	public virtual async Task<T> GetJsonAsync<T>(
		string url,
		bool allowNulls,
		CancellationToken cancellationToken)
	{
		string? data = default;

		try
		{
			HttpRequestMessage message = new(HttpMethod.Get, url);
			var response = await Client.SendAsync(message, cancellationToken);

			data = await response.Content
				.ReadAsStringAsync(cancellationToken);

			if (response.StatusCode == HttpStatusCode.Redirect)
				Logger.Verbose(response.ReasonPhrase ?? "null");

			if (response.IsSuccessStatusCode)
				return data is null
					? default!
					: data.FromJsonString<T>();

			Logger.Error(
				$"Failed Parsing {nameof(GetJsonAsync)} " +
				$"HttpClient Request {{message}} " +
				$"with {{Error}}.", message, data);

			response.EnsureSuccessStatusCode();

			return default!;
		}
		catch (Exception e)
		{
			while (e.InnerException is not null)
				e = e.InnerException;

			Logger.Error(e,
				"Get request failed {Content}.", data);

			throw;
		}
	}
}
