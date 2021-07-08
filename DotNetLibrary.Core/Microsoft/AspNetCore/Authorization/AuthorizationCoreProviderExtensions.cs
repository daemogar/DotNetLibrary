﻿using DotNetLibrary;
using DotNetLibrary.Configuration.Identifier;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

		public static IProgramOptions AddProgramOptions<TProgramOptions>(
			this IServiceCollection services)
			where TProgramOptions : IProgramOptions, new()
		{
			var provider = services.BuildServiceProvider();
			var options = provider.GetService<IProgramOptions>();

			if (options == null)
			{
				options = new TProgramOptions();
				services.AddSingleton(options);
			}

			return options;
		}

		public static void AddAuthorizationCore(
			this IServiceCollection services,
			Action<AuthorizationOptions> authorizationOptionsCallback)
			=> services.AddAuthorizationCore<BasicProgramOptions, BasicIdentifierFormatter>(
				authorizationOptionsCallback);

		public static void AddAuthorizationCore<TProgramOptions>(
			this IServiceCollection services,
			Action<AuthorizationOptions> authorizationOptionsCallback)
			where TProgramOptions : IProgramOptions, new()
			=> services.AddAuthorizationCore<TProgramOptions, BasicIdentifierFormatter>(
				authorizationOptionsCallback);

		public static void AddAuthorizationCore<TProgramOptions, TIdentifierFormatter>(
			this IServiceCollection services,
			Action<AuthorizationOptions> authorizationOptionsCallback)
			where TProgramOptions : IProgramOptions, new()
			where TIdentifierFormatter : class, IIdentifierFormatter, new()
		{
			var options = services.AddProgramOptions<TProgramOptions>();

			IssuerManager issuer = new(options.IssuerName
				?? services.BuildServiceProvider()
					.GetRequiredService<IConfiguration>()
					.GetValue<string>("ApplicationName"
				?? throw new NullReferenceException(
					$"IssuerName was not set in the options nor " +
					$"was an ApplicationName set in the settings.")));

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
