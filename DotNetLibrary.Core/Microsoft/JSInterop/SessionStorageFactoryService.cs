using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.JSInterop;

/// <summary>
/// Factory for creating session storage services. Use this to create
/// an object to get and set data reliably with session storage.
/// </summary>
public class SessionStorageFactoryService : DiscoverableService
{
	private IJSRuntime Runtime { get; }

	/// <summary>
	/// Create a service factory getting 
	/// </summary>
	/// <param name="runtime"></param>
	public SessionStorageFactoryService(IJSRuntime runtime)
	{
		Runtime = runtime;
	}

	/// <summary>
	/// Create a session storage state for the supplied <paramref name="key"/>.
	/// </summary>
	/// <typeparam name="T">Data type of the data stored in </typeparam>
	/// <param name="key">The key value for storing session data.</param>
	/// <returns>A session storage state object for the given <paramref name="key"/>.</returns>
	public SessionStorageState<T> Create<T>(string key) => new(this, key);

	/// <summary>
	/// Get data from session storage using <paramref name="key"/>
	/// with the JSON.parse(data).
	/// </summary>
	/// <typeparam name="T">The type of the data to be saved to session storage.</typeparam>
	/// <param name="key">The key value for storing session data.</param>
	/// <param name="cancellationToken"><inheritdoc cref="CancellationToken"/></param>
	/// <returns>A task representing the status of the request to delete a session storage data.</returns>
	public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken)
		=> await GetAsync<T>(key, default!, cancellationToken);

	/// <summary>
	/// Get data from session storage using <paramref name="key"/> 
	/// with the JSON.parse(data ?? <paramref name="defaultValue"/>).
	/// </summary>
	/// <typeparam name="T">The type of the data to be saved to session storage.</typeparam>
	/// <param name="key">The key value for storing session data.</param>
	/// <param name="defaultValue">A default value to return if nothing stored in session storage.</param>
	/// <param name="cancellationToken"><inheritdoc cref="CancellationToken"/></param>
	/// <returns>A task representing the status of the request to add or update session storage data.</returns>
	public async Task<T> GetAsync<T>(string key, T defaultValue, CancellationToken cancellationToken)
		=> await Runtime.InvokeAsync<T>("window.sessionStorage.getItem", cancellationToken, key)
			?? defaultValue;

	/// <summary>
	/// Set or update data in session storage using <paramref name="key"/> 
	/// with the JSON.stringify(<paramref name="data"/>).
	/// </summary>
	/// <typeparam name="T">The type of the data to be saved to session storage.</typeparam>
	/// <param name="key">The key value for storing session data.</param>
	/// <param name="data">The object of data to be stored. This data will be JSON.stringify(<paramref name="data"/>).</param>
	/// <param name="cancellationToken"><inheritdoc cref="CancellationToken"/></param>
	/// <returns>A task representing the status of the request to add or update session storage data.</returns>
	public async Task SetAsync<T>(string key, T data, CancellationToken cancellationToken)
	{
		if (data is null)
		{
			await DeleteAsync(key, cancellationToken);
			return;
		}

		await Runtime.InvokeVoidAsync("window.sessionStorage.setItem", cancellationToken, key, data);
	}

	/// <summary>
	/// Delete pair from session storage using <paramref name="key"/>.
	/// </summary>
	/// <param name="key">The key value for storing session data.</param>
	/// <param name="cancellationToken"><inheritdoc cref="CancellationToken"/></param>
	/// <returns>A task representing the status of the request to delete a session storage data.</returns>
	public async Task DeleteAsync(string key, CancellationToken cancellationToken)
		=> await Runtime.InvokeVoidAsync("window.sessionStorage.removeItem", cancellationToken, key);

	/// <inheritdoc/>
	protected internal override void ConfigureAsService(
		IServiceCollection services, IConfiguration configuration)
		=> services.AddScoped<SessionStorageFactoryService>();
}
