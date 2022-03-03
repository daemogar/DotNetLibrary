using Microsoft.Extensions.Diagnostics.HealthChecks;

using Serilog;

namespace Microsoft.Extensions.Hosting;

internal class ApplicationStartedBackgroundService : BackgroundService
{
	private readonly ApplicationStartedHealthCheck _healthCheck;

	public ApplicationStartedBackgroundService(
		ApplicationStartedHealthCheck healthCheck)
	{
		_healthCheck = healthCheck;

		Log.Logger.Information("Background Service Started {Name}",
			nameof(ApplicationStartedBackgroundService));
	}

	protected override Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		Log.Logger.Information("Background Service Executed {Name}",
			nameof(ApplicationStartedBackgroundService));
		_healthCheck.ApplicationLoaded();
		return Task.CompletedTask;
	}
}