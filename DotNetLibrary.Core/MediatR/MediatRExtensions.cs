using DotNetLibrary;

using Microsoft.Extensions.DependencyInjection;

using Serilog;

#if DEBUG
using System.Linq;
using System.Text.Json;
#endif

namespace MediatR
{
	public static class MediatRExtensions
	{
		public static void AddMediatR(
				this IServiceCollection services,
				ILogger logger,
				IProgramOptions options)
				=> services.AddMediatR(options.HandlerAssemblies, p =>
				{
					logger.Verbose("Loading MediatR {Assemblies}", options.HandlerAssemblies
#if DEBUG
					.Select(p => p.FullName).ToJsonString(true)
#endif
					);
				});
	}
}
