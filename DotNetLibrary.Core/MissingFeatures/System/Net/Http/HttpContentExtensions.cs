#if NETSTANDARD2_0_OR_GREATER
namespace System.Net.Http;

/// <inheritdoc cref="HttpContent"/>
public static class HttpContentExtenions
{
	/// <inheritdoc cref="HttpContent.ReadAsStringAsync"/>
	public static Task<string> ReadAsStringAsync(
		this HttpContent content,
		CancellationToken cancellationToken)
		=> Task.Run(content.ReadAsStringAsync, cancellationToken);
}
#endif