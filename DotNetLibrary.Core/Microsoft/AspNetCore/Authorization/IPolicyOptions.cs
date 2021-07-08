using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Authorization
{
	public interface IPolicyOptions
	{
		IReadOnlyList<Action<AuthorizationOptions>> Policies { get; }
	}
}