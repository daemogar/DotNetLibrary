namespace Microsoft.Extensions.DependencyInjection;

public class BasicApiKeyValidator<TApiKey> : IBasicApiKeyValidator<TApiKey>
	where TApiKey : IBasicApiKey
{
	protected IEnumerable<TApiKey> ApiKeys { get; private set; } = default!;

	internal void SetApiKeys(IEnumerable<TApiKey> apikeys)
	{
		ApiKeys = apikeys;
	}

	public virtual TApiKey? Validate(TApiKey apikey)
		=> ApiKeys.FirstOrDefault(p => p.Match(apikey));

	public virtual TApiKey? Validate(string apikey)
		=> ApiKeys.FirstOrDefault(p => p.Match(apikey));
}
