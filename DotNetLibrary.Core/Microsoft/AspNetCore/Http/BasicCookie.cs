#if !NETSTANDARD2_0_OR_GREATER
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Basic cookie accessor.
/// </summary>
public record class BasicCookie<T>
{
	private CookieOptions? Options { get; }

	/// <inheritdoc cref="BasicCookieManager" />
	protected BasicCookieManager Manager { get; }

	/// <summary>
	/// Cookie key name.
	/// </summary>
	public string CookieKey { get; }

	/// <summary>
	/// Constructor that takes a manager and cookie key.
	/// </summary>
	/// <param name="manager">A cookie manager for accessing the underlying cookies.</param>
	/// <param name="cookieKey">The key of the cookie.</param>
	/// <param name="options"><seealso cref="CookieOptions" /></param>
	public BasicCookie(
		BasicCookieManager manager,
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
	/// Set the cookie value using the manager and the constructor cookie key.
	/// </summary>
	/// <param name="value">Cookie value.</param>
	public async Task SetAsync(T value)
		=> await Manager.AppendResponseCookieAsync(CookieKey,
			value is null ? null! : JsonSerializer.Serialize(value, new JsonSerializerOptions
			{
				ReferenceHandler = ReferenceHandler.IgnoreCycles
			}), Options!);

	/// <summary>
	/// Delete cookie using manager and using the constructor cookie key.
	/// </summary>
	public async Task DeleteAsync()
		=> await Manager.DeleteCookieAsync(CookieKey, Options!);

	/// <summary>
	/// Get cookie from cookie manager using the constructor cookie key.
	/// </summary>
	/// <param name="allowNull">If a null value is allowed or not.</param>
	/// <returns>The string value of the cookie.</returns>
	public async Task<T> GetAsync(bool allowNull = false)
	{
		var value = await Manager.GetRequestCookieAsync(CookieKey, allowNull, Options!);
		return (string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(value))!;
	}
}
#endif