using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Abstract implimentation of the discoverable service. Override
/// this class by class that should be provided as a application service.
/// </summary>
public abstract class DiscoverableService
{
	/// <summary>
	/// The order in which the service should be registered. Lower order 
	/// numbers will be registered before higher order numbers. Orders can be 
	/// negative. Zero is the default order and order of registration is not
	/// garenteed if the order matches another services order number.
	/// </summary>
	protected internal virtual int Order { get; } = 0;

	/// <summary>
	/// Method called on the object to run the objects registration with
	/// the dependency injection system.
	/// </summary>
	/// <param name="services">The service collection used for registering application dependencies.</param>
	/// <param name="configuration">The application configuration.</param>
	protected internal abstract void ConfigureAsService(
		IServiceCollection services, IConfiguration configuration);
}
