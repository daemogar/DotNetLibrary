using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OneOf;

using System.Reflection;

using System.Runtime.Versioning;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

internal record DotNetVersionHealthCheck : BasicHealthCheck
{
	private HealthCheckResult? Result { get; set; }

	public DotNetVersionHealthCheck() : base(default!, new[] { "version" }) { }

	protected override async Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context,
		object? data,
		CancellationToken cancellationToken)
	{
		return await Task.FromResult(Result ??= GetState());

		HealthCheckResult GetState()
		{
			var assembly = Assembly.GetEntryAssembly();
			if (assembly is null)
				return FailureState("No entry assembly found.");

			var framework = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
			if (framework is null)
				return FailureState("No target framework attribute found.");

			if (framework.FrameworkDisplayName is null || !decimal
				.TryParse(framework.FrameworkDisplayName.Replace(".NET", "").Trim(), out var version)
				|| version < 5)
				return FailureState($"Unsupported DotNet version [{framework.FrameworkDisplayName}].");

			var released = SecondTuesday(2015 + (int)version, 11);
			if (released.IsT1)
				return FailureState($"Unsupported DotNet release date " +
					$"[{framework.FrameworkDisplayName}, {released.AsT1}].");

			var month = (released.AsT0.Year % 2) == 1 ? 11 : 5;
			var endSupport = SecondTuesday(released.AsT0.Year + 3, month);
			if (endSupport.IsT1)
				return FailureState($"Unsupported DotNet end of support date " +
					$"[{framework.FrameworkDisplayName}, {released.AsT1}, {endSupport.AsT1}].");

			var now = DateOnly.FromDateTime(DateTime.Now);
			var next = released.AsT0.AddYears(1);
			var next2 = released.AsT0.AddYears(2);

			if (now >= released.AsT0 && now < next)
				return HealthCheckResult.Healthy(Message("currently supported"));
			if (now >= next && now < endSupport.AsT0)
				return HealthCheckResult.Healthy(Message("currently supported, but nearing end of support"));
			if (now >= endSupport.AsT0 && now < next2)
				return HealthCheckResult.Degraded(Message("currenlty unsupported and end of life"));
			if (now >= next2)
				return FailureState(Message("currenlty unsupported and end of life, update to latest LTS"));
			if (now < released.AsT0)
				return HealthCheckResult.Degraded(Message("is in pre-release"));

			return FailureState(Message("condition could not be established"));

			string Message(string message) => $"Application framework {message} " +
				$"[{framework.FrameworkDisplayName}, {released.AsT0}, {endSupport.AsT0}].";

			OneOf<DateOnly, string> SecondTuesday(int year, int month)
			{
				if (month == 5)
					year--;

				foreach (var day in Enumerable.Range(8, 7))
				{
					DateOnly date = new(year, month, day);
					if (date.DayOfWeek == DayOfWeek.Tuesday)
						return date;
				}

				return $"{year}/{month}";
			}
		}
	}

	public override void RegisterHealthCheckServices(
		IServiceCollection services, IConfiguration configuration)
	{ }
}