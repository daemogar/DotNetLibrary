using Microsoft.AspNetCore.Http;

using Serilog;

using System;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Add <seealso cref="IHttpContextAccessor"/> and
/// <seealso cref="HttpContext"/> to dependency injection.
/// </summary>
public static class HttpContextExtensions
{
	private static bool IsHttpContextAdded { get; set; }

	/// <summary>
	/// <inheritdoc cref="HttpContextExtensions"/>
	/// </summary>
	/// <param name="services"><seealso cref="IServiceCollection"/></param>
	/// <returns><seealso cref="IServiceCollection"/></returns>
	/// <exception cref="NullReferenceException"></exception>
	public static IServiceCollection AddHttpContext(
			this IServiceCollection services)
	{
		if (IsHttpContextAdded)
			return services;

		IsHttpContextAdded = true;

		return services
			.AddHttpContextAccessor()
			.AddTransient(p =>
			{
				var context = p
					.GetRequiredService<IHttpContextAccessor>()
					.HttpContext;

				if (context != null)
					return context;

				Log.Logger.Error("No HttpContext");
				throw new NullReferenceException(
							$"The current context is missing.");
			});
	}
}