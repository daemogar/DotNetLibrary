using Microsoft.Extensions.Configuration;

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension method for discovering all services and registering them 
/// with the dependency injection system.
/// </summary>
public static class DiscoverableServicesExtensions
{
	/// <summary>
	/// Discovering all services and registering them with the 
	/// dependency injection services collection. This method will try to
	/// find all references classes the <seealso cref="IDiscoverableService"/>
	/// interface implimented.
	/// </summary>
	/// <param name="services">The service collection used for registering application dependencies.</param>
	/// <param name="configuration">The application configuration.</param>
	/// <returns>For chainging calls to the services collection, it is returned.</returns>
	public static IServiceCollection AddDiscoverableServices(
		this IServiceCollection services,
		IConfiguration configuration)
		=> services.AddDiscoverableServices(
			configuration,
			Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());

	/// <summary>
	/// Discovering all services and registering them with the 
	/// dependency injection services collection.
	/// </summary>
	/// <param name="services">The service collection used for registering application dependencies.</param>
	/// <param name="assembliesToSearch">List assembly that should be searched for references to the <seealso cref="IDiscoverableService"/> interface implmentation.</param>
	/// <param name="configuration">The application configuration.</param>
	/// <returns>For chainging calls to the services collection, it is returned.</returns>
	public static IServiceCollection AddDiscoverableServices(
		this IServiceCollection services,
		IConfiguration configuration,
		params Assembly[] assembliesToSearch)
	{
		var discoverableType = typeof(IDiscoverableService);

		assembliesToSearch
			.SelectMany(p => p
			.DefinedTypes
			.Where(IsDiscoverableType)
			.Select(Construct))
			.OrderBy(p => p.Order)
			.ForEach(p => p.Register(services, configuration));

		return services;

		bool IsDiscoverableType(Type type)
			=> type.IsAssignableTo(discoverableType)
				&& !type.IsInterface
				&& !type.IsAbstract;
	}

	/// <summary>
	/// Register the type as a service using its registration method defined by
	/// the <seealso cref="IDiscoverableService"/> interface.
	/// </summary>
	/// <typeparam name="TDiscoverableService">The object type that should be registerd with the dependency injection services collection.</typeparam>
	/// <param name="services">The service collection used for registering application dependencies.</param>
	/// <param name="configuration">The application configuration.</param>
	/// <returns>For chainging calls to the services collection, it is returned.</returns>
	public static IServiceCollection AddDiscoverableService<TDiscoverableService>(
		this IServiceCollection services,
		IConfiguration configuration)
		where TDiscoverableService : IDiscoverableService
	{
		var service = Construct(typeof(TDiscoverableService));
		service.Register(services, configuration);
		return services;
	}

	private static IDiscoverableService Construct(Type type)
		=> (IDiscoverableService)FormatterServices
			.GetUninitializedObject(type);
}