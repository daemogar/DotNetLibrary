using Microsoft.AspNetCore.Http;

using Serilog;

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
	public static IServiceCollection AddCookies(this IServiceCollection services)
	{
		services.AddLogging();
		services.AddHttpContextAccessor();

		services.AddTransient<CookieFactory>();
		services.AddTransient<BasicCookieManager, BasicHttpContextCookieManager>();

		return services;
	}

	internal class BasicHttpContextCookieManager : BasicCookieManager
	{
		/// <summary>
		/// Construct a cookie manager.
		/// </summary>
		/// <param name="logger"><inheritdoc cref="ILogger"/>.</param>
		/// <param name="context"><inheritdoc cref="HttpContext"/></param>
		public BasicHttpContextCookieManager(ILogger logger, HttpContext context)
			: base(logger, nameof(HttpContext), default!, context, new()) { }
	}
}
