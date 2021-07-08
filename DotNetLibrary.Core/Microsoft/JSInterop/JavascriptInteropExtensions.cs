using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using DotNetLibrary.Configuration.Environment;

namespace Microsoft.JSInterop
{
	public static class JavascriptInteropExtensions
	{
		public static async Task<IEnvironmentModel> LoadEnvironmentAsync(
			this IJSRuntime javascript)
			=> await javascript.LoadFromWindowAsync<EnvironmentModel>(
				JavascriptMethodNames.EnvironmentMethod);

		public static async Task<IEnumerable<Claim>> LoadUserClaimsAsync(
			this IJSRuntime javascript)
			=> (await javascript.LoadFromWindowAsync<List<SimpleClaim>>(
				JavascriptMethodNames.UserClaimsMethod))
				?.Select(p => new Claim(p.Type, p.Value, null, p.Issuer))
				?? new List<Claim>();

		public static async Task<T> LoadFromWindowAsync<T>(
			this IJSRuntime javascript,
			JavascriptMethodNames method,
			params object[] parameters)
			=> (await javascript
				.InvokeAsync<string>(method.ToString("G"), parameters))
				.FromBase64ByteEncodedString<T>();
	}
}
