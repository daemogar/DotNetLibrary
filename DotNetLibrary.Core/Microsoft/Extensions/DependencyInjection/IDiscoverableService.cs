using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Interface to add to a class so it is auto discoverable and setup 
/// for dependency injection.
/// </summary>
public interface IDiscoverableService
{
	/// <summary>
	/// The order in which the service should be registered. Lower order 
	/// numbers will be registered before higher order numbers. Orders can be 
	/// negative. Zero is the default order and order of registration is not
	/// garenteed if the order matches another services order number.
	/// </summary>
	int Order => 0;

	/// <summary>
	/// Method called on the object to run the objects registration with
	/// the dependency injection system.
	/// </summary>
	/// <param name="services">The service collection used for registering application dependencies.</param>
	/// <param name="configuration">The application configuration.</param>
	void Register(IServiceCollection services, IConfiguration configuration);
}
