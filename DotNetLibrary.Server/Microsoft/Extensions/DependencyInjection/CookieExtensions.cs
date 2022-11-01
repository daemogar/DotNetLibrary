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

		services.AddTransient<ICookieFactory, CookieFactory>();
		services.AddTransient<IBasicCookieManager>(p =>
		{
			var logger = p.GetRequiredService<ILogger>();
			var context = p.GetRequiredService<HttpContext>();

			return new CookieManager(logger, context);
		});

		return services;
	}
}
