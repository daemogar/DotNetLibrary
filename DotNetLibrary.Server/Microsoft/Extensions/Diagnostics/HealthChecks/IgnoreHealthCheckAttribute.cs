namespace Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Attribute use so that the health check will not be
/// added automatically. Primarily used for the
/// <seealso cref="ApplicationStartedHealthCheck"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class IgnoreHealthCheckAttribute : Attribute
{
	/// <summary>
	/// When overridden this method can dynamically control if a
	/// health check is added to the health check pool.
	/// </summary>
	/// <returns>True to include and false to exclude.</returns>
	public virtual bool Conditional() => true;
}
