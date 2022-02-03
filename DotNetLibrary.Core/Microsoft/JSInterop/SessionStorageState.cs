namespace Microsoft.JSInterop;

/// <summary>
/// Session storage stage object for managing gettin, adding, updating, and 
/// deleting data from session storage.
/// </summary>
/// <typeparam name="T">The type of data stored in session storage.</typeparam>
public class SessionStorageState<T>
{
	private SessionStorageFactoryService Service { get; }

	/// <summary>
	/// The key used for this session storage state.
	/// </summary>
	public string Key { get; }

	/// <summary>
	/// Create a session storage state for a specific <paramref name="key"/> and 
	/// using the <paramref name="service"/> factory.
	/// </summary>
	/// <param name="service">Factory for manageing the session storage operations.</param>
	/// <param name="key">The key used in session storage.</param>
	public SessionStorageState(SessionStorageFactoryService service, string key)
	{
		Service = service;
		Key = key;
	}

	/// <summary>
	/// Get data from session storage.
	/// </summary>
	/// <param name="cancellationToken"><seealso cref="CancellationToken"/></param>
	/// <returns>A task getting the session storage and returning the data.</returns>
	public async Task<T> GetAsync(CancellationToken cancellationToken)
		=> await Service.GetAsync<T>(Key, cancellationToken);

	/// <summary>
	/// Add or update data from session storage.
	/// </summary>
	/// <param name="data">Data to be stored in session storage.</param>
	/// <param name="cancellationToken"><seealso cref="CancellationToken"/></param>
	/// <returns>A task adding or updating the session storage data.</returns>
	public async Task SetAsync(T data, CancellationToken cancellationToken)
		=> await Service.SetAsync(Key, data, cancellationToken);

	/// <summary>
	/// Delete data from session storage.
	/// </summary>
	/// <param name="cancellationToken"><seealso cref="CancellationToken"/></param>
	/// <returns>A task deleting the session storage data.</returns>
	public async Task DeleteAsync(CancellationToken cancellationToken)
		=> await Service.DeleteAsync(Key, cancellationToken);
}
