using Microsoft.AspNetCore.Authentication.Cookies;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Manager for getting, setting, and deleting cookies.
/// </summary>
public interface IBasicCookieManager : ICookieManager
{
	/// <summary>
	/// If the value is null:<br />
	/// <inheritdoc cref="ChunkingCookieManager.DeleteCookie"/>
	/// <br />
	/// <br />
	/// If value is not null:<br />
	/// <inheritdoc cref="ChunkingCookieManager.AppendResponseCookie"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="value">The value to set the cookie to or delete if null.</param>
	public void AppendResponseCookie(string key, string value);

	/// <summary>
	/// <inheritdoc cref="AppendResponseCookie(string, string?)"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="value">The value to set the cookie to or delete if null.</param>
	/// <param name="options">CookieOptions</param>
	public void AppendResponseCookie(string key, string value, CookieOptions options);

	/// <inheritdoc cref="AppendResponseCookie(string, string?)"/>
	/// <summary>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="value">The value to set the cookie to or delete if null.</param>
	public void AppendResponseCookie(HttpContext context, string key, string? value);

	/// <summary>
	/// <inheritdoc cref="AppendResponseCookie(string, string?)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="value">The value to set the cookie to or delete if null.</param>
	/// <param name="options">CookieOptions</param>
	public new void AppendResponseCookie(HttpContext context, string key, string? value, CookieOptions options);

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.DeleteCookie"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	public void DeleteCookie(string key);

	/// <summary>
	/// <inheritdoc cref="DeleteCookie(string)"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="options">CookieOptions</param>
	public void DeleteCookie(string key, CookieOptions options);

	/// <summary>
	/// <inheritdoc cref="DeleteCookie(string)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	public void DeleteCookie(HttpContext context, string key);

	/// <summary>
	/// <inheritdoc cref="DeleteCookie(string)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="options">CookieOptions</param>
	public new void DeleteCookie(HttpContext context, string key, CookieOptions options);

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.GetRequestCookie(HttpContext, string)"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <returns>String value of cookie or null.</returns>
	public string? GetRequestCookie(string key);

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.GetRequestCookie(HttpContext, string)"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="allowNull">Return null if true or throw <seealso cref="NullCookieValueException"/> if false.</param>
	/// <returns>String value of cookie or null</returns>
	/// <exception cref="NullCookieValueException" />
	public string GetRequestCookie(string key, bool allowNull);

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.GetRequestCookie(HttpContext, string)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	/// <returns>String value of cookie or null</returns>
	public new string? GetRequestCookie(HttpContext context, string key);

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.GetRequestCookie(HttpContext, string)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="allowNull">Return null if true or throw <seealso cref="NullCookieValueException"/> if false.</param>
	/// <returns>String value of cookie or null</returns>
	/// <exception cref="NullCookieValueException"></exception>
	public string? GetRequestCookie(HttpContext context, string key, bool allowNull);
}
