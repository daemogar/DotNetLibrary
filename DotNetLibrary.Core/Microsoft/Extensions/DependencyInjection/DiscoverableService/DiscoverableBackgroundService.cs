using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Special case of a <seealso cref="DiscoverableService"/> and 
/// <seealso cref="BackgroundService"/>. <inheritdoc /> 
/// </summary>
/// <typeparam name="T">A type that implments <seealso cref="DiscoverableBackgroundService{T}"/>.</typeparam>
public abstract class DiscoverableBackgroundService<T> : DiscoverableService
	where T : DiscoverableBackgroundService<T>
{
	/// <inheritdoc cref="BackgroundService.ExecuteAsync(CancellationToken)" />
	protected abstract new Task ExecuteAsync(CancellationToken stoppingToken);

	/// <summary>
	/// <inheritdoc /> Base implementation just addes the <typeparamref name="T"/>
	/// as a hosted service to the services collection. Override to extend
	/// functionality.
	/// </summary>
	/// <param name="services"><inheritdoc /></param>
	/// <param name="configuration"><inheritdoc /></param>
	protected internal sealed override void ConfigureAsService(IServiceCollection services, IConfiguration configuration)
	{
		services.AddHostedService<T>();
		ConfigureAsBackgroundService(services, configuration);
	}

	/// <summary>
	/// <inheritdoc cref="DiscoverableService.ConfigureAsService(IServiceCollection, IConfiguration)" />
	/// This will call <see cref="ServiceCollectionHostedServiceExtensions.AddHostedService{T}(IServiceCollection)"/>
	/// </summary>
	/// before any addtional configuration.
	/// <param name="services"><inheritdoc cref="ConfigureAsService(IServiceCollection, IConfiguration)" path="/param[@name='services']" /></param>
	/// <param name="configuration"><inheritdoc cref="ConfigureAsService(IServiceCollection, IConfiguration)" path="/param[@name='configuration']" /></param>
	protected virtual void ConfigureAsBackgroundService(IServiceCollection services, IConfiguration configuration) { }
}
