using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using OneOf;

using Serilog;

using System.Reflection;
using System.Runtime.CompilerServices;
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
	/// <param name="services"><inheritdoc cref="HealthCheckServiceCollectionExtensions.AddHealthChecks(IServiceCollection)"/></param>
	/// <param name="options">The options used for configuring the auto discovery of health p.</param>
	/// <param name="assemblies">Assemblies to search for auto discoverable health p.</param>
	/// <returns>The <inheritdoc cref="IHealthChecksBuilder"/> created when adding health p.</returns>
	public static IHealthChecksBuilder AddDiscoverableHealthChecks(
		this IServiceCollection services,
		IConfiguration configuration,
		HealthCheckOptions options = default!,
		Assembly[] assemblies = default!)
		=> services.AddHealthChecks(
			configuration, options ?? new(),
			assemblies ?? Array.Empty<Assembly>());

	/// <summary>
	/// <inheritdoc cref="AddHealthChecks(IServiceCollection, HealthCheckOptions, Assembly[])"/>
	/// </summary>
	/// <param name="services"><inheritdoc cref="AddHealthChecks(IServiceCollection, HealthCheckOptions, Assembly[])"/></param>
	/// <param name="options"><inheritdoc cref="AddHealthChecks(IServiceCollection, HealthCheckOptions, Assembly[])"/></param>
	/// <param name="assembly1">Optional assembly to search for auto discoverable health p.</param>
	/// <param name="assembly2">Optional assembly to search for auto discoverable health p.</param>
	/// <param name="assembly3">Optional assembly to search for auto discoverable health p.</param>
	/// <returns><inheritdoc cref="AddHealthChecks(IServiceCollection, HealthCheckOptions, Assembly[])"/></returns>
	public static IHealthChecksBuilder AddHealthChecks(
		this IServiceCollection services,
		IConfiguration configuration,
		HealthCheckOptions options = default!,
		Assembly assembly1 = default!,
		Assembly assembly2 = default!,
		Assembly assembly3 = default!)
		=> services.AddHealthChecks(configuration, options, new[]
		{
			assembly1, assembly2, assembly3
		});

	/// <summary>
	/// <inheritdoc cref="HealthCheckServiceCollectionExtensions.AddHealthChecks(IServiceCollection)"/>
	/// </summary>
	/// <param name="services"><inheritdoc cref="HealthCheckServiceCollectionExtensions.AddHealthChecks(IServiceCollection)"/></param>
	/// <param name="options">The options used for configuring the auto discovery of health p.</param>
	/// <param name="assemblies">Assemblies to search for auto discoverable health p.</param>
	/// <returns>The <inheritdoc cref="IHealthChecksBuilder"/> created when adding health p.</returns>
	public static IHealthChecksBuilder AddHealthChecks(
		this IServiceCollection services,
		IConfiguration configuration,
		HealthCheckOptions options = default!,
		params Assembly[] assemblies)
	{
		if (Builder is not null)
			return Builder;

		var checks = GetBasicHealthChecks(options, assemblies, new[]
		{
			typeof(ApplicationStartedHealthCheck),
			typeof(DotNetVersionHealthCheck)
		});

		if (options.HealthCheckCount() == 0 && checks.Count <= 2)
			NoHealthChecksConfigured();

		var provider = services.BuildServiceProvider();

		Builder = services.AddHealthChecks();
		options.InvokeHealthChecks(Builder);

		foreach (var check in checks)
			Create(check.GetType())
				.Switch(
					AddHealthCheck,
					p => p.CreateHealthChecksAndValidate().ForEach(AddHealthCheck),
					p =>
					{
						if (p is not null)
							throw p;
					});

		return Builder;

		OneOf<BasicHealthCheck, BasicHealthChecks, Exception?> Create(Type type)
		{
			if (provider.GetService(type) is BasicHealthCheck check)
				return check;

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

			Exception? exception = null;

			foreach (var (_, constructor, parameters) in constructors)
			{
				try
				{
					var args = parameters
						.Select(p => !p.IsOptional
							? provider.GetRequiredService(p.ParameterType)
							: (provider.GetService(p.ParameterType)
									?? p.DefaultValue))
						.ToArray();

					if (args.Length > 0 && args[0] is string)
						args[0] = type.Name.Replace("HealthCheck", "").ToTitleCase();

					var instance = Activator.CreateInstance(type, args);

					if (instance is BasicHealthChecks healthChecks)
						return healthChecks;

					if (instance is BasicHealthCheck healthCheck)
						return healthCheck;
				}
				catch (Exception e)
				{
					exception ??= e;

					Log.Logger.Error(e,
						"Failed Instantiating Health Check {Type} {Parameter Count}",
						type, parameters.Length);
				}
			}

			return exception;
		}

		static void AddHealthCheck(BasicHealthCheck check)
		{
			Builder!.AddCheck(
				check.Name, check,
				check.FailureStatus,
				check.Tags, check.Timeout);

			Log.Logger.Information(
				"Added Health Check {HealthCheckName} {HealthCheck}",
				check.Name, check);
		}

		static void NoHealthChecksConfigured()
		{
			Builder!.AddCheck("Health Check Setup",
				() => HealthCheckResult.Degraded(
					"Application does not have any health checks setup. " +
					"The state of the application cannot be determined " +
					"successfully."));

			Log.Logger.Warning("No Health Checks Setup");
		}

		List<BasicHealthCheck> GetBasicHealthChecks(
			HealthCheckOptions options, Assembly[] assemblies, Type[] types)
			=> options
				.HealthCheckAssemblyReferenceTypes
				.Union(assemblies)
				.Union(Assembly.GetCallingAssembly())
				.Union(Assembly.GetExecutingAssembly())
				.Union(Assembly.GetEntryAssembly())
				.Where(p => p is not null)
				.SelectMany(p => p!.GetTypes())
				.Union(options.GetType())
				.Union(types)
				.Where(p =>
				{
					if (!p.IsClass || p.IsAbstract)
						return false;

					if (!p.IsSubclassOf(typeof(BasicHealthCheck)))
						return false;

					var attribute = p.GetCustomAttribute<IgnoreHealthCheckAttribute>();
					return attribute is null || attribute.Conditional();
				})
				.DistinctBy(p => p.Name)
				.Select(p =>
				{
					var check = (BasicHealthCheck)RuntimeHelpers.GetUninitializedObject(p);
					check.RegisterHealthCheckServices(services, configuration);
					return check;
				})
				.ToList();
	}

#if !NETSTANDARD2_0_OR_GREATER
	/// <summary>
	/// Map health check endpoints. There are four endpoints:
	/// <list check="number">
	/// <item>
	///		<path>/heartbeat/</path>
	///		<description>Runs all health p and ruturns the basic information of <seealso cref="HealthStatus.Healthy"/>, <seealso cref="HealthStatus.Degraded"/>, or <seealso cref="HealthStatus.Unhealthy"/></description>
	/// </item>
	/// <item>
	///		<path>/heartbeat/live/</path>
	///		<description>Ignores all health p and returns <seealso cref="HealthStatus.Healthy"/> if the application is running.</description>
	/// </item>
	/// <item>
	///		<path>/heatbeat/ready/</path>
	///		<description>Checks only the health p that are tagged as "ready".</description>
	/// </item>
	/// <item>
	///		<path>/heatbeat/details/</path>
	///		<description>Runs all health p and returns a detailed response with each check including status and messages.</description>
	/// </item>
	/// </list>
	/// </summary>
	/// <param name="application">The <seealso cref="WebApplication"/>.</param>
	/// <param name="basePath">The root or base path for health check endpoints.</param>
	public static void MapHealthCheckEndpoints(
		this WebApplication application,
		string basePath = "/heartbeat/")
	{
		if (HealthCheckMapped)
			return;

		HealthCheckMapped = true;

		basePath = $"/{basePath}/".Replace("//", "/");

		// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-p?view=aspnetcore-6.0

		application.MapHealthChecks(basePath);
		application.MapHealthChecks($"{basePath}live", new()
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

		Log.Logger.Information("Health Check Endpoints Mapped {Path}", basePath);
	}

	private static bool HealthCheckMapped { get; set; }

	private static async Task ResponseWriter(
		HttpContext context, HealthReport report)
	{
		context.Response.ContentType = "application/json; charset=utf-8";

		var options = new JsonWriterOptions { Indented = true };

		using var stream = new MemoryStream();
		using var writer = new Utf8JsonWriter(stream, options);

		writer.WriteStartObject();
		Write(report);
		writer.WriteEndObject();

		CancellationTokenSource source = new();
		source.CancelAfter(TimeSpan.FromSeconds(2));

		await writer.FlushAsync(source.Token);
		await context.Response.WriteAsync(
			Encoding.UTF8.GetString(stream.ToArray()), source.Token);

		void Write(HealthReport report)
		{
			writer.WriteString("status", report.Status.ToString());
			writer.WriteString("duration", report.TotalDuration.ToString("G"));

			if (!report.Entries.Any())
				return;

			writer.WriteStartArray("results");
			foreach (var entry in report.Entries)
			{
				writer.WriteStartObject();
				WriteEntry(entry.Key, entry.Value);
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
		}

		void WriteEntry(string name, HealthReportEntry entry)
		{
			writer.WriteString("name", name);
			writer.WriteString("status", entry.Status.ToString());
			writer.WriteString("description", Filter(entry.Description));
			writer.WriteString("duration", entry.Duration.ToString("G"));


			if (entry.Tags.Any()) WriteArray("tags", entry.Tags);
			WriteException(entry.Exception);

			var list = entry.Data
				.Where(p => !p.Key.StartsWith($"{nameof(Exception)}."))
				.ToDictionary(p => p.Key, p => p.Value);

			if (!list.Any())
				return;

			writer.WriteStartObject("data");
			foreach (var item in entry.Data)
			{
				writer.WritePropertyName(item.Key);

				JsonSerializer.Serialize(writer, item.Value,
					item.Value?.GetType() ?? typeof(object));
			}
			writer.WriteEndObject();

			void WriteException(Exception? e)
			{
				string message;
				string? stackTrace;

				if (e is null)
				{
					if (entry.Data is null)
						return;

					if (!entry.Data.TryGetValue($"{nameof(Exception)}.{nameof(Exception.Message)}", out var value) || value is null)
						return;

					message = value.ToString()!;
					if (message is null)
						return;

					entry.Data.TryGetValue($"{nameof(Exception)}.{nameof(Exception.StackTrace)}", out value);
					stackTrace = value?.ToString();
				}
				else
				{
					while (e.InnerException is not null)
						e = e.InnerException;

					message = e.Message;
					stackTrace = e.StackTrace;
				}

				writer.WriteStartObject("exception");

				if (message.Contains(Environment.NewLine))
					writer.WriteString("message", Filter(message));
				else
					WriteArray("messages", message.Split(Environment.NewLine));

				if (stackTrace is not null)
					WriteArray("trace", stackTrace.Split(Environment.NewLine));

				writer.WriteEndObject();
			}

			void WriteArray(string propertyName, IEnumerable<string> lines)
			{
				writer.WriteStartArray(propertyName);
				foreach (var line in lines)
					writer.WriteStringValue(Filter(line));
				writer.WriteEndArray();
			}
		}

		string? Filter(string? text)
			=> text?.Replace("'", "`").Replace('\'', '`');
	}
#endif
}
