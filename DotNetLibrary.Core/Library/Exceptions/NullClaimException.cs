using System;

namespace DotNetLibrary.Exceptions
{
	public class NullClaimException : Exception
	{
		public string Type { get; }
		public string? Value { get; }

		public NullClaimException(string type)
			: base($"Null claim of type [{type}] not allowed.")
		{
			Type = type;
		}

		public NullClaimException(string type, string value)
			: base($"The claim [{value}] of type [{type}] not allowed.")
		{
			Type = type;
			Value = value;
		}
	}
}
