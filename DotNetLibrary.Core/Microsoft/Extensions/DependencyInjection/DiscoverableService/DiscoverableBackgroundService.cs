using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Special case of a <seealso cref="DiscoverableService"/> and 
/// <seealso cref="BackgroundService"/>. <inheritdoc /> 
/// </summary>
/// <typeparam name="T">A type that implments <seealso cref="DiscoverableBackgroundService{T}"/>.</typeparam>
public abstract class DiscoverableBackgroundService<T> : DiscoverableService, IHostedService, IDisposable
	where T : DiscoverableBackgroundService<T>
{
	private Task? Task;

	private readonly CancellationTokenSource Source = new();

	/// <summary>
	/// <inheritdoc /> Base implementation just addes the <typeparamref name="T"/>
	/// as a hosted service to the services collection. Override to extend
	/// functionality.
	/// </summary>
	/// <param name="services"><inheritdoc /></param>
	/// <param name="configuration"><inheritdoc /></param>
	protected internal override void ConfigureAsService(
		IServiceCollection services, IConfiguration configuration)
		=> services.AddHostedService<T>();

	/// <summary>
	/// This method is called once on startup.
	/// </summary>
	/// <param name="stoppingToken">Trigger when <seealso cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
	/// <returns>Returns a Task of a long running operation.</returns>
	protected abstract Task InitializeAsync(CancellationToken stoppingToken);

	/// <inheritdoc cref="BackgroundService.ExecuteAsync(CancellationToken)"/>
	protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

	/// <inheritdoc cref="BackgroundService.StartAsync(CancellationToken)"/>
	public virtual Task StartAsync(CancellationToken cancellationToken)
	{
		Task = InitializeAsync(cancellationToken)
			.ContinueWith(_ => ExecuteAsync(Source.Token));

		return Task.IsCompleted ? Task : Task.CompletedTask;
	}

	/// <inheritdoc cref="BackgroundService.StopAsync(CancellationToken)"/>
	public virtual async Task StopAsync(CancellationToken cancellationToken)
	{
		if (Task is null)
			return;

		try
		{
			Source.Cancel();
		}
		finally
		{
			await Task.WhenAny(new[]
			{
				Task,
				Task.Delay(-1, cancellationToken)
			});
		}
	}

	/// <inheritdoc cref="BackgroundService.Dispose"/>
	public virtual void Dispose() => Source.Cancel();
}
