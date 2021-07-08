using Microsoft.AspNetCore.Authorization;

using System.Collections.Generic;
using System.Reflection;

namespace DotNetLibrary
{
	public interface IProgramOptions : IPolicyOptions
	{
		string? IssuerName { get; }
		IEnumerable<Assembly> HandlerAssemblies { get; }
		void AddHandlers(params Assembly[] assemblies);
		void AddRoutes(params Assembly[] assemblies);
		string? BaseAddress { get; }
		bool UseMediatR { get; }
	}
}