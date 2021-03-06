using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{ 
	/// <summary>
	/// Extensions for managing cookies.
	/// </summary>
	public static class CookieExtensions
	{
		/// <summary>
		/// Extension method for adding <seealso cref="CookieManager"/> and <seealso cref="CookieFactory"/> to the services.
		/// collection.
		/// </summary>
		/// <param name="services"><inheritdoc cref="ServiceCollectionServiceExtensions.AddTransient{TService}(IServiceCollection)"/></param>
		/// <returns><inheritdoc cref="ServiceCollectionServiceExtensions.AddTransient{TService}(IServiceCollection)"/></returns>
		public static IServiceCollection AddCookies(this IServiceCollection services)
		{
			services.AddTransient<CookieManager>();
			services.AddTransient<CookieFactory>();

			return services;
		}
	}
}
