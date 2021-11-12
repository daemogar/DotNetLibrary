namespace Microsoft.AspNetCore.Http;

/// <summary>
/// A factory for creating individual cookies.
/// </summary>
public class CookieFactory
{
	private CookieManager Manager { get; }

	/// <summary>
	/// Constructor for cookie factory
	/// </summary>
	/// <param name="manager">CookieManager</param>
	public CookieFactory(CookieManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	/// Create the cookie.
	/// </summary>
	/// <param name="cookieKey">The cookie key.</param>
	/// <returns>A basic cookie for getting, setting, and deleting cookie values.</returns>
	public BasicCookie GetCookie(string cookieKey)
		=> new(Manager, cookieKey);
}
