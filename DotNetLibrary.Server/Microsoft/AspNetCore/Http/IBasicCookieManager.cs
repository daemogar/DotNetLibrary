using Microsoft.AspNetCore.Authentication.Cookies;

namespace Microsoft.AspNetCore.Http;

public interface IBasicCookieManager : ICookieManager, IBasicCookieManager<HttpContext> {

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.GetRequestCookie(HttpContext, string)"/>
	/// </summary>
	/// <param name="context"><seealso cref="HttpContext"/></param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="allowNull">If false then will throw an exception when the value is null. The default should be false.</param>
	/// <param name="options"><inheritdoc cref="CookieOptions"/></param>
	/// <returns></returns>
	public string? GetRequestCookie(HttpContext context, string key, bool allowNull, CookieOptions options);

	/// <inheritdoc cref="GetRequestCookie(HttpContext, string, bool, CookieOptions)"/>
	public string? GetRequestCookie(HttpContext context, string key, bool allowNull)
		=> GetRequestCookie(context, key, allowNull, new());

	/// <inheritdoc cref="GetRequestCookie(HttpContext, string, bool, CookieOptions)"/>
	public string? GetRequestCookie(HttpContext context, string key, CookieOptions options)
		=> GetRequestCookie(context, key, false, options);
}