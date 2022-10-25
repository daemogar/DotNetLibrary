using Microsoft.AspNetCore.Authentication.Cookies;

using Serilog;

namespace Microsoft.AspNetCore.Http;

/// <inheritdoc cref="IBasicCookieManager"/>
public class CookieManager : IBasicCookieManager
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

	/// <inheritdoc cref="IBasicCookieManager.AppendResponseCookie(HttpContext, string, string, CookieOptions)"/>
	public void AppendResponseCookie(string key, string? value)
		=> AppendResponseCookie(GetContext(), key, value, new());

	/// <inheritdoc cref="IBasicCookieManager.AppendResponseCookie(HttpContext, string, string, CookieOptions)"/>
	public void AppendResponseCookie(string key, string? value, CookieOptions options)
		=> AppendResponseCookie(GetContext(), key, value, options);

	/// <inheritdoc cref="IBasicCookieManager.AppendResponseCookie(HttpContext, string, string, CookieOptions)"/>
	public void AppendResponseCookie(HttpContext context, string key, string? value)
		=> AppendResponseCookie(context, key, value, new());

	/// <inheritdoc cref="IBasicCookieManager.AppendResponseCookie(HttpContext, string, string, CookieOptions)"/>
	public void AppendResponseCookie(HttpContext context, string key, string? value, CookieOptions options)
	{
		if (context == null)
			return;

		if (value == null)
			DeleteCookie(context, key, options);
		else
			BaseManager.AppendResponseCookie(context, key, value, options);
	}

	/// <inheritdoc cref="IBasicCookieManager.DeleteCookie(HttpContext, string, CookieOptions)"/>
	public void DeleteCookie(string key)
		=> DeleteCookie(GetContext(), key, new());

	/// <inheritdoc cref="IBasicCookieManager.DeleteCookie(HttpContext, string, CookieOptions)"/>
	public void DeleteCookie(string key, CookieOptions options)
		=> DeleteCookie(GetContext(), key, options);

	/// <inheritdoc cref="IBasicCookieManager.DeleteCookie(HttpContext, string, CookieOptions)"/>
	public void DeleteCookie(HttpContext context, string key)
		=> DeleteCookie(context, key, new());

	/// <inheritdoc cref="IBasicCookieManager.DeleteCookie(HttpContext, string, CookieOptions)"/>
	public void DeleteCookie(HttpContext context, string key, CookieOptions options)
	{
		if (context == null)
			return;

		BaseManager.DeleteCookie(context, key, options);
	}

	/// <inheritdoc cref="IBasicCookieManager.GetRequestCookie(HttpContext, string, bool)"/>
	public string? GetRequestCookie(string key)
		=> GetRequestCookie(GetContext(), key, true);

	/// <inheritdoc cref="IBasicCookieManager.GetRequestCookie(HttpContext, string, bool)"/>
	public string GetRequestCookie(string key, bool allowNull)
		=> GetRequestCookie(GetContext(), key, allowNull)!;

	/// <inheritdoc cref="IBasicCookieManager.GetRequestCookie(HttpContext, string, bool)"/>
	public string? GetRequestCookie(HttpContext context, string key)
		=> GetRequestCookie(context, key, true);

	/// <inheritdoc cref="IBasicCookieManager.GetRequestCookie(HttpContext, string, bool)"/>
	public string? GetRequestCookie(HttpContext context, string key, bool allowNull)
	{
		if (context == null)
			return GetValue(key, null, allowNull);
		
		string value;

		try
		{
			Logger.Verbose("Looking Up Cookie: {Key}", key);
			value = BaseManager.GetRequestCookie(context, key);
		}
		catch (Exception e)
		{
			Logger.Error(e,
				"What is going on? Where is the {Cookie} getting eatin?", key);
			throw;
		}

		string? cookie = GetValue(key, value, allowNull);
		Logger.Debug("Retrieve Cookie: {Key} = {Value}", key, cookie);
		return cookie;

		static string? GetValue(
			string key, string? value, bool allowNull)
			=> value
				?? (allowNull
					? null
					: throw new NullCookieValueException(key));
	}
}
