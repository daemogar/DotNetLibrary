using System;

namespace DotNetLibrary.Exceptions
{
	internal class NullCookieValueException : Exception
	{
		public string CookieKey { get; }

		public NullCookieValueException(string cookieKey)
			: base($"The {cookieKey} cookie was null, and null for this cookie is not allowed.")
		{
			CookieKey = cookieKey;
		}
	}
}
