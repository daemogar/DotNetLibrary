using System;

namespace DotNetLibrary.Exceptions
{
	public class NullClaimsException : Exception
	{
		public object? UserIdentifier { get; }

		public NullClaimsException(object? userIdentifier)
			: base($"The user identifier [{userIdentifier}] does not have any claims.")
		{
			UserIdentifier = userIdentifier;
		}
	}
}
