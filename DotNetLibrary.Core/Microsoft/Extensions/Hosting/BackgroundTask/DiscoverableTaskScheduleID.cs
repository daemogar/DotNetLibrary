namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Get and set a task id.
/// </summary>
public class DiscoverableTaskScheduleID
{
	private object? Value { get; set; }

	/// <summary>
	/// Set the task id as a string.
	/// </summary>
	/// <param name="identifier"><inheritdoc cref="SetTaskID{T}(T)" /></param>
	public void SetTaskID(string identifier) => SetTaskID<string>(identifier);

	/// <summary>
	/// Set the task id as an integer.
	/// </summary>
	/// <param name="id"><inheritdoc cref="SetTaskID{T}(T)"/></param>
	public void SetTaskID(int id) => SetTaskID<int>(id);

	/// <summary>
	/// Set the task id as an guid.
	/// </summary>
	/// <param name="guid"><inheritdoc cref="SetTaskID{T}(T)"/></param>
	public void SetTaskID(Guid guid) => SetTaskID<Guid>(guid);
	
	/// <summary>
	/// Set the Task ID to a custom type.
	/// </summary>
	/// <typeparam name="T">The user defined type for use as a task id handler.</typeparam>
	/// <param name="taskID">The identifier of the task.</param>
	public void SetTaskID<T>(T taskID) => Value = taskID;

	/// <summary>
	/// Get the task id.
	/// </summary>
	/// <typeparam name="T">The type of the ID.</typeparam>
	/// <returns>The value of the task id.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the task id has not been set.</exception>
	/// <exception cref="InvalidCastException">Thrown if trying to get the task id as a different type then when it was set.</exception>
	public T GetTaskID<T>()
		=> Value is null
			?	throw new ArgumentNullException(nameof(Value),
				$"Task ID has not been set and cannot be returned.")
			: (Value is T taskID ? taskID : throw new InvalidCastException(
				$"Task ID is of type {Value.GetType().Name} and cannot be returned" +
				$"as type {typeof(T).Name}."));

	/// <summary>
	/// Get the task id as a string.
	/// </summary>
	/// <returns>The string task id.</returns>
	public string GetStringTaskID() => GetTaskID<string>();

	/// <summary>
	/// Get the task id as an integer.
	/// </summary>
	/// <returns>The integer task id.</returns>
	public int GetIntTaskID() => GetTaskID<int>();

	/// <summary>
	/// Get the task id as a guid.
	/// </summary>
	/// <returns>The guid task id.</returns>
	public Guid GetGuidTaskID() => GetTaskID<Guid>();
}
