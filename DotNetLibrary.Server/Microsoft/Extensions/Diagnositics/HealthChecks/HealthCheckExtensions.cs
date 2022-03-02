using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using Serilog;

using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <inheritdoc cref="HealthCheckServiceCollectionExtensions.AddHealthChecks(IServiceCollection)"/>
/// These extenion methos will also auto discover any
/// classes that inherit from <seealso cref="BasicHealthCheck"/>.
/// </summary>
public static class HealthCheckExtensions
{
	private static IHealthChecksBuilder? Builder { get; set; }

	/// <summary>
	/// <inheritdoc cref="AddHealthChecks(IServiceCollection, HealthCheckOptions, Assembly[])"/>
	/// </summary>
	/// <param name="services"><inheritdoc cref="AddHealthChecks(IServiceCollection, HealthCheckOptions, Assembly[])"/></param>
	/// <param name="options"><inheritdoc cref="AddHealthChecks(IServiceCollection, HealthCheckOptions, Assembly[])"/></param>
	/// <param name="assembly1">Optional assembly to search for auto discoverable health checks.</param>
	/// <param name="assembly2">Optional assembly to search for auto discoverable health checks.</param>
	/// <param name="assembly3">Optional assembly to search for auto discoverable health checks.</param>
	/// <returns><inheritdoc cref="AddHealthChecks(IServiceCollection, HealthCheckOptions, Assembly[])"/></returns>
	public static IHealthChecksBuilder AddHealthChecks(
		this IServiceCollection services,
		HealthCheckOptions options,
		Assembly assembly1 = default!,
		Assembly assembly2 = default!,
		Assembly assembly3 = default!)
		=> AddHealthChecks(services, options, new[]
		{
			assembly1, assembly2, assembly3
		});

	/// <summary>
	/// <inheritdoc cref="HealthCheckServiceCollectionExtensions.AddHealthChecks(IServiceCollection)"/>
	/// </summary>
	/// <param name="services"><inheritdoc cref="HealthCheckServiceCollectionExtensions.AddHealthChecks(IServiceCollection)"/></param>
	/// <param name="options">The options used for configuring the auto discovery of health checks.</param>
	/// <param name="assemblies">Assemblies to search for auto discoverable health checks.</param>
	/// <returns>The <inheritdoc cref="IHealthChecksBuilder"/> created when adding health checks.</returns>
	public static IHealthChecksBuilder AddHealthChecks(
		this IServiceCollection services,
		HealthCheckOptions options,
		params Assembly[] assemblies)
	{
		if (Builder is not null)
			return Builder;

		services.AddHostedService<ApplicationStartedBackgroundService>();
		services.AddSingleton<ApplicationStartedHealthCheck>();

		Builder = services
			.AddHealthChecks()
			.AddCheck<ApplicationStartedHealthCheck>(
				"Application Started",
				HealthStatus.Unhealthy,
				new[] { "ready" },
				TimeSpan.FromMinutes(2));

		var types = GetTypes();

		if (options.HealthCheckCount() == 0 && !types.Any())
			options.HealthChecks += NoHealthChecksConfigured;
		else
			options.InvokeHealthChecks(Builder);

		var provider = services.BuildServiceProvider();
		foreach (var type in types)
			CreateHealthCheck(type);

		return Builder;

		void CreateHealthCheck(Type type)
		{
			var healthCheck = provider.Create<BasicHealthCheck>(type);
			healthCheck.Name
				??= type.Name.Replace("HealthCheck", "").ToTitleCase();

			Builder!.AddCheck(
				healthCheck.Name, healthCheck,
				healthCheck.FailureStatus,
				healthCheck.Tags, healthCheck.Timeout);
		}

		void NoHealthChecksConfigured(IHealthChecksBuilder builder)
		{
			builder.AddCheck("Health Check Setup",
				() => HealthCheckResult.Degraded(
					"Application does not have any health checks setup. " +
					"The state of the application cannot be determined " +
					"successfully."));
		}

		List<Type> GetTypes()
		{
			var types = options
				.HealthCheckAssemblyReferenceTypes
				.Union(assemblies.Where(p => p is not null))
				.Union(Assembly.GetCallingAssembly())
				.Union(Assembly.GetExecutingAssembly())
				.SelectMany(p => p.GetTypes())
				.Union(options.GetType())
				.ToList();

			var entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly is not null)
				types.AddRange(entryAssembly.GetTypes());

			return types
				.Where(p => p.IsClass && !p.IsAbstract
					&& p.IsSubclassOf(typeof(BasicHealthCheck))
					&& p.GetCustomAttribute<IgnoreHealthCheckAttribute>() is null)
				.Distinct()
				.ToList();
		}
	}

	internal static T Create<T>(this IServiceProvider services, Type type)
	{
		var constructors = type
			.GetConstructors()
			.Select(p =>
			{
				var parameters = p.GetParameters();
				return (
					parameters.Length,
					Constructor: p,
					parameters
				);
			})
			.OrderByDescending(p => p.Length);

		foreach (var (_, constructor, parameters) in constructors)
		{
			try
			{
				var args = parameters
					.Select(p => !p.IsOptional
						? services.GetRequiredService(p.ParameterType)
						: (services.GetService(p.ParameterType)
								?? p.DefaultValue))
					.ToArray();

				if (Activator.CreateInstance(type, args) is T healthCheck)
					return healthCheck;
			}
			catch (Exception e)
			{
				Log.Logger.Error(e,
					"Failed Instantiating Health Check {Type} {Parameter Count}",
					type, parameters.Length);
			}
		}

		return default!;
	}

	/// <summary>
	/// Map health check endpoints. There are four endpoints:
	/// <list type="number">
	/// <item>
	///		<path>/heartbeat/</path>
	///		<description>Runs all health checks and ruturns the basic information of <seealso cref="HealthStatus.Healthy"/>, <seealso cref="HealthStatus.Degraded"/>, or <seealso cref="HealthStatus.Unhealthy"/></description>
	/// </item>
	/// <item>
	///		<path>/heartbeat/live/</path>
	///		<description>Ignores all health checks and returns <seealso cref="HealthStatus.Healthy"/> if the application is running.</description>
	/// </item>
	/// <item>
	///		<path>/heatbeat/ready/</path>
	///		<description>Checks only the health checks that are tagged as "ready".</description>
	/// </item>
	/// <item>
	///		<path>/heatbeat/details/</path>
	///		<description>Runs all health checks and returns a detailed response with each check including status and messages.</description>
	/// </item>
	/// </list>
	/// </summary>
	/// <param name="application">The <seealso cref="WebApplication"/>.</param>
	/// <param name="basePath">The root or base path for health check endpoints.</param>
	public static void MapHealthChecks(
		this WebApplication application,
		string basePath = "/heartbeat/")
	{
		basePath = $"/{basePath}/".Replace("//", "/");
		
		// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0

		application.MapHealthChecks(basePath);
		application.MapHealthChecks($"/{basePath}live", new()
		{
			Predicate = _ => false
		});
		application.MapHealthChecks($"{basePath}ready", new()
		{
			Predicate = p => p.Tags.Contains("ready")
		});
		application.MapHealthChecks($"{basePath}details", new()
		{
			ResponseWriter = ResponseWriter
		});
	}

	private static Task ResponseWriter(
		HttpContext context, HealthReport report)
	{
		context.Response.ContentType = "application/json; charset=utf-8";

		var options = new JsonWriterOptions { Indented = true };

		using var stream = new MemoryStream();
		using var writer = new Utf8JsonWriter(stream, options);

		writer.WriteStartObject();
		Write(report);
		writer.WriteEndObject();

		return context.Response.WriteAsync(
			Encoding.UTF8.GetString(stream.ToArray()));

		void Write(HealthReport report)
		{
			writer.WriteString("status", report.Status.ToString());
			writer.WriteString("duration", report.TotalDuration.ToString("G"));

			if (!report.Entries.Any())
				return;

			writer.WriteStartObject("results");
			foreach (var entry in report.Entries)
			{
				writer.WriteStartObject(entry.Key);
				NewMethod(entry.Key, entry.Value);
				writer.WriteEndObject();
			}
			writer.WriteEndObject();
		}

		void NewMethod(string key, HealthReportEntry entry)
		{
			writer.WriteString("status", entry.Status.ToString());
			writer.WriteString("description", entry.Description);
			writer.WriteString("duration", entry.Duration.ToString("G"));

			WriteTags(entry.Tags);
			WriteException(entry.Exception);

			if (!entry.Data.Any())
				return;

			writer.WriteStartObject("data");
			foreach (var item in entry.Data)
			{
				writer.WritePropertyName(item.Key);

				JsonSerializer.Serialize(writer, item.Value,
						item.Value?.GetType() ?? typeof(object));
			}
			writer.WriteEndObject();
		}

		void WriteTags(IEnumerable<string> tags)
		{
			if (!tags.Any())
				return;

			writer.WriteStartArray("tags");
			foreach (var tag in tags)
				writer.WriteStringValue(tag);
			writer.WriteEndArray();
		}

		void WriteException(Exception? e)
		{
			if (e is null)
				return;

			while (e.InnerException is not null)
				e = e.InnerException;

			writer.WriteStartObject("exception");
			writer.WriteString("message", e.Message);
			writer.WriteString("trace", e.StackTrace);
			writer.WriteEndObject();
		}
	}
}
