using DotNetLibrary.Authorization;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using Serilog;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNetLibrary.Configuration.Menu
{
	public abstract class BasicHtmlMenuService<TProviderType, TUserType, TRoleType, TMenuType>
		: IHtmlMenuService, IDisposable
		where TProviderType : BasicAuthenticationStateProvider<TUserType, TRoleType>
		where TUserType : class, IBasicUser<TRoleType>, new()
		where TRoleType : class, IBasicRole
	{
		public event Action? StateChanged;

		private ILogger Logger { get; }

		protected HttpClient HttpClient { get; }

		protected BasicAuthenticationStateProvider<TUserType, TRoleType> Provider { get; } = null!;

		private string Username { get; set; } = "";

		public bool HasMenu { get; private set; }

		protected BasicHtmlMenuService(
			ILogger logger,
			HttpClient client,
			TProviderType provider)
		{
			Logger = logger;
			HttpClient = client;

			if (provider == null)
				return;

			Provider = provider;

			provider.AuthenticationStateChanged += GetMenuAsync;
		}
		
		public MarkupString Markup { get; private set; } = new();

		public async Task Refresh() => await GetMenuAsync(Username);
				
		protected abstract MarkupString Format(TMenuType menu);

		protected abstract Task<TMenuType?> GetMenuAsync(string username);
		
		private async void GetMenuAsync(Task<BasicAuthenticationState<TUserType, TRoleType>> state)
		{
			Username = (await state).CurrentUser?.Username!;
			Logger.Debug("Getting Menu for {User}", Username);

			var value = await GetMenuAsync(Username);

			if (value == null)
			{
				Markup = new("");
				HasMenu = false;
			}
			else
			{
				Markup = Format(value);
				HasMenu = true;
			}

			StateChanged?.Invoke();
		}

		public void Dispose()
		{
			if (Provider != null)
				Provider.AuthenticationStateChanged -= GetMenuAsync;

			GC.SuppressFinalize(this);
		}
	}
}
