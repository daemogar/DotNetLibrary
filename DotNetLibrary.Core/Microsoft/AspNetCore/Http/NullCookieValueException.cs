namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Exception throw when a cookie has a null value and it is not allowed.
/// </summary>
public class NullCookieValueException : Exception
{
	/// <summary>
	/// The cookie key that has the null value.
	/// </summary>
	public string CookieKey { get; }

	/// <summary>
	/// Create a null cookie exception for the given <paramref name="cookieKey"/>
	/// </summary>
	/// <param name="cookieKey">The cookie key that has the null value.</param>
	public NullCookieValueException(string cookieKey)
		: base($"The {cookieKey} cookie was null, and null for this cookie is not allowed.")
	{
		CookieKey = cookieKey;
	}
}