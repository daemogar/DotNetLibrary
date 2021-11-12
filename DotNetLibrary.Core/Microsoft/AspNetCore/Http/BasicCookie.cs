namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Basic cookie accessor.
/// </summary>
public record class BasicCookie
{
	private CookieManager Manager { get; }

	/// <summary>
	/// Cookie key name.
	/// </summary>
	public string CookieKey { get; }

	/// <summary>
	/// Constructor that takes a manager and cookie key.
	/// </summary>
	/// <param name="manager">A cookie manager for accessing the underlying cookies.</param>
	/// <param name="cookieKey">The key of the cookie.</param>
	public BasicCookie(CookieManager manager, string cookieKey)
	{
		Manager = manager;
		CookieKey = cookieKey;
	}

	/// <summary>
	/// Get cookie from cookie manager using the constructor cookie key.
	/// </summary>
	/// <returns>The string value of the cookie.</returns>
	public string? Get() => Manager.GetRequestCookie(CookieKey);

	/// <summary>
	/// Set the cookie value using the manager and the constructor cookie key.
	/// </summary>
	/// <param name="value">Cookie value.</param>
	public void Set(string value) => Set(value, new());

	/// <summary>
	/// Set the cookie value using the manager and the constructor cookie key.
	/// </summary>
	/// <param name="value">Cookie value.</param>
	/// <param name="options">CookieOptions</param>
	public void Set(string value, CookieOptions options)
		=> Manager.AppendResponseCookie(CookieKey, value, options);

	/// <summary>
	/// Delete cookie using manager and using the constructor cookie key.
	/// </summary>
	public void Delete() => Delete(new());

	/// <summary>
	/// Delete cookie using manager and using the constructor cookie key.
	/// </summary>
	/// <param name="options">CookieOptions</param>
	public void Delete(CookieOptions options)
		=> Manager.DeleteCookie(CookieKey, options);
}
