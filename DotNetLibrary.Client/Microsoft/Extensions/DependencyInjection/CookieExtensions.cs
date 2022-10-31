﻿using DotNetLibrary.Cookie.Internal;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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

		services.AddTransient<IBasicCookieManager, CookieManager>();
		services.AddTransient<ICookieFactory, CookieFactory>();

		return services;
	}
}
