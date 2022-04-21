namespace Microsoft.AspNetCore.Components;

/// <summary>
/// Interface defining a property for triggering user defined 
/// events before the <see cref="ComponentBase.StateHasChanged"/>
/// method.
/// </summary>
public interface IUpdateNotifier
{
	/// <summary>
	/// Custom events called before 
	/// <see cref="ComponentBase.StateHasChanged"/>
	/// has been called.
	/// </summary>
	event Action OnUpdate;
}

/// <inheritdoc cref="IUpdateNotifier"/>
/// <typeparam name="TIn">User defined value passed in during updates.</typeparam>
public interface IUpdateNotifier<TIn>
{
	/// <inheritdoc cref="IUpdateNotifier"/>
	event Action<TIn> OnUpdate;
}

/// <inheritdoc cref="IUpdateNotifier"/>
/// <typeparam name="TIn">User defined value passed in during updates.</typeparam>
/// <typeparam name="TOut">User defined type returned from updates.</typeparam>
public interface IUpdateNotifier<TIn, TOut>
{
	/// <inheritdoc cref="IUpdateNotifier"/>
	event Func<TIn, TOut> OnUpdate;
}
