namespace DotNetLibrary.Authorization
{
	public interface IBasicRole
	{
		IRolename Rolename { get; }
		string Issuer { get; }
		bool Equals(IRolename rolename);
		bool Equals(string rolename);
	}
}