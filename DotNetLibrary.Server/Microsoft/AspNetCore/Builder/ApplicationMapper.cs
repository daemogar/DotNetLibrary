using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Limited <seealso cref="IEndpointRouteBuilder"/> mapping interface.
/// </summary>
public class ApplicationMapper
{
	private IEndpointRouteBuilder Builder { get; }

	/// <summary>
	/// Constructor to create a limited 
	/// <seealso cref="IEndpointRouteBuilder"/> mapping interface.
	/// </summary>
	/// <param name="builder"><seealso cref="IEndpointRouteBuilder"/></param>
	public ApplicationMapper(IEndpointRouteBuilder builder)
	{
		Builder = builder;
	}

	/// <summary>
	/// <inheritdoc cref="EndpointRouteBuilderExtensions.MapGet(IEndpointRouteBuilder, string, RequestDelegate)"/>	/// </summary>
	/// <param name="pattern">The route pattern.</param>
	/// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
	/// <returns>A Microsoft.AspNetCore.Builder.IEndpointConventionBuilder that can be used to further customize the endpoint.</returns>
	public IEndpointConventionBuilder MapGet(
		string pattern, RequestDelegate requestDelegate)
		=> Builder.MapGet(pattern, requestDelegate);

	/// <summary>
	/// <inheritdoc cref="EndpointRouteBuilderExtensions.MapPost(IEndpointRouteBuilder, string, RequestDelegate)"/>
	/// </summary>
	/// <param name="pattern">The route pattern.</param>
	/// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
	/// <returns>A Microsoft.AspNetCore.Builder.IEndpointConventionBuilder that can be used to further customize the endpoint.</returns>
	public IEndpointConventionBuilder MapPost(
		string pattern, RequestDelegate requestDelegate)
		=> Builder.MapPost(pattern, requestDelegate);

	/// <summary>
	/// <inheritdoc cref="EndpointRouteBuilderExtensions.MapPut(IEndpointRouteBuilder, string, RequestDelegate)"/>
	/// </summary>
	/// <param name="pattern">The route pattern.</param>
	/// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
	/// <returns>A Microsoft.AspNetCore.Builder.IEndpointConventionBuilder that can be used to further customize the endpoint.</returns>
	public IEndpointConventionBuilder MapPut(
		string pattern, RequestDelegate requestDelegate)
		=> Builder.MapPut(pattern, requestDelegate);

	/// <summary>
	/// <inheritdoc cref="EndpointRouteBuilderExtensions.MapDelete(IEndpointRouteBuilder, string, RequestDelegate)"/>
	/// </summary>
	/// <param name="pattern">The route pattern.</param>
	/// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
	/// <returns>A Microsoft.AspNetCore.Builder.IEndpointConventionBuilder that can be used to further customize the endpoint.</returns>
	public IEndpointConventionBuilder MapDelete(
		string pattern, RequestDelegate requestDelegate)
		=> Builder.MapDelete(pattern, requestDelegate);

	/// <summary>
	/// <inheritdoc cref="EndpointRouteBuilderExtensions.MapMethods(IEndpointRouteBuilder, string, IEnumerable{string}, RequestDelegate)"/>
	/// </summary>
	/// <param name="pattern">The route pattern.</param>
	/// <param name="httpMethods">HTTP methods that the endpoint will match.</param>
	/// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
	/// <returns>A Microsoft.AspNetCore.Builder.IEndpointConventionBuilder that can be used to further customize the endpoint.</returns>
	public IEndpointConventionBuilder MapMethods(
		string pattern, IEnumerable<string> httpMethods, RequestDelegate requestDelegate)
		=> Builder.MapMethods(pattern, httpMethods, requestDelegate);

	/// <summary>
	/// <inheritdoc cref="EndpointRouteBuilderExtensions.Map(IEndpointRouteBuilder, string, RequestDelegate)"/>
	/// </summary>
	/// <param name="pattern">The route pattern.</param>
	/// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
	/// <returns>A Microsoft.AspNetCore.Builder.IEndpointConventionBuilder that can be used to further customize the endpoint.</returns>
	public IEndpointConventionBuilder Map(
		string pattern, RequestDelegate requestDelegate)
		=> Builder.Map(pattern, requestDelegate);

	/// <summary>
	/// <inheritdoc cref="EndpointRouteBuilderExtensions.Map(IEndpointRouteBuilder, RoutePattern, RequestDelegate)"/>
	/// </summary>
	/// <param name="pattern">The route pattern.</param>
	/// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
	/// <returns>A Microsoft.AspNetCore.Builder.IEndpointConventionBuilder that can be used to further customize the endpoint.</returns>
	public IEndpointConventionBuilder Map(
		RoutePattern pattern, RequestDelegate requestDelegate)
		=> Builder.Map(pattern, requestDelegate);
}
