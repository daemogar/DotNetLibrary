using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.JSInterop;

using Serilog;

using System.Runtime.CompilerServices;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Manager for getting, setting, and deleting cookies.
/// </summary>
public abstract class BasicCookieManager
{
	private ChunkingCookieManager BaseManager { get; }

	/// <inheritdoc cref="IJSRuntime" />
	private IJSRuntime Runtime { get; } = default!;

	/// <inheritdoc cref="HttpContext" />
	private HttpContext Context { get; } = default!;

	/// <inheritdoc cref="ILogger"/>
	protected ILogger Logger { get; }

	/// <summary>
	/// Returns the type of the underlying cookie management.
	/// Either <seealso cref="IJSRuntime" /> 
	/// or <seealso cref="HttpContext" /> 
	/// and <seealso cref="ChunkingCookieManager" />.
	/// </summary>
	public string RuntimeContextType { get; }

	/// <summary>
	/// Create the underlying parent cookie manager.
	/// </summary>
	/// <param name="logger"><inheritdoc cref="ILogger"/>.</param>
	/// <param name="type">The name of either <seealso cref="IJSRuntime" /> or <seealso cref="HttpContext" /></param>
	/// <param name="runtime"><inheritdoc cref="IJSRuntime"/></param>
	/// <param name="context"><inheritdoc cref="HttpContext"/></param>
	/// <param name="manager"><inheritdoc cref="ChunkingCookieManager"/></param>
	internal protected BasicCookieManager(
		ILogger logger, string type,
		IJSRuntime runtime,
		HttpContext context, ChunkingCookieManager manager)
	{
		RuntimeContextType = type;
		Logger = logger;
		Runtime = runtime;
		Context = context;
		BaseManager = manager;
	}

	/// <summary>
	/// Create a cookie.
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="value">The value to set the cookie to or delete if null.</param>
	/// <param name="options"><inheritdoc cref="CookieOptions"/></param>
	public async Task AppendResponseCookieAsync(
	string key, string value, CookieOptions options = default!)
	{
		if (Context is not null)
			await AppendResponseCookieAsync(Context, key, value, options);
		else
		if (Runtime is not null)
			await AppendResponseCookieAsync(Runtime, key, value, options);
		else
			throw new NullReferenceException(
				$"The RuntimeContext does not match either the " +
				$"{nameof(Runtime)} or {nameof(Context)} and cannot be null.");
	}

	/// <inheritdoc cref="AppendResponseCookieAsync(string, string, CookieOptions)" />
	public async Task AppendResponseCookieAsync(
		IJSRuntime runtime, string key, string value, CookieOptions options = default!)
	{
		if (value is null)
		{
			await DeleteCookieAsync(runtime, key, options);
			return;
		}

		options ??= new();

		var cookie = string
			.Join("; ", new[] {
				CookieProperty(true, key, value),
				CookieProperty(null, options.Domain),
				CookieProperty(true, options.Path),
				CookieExpiration(),
				CookieBoolean(options.Secure),
				CookieBoolean(options.HttpOnly),
				CookieProperty(true, options.SameSite)
			}
			.Where(p => p is not null));

		await runtime.InvokeVoidAsync(key, "DotNetLibrary.Cookies.Set", cookie);

		string? CookieExpiration()
			=> options.MaxAge is not null
				? CookieProperty(true, options.MaxAge)
				: CookieProperty(true, options.Expires ?? DateTime.MinValue, "Expires");

		static string? CookieBoolean(
			bool value,
			[CallerArgumentExpression("value")] string name = default!)
			=> CookieProperty(value ? false : null, value, name);

		static string? CookieProperty(
			bool? show, object? value,
			[CallerArgumentExpression("value")] string name = default!)
			=> show is not null || value is not null
				? $"{name}{(show != false ? $"={value}" : "")}"
				: null;
	}

	/// <inheritdoc cref="AppendResponseCookieAsync(string, string, CookieOptions)" />
	public async Task AppendResponseCookieAsync(
			HttpContext context, string key, string value, CookieOptions options = default!)
	{
		if (value is null)
			await DeleteCookieAsync(context, key, options);
		else
			BaseManager.AppendResponseCookie(context, key, value, options ?? new());
	}

	/// <summary>
	/// Delete a cookie.
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="options"><inheritdoc cref="CookieOptions"/></param>
	public async Task DeleteCookieAsync(string key, CookieOptions options = default!)
	{
		if (Context is not null)
			await DeleteCookieAsync(Context, key, options);
		else
		if (Runtime is not null)
			await DeleteCookieAsync(Runtime, key, options);
		else
			throw new NullReferenceException(
				$"The RuntimeContext does not match either the " +
				$"{nameof(Runtime)} or {nameof(Context)} and cannot be null.");
	}

	/// <inheritdoc cref="DeleteCookieAsync(string, CookieOptions)" />
	public async Task DeleteCookieAsync(IJSRuntime runtime, string key, CookieOptions _ = default!)
	{
		var cookie = $"{key}=; Expires={DateTime.MinValue}";
		await runtime.InvokeVoidAsync(key, "DotNetLibrary.Cookies.Set", cookie);
	}

	/// <inheritdoc cref="DeleteCookieAsync(string, CookieOptions)" />
	public async Task DeleteCookieAsync(HttpContext context, string key, CookieOptions options = default!)
		=> await Task.Run(() => BaseManager.DeleteCookie(context, key, options ?? new()));

	/// <summary>
	/// Get a cookie.
	/// </summary>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="allowNull">If false then will throw an exception when the value is null. The default should be false.</param>
	/// <param name="options"><inheritdoc cref="CookieOptions"/></param>
	/// <returns>The value of a cookie specified by </returns>
	public async Task<string> GetRequestCookieAsync(
		string key, bool allowNull = false, CookieOptions options = default!)
	{
		if (Context is not null)
			return await GetRequestCookieAsync(Context, key, allowNull, options);

		if (Runtime is not null)
			return await GetRequestCookieAsync(Runtime, key, allowNull, options);

		throw new NullReferenceException(
			$"The RuntimeContext does not match either the " +
			$"{nameof(Runtime)} or {nameof(Context)} and cannot be null.");
	}

	/// <inheritdoc cref="GetRequestCookieAsync(string, bool, CookieOptions)" />
	public async Task<string> GetRequestCookieAsync(
		IJSRuntime runtime, string key, bool allowNull = false, CookieOptions _ = default!)
		=> await GetRequestCookieAsync(key, allowNull, async ()
			=> await runtime.InvokeAsync<string>(key));

	/// <inheritdoc cref="GetRequestCookieAsync(string, bool, CookieOptions)" />
	public async Task<string> GetRequestCookieAsync(
		HttpContext context, string key, bool allowNull = false, CookieOptions _ = default!)
		=> await GetRequestCookieAsync(key, allowNull, async ()
			=> await Task.FromResult(BaseManager.GetRequestCookie(context, key)));

	private async Task<string> GetRequestCookieAsync(
		string key, bool allowNull, Func<Task<string>> callback)
	{
		string? value;

		try
		{
			Logger.Verbose("Looking Up Cookie: {Key}", key);
			value = await callback();
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
	}

	private static string GetValue(
		string key, string? value, bool allowNull)
		=> value
			?? (allowNull
				? null!
				: throw new NullCookieValueException(key));
}