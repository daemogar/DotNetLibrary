using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore.Authentication;

public class ApiKeyOptions : AuthenticationSchemeOptions
{
	/// <summary>
	/// The apikey to use with the options.
	/// </summary>
	public ApiKeyValue ApiKey { get; set; } = default!;

	public string QueryStringKey { get; set; } = ApiKeyDefaults.SchemaName;
}
