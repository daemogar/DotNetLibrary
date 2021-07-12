using System;

namespace DotNetLibrary.Exceptions
{
	public class UserNullException : Exception
	{
		public string? LookupIdentifier { get; }

		public string? CurrentIdentifier { get; }

		private static string AllowNull(string message) =>
			$"{message} If this is intentional, consider setting the parameter " +
			$"[allowNullUser] to true when getting the user information.";

		public UserNullException()
			: base(AllowNull("Current user is not set and therefore anonymous."))
		{

		}

		public UserNullException(string current)
			: base(AllowNull($"Current user [{current}] is not set and therefore anonymous."))
		{
			CurrentIdentifier = current;
		}

		public UserNullException(string lookup, string current)
			: base(AllowNull($"User [{current}] looking up user [{lookup}] could not be found or was invalid search."))
		{
			LookupIdentifier = lookup;
			CurrentIdentifier = current;
		}
	}
}
