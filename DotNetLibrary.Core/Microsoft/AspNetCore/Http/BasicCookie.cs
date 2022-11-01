using System.Diagnostics;

namespace Microsoft.AspNetCore.Http;
/// <summary>
/// Basic cookie accessor.
/// </summary>
public record class BasicCookie<T>: IBasicCookie
{
	/// <inheritdoc cref="IBasicCookieManager" />
	protected IBasicCookieManager Manager { get; }

	/// <summary>
	/// Cookie key name.
	/// </summary>
	public string CookieKey { get; }

	private CookieOptions? Options { get; }

	/// <summary>
	/// Constructor that takes a manager and cookie key.
	/// </summary>
	/// <param name="manager">A cookie manager for accessing the underlying cookies.</param>
	/// <param name="cookieKey">The key of the cookie.</param>
	/// <param name="options"><seealso cref="CookieOptions" /></param>
	public BasicCookie(
		IBasicCookieManager manager,
		string cookieKey,
		CookieOptions? options = default!)
	{
		Debug.Assert(manager is not null);
		Debug.Assert(cookieKey is not null);

		Manager = manager;
		CookieKey = cookieKey!;
		Options = options;
	}

	/// <summary>
	/// Get cookie from cookie manager using the constructor cookie key.
	/// </summary>
	/// <param name="allowNull">If a null value is allowed or not.</param>
	/// <returns>The string value of the cookie.</returns>
	public async Task<string> GetAsync(bool allowNull = false)
		=> await Manager.GetRequestCookieAsync(CookieKey, allowNull, Options!);

	/// <summary>
	/// Set the cookie value using the manager and the constructor cookie key.
	/// </summary>
	/// <param name="value">Cookie value.</param>
	public async Task SetAsync(string value)
		=> await Manager.AppendResponseCookieAsync(CookieKey, value, Options!);

	/// <summary>
	/// Delete cookie using manager and using the constructor cookie key.
	/// </summary>
	public async Task DeleteAsync()
		=> await Manager.DeleteCookieAsync(CookieKey, Options!);
}
