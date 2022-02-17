namespace Serilog;

/// <summary>
/// Extension methods for logging stack traces not 
/// associated with an exception.
/// </summary>
public static class LogStackTrace
{
	/// <inheritdoc cref="StackTrace{T}(ILogger, string, object?, T, ushort)"/>
	public static T StackTrace<T>(this ILogger logger, string target, T result, ushort skipFrames = 0)
	{
		logger.Debug(
			"{Method} was {Result}", target, result);
		logger.StackTrace(skipFrames++);
		return result;
	}

	/// <summary>
	/// Log results for debugging if debug logging is enabled.
	/// <inheritdoc cref="StackTrace(ILogger, ushort)" />
	/// </summary>
	/// <typeparam name="T">A type generic for passing through the results for chaining purposes.</typeparam>
	/// <param name="logger"><inheritdoc cref="StackTrace(ILogger, ushort)"/></param>
	/// <param name="target">An custom identifier for what this log is being called against.</param>
	/// <param name="data">Additional details that should be logged with stack trace for debugging.</param>
	/// <param name="result">The interesting data to be returned after logging.</param>
	/// <param name="skipFrames"><inheritdoc cref="StackTrace(ILogger, ushort)"/></param>
	/// <returns>The <paramref name="result"/> passed in without modifications.</returns>
	public static T StackTrace<T>(this ILogger logger, string target, object? data, T result, ushort skipFrames = 0)
	{
		logger.Debug(
			"{Target} was {Result} with {Data}", target, result, data);
		logger.StackTrace(skipFrames++);
		return result;
	}

	/// <summary>
	/// Log a stack trace at the point of caller if verbose logging is enabled.
	/// </summary>
	/// <param name="logger">The serilog logger.</param>
	/// <param name="skipFrames">Number of stack frames to ignore. This is usuful if wanting to ignore a part of the stack frame because the caller is not the important part of the stack frame.</param>
	public static void StackTrace(this ILogger logger, ushort skipFrames = 0)
	{
		if (logger?.IsEnabled(Events.LogEventLevel.Verbose) != true)
			return;

		System.Diagnostics.StackTrace trace = new(true);
		var indent = "";
		foreach (var frame in trace.GetFrames().Reverse().Skip(1 + skipFrames))
		{
			var filename = frame.GetFileName()?.Split('\\').Last();
			if (filename is null)
				continue;

			logger.Verbose(
				$"{indent}[{{Line}}] {{Method}} in {{FileName}}",
					frame.GetFileLineNumber(),
					frame.GetMethod(),
					filename);

			indent += "  ";
		}
	}
}
