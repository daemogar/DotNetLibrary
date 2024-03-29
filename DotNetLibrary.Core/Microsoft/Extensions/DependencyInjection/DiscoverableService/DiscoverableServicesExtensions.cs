﻿using Microsoft.Extensions.Configuration;

using Serilog;

using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension method for discovering all services and registering them 
/// with the dependency injection system.
/// </summary>
public static class DiscoverableServicesExtensions
{
	/// <summary>
	/// Discovering all services and registering them with the 
	/// dependency injection services collection. This method will
	/// try to find all references classes implmenting interface
	/// <seealso cref="IDiscoverable"/>.
	/// </summary>
	/// <param name="services">The service collection used for registering application dependencies.</param>
	/// <param name="configuration">The application configuration.</param>
	/// <param name="excludeLibraryAssemblies">Only include the current assembly for searching.</param>
	/// <returns>For chainging calls to the services collection, it is returned.</returns>
	public static IServiceCollection AddDiscoverableServices(
		this IServiceCollection services,
		IConfiguration configuration,
			bool excludeLibraryAssemblies = false)
		=> services.AddDiscoverableServices(
			configuration,
			excludeLibraryAssemblies,
			Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());

	/// <inheritdoc cref="AddDiscoverableServices(IServiceCollection, IConfiguration, bool, Assembly[])"/>
	public static IServiceCollection AddDiscoverableServices(
		this IServiceCollection services,
		IConfiguration configuration,
		params Assembly[] assembliesToSearch)
		=> services.AddDiscoverableServices(configuration, false, assembliesToSearch);

	/// <summary>
	/// Discovering all services and registering them with the 
	/// dependency injection services collection.
	/// </summary>
	/// <param name="services">The service collection used for registering application dependencies.</param>
	/// <param name="assembliesToSearch">List assembly that should be searched for references to classes implementing the interface <seealso cref="IDiscoverable"/>.</param>
	/// <param name="configuration">The application configuration.</param>
	/// <param name="excludeLibraryAssemblies">Only include the provided assemblies for searching.</param>	/// <returns>For chainging calls to the services collection, it is returned.</returns>
	public static IServiceCollection AddDiscoverableServices(
		this IServiceCollection services,
		IConfiguration configuration,
		bool excludeLibraryAssemblies,
		params Assembly[] assembliesToSearch)
	{
		var discoverableType = typeof(IDiscoverable);

		assembliesToSearch
			.UnionIf(() => !excludeLibraryAssemblies, discoverableType.Assembly)
			.SelectMany(p => p
			.DefinedTypes
			.Where(p => IsDiscoverableType(p))
			.Select(Construct))
			.OrderBy(p => p.Order)
			.ForEach(p => Register(p, services, configuration));

		return services;

		bool IsDiscoverableType(TypeInfo type)
			=> type.IsAssignableTo(discoverableType)
				&& !type.IsInterface
				&& !type.IsAbstract;
	}

	private static readonly List<string> RegisteredTypes = new();
	private static void Register(
		IDiscoverable service,
		IServiceCollection services,
		IConfiguration configuration)
	{
		var type = service.GetType();
		var key = type.FullName ?? type.Name;
		if (RegisteredTypes.Contains(key))
		{
			Log.Logger.Warning(
				"Trying to register {Service} more than once.",
				type.Name);
		}
		else
		{
			RegisteredTypes.Add(key);
			service.ConfigureAsService(services, configuration);
		}
	}

	/// <summary>
	/// Register the type as a service using its registration method defined by
	/// the interface <seealso cref="IDiscoverable"/>.
	/// </summary>
	/// <typeparam name="TDiscoverableService">The object type that should be registerd with the dependency injection services collection.</typeparam>
	/// <param name="services">The service collection used for registering application dependencies.</param>
	/// <param name="configuration">The application configuration.</param>
	/// <returns>For chainging calls to the services collection, it is returned.</returns>
	public static IServiceCollection AddDiscoverableService<TDiscoverableService>(
		this IServiceCollection services,
		IConfiguration configuration)
		where TDiscoverableService : IDiscoverable
	{
		var service = Construct(typeof(TDiscoverableService));
		Register(service, services, configuration);
		return services;
	}
	
	private static IDiscoverable Construct(Type type)
#if NETSTANDARD2_0
		=> (IDiscoverable)System.Runtime.Serialization.FormatterServices
#else
		=> (IDiscoverable)System.Runtime.CompilerServices.RuntimeHelpers
#endif
			.GetUninitializedObject(type);
}