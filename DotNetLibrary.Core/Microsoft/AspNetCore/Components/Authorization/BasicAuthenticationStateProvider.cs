using DotNetLibrary.Authorization;

using Serilog;

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components.Authorization
{
	public abstract class BasicAuthenticationStateProvider<TUserType, TRoleType>
		: AuthenticationStateProvider, IBasicAuthenticationStateProvider
		where TUserType : class, IBasicUser<TRoleType>, new()
		where TRoleType : class, IBasicRole
	{
		public new event BasicAuthenticationStateChangedHandler<TUserType, TRoleType> AuthenticationStateChanged
		{
			add => base.AuthenticationStateChanged += Translate(value);
			remove => base.AuthenticationStateChanged -= Translate(value);
		}

		private static AuthenticationStateChangedHandler Translate(
			BasicAuthenticationStateChangedHandler<TUserType, TRoleType> handler)
			=> async stateTask => handler(Task
				.FromResult((BasicAuthenticationState<TUserType, TRoleType>)await stateTask));

		protected ILogger Logger { get; }

		public BasicAuthenticationStateProvider(ILogger logger)
		{
			Logger = logger;
		}

		/// <summary>
		/// <inheritdoc cref="GetAuthenticationStateAsync()"/>
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<BasicAuthenticationState<TUserType, TRoleType>> GetAuthenticationStateAsync(
			CancellationToken cancellationToken)
		{
			var state = await GetStateAsync(cancellationToken);
			NotifyAuthenticationStateChanged(Task.FromResult((AuthenticationState)state));
			return state;
		}
		async Task<IBasicAuthenticationState> IBasicAuthenticationStateProvider.GetAuthenticationStateAsync(CancellationToken cancellationToken)
			=> await GetAuthenticationStateAsync(cancellationToken);

		//async Task<IBasicAuthenticationState> IBasicAuthenticationStateProvider.GetAuthenticationStateAsync(CancellationToken cancellationToken)
		//{
		//	return await GetAuthenticationStateAsync(cancellationToken);
		//}

		/// <summary>
		/// <inheritdoc cref="GetAuthenticationStateAsync()"/>
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		protected abstract Task<BasicAuthenticationState<TUserType, TRoleType>> GetStateAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Return the authorized user after populating from IM DB.
		/// 
		/// There are 3 ways to access this data:
		/// 1. Inside an <AuthorizeView></AuthorizeView> within a 
		///		razor page/component.
		///	2. Using a [CascadingParameter] of type Task&lt;AuthenticationState>.
		///	3. Dependency injecting a AuthenticationStateProvider into 
		///		the page/component or other service.
		/// </summary>
		/// <returns></returns>
		public sealed override async Task<AuthenticationState> GetAuthenticationStateAsync()
			=> await GetAuthenticationStateAsync(default);

		async Task<IBasicAuthenticationState> IBasicAuthenticationStateProvider.GetStateAsync(CancellationToken cancellationToken)
		=> await GetAuthenticationStateAsync(default);
	}
}
