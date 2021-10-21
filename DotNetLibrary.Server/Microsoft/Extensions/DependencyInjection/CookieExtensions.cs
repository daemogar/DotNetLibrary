using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{ 
	public static class CookieExtensions
	{
		public static IServiceCollection AddCookies(this IServiceCollection services)
		{
			services.AddTransient<CookieManager>();
			services.AddTransient<CookieFactory>();

			return services;
		}
	}
}
