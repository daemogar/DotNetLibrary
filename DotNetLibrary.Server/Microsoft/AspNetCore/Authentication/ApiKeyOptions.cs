namespace Microsoft.AspNetCore.Authentication;

public class ApiKeyOptions : AuthenticationSchemeOptions
{
	public ApiKeyValue ApiKey { get; set; }

	public string QueryStringKey { get; set; } = ApiKeyDefaults.SchemaName;
}
