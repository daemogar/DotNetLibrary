#if NETSTANDARD2_0_OR_GREATER
namespace Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Adds missing features for netstandard2.0
/// </summary>
public interface IWebHostEnvironment
{
	/// <summary>
	/// Adds missing features for netstandard2.0
	/// </summary>
	string ContentRootPath { get; }
}
#endif