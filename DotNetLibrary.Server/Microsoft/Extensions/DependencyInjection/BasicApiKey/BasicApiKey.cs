namespace Microsoft.Extensions.DependencyInjection;

public record BasicApiKey(
	int ApiKeyID,
	Guid Value,
	string ClaimsIssuer,
	string ApplicationName
	) : IBasicApiKey
{	
	public string? Controllers { get; init; }

	public string[]? Roles { get; init; }

	public bool Match(string apikey)
		=> Value.ToString().Equals(apikey);

	public bool Match(IBasicApiKey apikey)
		=> Value.Equals(apikey.Value);
}
