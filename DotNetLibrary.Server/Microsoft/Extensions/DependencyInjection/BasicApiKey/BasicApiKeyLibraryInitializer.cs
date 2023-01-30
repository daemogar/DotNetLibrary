namespace Microsoft.Extensions.DependencyInjection;

public abstract class BasicApiKeyLibraryInitializer
	: BasicApiKeyLibraryInitializer<BasicApiKey>
{ }

public abstract class BasicApiKeyLibraryInitializer<TApiKey>
	where TApiKey : IBasicApiKey
{
	public abstract Task<IEnumerable<TApiKey>> ExecuteAsync();
}
