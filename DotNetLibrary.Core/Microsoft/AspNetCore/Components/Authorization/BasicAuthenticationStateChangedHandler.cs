using DotNetLibrary.Authorization;

using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components.Authorization
{
	public delegate void BasicAuthenticationStateChangedHandler<TUserType, TRoleType>(
		Task<BasicAuthenticationState<TUserType, TRoleType>> newStateAsync)
		where TUserType : class, IBasicUser<TRoleType>, new()
		where TRoleType : class, IBasicRole;

}
