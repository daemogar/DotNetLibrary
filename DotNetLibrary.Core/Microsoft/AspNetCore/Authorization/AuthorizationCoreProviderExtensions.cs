using DotNetLibrary;
using DotNetLibrary.Configuration.Identifier;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Authorization
{
	public static class AuthProviderExtensions
	{
		private static void ConfigureClaimsIdentitySelector(IssuerManager manager)
		{
			var selector = ClaimsPrincipal.PrimaryIdentitySelector;
			ClaimsPrincipal.PrimaryIdentitySelector = p => PrimarySelector(p.ToList());

			ClaimsIdentity? PrimarySelector(IReadOnlyCollection<ClaimsIdentity> p)
				=> p.FirstOrDefault(q => q.AuthenticationType?.Equals(manager.IssuerName) == true)
					 ?? selector(p);
		}

		public static void AddAuthorizationCore<TProgramOptions>(
			this IServiceCollection services,
			ILogger logger,
			TProgramOptions options,
			IConfiguration configuration,
			Action<AuthorizationOptions> authorizationOptionsCallback)
			where TProgramOptions : IProgramOptions
			=> services.AddAuthorizationCore<TProgramOptions, BasicIdentifierFormatter>(
				logger, options, configuration, authorizationOptionsCallback);

		public static void AddAuthorizationCore<TProgramOptions, TIdentifierFormatter>(
			this IServiceCollection services,
			ILogger logger,
			TProgramOptions options,
			IConfiguration configuration,
			Action<AuthorizationOptions> authorizationOptionsCallback)
			where TProgramOptions : IProgramOptions
			where TIdentifierFormatter : class, IIdentifierFormatter, new()
		{
			var applicationName = configuration.GetValue<string>("ApplicationName");
			IssuerManager issuer = new(options.IssuerName ?? applicationName);

			services.AddSingleton(issuer);

			ConfigureClaimsIdentitySelector(issuer);

			services.AddSingleton<IIdentifierFormatter, TIdentifierFormatter>();

			services.AddAuthorizationCore(p =>
			{
				authorizationOptionsCallback(p);
				options.Policies.ForEach(policy => policy(p));
			});
		}
	}
}
