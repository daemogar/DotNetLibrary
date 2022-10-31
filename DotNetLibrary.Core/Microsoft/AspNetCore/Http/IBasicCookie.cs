namespace Microsoft.AspNetCore.Http;

public interface IBasicCookie
{
	string CookieKey { get; }
	Task<string> GetAsync();
	Task SetAsync(string value);
	Task SetAsync(string value, CookieOptions options);
	Task DeleteAsync();
	Task DeleteAsync(CookieOptions options);
}
