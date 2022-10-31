namespace Microsoft.AspNetCore.Http;

public interface ICookieFactory
{
	IBasicCookie GetCookie(string cookieKey);
}
