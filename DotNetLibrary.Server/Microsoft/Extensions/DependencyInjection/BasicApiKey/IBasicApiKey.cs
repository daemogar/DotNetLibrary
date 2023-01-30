namespace Microsoft.Extensions.DependencyInjection;

public interface IBasicApiKey
{
	int ApiKeyID { get; }

	Guid Value { get; }

	string ApplicationName { get; }

	string? Controllers { get; }

	string[]? Roles { get; }

	bool Match(string apikey);

	bool Match(IBasicApiKey apikey);
}
