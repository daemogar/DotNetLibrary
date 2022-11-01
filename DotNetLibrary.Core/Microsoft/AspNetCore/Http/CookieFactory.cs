using Microsoft.JSInterop;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// A factory for creating individual cookies.
/// </summary>
public class CookieFactory : ICookieFactory
{
	private IBasicCookieManager Manager { get; }

	/// <summary>
	/// Constructor for cookie factory
	/// </summary>
	/// <param name="manager">CookieManager</param>
	public CookieFactory(IBasicCookieManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	/// Create the cookie.
	/// </summary>
	/// <param name="cookieKey">The cookie key.</param>
	/// <returns>A basic cookie for getting, setting, and deleting cookie values.</returns>
	public IBasicCookie GetCookie(string cookieKey)
		=> new BasicCookie<IJSRuntime>(Manager, cookieKey);
}
