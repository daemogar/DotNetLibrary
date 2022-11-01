using Microsoft.JSInterop;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Manager for getting, setting, and deleting cookies.
/// </summary>
public interface IBasicCookieManager
{
	/// <summary>
	/// Create a cookie.
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="value">The value to set the cookie to or delete if null.</param>
	/// <param name="options"><inheritdoc cref="CookieOptions"/></param>
	public Task AppendResponseCookieAsync(string key, string value, CookieOptions options = default!);

	/// <inheritdoc cref="AppendResponseCookieAsync(string, string, CookieOptions)" />
	public Task AppendResponseCookieAsync(IJSRuntime runtime, string key, string value, CookieOptions options = default!);

	/// <inheritdoc cref="AppendResponseCookieAsync(string, string, CookieOptions)" />
	public Task AppendResponseCookieAsync(HttpContext context, string key, string value, CookieOptions options = default!);

	/// <summary>
	/// Delete a cookie.
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="options"><inheritdoc cref="CookieOptions"/></param>
	public Task DeleteCookieAsync(string key, CookieOptions options = default!);

	/// <inheritdoc cref="DeleteCookieAsync(string, CookieOptions)" />
	public Task DeleteCookieAsync(IJSRuntime runtime, string key, CookieOptions options = default!);

	/// <inheritdoc cref="DeleteCookieAsync(string, CookieOptions)" />
	public Task DeleteCookieAsync(HttpContext Context, string key, CookieOptions options = default!);

	/// <summary>
	/// Get a cookie.
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="allowNull">If false then will throw an exception when the value is null. The default should be false.</param>
	/// <param name="options"><inheritdoc cref="CookieOptions"/></param>
	/// <returns>The value of a cookie specified by </returns>
	public Task<string> GetRequestCookieAsync(string key, bool allowNull = false, CookieOptions options = default!);

	/// <inheritdoc cref="GetRequestCookieAsync(string, bool, CookieOptions)" />
	public Task<string> GetRequestCookieAsync(IJSRuntime runtime, string key, bool allowNull = false, CookieOptions options = default!);

	/// <inheritdoc cref="GetRequestCookieAsync(string, bool, CookieOptions)" />
	public Task<string> GetRequestCookieAsync(HttpContext context, string key, bool allowNull = false, CookieOptions options = default!);
}
