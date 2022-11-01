using System.Diagnostics;

namespace Microsoft.AspNetCore.Http;
/// <summary>
/// Basic cookie accessor.
/// </summary>
public record class BasicCookie<T>: IBasicCookie
{
	/// <summary>
	/// The underlying Cookie Manager.
	/// </summary>
	protected IBasicCookieManager Manager { get; }

	/// <summary>
	/// Cookie key name.
	/// </summary>
	public string CookieKey { get; }

	/// <summary>
	/// Constructor that takes a manager and cookie key.
	/// </summary>
	/// <param name="manager">A cookie manager for accessing the underlying cookies.</param>
	/// <param name="cookieKey">The key of the cookie.</param>
	public BasicCookie(IBasicCookieManager manager, string cookieKey)
	{
		Debug.Assert(manager is not null);
		Debug.Assert(cookieKey is not null);

		Manager = manager;
		CookieKey = cookieKey!;
	}

	/// <summary>
	/// Get cookie from cookie manager using the constructor cookie key.
	/// </summary>
	/// <returns>The string value of the cookie.</returns>
	public async Task<string> GetAsync()
		=> await Manager.GetRequestCookieAsync(CookieKey);

	/// <summary>
	/// Set the cookie value using the manager and the constructor cookie key.
	/// </summary>
	/// <param name="value">Cookie value.</param>
	public async Task SetAsync(string value)
		=> await SetAsync(value, new());

	/// <summary>
	/// Set the cookie value using the manager and the constructor cookie key.
	/// </summary>
	/// <param name="value">Cookie value.</param>
	/// <param name="options">CookieOptions</param>
	public async Task SetAsync(string value, CookieOptions options)
		=> await Manager.AppendResponseCookieAsync(CookieKey, value, options);

	/// <summary>
	/// Delete cookie using manager and using the constructor cookie key.
	/// </summary>
	public async Task DeleteAsync() => await DeleteAsync(new());

	/// <summary>
	/// Delete cookie using manager and using the constructor cookie key.
	/// </summary>
	/// <param name="options">CookieOptions</param>
	public async Task DeleteAsync(CookieOptions options)
		=> await Manager.DeleteCookieAsync(CookieKey, options);
}
