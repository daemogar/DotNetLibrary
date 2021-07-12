using DotNetLibrary.Authorization;
using DotNetLibrary.Exceptions;

namespace Microsoft.AspNetCore.Components.Authorization
{
	public abstract class BasicAuthenticationState<TUserType, TRoleType>
		: AuthenticationState
		where TUserType : class, IBasicUser<TRoleType>, new()
		where TRoleType : class, IBasicRole
	{
		private readonly TUserType currentUser;
		private bool IsCurrentUserSet { get; init; }

		public TUserType CurrentUser
		{
			get => IsCurrentUserSet
				? currentUser
				: throw new UserNullException();
			private init {
				currentUser = value;
				IsCurrentUserSet = true;
			}
		}

		public TUserType OriginalUser { get; } = null!;
		public bool IsImpersonating { get; }

		public BasicAuthenticationState() : base(new()) { CurrentUser = null!; }
		public BasicAuthenticationState(TUserType currentUser)
			: this(currentUser, currentUser)
		{
			IsImpersonating = false;
		}
		public BasicAuthenticationState(TUserType originalUser, TUserType currentUser)
			: base(currentUser.AsClaimsPrincipal())
		{
			IsImpersonating = true;

			OriginalUser = originalUser;
			CurrentUser = currentUser;
		}
	}
}
