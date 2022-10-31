using Microsoft.JSInterop;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Manager for getting, setting, and deleting cookies.
/// </summary>
public interface IBasicCookieManager<T>
{
	/// <summary>
	/// <seealso cref="IJSRuntime"/> on client and <seealso cref="HttpContext"/> on server.
	/// This value is supplied when none is provided by the caller.
	/// </summary>
	protected T RuntimeContext { get; }

	/// <summary>
	/// Create a cookie.
	/// </summary>
	/// <param name="runtimeContext"><seealso cref="IJSRuntime"/> on client and <seealso cref="HttpContext"/> on server.</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="value">The value to set the cookie to or delete if null.</param>
	/// <param name="options"><inheritdoc cref="CookieOptions"/></param>
	public Task AppendResponseCookieAsync(T runtimeContext, string key, string value, CookieOptions options);

	/// <inheritdoc cref="AppendResponseCookieAsync(T, string, string, CookieOptions)" />
	public async Task AppendResponseCookieAsync(T runtimeContext, string key, string value)
		=> await AppendResponseCookieAsync(runtimeContext, key, value, new());

	/// <inheritdoc cref="AppendResponseCookieAsync(T, string, string, CookieOptions)" />
	public async Task AppendResponseCookieAsync(string key, string value)
		=> await AppendResponseCookieAsync(RuntimeContext, key, value, new());

	/// <inheritdoc cref="AppendResponseCookieAsync(T, string, string, CookieOptions)" />
	public async Task AppendResponseCookieAsync(string key, string value, CookieOptions options)
		=> await AppendResponseCookieAsync(RuntimeContext, key, value, options);

	/// <summary>
	/// Delete a cookie.
	/// </summary>
	/// <param name="runtimeContext"><seealso cref="IJSRuntime"/> on client and <seealso cref="HttpContext"/> on server.</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="options"><inheritdoc cref="CookieOptions"/></param>
	/// <returns></returns>
	public Task DeleteCookieAsync(T runtimeContext, string key, CookieOptions options);

	/// <inheritdoc cref="DeleteCookieAsync(T, string, CookieOptions)" />
	public async Task DeleteCookieAsync(T runtime, string key)
		=> await DeleteCookieAsync(runtime, key, new());

	/// <inheritdoc cref="DeleteCookieAsync(T, string, CookieOptions)" />
	public async Task DeleteCookieAsync(string key)
		=> await DeleteCookieAsync(RuntimeContext, key, new());

	/// <inheritdoc cref="DeleteCookieAsync(T, string, CookieOptions)" />
	public async Task DeleteCookieAsync(string key, CookieOptions options)
		=> await DeleteCookieAsync(RuntimeContext, key, options);

	/// <summary>
	/// Get a cookie value.
	/// </summary>
	/// <param name="runtimeContext"><seealso cref="IJSRuntime"/> on client and <seealso cref="HttpContext"/> on server.</param>
	/// <param name="key">The key of the cookie.</param>
	/// <param name="allowNull">If false then will throw an exception when the value is null. The default should be false.</param>
	/// <param name="options"><inheritdoc cref="CookieOptions"/></param>
	/// <returns>The value of a cookie specified by </returns>
	public Task<string> GetRequestCookieAsync(T runtimeContext, string key, bool allowNull, CookieOptions options);

	/// <inheritdoc cref="GetRequestCookieAsync(T, string, bool, CookieOptions)" />
	public async Task<string> GetRequestCookieAsync(T runtimeContext, string key, bool allowNull)
		=> await GetRequestCookieAsync(runtimeContext, key, allowNull, new());

	/// <inheritdoc cref="GetRequestCookieAsync(T, string, bool, CookieOptions)" />
	public async Task<string> GetRequestCookieAsync(T runtimeContext, string key, CookieOptions options)
		=> await GetRequestCookieAsync(runtimeContext, key, false, options);

	/// <inheritdoc cref="GetRequestCookieAsync(T, string, bool, CookieOptions)" />
	public async Task<string> GetRequestCookieAsync(T runtimeContext, string key)
		=> await GetRequestCookieAsync(runtimeContext, key, false, new());

	/// <inheritdoc cref="GetRequestCookieAsync(T, string, bool, CookieOptions)" />
	public async Task<string> GetRequestCookieAsync(string key, bool allowNull, CookieOptions options)
		=> await GetRequestCookieAsync(RuntimeContext, key, allowNull, options);

	/// <inheritdoc cref="GetRequestCookieAsync(T, string, bool, CookieOptions)" />
	public async Task<string> GetRequestCookieAsync(string key, bool allowNull)
		=> await GetRequestCookieAsync(RuntimeContext, key, allowNull, new());

	/// <inheritdoc cref="GetRequestCookieAsync(T, string, bool, CookieOptions)" />
	public async Task<string> GetRequestCookieAsync(string key, CookieOptions options)
		=> await GetRequestCookieAsync(RuntimeContext, key, false, options);

	/// <inheritdoc cref="GetRequestCookieAsync(T, string, bool, CookieOptions)" />
	public async Task<string> GetRequestCookieAsync(string key)
		=> await GetRequestCookieAsync(RuntimeContext, key, false, new());
}
