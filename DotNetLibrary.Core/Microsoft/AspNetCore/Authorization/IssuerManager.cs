using System.Linq;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Authorization
{
	public class IssuerManager
	{
		public string IssuerName { get; }
		public string NameType { get; }
		public string RoleType { get; }

		internal IssuerManager(
			string issuerName,
			string nameType = ClaimTypes.WindowsAccountName,
			string roleType = ClaimTypes.Role)
		{
			IssuerName = issuerName;
			NameType = nameType;
			RoleType = roleType;
		}

		public Claim CreateClaimRole(string value, string? issuer = null)
			=> new(RoleType, value, null, issuer ?? IssuerName);
		public Claim CreateClaimProperty(string type, string value, string? issuer = null)
			=> new(type, value, null, issuer ?? IssuerName);

		public ClaimsPrincipal CreateClaimsPrincipal(params Claim[] claims)
			=> new(claims
				.GroupBy(p => p.Issuer ?? "", p => p)
				.Select(p => new ClaimsIdentity(p,
					p.Key.Length == 0 ? null : p.Key, NameType, RoleType)));
	}
}
