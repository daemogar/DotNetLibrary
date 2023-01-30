using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection.Extensions
{

	public static class ApiKeyExtensions
	{
		private static string? LoadedApiKey { get; set; }

		public static IServiceCollection AddApiKeyAuthentication(
			this IServiceCollection services, ApiKeyValue apikey)
		{
			if (LoadedApiKey is not null)
				throw new Exception(
					$"ApiKey Authentication has already been added. [{LoadedApiKey}...]");

			LoadedApiKey = apikey.Value[..5];

			services.TryAddSingleton(apikey);

			services
				.AddAuthentication(ApiKeyDefaults.SchemaName)
				.AddScheme<ApiKeyOptions, ApiKeyHandler>(ApiKeyDefaults.SchemaName, options =>
				{
					options.ApiKey = apikey;
				});

			return services;
		}
	}
}
