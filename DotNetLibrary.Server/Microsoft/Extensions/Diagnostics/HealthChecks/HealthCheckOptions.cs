using Microsoft.Extensions.DependencyInjection;

using Serilog;

using System.Reflection;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Health check options. Use this options to register 
/// all managed health checks.
/// </summary>
public class HealthCheckOptions
{
	/// <summary>
	/// Event handler for all health checks to be notified 
	/// to run their health check.
	/// </summary>
	public event Action<IHealthChecksBuilder> HealthChecks = default!;

	/// <summary>
	/// Total number of health checks registered.
	/// </summary>
	public int HealthCheckCount()
		=> HealthChecks?.GetInvocationList()?.Length ?? 0;

	internal List<Assembly> HealthCheckAssemblyReferenceTypes { get; } = new();

	/// <summary>
	/// Add assemblies to search for health checks.
	/// </summary>
	/// <typeparam name="T">A type in an assembly to search.</typeparam>
	public void AddAssemblyReferenceType<T>()
		=> AddAssemblyReferences(typeof(T).Assembly);

	/// <summary>
	/// Add assemblies to search for health checks.
	/// </summary>
	/// <param name="assemblies">Additional assemblies to search for health checks.</param>
	public void AddAssemblyReferences(params Assembly[] assemblies)
	{
		foreach (var assembly in assemblies)
			AddAssemblyReferences(assembly);
	}

	/// <summary>
	/// Add an assembly to search for health checks.
	/// </summary>
	/// <param name="assembly">An additional assembly to search for health checks.</param>
	public void AddAssemblyReferences(Assembly assembly)
	{
		if (HealthCheckAssemblyReferenceTypes.Contains(assembly))
			return;

		Log.Logger.Verbose("Adding Health Check Assembly {Assembly}", assembly.FullName);
		HealthCheckAssemblyReferenceTypes.Add(assembly);
	}

	/// <summary>
	/// Health check invoker. Should be called after all 
	/// health checks have been registered.
	/// </summary>
	/// <param name="builder"><seealso cref="IHealthChecksBuilder"/></param>
	public void InvokeHealthChecks(
		IHealthChecksBuilder builder)
	{
		HealthChecks?.Invoke(builder);

		Log.Logger.Information(
			"Invoking Health Check Builders {Count}",
			HealthCheckCount());
	}
}
