using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Reflection;

using DotNetLibrary.Routing;

namespace DotNetLibrary
{
	public abstract class BasicProgramOptions : IProgramOptions
	{
		public virtual string? IssuerName { get; init; }

		public virtual bool AuthenticationEnabled { get; init; } = true;

		public IEnumerable<Assembly> HandlerAssemblies { get; } = new List<Assembly>();

		public bool UseMediatR { get; init; } = true;
		
		public string? BaseAddress { get; init; }

		public BasicProgramOptions()
		{
			((List<Assembly>)HandlerAssemblies).Add(GetType().Assembly);
		}

		public void AddHandlers(params Assembly[] handlers)
			=> ((List<Assembly>)HandlerAssemblies).AddRange(handlers);

		public void AddRoutes(params Assembly[] routes)
			=> RoutingExtensions.AddAssemblies(routes);

		public IReadOnlyList<Action<AuthorizationOptions>> Policies { get; }
			= new List<Action<AuthorizationOptions>>();
	}
}
