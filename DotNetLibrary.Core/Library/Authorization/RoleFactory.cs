using DotNetLibrary.Exceptions;

using Microsoft.AspNetCore.Authorization;

using System;
using System.Security.Claims;

namespace DotNetLibrary.Authorization
{
	public class RoleFactory
	{
		private IssuerManager Issuer { get; }

		public RoleFactory(
			IAuthorizationService service,
			IssuerManager issuer)
		{
			Issuer = issuer;
		}

		public TRoleType Create<TRoleType>(Claim claim, bool allowNullClaim = false)
			where TRoleType : class, IBasicRole
			=> Create<TRoleType>(claim, Issuer.IssuerName, allowNullClaim);
		public TRoleType Create<TRoleType>(Claim claim, string issuer, bool allowNullClaim = false)
				where TRoleType : class, IBasicRole
		{
			if (!claim.Type.Equals(ClaimTypes.Role))
				throw new InvalidRoleTypeClaimException(claim);

			return Create<TRoleType>(claim.Value, issuer, allowNullClaim);
		}

		public TRoleType Create<TRoleType>(IRolename rolename, bool allowNullClaim = false)
				where TRoleType : class, IBasicRole
			=> Create<TRoleType>(rolename, Issuer.IssuerName, allowNullClaim);
		public TRoleType Create<TRoleType>(IRolename rolename, string issuer, bool allowNullClaim = false)
				where TRoleType : class, IBasicRole
			=> Create<TRoleType>(rolename.GetValue(), issuer, allowNullClaim);

		public TRoleType Create<TRoleType>(string value, bool allowNullClaim = false)
				where TRoleType : class, IBasicRole
			=> Create<TRoleType>(value, Issuer.IssuerName, allowNullClaim);
		public TRoleType Create<TRoleType>(string value, string issuer, bool allowNullClaim = false)
				where TRoleType : class, IBasicRole
		{
			if (!allowNullClaim && value == null)
				throw new NullClaimException(ClaimTypes.Role);

			return (TRoleType)Activator.CreateInstance(typeof(TRoleType), new[]
			{
				ClaimTypes.Role,
				value,
				issuer
			})!;
		}
	}
}
