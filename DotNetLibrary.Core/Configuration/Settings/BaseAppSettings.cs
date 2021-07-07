using System;

using DotNetLibrary.Configuration.Environment;

using Microsoft.Extensions.Configuration;

namespace DotNetLibrary.Configuration.Settings
{
	public abstract class BaseAppSettings : IAppSettings
	{
		internal AppSettingsElement Base { get; }

		protected IConfiguration Configuration { get; }

		public IAppSettings.IEmailer Email { get; }

		public BaseAppSettings(IConfiguration configuration, bool isServerSideBlazor)
		{
			Configuration = configuration;
			Base = new AppSettingsElement(Configuration);
			Email = new EmailerSettings(Base);
			
			ApplicationName = Get("ApplicationName", false);

			Environment.SetType(Get("ApplicationEnvironment", false));
			Environment.SetRendered("Server");

			IsServerSideBlazor = isServerSideBlazor;

			if (Environment.IsProduction)
				return;

			DefaultUser = Get("DefaultUser");
		}

		public IEnvironmentModel Environment { get; } = new EnvironmentModel();
		public string ApplicationName { get; }
		public string? DefaultUser { get; }
		public bool IsServerSideBlazor { get; }
		public bool IsMockUser => DefaultUser != null;

		public string Get(string name, bool allowNulls)
			=> Base.Get(name, allowNulls);

		public string? Get(string name)
			=> Base.Get(name);

		public T Get<T>(string name, bool allowNulls = false)
			=> Base.Get<T>(name, allowNulls);

		public T Get<T>(string name, T defaultValue)
			=> Base.Get(name, defaultValue);

		public T Get<T>(string name, Func<T> defaultCallback)
			=> Base.Get(name, defaultCallback);

		public T Get<T>(Type type, string name, Func<T> defaultCallback, bool allowNulls = false)
			=> Base.Get(name, defaultCallback, allowNulls);

	}
}
