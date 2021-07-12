using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using DotNetLibrary.Configuration.Environment;
using System.Threading;
using DotNetLibrary.Authorization;

namespace DotNetLibrary.Components
{
	public partial class BasicApplication<TLayoutType, TProviderType, TUserType, TRoleType>
		: ComponentBase, IDisposable
		where TLayoutType : LayoutComponentBase
		where TProviderType : BasicAuthenticationStateProvider<TUserType, TRoleType>
		where TUserType : class, IBasicUser<TRoleType>, new()
		where TRoleType : class, IBasicRole
	{
		private readonly CancellationTokenSource Source = new();

		protected RenderFragment Content { get; set; } = null!;

		private IBasicAuthenticationState CurrentAuthenticationState { get; set; } = null!;

		[Inject] public IEnvironmentModel Environment { get; set; }

		[Inject] public TProviderType Provider { get; set; }

		protected override void OnInitialized()
		{
			if (!Environment.IsAuthenticationEnabled)
			{
				Content = BuildRouter;
				return;
			}

			Content = Wrapper;

			Provider.AuthenticationStateChanged += OnAuthenticationStateChanged;
			OnAuthenticationStateChanged(Provider.GetAuthenticationStateAsync(Source.Token));
		}

		private void OnAuthenticationStateChanged(Task<BasicAuthenticationState<TUserType, TRoleType>> newAuthStateTask)
		{
			_ = InvokeAsync(async () =>
			{
				CurrentAuthenticationState = await newAuthStateTask;
				RenderCustom = CurrentAuthenticationState is not null && Environment.AuthComponentType is not null;
				StateHasChanged();
			});
		}

		private void Wrapper(RenderTreeBuilder builder)
		{
			builder.OpenComponent(0, typeof(CascadingAuthenticationState));

			if (RenderCustom)
				BuildCustom(builder);

			BuildRouter(builder);

			if (RenderCustom)
				builder.CloseComponent();

			builder.CloseComponent();
		}

		private bool RenderCustom { get; set; }
		private void BuildCustom(RenderTreeBuilder builder)
		{
			builder.OpenComponent(1, Environment.AuthComponentType);
			builder.AddAttribute(2, nameof(CascadingValue<IBasicAuthenticationState>.Value), CurrentAuthenticationState);
		}

		private void BuildRouter(RenderTreeBuilder builder)
		{
			builder.OpenComponent<BasicRouter>(3);
			builder.AddAttribute(4, nameof(BasicRouter.LayoutType), typeof(TLayoutType));
			builder.CloseComponent();
		}

		public void Dispose()
		{
			if (Provider is not null)
				Provider.AuthenticationStateChanged -= OnAuthenticationStateChanged;

			Source?.Cancel();
			Source?.Dispose();
		}
	}
}