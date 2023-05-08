using System.Text;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Base logic used for auto discovered health checks.
/// <inheritdoc cref="IHealthCheck"/>
/// </summary>
public abstract class BasicHealthCheck : IHealthCheck
{
	/// <summary>
	/// The status used for the failure state. The default is
	/// <seealso cref="HealthStatus.Unhealthy"/>.
	/// </summary>
	public virtual HealthStatus FailureStatus { get; } = HealthStatus.Unhealthy;

	/// <summary>
	/// Returns a failure state using the <paramref name="description"/> 
	/// of the failure and optional <paramref name="data"/>.
	/// </summary>
	/// <param name="description">A discription of the failure and how to resolve the failure.</param>
	/// <param name="data">Additional details used in diagnosing the failure.</param>
	/// <returns>Returns a failure state <seealso cref="HealthCheckResult"/>.</returns>
	protected HealthCheckResult FailureState(string description,
		IReadOnlyDictionary<string, object> data = null!)
		=> new(FailureStatus, description, null, data);

	/// <summary>
	/// Returns a failure state using the <paramref name="exception"/> 
	/// of the failure and optional <paramref name="data"/>.
	/// </summary>
	/// <param name="exception">The exception that was thrown during the health check.</param>
	/// <param name="data">Additional details used in diagnosing the failure.</param>
	/// <returns>Returns a failure state <seealso cref="HealthCheckResult"/>.</returns>
	protected HealthCheckResult FailureState(Exception exception,
		IReadOnlyDictionary<string, object> data = null!)
		=> new(FailureStatus, null, exception, data);

	/// <summary>
	/// The name of the health check.
	/// </summary>
	public string Name { get; internal set; } = default!;

	/// <summary>
	/// A list of tags that can be used to filter health checks.
	/// </summary>
	public IReadOnlyList<string> Tags { get; internal set; } = default!;

	/// <summary>
	/// A <seealso cref="TimeSpan"/> representing the timeout 
	/// of the check. The default is 2 minutes.
	/// </summary>
	public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(2);

	/// <summary>
	/// Basic health check without <seealso cref="Name"/> and
	/// <seealso cref="Tags"/>. The <seealso cref="Name"/> will be 
	/// set automatically using the class type and formatted with 
	/// <seealso cref="System.Text.StringExtensions.ToTitleCase(string, bool)"/>.
	/// </summary>
	protected BasicHealthCheck() : this(default!, default!) { }

	/// <summary>
	/// Basic health check without <seealso cref="Name"/> and optional
	/// <seealso cref="Tags"/>. The <seealso cref="Name"/> will be 
	/// set automatically using the class type and formatted with 
	/// <seealso cref="System.Text.StringExtensions.ToTitleCase(string, bool)"/>.
	/// </summary>
	/// <param name="tags">A list of tags that can be used to filter health checks.</param>
	protected BasicHealthCheck(string[] tags) : this(default!, tags)
	{
	}

	/// <summary>
	/// Basic health check with <seealso cref="Name"/> and optional
	/// <seealso cref="Tags"/>.
	/// </summary>
	/// <param name="name">The name of the health check.</param>
	/// <param name="tags">A list of tags that can be used to filter health checks.</param>
	protected BasicHealthCheck(string name, string[] tags)
	{
		Name = name ?? GetType().Name.ToTitleCase();
		Tags = tags?.ToList() ?? new();
	}

	/// <inheritdoc cref="IHealthCheck.CheckHealthAsync(HealthCheckContext, CancellationToken)"/>
	protected abstract Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context,
		object? data,
		CancellationToken cancellationToken);

	/// <inheritdoc cref="IHealthCheck.CheckHealthAsync(HealthCheckContext, CancellationToken)"/>
	public async Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		try
		{
			return await CheckHealthAsync(context, default, cancellationToken);
		}
		catch (Exception e)
		{
			return FailureState(e);
		}
	}
}