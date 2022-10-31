using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

using Serilog;

using System.Runtime.CompilerServices;

namespace DotNetLibrary.Microsoft.AspNetCore.Http;

/// <inheritdoc cref="IBasicCookieManager{IJSRuntime}"/>
public class CookieManager : IBasicCookieManager
{
	private ILogger Logger { get; }

	private IJSRuntime Runtime { get; }

	IJSRuntime IBasicCookieManager<IJSRuntime>.RuntimeContext => Runtime;

	/// <summary>
	/// Construct a cookie manager.
	/// </summary>
	/// <param name="logger"><inheritdoc cref="Serilog.ILogger"/>.</param>
	/// <param name="runtime"><inheritdoc cref="IJSRuntime"/></param>
	public CookieManager(ILogger logger, IJSRuntime runtime)
	{
		Logger = logger;
		Runtime = runtime;
	}

	/// <inheritdoc cref="IBasicCookieManager{IJSRuntime}.AppendResponseCookieAsync(IJSRuntime, string, string, CookieOptions)"/>
	public async Task AppendResponseCookieAsync(IJSRuntime runtime, string key, string value, CookieOptions options)
	{
		if (runtime == null)
			return;

		if (value == null)
		{
			await DeleteCookieAsync(runtime, key, options);
			return;
		}

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

	/// <inheritdoc cref="IBasicCookieManager{IJSRuntime}.DeleteCookieAsync(IJSRuntime, string, CookieOptions)"/>
	public async Task DeleteCookieAsync(IJSRuntime runtime, string key, CookieOptions options)
	{
		if (runtime == null)
			return;

		var cookie = $"{key}=; Expires={DateTime.MinValue}";
		await runtime.InvokeVoidAsync(key, "DotNetLibrary.Cookies.Set", cookie);
	}

	/// <inheritdoc cref="IBasicCookieManager{IJSRuntime}.GetRequestCookieAsync(IJSRuntime, string, bool, CookieOptions)"/>
	public async Task<string> GetRequestCookieAsync(IJSRuntime runtime, string key, bool allowNull, CookieOptions options)
	{
		if (runtime == null)
			return GetValue(key, null!, allowNull);

		string value;

		try
		{
			Logger.Verbose("Looking Up Cookie: {Key}", key);
			value = await runtime.InvokeAsync<string>(key);
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
			string key, string value, bool allowNull)
			=> value
				?? (allowNull
					? null!
					: throw new NullCookieValueException(key));
	}
}
