using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <inheritdoc cref="DiscoverableSingletonService{TService, TImplementation}" />
public abstract class DiscoverableSingletonService<T> : DiscoverableSingletonService<T, T> where T : class { }

/// <summary>/// 
/// Abstract implimentation of the discoverable service. Overriding
/// this class should be provided as a application service. This implements
/// adding the parent class as a singleton in the DI before calling additional
/// logic.
/// </summary>
/// <typeparam name="TService">The type of the service to register.</typeparam>
/// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
public abstract class DiscoverableSingletonService<TService, TImplementation> : DiscoverableService where TService : class
	where TImplementation : class, TService
{
	/// <summary>
	/// Used to keep from loading the service more then once.
	/// </summary>
	protected internal bool IsLoaded { get; set; }

	/// <summary>
	/// <inheritdoc cref="IDiscoverable.ConfigureAsService(IServiceCollection, IConfiguration)" />
	/// The parent class will be added as a singleton to the DI before calling this method.
	/// </summary>
	/// <param name="services"><inheritdoc cref="IDiscoverable.ConfigureAsService(IServiceCollection, IConfiguration)" /></param>
	/// <param name="configuration"><inheritdoc cref="IDiscoverable.ConfigureAsService(IServiceCollection, IConfiguration)" /></param>
	protected internal abstract void ConfigureAsSingletonService(
		IServiceCollection services, IConfiguration configuration);

	/// <inheritdoc cref="IDiscoverable.ConfigureAsService(IServiceCollection, IConfiguration)" />
	protected internal override void ConfigureAsService(
				IServiceCollection services, IConfiguration configuration)
	{
		if (IsLoaded)
			return;

		IsLoaded = true;

		services.AddSingleton<TService, TImplementation>();
		ConfigureAsSingletonService(services, configuration);
	}
}