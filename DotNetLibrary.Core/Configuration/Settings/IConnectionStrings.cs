namespace DotNetLibrary.Configuration.Settings
{
	public interface IConnectionStrings
	{
		string Get(string name);
	}
}