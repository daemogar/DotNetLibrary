using Microsoft.AspNetCore.Authentication.Cookies;

using Serilog;

using System;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Manager for getting, setting, and deleting cookies.
/// </summary>
public class CookieManager : ICookieManager
{
	private ChunkingCookieManager BaseManager { get; }

	private ILogger Logger { get; }

	private IHttpContextAccessor ContextAccessor { get; }

	private HttpContext GetContext()
		=> ContextAccessor.HttpContext!;

	/// <summary>
	/// Construct a cookie manager.
	/// </summary>
	/// <param name="logger">Serilog Logger.</param>
	/// <param name="context">IHttpContextAccessor</param>
	public CookieManager(ILogger logger, IHttpContextAccessor context)
	{
		Logger = logger;
		ContextAccessor = context;
		BaseManager = new();
	}

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
	public void AppendResponseCookie(string key, string? value)
		=> AppendResponseCookie(GetContext(), key, value, new());

	/// <summary>
	/// <inheritdoc cref="AppendResponseCookie(string, string?)"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="value">The value to set the cookie to or delete if null.</param>
	/// <param name="options">CookieOptions</param>
	public void AppendResponseCookie(string key, string? value, CookieOptions options)
		=> AppendResponseCookie(GetContext(), key, value, options);

	/// <summary>
	/// <inheritdoc cref="AppendResponseCookie(string, string?)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="value">The value to set the cookie to or delete if null.</param>
	public void AppendResponseCookie(HttpContext context, string key, string? value)
		=> AppendResponseCookie(context, key, value, new());

	/// <summary>
	/// <inheritdoc cref="AppendResponseCookie(string, string?)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="value">The value to set the cookie to or delete if null.</param>
	/// <param name="options">CookieOptions</param>
	public void AppendResponseCookie(HttpContext context, string key, string? value, CookieOptions options)
	{
		if (context == null)
			return;

		if (value == null)
			DeleteCookie(context, key, options);
		else
			BaseManager.AppendResponseCookie(context, key, value, options);
	}

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.DeleteCookie"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	public void DeleteCookie(string key)
		=> DeleteCookie(GetContext(), key, new());

	/// <summary>
	/// <inheritdoc cref="DeleteCookie(string)"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="options">CookieOptions</param>
	public void DeleteCookie(string key, CookieOptions options)
		=> DeleteCookie(GetContext(), key, options);

	/// <summary>
	/// <inheritdoc cref="DeleteCookie(string)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	public void DeleteCookie(HttpContext context, string key)
		=> DeleteCookie(context, key, new());

	/// <summary>
	/// <inheritdoc cref="DeleteCookie(string)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="options">CookieOptions</param>
	public void DeleteCookie(HttpContext context, string key, CookieOptions options)
	{
		if (context == null)
			return;

		BaseManager.DeleteCookie(context, key, options);
	}

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.GetRequestCookie(HttpContext, string)"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <returns>String value of cookie or null.</returns>
	public string? GetRequestCookie(string key)
		=> GetRequestCookie(GetContext(), key, true);

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.GetRequestCookie(HttpContext, string)"/>
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="allowNull">Return null if true or throw <seealso cref="NullCookieValueException"/> if false.</param>
	/// <returns>String value of cookie or null</returns>
	/// <exception cref="NullCookieValueException" />
	public string GetRequestCookie(string key, bool allowNull)
		=> GetRequestCookie(GetContext(), key, allowNull)!;

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.GetRequestCookie(HttpContext, string)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	/// <returns>String value of cookie or null</returns>
	public string? GetRequestCookie(HttpContext context, string key)
		=> GetRequestCookie(context, key, true);

	/// <summary>
	/// <inheritdoc cref="ChunkingCookieManager.GetRequestCookie(HttpContext, string)"/>
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="allowNull">Return null if true or throw <seealso cref="NullCookieValueException"/> if false.</param>
	/// <returns>String value of cookie or null</returns>
	/// <exception cref="NullCookieValueException"></exception>
	public string? GetRequestCookie(HttpContext context, string key, bool allowNull)
	{
		if (context == null)
			return GetValue(key, null, allowNull);

		try
		{
			Logger.Verbose("Looking Up Cookie: {Key}", key);
			var value = BaseManager.GetRequestCookie(context, key);
			string? cookie = GetValue(key, value, allowNull);
			Logger.Debug("Retrieve Cookie: {Key} = {Value}", key, cookie);
			return cookie;
		}
		catch (Exception e)
		{
			Logger.Error(e,
				"What is going on? Where is this getting eatin?");
			throw;
		}

		static string? GetValue(
			string key, string? value, bool allowNull)
			=> value
				?? (allowNull
					? null
					: throw new NullCookieValueException(key));
	}
}
