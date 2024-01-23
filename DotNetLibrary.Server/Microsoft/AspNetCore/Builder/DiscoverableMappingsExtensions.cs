using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Auto discoverable endpoint mappings.
/// </summary>
public static class DiscoverableMappingsExtensions
{
	/// <summary>
	/// Discovering all services and registering them with the 
	/// dependency injection services collection. This method will try to
	/// find all references classes the <seealso cref="IDiscoverableService"/>
	/// interface implimented.
	/// </summary>
	/// <param name="application">The server side application.</param>
	/// <returns>For chainging calls to the application, it is returned.</returns>
	public static WebApplication MapDiscoverableMappings(
		this WebApplication application)
		=> application.MapDiscoverableMappings(
			Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());

	/// <summary>
	/// <inheritdoc cref="MapDiscoverableMappings(WebApplication)"/>
	/// </summary>
	/// <param name="application">The server side application.</param>
	/// <param name="assembliesToSearch">List assembly that should be searched for references to the <seealso cref="IDiscoverableService"/> interface implmentation.</param>
	/// <returns>For chainging calls to the application, it is returned.</returns>
	public static WebApplication MapDiscoverableMappings(
		this WebApplication application,
		params Assembly[] assembliesToSearch)
	{
		ApplicationMapper builder = new(application);
		var discoverableType = typeof(IDiscoverableMapping);

		assembliesToSearch
			.SelectMany(p => p
			.DefinedTypes
			.Where(IsDiscoverableType)
			.Select(Construct))
			.OrderBy(p => p.Order)
			.ForEach(p => p.MapEndpoint(builder));

		return application;

		bool IsDiscoverableType(Type type)
			=> type.IsAssignableTo(discoverableType)
				&& !type.IsInterface
				&& !type.IsAbstract;
	}

	/// <summary>
	/// <inheritdoc cref="MapDiscoverableMappings(WebApplication)"/>
	/// </summary>
	/// <typeparam name="TDiscoverableMapping">The object type that should be registerd with the dependency injection services collection.</typeparam>
	/// <param name="application">The server side application.</param>
	/// <returns>For chainging calls to the application, it is returned.</returns>
	public static WebApplication AddDiscoverableService<TDiscoverableMapping>(
		this WebApplication application)
		where TDiscoverableMapping : IDiscoverableMapping
	{
		ApplicationMapper builder = new(application);
		var service = Construct(typeof(TDiscoverableMapping));
		service.MapEndpoint(builder);
		return application;
	}

	private static IDiscoverableMapping Construct(Type type)
		=> (IDiscoverableMapping)RuntimeHelpers
			.GetUninitializedObject(type);
}
