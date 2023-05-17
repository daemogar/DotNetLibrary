using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

internal record ApplicationStartedHealthCheck : BasicHealthCheck
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

	public override void RegisterHealthCheckServices(
		IServiceCollection services, IConfiguration configuration)
	{
		services.TryAddSingleton<ApplicationStartedHealthCheck>();
		services.AddHostedService<ApplicationStartedBackgroundService>();
	}
}
