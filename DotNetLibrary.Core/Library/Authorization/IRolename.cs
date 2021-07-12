namespace DotNetLibrary.Authorization
{
	public interface IRolename
	{
		string AccessCode { get; }
		string? AccessKey { get; }
		string GetValue();
	}
}