#if !NETSTANDARD2_0_OR_GREATER
namespace Microsoft.AspNetCore.Components;

/// <summary>
/// Update component base. This can be used to replace the
/// <see cref="ComponentBase"/> of a razor file. When using
/// <code>@inhert <see cref="UpdateComponent{TComponent}"/></code>
/// in a razor file, the <typeparamref name="TComponent"/> 
/// will be injected into the component and using the 
/// <see cref="ComponentBase.OnInitialized"/> method, hook
/// into the event Action callback of the component.
/// When disposed, the hook will be removed by calling the 
/// <see cref="IDisposable.Dispose"/> method.
/// </summary>
/// <typeparam name="TComponent">An injectable service that implments the <seealso cref="IUpdateNotifier"/>.</typeparam>
public abstract class UpdateComponent<TComponent>
	: ComponentBase, IDisposable
	where TComponent : IUpdateNotifier
{
	/// <summary>
	/// The component associate with the razor file. This
	/// is likley a state object used between multiple 
	/// razor components and handles changes made to the state.
	/// </summary>
	[Inject]
	protected TComponent Component { get; set; } = default!;

	/// <seealso cref="ComponentBase.OnInitialized"/>
	protected override void OnInitialized()
	{
		Component.OnUpdate += OnStateChanged;
	}

	private void OnStateChanged()
	{
		Update();
		StateHasChanged();
	}

	/// <inheritdoc cref="UpdateComponent{TComponent, TIn, TOut}.Update(TIn)"/>
	public virtual void Update() { }

	/// <seealso cref="IDisposable.Dispose"/>
	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
		Component.OnUpdate -= OnStateChanged;
	}
}

/// <summary>
/// Update component base. This can be used to replace the
/// <see cref="ComponentBase"/> of a razor file. When using
/// <code>@inhert <see cref="UpdateComponent{TComponent, TIn}"/></code>
/// in a razor file, the <typeparamref name="TComponent"/> 
/// will be injected into the component and using the 
/// <see cref="ComponentBase.OnInitialized"/> method, hook
/// into the event Action callback of the component.
/// When disposed, the hook will be removed by calling the 
/// <see cref="IDisposable.Dispose"/> method.
/// </summary>
/// <typeparam name="TComponent">An injectable service that implments the <seealso cref="IUpdateNotifier{TIn}"/>.</typeparam>
/// <typeparam name="TIn">The user defined type passed into the <see cref="Update(TIn)"/> method.</typeparam>
public abstract class UpdateComponent<TComponent, TIn>
	: ComponentBase, IDisposable
	where TComponent : IUpdateNotifier<TIn>
{
	/// <summary>
	/// The component associate with the razor file. This
	/// is likley a state object used between multiple 
	/// razor components and handles changes made to the state.
	/// </summary>
	[Inject]
	protected TComponent Component { get; set; } = default!;

	/// <seealso cref="ComponentBase.OnInitialized"/>
	protected override void OnInitialized()
	{
		Component.OnUpdate += OnStateChanged;
	}

	private void OnStateChanged(TIn value)
	{
		Update(value);
		StateHasChanged();
	}

	/// <inheritdoc cref="UpdateComponent{TComponent, TIn, TOut}.Update(TIn)"/>
	public virtual void Update(TIn value) { }

	/// <seealso cref="IDisposable.Dispose"/>
	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
		Component.OnUpdate -= OnStateChanged;
	}
}

/// <summary>
/// Update component base. This can be used to replace the
/// <see cref="ComponentBase"/> of a razor file. When using
/// <code>@inhert <see cref="UpdateComponent{TComponent, TIn, TOut}"/></code>
/// in a razor file, the <typeparamref name="TComponent"/> 
/// will be injected into the component and using the 
/// <see cref="ComponentBase.OnInitialized"/> method, hook
/// into the event Action callback of the component.
/// When disposed, the hook will be removed by calling the 
/// <see cref="IDisposable.Dispose"/> method.
/// </summary>
/// <typeparam name="TComponent">An injectable service that implments the <seealso cref="IUpdateNotifier{TIn}"/>.</typeparam>
/// <typeparam name="TIn">The user defined type passed into the <see cref="Update(TIn)"/> method.</typeparam>
/// <typeparam name="TOut">The user defined type returned back from a call to the <see cref="Update(TIn)"/> method.</typeparam>
public abstract class UpdateComponent<TComponent, TIn, TOut>
	: ComponentBase, IDisposable
	where TComponent : IUpdateNotifier<TIn, TOut>
{
	/// <summary>
	/// The component associate with the razor file. This
	/// is likley a state object used between multiple 
	/// razor components and handles changes made to the state.
	/// </summary>
	[Inject]
	protected TComponent Component { get; set; } = default!;

	/// <seealso cref="ComponentBase.OnInitialized"/>
	protected override void OnInitialized()
	{
		Component.OnUpdate += OnStateChanged;
	}

	private TOut OnStateChanged(TIn value)
	{
		var result = Update(value);
		StateHasChanged();
		return result;
	}

	/// <summary>
	/// Method triggered when an update event happens.
	/// </summary>
	/// <param name="value">User defined response value.</param>
	/// <returns>User defined response object.</returns>
	public abstract TOut Update(TIn value);

	/// <seealso cref="IDisposable.Dispose"/>
	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
		Component.OnUpdate -= OnStateChanged;
	}
}
#endif