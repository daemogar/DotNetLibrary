#if NETSTANDARD2_0_OR_GREATER

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Net.Http;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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