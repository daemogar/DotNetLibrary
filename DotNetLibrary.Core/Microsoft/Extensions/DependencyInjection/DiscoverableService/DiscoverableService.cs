using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Abstract implimentation of the discoverable service. Overriding
/// this class should be provided as a application service.
/// </summary>
public abstract class DiscoverableService : IDiscoverable
{
	private bool IsDiscovered { get; set; }

	/// <inheritdoc cref="IDiscoverable.Order" />
	protected internal virtual int Order { get; } = 0;

	int IDiscoverable.Order => Order;

	/// <inheritdoc cref="IDiscoverable.ConfigureAsService(IServiceCollection, IConfiguration)" />
	protected internal abstract void ConfigureAsService(
		IServiceCollection services, IConfiguration configuration);

	void IDiscoverable.ConfigureAsService(
		IServiceCollection services, IConfiguration configuration)
	{
		if (IsDiscovered)
			return;

		IsDiscovered = true;

		ConfigureAsService(services, configuration);
	}
}
