namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Interface to add to a class so it is auto discoverable and 
/// mapping a route.
/// </summary>
public interface IDiscoverableMapping
{
	/// <summary>
	/// The order in which the mapping should be registered. Lower 
	/// order numbers will be registered before higher order 
	/// numbers. Orders can be negative. Zero is the default order 
	/// and order of registration is not garenteed if the order 
	/// matches another mapping order number.
	/// </summary>
	int Order => 0;

	/// <summary>
	/// Endpoint configuration for mapping endpoints.
	/// </summary>
	/// <param name="builder">Limited application map builder.</param>
	void MapEndpoint(ApplicationMapper builder);
}
