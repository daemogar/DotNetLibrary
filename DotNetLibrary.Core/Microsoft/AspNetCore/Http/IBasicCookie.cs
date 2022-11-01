namespace Microsoft.AspNetCore.Http;

public interface IBasicCookie
{
	string CookieKey { get; }
	Task<string> GetAsync(bool allowNull = false);
	Task SetAsync(string value);
	Task DeleteAsync();
}
