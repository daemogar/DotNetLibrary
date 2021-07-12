using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNetLibrary.Authorization
{
	public interface IBasicUser<TRoleType>
		where TRoleType : IBasicRole
	{
		Guid UserUID { get; }
		string Username { get; }
		string Email { get; }
		bool IsAuthenticated { get; }
		bool IsImpersonated { get; }
		ClaimsPrincipal AsClaimsPrincipal();
		IEnumerable<TRoleType> GetAllRoles();
		IEnumerable<Claim> GetAllClaims(bool includeRoles = false);
		IEnumerable<TRoleType> FindRoles(Func<TRoleType, bool> match);
		IEnumerable<TRoleType> FindRoles(string type);
		IEnumerable<Claim> FindClaims(Func<Claim, bool> match);
		IEnumerable<Claim> FindClaims(string type);
		bool CheckPolicy(string policyName, out AuthorizationFailure failure);
		bool HasRole(Func<TRoleType, bool> match);
		bool HasRole(string type);
		bool HasClaim(Func<Claim, bool> match);
		bool HasClaim(string type, object value);
	}
}