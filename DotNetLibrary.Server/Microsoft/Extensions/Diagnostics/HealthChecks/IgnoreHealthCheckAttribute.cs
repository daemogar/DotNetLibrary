namespace Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Attribute use so that the health check will not be
/// added automatically. Primarily used for the
/// <seealso cref="ApplicationStartedHealthCheck"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class IgnoreHealthCheckAttribute : Attribute { }
