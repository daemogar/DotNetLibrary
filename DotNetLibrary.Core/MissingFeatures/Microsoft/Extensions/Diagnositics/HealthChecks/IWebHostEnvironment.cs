#if NETSTANDARD2_0_OR_GREATER

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Diagnostics.HealthChecks;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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