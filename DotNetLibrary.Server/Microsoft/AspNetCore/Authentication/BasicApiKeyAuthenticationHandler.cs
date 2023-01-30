using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Microsoft.AspNetCore.Authentication;

public class BasicApiKeyAuthenticationHandler : AuthenticationHandler<BasicApiKeyAuthenticationOptions>
{
	public IBasicApiKeyValidator<IBasicApiKey> Validator { get; }

	public BasicApiKeyAuthenticationHandler(
		IOptionsMonitor<BasicApiKeyAuthenticationOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		ISystemClock clock,
		IBasicApiKeyValidator<IBasicApiKey> validator)
		: base(options, logger, encoder, clock)
	{
		Validator = validator;
	}

	protected virtual async Task<List<Claim>> GetClaimsAsync(IBasicApiKey apikey)
	{
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, apikey.ApplicationName, default, apikey.ApplicationName)
		};

		if (apikey.Roles is not null)
			claims.AddRange(apikey.Roles
				.Select(role => new Claim(ClaimTypes.Role, role, default, apikey.ApplicationName)));

		return await Task.FromResult(claims);
	}

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (!Request.Headers
			.TryGetValue(BasicApiKeyAuthenticationOptions.DefaultScheme, out var apiKeyHeaderValues))
			return AuthenticateResult.Fail($"No Authentication");

		if (apiKeyHeaderValues.Count != 1)
			return AuthenticateResult.Fail(
				$"{apiKeyHeaderValues.Count} Authentication");

		var providedApiKey = apiKeyHeaderValues.Single();
		if (string.IsNullOrWhiteSpace(providedApiKey))
			return AuthenticateResult.Fail($"Empty Authentication");

		var existingApiKey = Validator.Validate(providedApiKey);
		if (existingApiKey is null)
			return AuthenticateResult.Fail($"Authentication Failed");

		var claims = await GetClaimsAsync(existingApiKey);
		var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
		var identities = new List<ClaimsIdentity> { identity };
		var principal = new ClaimsPrincipal(identities);
		var ticket = new AuthenticationTicket(principal, Options.Scheme);

		return AuthenticateResult.Success(ticket);
	}

	protected override async Task HandleChallengeAsync(
		AuthenticationProperties properties)
		=> await HandleIssuesAsync(401, "Unauthorized Access");

	protected override async Task HandleForbiddenAsync(
		AuthenticationProperties properties)
		=> await HandleIssuesAsync(403, "Access Denied");

	private async Task HandleIssuesAsync(int code, string message)
	{
		Response.StatusCode = code;
		Response.ContentType = "application/problem+json";
		await Response.WriteAsync(message);
	}
}