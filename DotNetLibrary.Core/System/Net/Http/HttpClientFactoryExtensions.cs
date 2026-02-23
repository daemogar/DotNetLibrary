using Serilog;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Net.Http;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
