using System;

namespace DotNetLibrary.Configuration.Identifier
{
	public class BasicIdentifierFormatter : IIdentifierFormatter
	{
		public string? FormatAsStringIdentifier(object? identifier)
		{
			if (identifier == default || identifier == null)
				return null;

			var stringIdentifier = Convert.ToString(identifier);
			if (string.IsNullOrWhiteSpace(stringIdentifier))
				return null;

			return FormatAsStringIdentifier(stringIdentifier);
		}

		public virtual string FormatAsStringIdentifier(string identifier)
			=> identifier;
	}
}
