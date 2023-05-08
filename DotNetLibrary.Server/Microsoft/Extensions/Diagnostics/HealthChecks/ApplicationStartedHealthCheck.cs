using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Reflection;

using System.Runtime.Versioning;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

[IgnoreHealthCheck]
internal class ApplicationStartedHealthCheck : BasicHealthCheck
{
	private volatile bool IsReady;

	public void ApplicationLoaded() => IsReady = true;

	private IWebHostEnvironment Environment { get; }

	public ApplicationStartedHealthCheck(
		IWebHostEnvironment environment)
		: base("Application Started", new[] { "ready" })
	{
		Environment = environment;
	}

	protected override async Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context,
		object? data,
		CancellationToken cancellationToken)
		=> await Task.FromResult(IsReady
			? HealthCheckResult.Healthy(Environment.ContentRootPath)
			: FailureState(
				"Program does not appear to have started completely. " +
				$"Check that {nameof(ApplicationStartedBackgroundService)}" +
				"has been added and started. Enable logging level " +
				"to at least Information."));
}
