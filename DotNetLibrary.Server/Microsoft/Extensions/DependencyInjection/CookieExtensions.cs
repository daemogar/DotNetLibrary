using DotNetLibrary.Cookies;

namespace Microsoft.Extensions.DependencyInjection
{ 
	public static class CookieExtensions
	{
		public static void AddCookies(this IServiceCollection services)
		{
			services.AddTransient<CookieManager>();
			services.AddTransient<ImpersonationCookie>();
		}
	}
}
