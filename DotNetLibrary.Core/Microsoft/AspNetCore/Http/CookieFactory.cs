using System.Text.Json;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// A factory for creating individual cookies.
/// </summary>
public class CookieFactory
{
	private BasicCookieManager Manager { get; }

	/// <summary>
	/// Constructor for cookie factory
	/// </summary>
	/// <param name="manager">BasicCookieManager</param>
	public CookieFactory(BasicCookieManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	/// Create the cookie.
	/// </summary>
	/// <typeparam name="T">The <seealso cref="Type" /> the object is that is stored in the cookie. The <seealso cref="JsonSerializer" /> will be used to store and retrieve this value.</typeparam>
	/// <param name="cookieKey">The cookie key.</param>
	/// <returns>A basic cookie for getting, setting, and deleting cookie values.</returns>
	public BasicCookie<T> CreateCookie<T>(string cookieKey)
		=> new(Manager, cookieKey);
}
