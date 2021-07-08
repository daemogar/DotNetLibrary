using CommandLine;

using DotNetLibrary.Configuration;

using Microsoft.Extensions.Configuration;

using Serilog;

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetLibrary
{
	/// <summary>
	/// Program.main class methods. The application's program class should
	/// inherit this class and override any methods appropriately.
	/// </summary>
	public abstract class BasicProgram<T> where T : class, IProgramOptions
	{
		protected abstract void ConfigureProgramOptions(T options);

		/// <summary>
		/// Start the web application with the supplied <paramref name="endPointAppSettings" />.
		/// </summary>
		/// <param name="args">Command line arguments to be passed to <see cref="Host.CreateDefaultBuilder()"/>.</param>
		/// <returns>Task of a running web application.</returns>
		public abstract Task RunAsync(string[] args);

		/// <summary>
		/// Gets a list of assemblies that contain MediatR handlers.
		/// </summary>
		/// <returns>List of assemblies with MediatR handlers.</returns>
		protected void ConfigureHandlerAndRoutingAssemblies(
			BasicProgramOptions options,
			params Assembly[] additionalAssemblies)
		{
			var assemblies = new[] { GetType().Assembly, typeof(BasicProgram<>).Assembly };

			options.AddHandlers(assemblies);
			options.AddHandlers(additionalAssemblies);

			options.AddRoutes(assemblies);
			options.AddRoutes(additionalAssemblies);
		}

		/// <summary>
		/// Optionally override this method to configure the logger manually. The default
		/// is to read from the appsettings configuration for the project.
		/// </summary>
		/// <returns>This should return a manually created <see cref="Serilog"/> Logger or null to use the configuration file to create it.</returns>
		protected virtual ILogger? CreateLogger(IConfiguration simpleConfiguration)
		{
			var config = new LoggerConfiguration()
				.Enrich.With(new BenDemystifyExceptionsSerilogEnricher())
				.ReadFrom.Configuration(simpleConfiguration);

			if (simpleConfiguration.GetValue("ApplicationEnvironment", "").ToUpper().Equals("DEBUG") == true)
				config = config.WriteTo.Console();

			return Log.Logger = config.CreateLogger();
		}

		protected static CommandLineOptions ParseArguments(string[] args)
		{
			var helpWriter = new StringWriter();

			CommandLineOptions options = null!;

			new Parser(p => p.HelpWriter = helpWriter)
				.ParseArguments<CommandLineOptions>(args)
				.WithParsed(p => options = p with
				{
					Args = args
				})
				.WithNotParsed(p
					=> throw new ArgumentException(helpWriter.ToString()));

			return options;
		}
	}
}
