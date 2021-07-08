namespace DotNetLibrary.Configuration.Identifier
{
	public interface IIdentifierFormatter
	{
		string? FormatAsStringIdentifier(object? identifier);
	}
}
