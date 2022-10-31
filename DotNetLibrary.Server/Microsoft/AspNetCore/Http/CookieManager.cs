using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

using Serilog;

namespace DotNetLibrary.Microsoft.AspNetCore.Http;

/// <inheritdoc cref="IBasicCookieManager{HttpContext}"/>
public class CookieManager : IBasicCookieManager
{
	private ChunkingCookieManager BaseManager { get; } = new();

	private ILogger Logger { get; }

	private HttpContext Context { get; }

	HttpContext IBasicCookieManager<HttpContext>.RuntimeContext => Context;

	/// <summary>
	/// Construct a cookie manager.
	/// </summary>
	/// <param name="logger">Serilog Logger.</param>
	/// <param name="context">IHttpContextAccessor</param>
	public CookieManager(ILogger logger, HttpContext context)
	{
		Logger = logger;
		Context = context;
	}

	public void AppendResponseCookie(HttpContext context, string key, string? value, CookieOptions options)
	{
		if (context == null)
			return;

		if (value == null)
			DeleteCookie(context, key, options);
		else
			BaseManager.AppendResponseCookie(context, key, value, options);
	}

	public void DeleteCookie(HttpContext context, string key, CookieOptions options)
	{
		if (context == null)
			return;

		BaseManager.DeleteCookie(context, key, options);
	}

	public string? GetRequestCookie(HttpContext context, string key, bool allowNull, CookieOptions options)
	{
		if (context == null)
			return GetValue(key, null, allowNull);

		string? value;

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

		string cookie = GetValue(key, value, allowNull);
		Logger.Debug("Retrieve Cookie: {Key} = {Value}", key, cookie);
		return cookie;

		static string GetValue(
			string key, string? value, bool allowNull)
			=> value
				?? (allowNull
					? null!
					: throw new NullCookieValueException(key));
	}

	#region Inherited Members

	/// <inheritdoc cref="AppendResponseCookie(HttpContext, string, string, CookieOptions)"/>
	public async Task AppendResponseCookieAsync(HttpContext context, string key, string value, CookieOptions options)
		 => await Task.Run(() => AppendResponseCookie(context, key, value, options));

	/// <inheritdoc cref="DeleteCookie(HttpContext, string, CookieOptions)"/>
	public async Task DeleteCookieAsync(HttpContext context, string key, CookieOptions options)
		=> await Task.Run(() => DeleteCookie(context, key, options));

	/// <inheritdoc cref="GetRequestCookie(HttpContext, string, bool, CookieOptions)"/>
	public async Task<string> GetRequestCookieAsync(HttpContext context, string key, bool allowNull, CookieOptions options)
		=> await Task.Run(() => GetRequestCookie(context, key, allowNull, options)!);

	/// <inheritdoc cref="GetRequestCookie(HttpContext, string, bool, CookieOptions)"/>
	public string? GetRequestCookie(HttpContext context, string key)
		=> GetRequestCookie(context, key, false, new());

	#endregion
}