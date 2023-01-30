using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Microsoft.AspNetCore.Authentication;

public class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions>
{
	public ApiKeyHandler(
					IOptionsMonitor<ApiKeyOptions> options,
					ILoggerFactory logger,
					UrlEncoder encoder,
					ISystemClock clock)
					: base(options, logger, encoder, clock)
	{
	}

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var apiKey = ParseApiKey();

		if (string.IsNullOrEmpty(apiKey))
			return Task.FromResult(AuthenticateResult.NoResult());

		if (string.Compare(apiKey, Options.ApiKey.Value, StringComparison.Ordinal) != 0)
			return Task.FromResult(AuthenticateResult.Fail($"Invalid API Key provided."));

		var principal = BuildPrincipal(Scheme.Name, Options.QueryStringKey, Options.ClaimsIssuer ?? ApiKeyDefaults.SchemaName);
		return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
	}

	static ClaimsPrincipal BuildPrincipal(
			string schemeName,
			string name,
			string issuer,
			params Claim[] claims)
	{
		var identity = new ClaimsIdentity(schemeName);

		identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, name, ClaimValueTypes.String, issuer));
		identity.AddClaim(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, issuer));

		identity.AddClaims(claims);

		return new(identity);
	}

	protected string ParseApiKey()
	{
		if (Request.Headers.TryGetValue(Options.QueryStringKey, out var value)
			&& value.Count > 0
			&& value[0] is not null)
			return value[0]!;

		if (Request.Query.TryGetValue(Options.QueryStringKey, out value)
			&& value.Count > 0
			&& value[0] is not null)
			return value[0]!;

		return string.Empty;
	}
}
