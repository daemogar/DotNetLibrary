#if NETSTANDARD2_0_OR_GREATER

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.IO;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <inheritdoc cref="Stream"/>
public static class StreamExtensions
{
	/// <inheritdoc cref="Stream.Dispose()"/>
	public static async Task DisposeAsync(this Stream stream)
		=> await Task.Run(stream.Dispose);
}
#endif
