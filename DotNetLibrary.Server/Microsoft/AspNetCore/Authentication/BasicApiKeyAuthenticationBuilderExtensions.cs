using System.Text.Json;

namespace Microsoft.AspNetCore.Authentication;

public static class BasicApiKeyAuthenticationBuilderExtensions
{
	public static AuthenticationBuilder AddApiKeySupport(
		this AuthenticationBuilder builder,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback)
		=> AddApiKeySupport<BasicApiKeyAuthenticationHandler>(
			builder, optionsCallback);

	public static AuthenticationBuilder AddApiKeySupport<THandler>(
		this AuthenticationBuilder builder,
		Action<BasicApiKeyAuthenticationOptions>? optionsCallback)
		where THandler : BasicApiKeyAuthenticationHandler
	{
		builder.AddScheme<BasicApiKeyAuthenticationOptions, THandler>(
			BasicApiKeyAuthenticationOptions.DefaultScheme,
			optionsCallback ?? (_ => { }));
		return builder;
	}
}
