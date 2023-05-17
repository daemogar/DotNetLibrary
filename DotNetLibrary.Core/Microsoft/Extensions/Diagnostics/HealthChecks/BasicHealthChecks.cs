namespace Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Helper to creating a
/// <inheritdoc cref="BasicHealthCheck"/>
/// </summary>
public abstract record BasicHealthChecks : BasicHealthCheck
{
	/// <inheritdoc cref="BasicHealthCheck()"/>
	protected BasicHealthChecks() { }

	/// <inheritdoc cref="BasicHealthCheck(string[])"/>
	protected BasicHealthChecks(string[] tags) : base(tags) { }

	/// <inheritdoc cref="BasicHealthCheck(string, string[])"/>
	protected BasicHealthChecks(string name, string[] tags) : base(name, tags) { }

	/// <summary>
	/// Create multiple health checks based on the base object.
	/// </summary>
	/// <returns>A list of health checks based on the base object.</returns>
	protected abstract IEnumerable<BasicHealthCheck> CreateHealthChecks();

	/// <summary>
	/// Returns a list of health checks configured for the overriding type.
	/// By default this just returns the target object as a single instance
	/// created. But this can be overridden to return any number of health
	/// checks. If this is the desire, consider calling 
	/// <seealso cref="BasicHealthChecks"/>.
	/// </summary>
	/// <returns>An enumerable list of health checks to be added to the list of checks. the default is to return the target object itself. Consider <seealso cref="BasicHealthChecks"/> if wanting to return more than one.</returns>
	/// <exception cref="Exception">If the default health check method is not overridden then an exception is thrown.</exception>
	public IEnumerable<BasicHealthCheck> CreateHealthChecksAndValidate()
	{
		foreach (var check in CreateHealthChecks())
		{
			check.PostCreateHealthCheckProcessing();

			if (check.IsDefaultFuncHealthCheckAsync)
				throw new Exception(
					$"Health checks created in during the call to " +
					$"{nameof(CreateHealthChecks)} must set the " +
					$"{nameof(HealthCheckAsync)} method.");

			yield return check;
		}
	}
}
