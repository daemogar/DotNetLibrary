namespace Microsoft.AspNetCore.Authentication;

public class BasicApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
	public const string DefaultScheme = "ApiKey";

	public string Scheme => DefaultScheme;

	public string AuthenticationType = DefaultScheme;
}
