using DotNetLibrary.Authorization;

using Microsoft.AspNetCore.Components.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class AuthorizationExtensions
	{
		public static void AddAuthenticationBasics
			<TProviderType, TUserType, TRoleType>(
			this IServiceCollection services)
			where TProviderType : BasicAuthenticationStateProvider<TUserType, TRoleType>
			where TUserType : class, IBasicUser<TRoleType>, new()
			where TRoleType : class, IBasicRole
		{
			services.AddCookies();

			services.AddSingleton<RoleFactory>();
			services.AddSingleton<UserFactory>();

			services.AddScoped<TProviderType>();
			services.AddScoped<AuthenticationStateProvider>(p
				=> p.GetRequiredService<TProviderType>());
		}
	}
}
