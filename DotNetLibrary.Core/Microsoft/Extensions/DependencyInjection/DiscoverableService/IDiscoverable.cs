using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Override this interface to implement the order and add a configure services
/// method for adding DI components.
/// </summary>
public interface IDiscoverable
{
	/// <summary>
	/// The order in which the service should be registered. Lower order 
	/// numbers will be registered before higher order numbers. Orders can be 
	/// negative. Zero is the default order and order of registration is not
	/// garenteed if the order matches another services order number.
	/// </summary>
	protected internal int Order { get; }

	/// <summary>
	/// Method called on the object to run the objects registration with
	/// the dependency injection system.
	/// </summary>
	/// <param name="services">The service collection used for registering application dependencies.</param>
	/// <param name="configuration">The application configuration.</param>
	protected internal abstract void ConfigureAsService(
		IServiceCollection services, IConfiguration configuration);
}
