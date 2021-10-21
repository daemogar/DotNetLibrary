using Serilog;

namespace System.Net.Http;

/// <summary>
/// <seealso cref="IHttpClientFactory"/> extension method helpers.
/// </summary>
public static class HttpClientFactoryExtensions
{
	/// <summary>
	/// Get an <seealso cref="EnhancedLimitedHttpClient"/> by calling the 
	/// create client method 
	/// </summary>
	/// <param name="factory">A factory abstraction for creating HttpClients.</param>
	/// <param name="name">The logical name of the client to create.</param>
	/// <returns>A <seealso cref="EnhancedLimitedHttpClient"/> created with the <seealso cref="IHttpClientFactory.CreateClient(string)"/>.</returns>
	public static EnhancedLimitedHttpClient GetClient(
		this IHttpClientFactory factory, string name)
		=> new(Log.Logger, factory.CreateClient(name));
}
