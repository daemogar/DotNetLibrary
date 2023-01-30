using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class BasicApiKeyHandlerExtensions
{
	public static async Task<IServiceCollection> AddApiKeyHandlerAsync<TApiKey, TInitializer>(
		this IServiceCollection services,
		TInitializer? initializerModel = default,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback = default)
		where TInitializer : BasicApiKeyLibraryInitializer<TApiKey>, new()
		where TApiKey : IBasicApiKey
		=> await services.AddApiKeyHandlerAsync(Create<TApiKey, TInitializer>(initializerModel), optionsCallback);

	public static async Task<IServiceCollection> AddApiKeyHandlerAsync<TApiKey, TInitializer, TValidator>(
		this IServiceCollection services,
		TInitializer? initializerModel = default,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback = default)
		where TValidator : BasicApiKeyValidator<TApiKey>, new()
		where TInitializer : BasicApiKeyLibraryInitializer<TApiKey>, new()
		where TApiKey : IBasicApiKey
		=> await services.AddApiKeyHandlerAsync<TApiKey, TValidator>(Create<TApiKey, TInitializer>(initializerModel), optionsCallback);

	public static async Task<IServiceCollection> AddApiKeyHandlerAsync<TInitializer>(
		this IServiceCollection services,
		TInitializer? initializerModel = default,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback = default)
		where TInitializer : BasicApiKeyLibraryInitializer<BasicApiKey>, new()
		=> await services.AddApiKeyHandlerAsync(Create<BasicApiKey, TInitializer>(initializerModel), optionsCallback);

	public static async Task<IServiceCollection> AddApiKeyHandlerAsync<TInitializer, TValidator>(
		this IServiceCollection services,
		TInitializer? initializerModel = default,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback = default)
		where TValidator : BasicApiKeyValidator<BasicApiKey>, new()
		where TInitializer : BasicApiKeyLibraryInitializer<BasicApiKey>, new()
		=> await services.AddApiKeyHandlerAsync<BasicApiKey, TValidator>(Create<BasicApiKey, TInitializer>(initializerModel), optionsCallback);

	public static async Task<IServiceCollection> AddApiKeyHandlerAsync(
		this IServiceCollection services,
		Func<Task<IEnumerable<BasicApiKey>>> apikeyLibraryInitializer,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback = default)
		=> await services.AddApiKeyHandlerAsync<BasicApiKey>(apikeyLibraryInitializer, optionsCallback);

	public static async Task<IServiceCollection> AddApiKeyHandlerAsync<TValidator>(
		this IServiceCollection services,
		Func<Task<IEnumerable<BasicApiKey>>> apikeyLibraryInitializer,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback = default)
		where TValidator : BasicApiKeyValidator<BasicApiKey>, new()
		=> await services.AddApiKeyHandlerAsync<BasicApiKey, TValidator>(apikeyLibraryInitializer, optionsCallback);

	public static async Task<IServiceCollection> AddApiKeyHandlerAsync<TApiKey>(
		this IServiceCollection services,
		Func<Task<IEnumerable<TApiKey>>> apikeyLibraryInitializer,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback = default)
		where TApiKey : IBasicApiKey
		=> services.AddApiKeyHandler(await apikeyLibraryInitializer(), optionsCallback);

	public static async Task<IServiceCollection> AddApiKeyHandlerAsync<TApiKey, TValidator>(
		this IServiceCollection services,
		Func<Task<IEnumerable<TApiKey>>> apikeyLibraryInitializer,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback = default)
		where TValidator : BasicApiKeyValidator<TApiKey>, new()
		where TApiKey : IBasicApiKey
		=> services.AddApiKeyHandler<TApiKey, TValidator>(await apikeyLibraryInitializer(), optionsCallback);

	public static IServiceCollection AddApiKeyHandler<TApiKey>(
		this IServiceCollection services,
		IEnumerable<TApiKey> apikeys,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback = default)
		where TApiKey : IBasicApiKey
		=> services.AddApiKeyHandler<TApiKey, BasicApiKeyValidator<TApiKey>>(apikeys, optionsCallback);

	private static bool IsLoaded { get; set; }

	public static IServiceCollection AddApiKeyHandler<TApiKey, TValidator>(
		this IServiceCollection services,
		IEnumerable<TApiKey> apikeys,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback = default)
		where TValidator : BasicApiKeyValidator<TApiKey>, new()
		where TApiKey : IBasicApiKey
	{
		if (!IsLoaded)
		{
			IsLoaded = true;

			TValidator validator = new();
			validator.SetApiKeys(apikeys);
			
			services.AddSingleton(validator);
			services.AddSingleton((IBasicApiKeyValidator<TApiKey>)validator);

			services.AddApiKeySupport(optionsCallback);
		}

		return services;
	}

	private static Func<Task<IEnumerable<TApiKey>>> Create<TApiKey, TInitializer>(
		TInitializer? initializerModel)
		where TInitializer : BasicApiKeyLibraryInitializer<TApiKey>, new()
		where TApiKey : IBasicApiKey
		=> async () => await (initializerModel ?? new TInitializer()).ExecuteAsync();

	public static AuthenticationBuilder AddApiKeySupport(
		this IServiceCollection services,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback)
		=> AddApiKeySupport<BasicApiKeyAuthenticationHandler>(
			services, optionsCallback);

	public static AuthenticationBuilder AddApiKeySupport<THandler>(
		this IServiceCollection services,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback)
		where THandler : BasicApiKeyAuthenticationHandler
		=> services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = BasicApiKeyAuthenticationOptions.DefaultScheme;
				options.DefaultChallengeScheme = BasicApiKeyAuthenticationOptions.DefaultScheme;
			})
			.AddApiKeySupport(optionsCallback);

}