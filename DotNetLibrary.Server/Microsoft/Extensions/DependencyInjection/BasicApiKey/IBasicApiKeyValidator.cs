namespace Microsoft.Extensions.DependencyInjection;

public interface IBasicApiKeyValidator<TApiKey>
	where TApiKey : IBasicApiKey
{

	TApiKey? Validate(TApiKey apikey);

	TApiKey? Validate(string apikey);
}
