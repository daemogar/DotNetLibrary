using System.Security.Claims;

namespace DotNetLibrary.Authorization
{
	public class BasicRole : Claim, IBasicRole
	{
		public IRolename Rolename { get; protected set; }

		public string Issuer { get; protected set; }

		public BasicRole(string type, string value, string issuer)
			: base(type, value, null, issuer)
		{
			Rolename = new Rolename(value);
		}

		public bool Equals(string rolename)
			=> Rolename.ToString()?.Equals(rolename) == true;

		public bool Equals(IRolename rolename)
			=> Rolename.ToString()?.Equals(rolename.ToString()) == true;
	}
}
