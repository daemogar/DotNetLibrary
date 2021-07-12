
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components.Authorization
{
	public interface IBasicAuthenticationStateProvider
	{
		event AuthenticationStateChangedHandler AuthenticationStateChanged;
		Task<IBasicAuthenticationState> GetAuthenticationStateAsync(CancellationToken cancellationToken);
		Task<IBasicAuthenticationState> GetStateAsync(CancellationToken cancellationToken);
		Task<AuthenticationState> GetAuthenticationStateAsync();
	}
}
