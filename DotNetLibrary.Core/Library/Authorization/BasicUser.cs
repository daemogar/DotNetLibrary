using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DotNetLibrary.Authorization
{
	public class BasicUser<TRoleType> : IBasicUser<TRoleType>
		where TRoleType : class, IBasicRole
	{
		public Guid UserUID { get; protected set; }
		public string Username { get; protected set; }
		public string Email { get; protected set; }
		public bool IsAuthenticated => ClaimsPrincipal.Claims.Any();
		public bool IsImpersonated { get; protected set; }

		#region Policies

		protected IAuthorizationService PService { get; private set; }

		internal void SetAuthorizationService(IAuthorizationService service)
			=> PService = service;

		public bool CheckPolicy(string policyName, out AuthorizationFailure failure)
		{
			var response = PService
				.AuthorizeAsync(ClaimsPrincipal, policyName)
				.GetAwaiter()
				.GetResult();

			failure = response.Failure!;

			return response.Succeeded;
		}

		#endregion

		#region Roles

		protected RoleFactory RoleFactory { get; private set; }

		internal void SetRoleFactory(RoleFactory factory)
			=> RoleFactory = factory;

		public IEnumerable<TRoleType> GetAllRoles()
			=> FindRoles(p => true);

		public IEnumerable<TRoleType> FindRoles(string role)
			=> FindRoles(p => p.Equals(role));

		public bool HasRole(Func<TRoleType, bool> match)
			=> FindRoles(match).Any();

		public bool HasRole(string role)
			=> FindRoles(role).Any();

		public IEnumerable<TRoleType> FindRoles(Func<TRoleType, bool> match)
			=> ClaimsPrincipal.Claims
				.Where(p => p.Type.Equals(ClaimTypes.Role))
				.Select(p => RoleFactory.Create<TRoleType>(p))
				.Where(match);

		#endregion

		#region Claims

		protected ClaimsPrincipal ClaimsPrincipal { get; private set; }

		public ClaimsPrincipal AsClaimsPrincipal() => ClaimsPrincipal;

		internal void SetClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
			=> ClaimsPrincipal = claimsPrincipal;

		public IEnumerable<Claim> GetAllClaims(bool includeRoles = false)
			=> ClaimsPrincipal.Claims.Where(p => includeRoles || !p.Type.Equals(ClaimTypes.Role));

		public IEnumerable<Claim> FindClaims(string type)
			=> ClaimsPrincipal.Claims.Where(p => p.Type.Equals(type));

		public IEnumerable<Claim> FindClaims(Func<Claim, bool> match)
			=> GetAllClaims().Where(match);

		public bool HasClaim(Func<Claim, bool> match)
			=> FindClaims(match).Any();

		public bool HasClaim(string type, object value)
		{
			var stringValue = Convert.ToString(value);
			return HasClaim(p => p.Type.Equals(type) && p.Equals(stringValue));
		}

		#endregion
	}
}
