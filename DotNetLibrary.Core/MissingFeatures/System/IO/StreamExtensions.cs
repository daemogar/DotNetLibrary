#if NETSTANDARD2_0_OR_GREATER
namespace System.IO;

/// <inheritdoc cref="Stream"/>
public static class StreamExtensions
{
	/// <inheritdoc cref="Stream.Dispose()"/>
	public static async Task DisposeAsync(this Stream stream)
		=> await Task.Run(stream.Dispose);
}
#endif
