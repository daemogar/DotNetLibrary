using DotNetLibrary.Cookie.Internal;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Microsoft.JSInterop;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for managing cookies.
/// </summary>
public static class CookieExtensions
{
	/// <summary>
	/// Extension method for adding <seealso cref="IBasicCookieManager"/> and <seealso cref="CookieFactory"/> to the services.
	/// collection.
	/// </summary>
	/// <param name="services"><inheritdoc cref="ServiceCollectionServiceExtensions.AddTransient{TService}(IServiceCollection)"/></param>
	/// <returns><inheritdoc cref="ServiceCollectionServiceExtensions.AddTransient{TService}(IServiceCollection)"/></returns>
	public static IServiceCollection AddCookies(
		this IServiceCollection services, RootComponentMappingCollection root)
	{
		root.Add<CookieJavascript>("body::after");

		services.AddLogging();
		services.AddHttpContextAccessor();

		services.AddTransient<CookieFactory>();
		services.AddTransient<BasicCookieManager, BasicRuntimeCookieManager>();

		return services;
	}

	internal class BasicRuntimeCookieManager : BasicCookieManager
	{
		/// <summary>
		/// Construct a cookie manager.
		/// </summary>
		/// <param name="logger"><inheritdoc cref="ILogger"/>.</param>
		/// <param name="runtime"><inheritdoc cref="IJSRuntime"/></param>
		public BasicRuntimeCookieManager(ILogger logger, IJSRuntime runtime)
			: base(logger, nameof(IJSRuntime), runtime, default!, default!) { }
	}
}