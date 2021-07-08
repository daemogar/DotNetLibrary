using Serilog;

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
#if DEBUG
using System.Text.Json;
#endif

namespace DotNetLibrary.Routing
{
	public static class RoutingExtensions

	{
		private static Assembly[] Assemblies { get; set; } = new[]
		{
			typeof(RoutingExtensions).Assembly
		};

		public static ImmutableArray<Assembly> GetAssemblies()
		{
			Log.Logger
				.ForContext(nameof(Assemblies), Assemblies)
				.Information(
					"Loaded {Count} Routing {Assemblies}",
					Assemblies.Length,
					Assemblies
#if DEBUG
						.Select(p => p.FullName).ToJsonString(true)
#endif
					);

			return Assemblies.ToImmutableArray();
		}

		public static void AddAssemblies(
			params Assembly[] assembliesWithRoutingInformation)
			=> Assemblies = assembliesWithRoutingInformation
				.Union(Assemblies)
				.GroupBy(p => p.GetName(), p => p)
				.Select(p => p.First())
				.ToArray();
	}
}
