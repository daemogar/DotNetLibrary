using Microsoft.AspNetCore.Authorization;

using Serilog;

using System;
using System.Collections.Generic;
using System.Reflection;

using DotNetLibrary.Routing;

namespace DotNetLibrary
{
	public abstract class BasicProgramOptions : IProgramOptions
	{
		public abstract string? IssuerName { get; }
	
		protected ILogger Logger { get; }

		public IEnumerable<Assembly> HandlerAssemblies { get; } = new List<Assembly>();
		
		public bool UseMediatR { get; set; } = true;

		public string? BaseAddress { get; set; }

		public BasicProgramOptions(ILogger logger)
		{
			Logger = logger;
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
