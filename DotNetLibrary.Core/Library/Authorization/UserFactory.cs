
using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace DotNetLibrary.Authorization
{
	public class UserFactory
	{
		private IAuthorizationService Service { get; }

		private IssuerManager Issuer { get; }
		public RoleFactory Factory { get; }
		private Func<IBasicUser<IBasicRole>, Func<Claim, Claim>> Properties { get; set; }

		public UserFactory(
			IAuthorizationService service,
			IssuerManager issuer,
			RoleFactory factory)
		{
			Service = service;
			Issuer = issuer;
			Factory = factory;

			var dictionary = GetType()
					.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.Where(p => p.CanWrite)
					.ToDictionary(
						p => p.Name.ToUpper(),
						p => new Action<IBasicUser<IBasicRole>, Claim>((q, r) =>
						{
							object value = r.Value;
							if (p.PropertyType == typeof(Guid))
								value = Guid.Parse(r.Value);
							else if (p.PropertyType == typeof(int?))
								value = int.TryParse(r.Value, out var number)
									? number
									: (int?)null!;

							p.SetValue(q, value);
						}));

			Properties = p => new Func<Claim, Claim>(q =>
			{
				var type = q.Type.ToUpper();

				if (dictionary.ContainsKey(type))
					dictionary[type](p, q);

				return q;
			});
		}

		public TUserType Create<TUserType>(IEnumerable<Claim> claims)
			where TUserType : BasicUser<IBasicRole>, new()
		{
			TUserType user = new();

			var setProperty = Properties(user);

			user.SetRoleFactory(Factory);
			user.SetAuthorizationService(Service);
			user.SetClaimsPrincipal(Issuer.CreateClaimsPrincipal(
				claims.Select(setProperty)));

			return user;
		}
	}
}
