using System;
using System.Text.Json;

using DotNetLibrary.Exceptions;

using Microsoft.Extensions.Configuration;

namespace DotNetLibrary.Configuration.Settings
{
	internal class AppSettingsElement
	{
		private class InternalValue {
			public string Value { get; } = null!;
			public IConfigurationSection Section { get; } = null!;

			public bool IsNull { get; }

			public InternalValue(IConfiguration configuration, string name)
				: this(configuration, name, false) { }
			public InternalValue(IConfiguration configuration, string name, bool allowNulls)
			{
				Value = configuration[name];
				Section = configuration.GetSection(name);

				IsNull = Value is null && Section is null;

				if (!IsNull || allowNulls)
					return;

				throw new AppSettingNotConfiguredException(name);
			}
		}

		private IConfiguration Configuration { get; }

		public AppSettingsElement(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public string Get(string name, bool allowNull)
		{
			InternalValue setting = new(Configuration, name);

			if (setting.IsNull && allowNull)
				return null!;

			return setting.Value;
		}

		public string? Get(string name) => new InternalValue(Configuration, name).Value;

		public T Get<T>(string name, T defaultValue)
			=> Get(name, () => defaultValue, true);

		public T Get<T>(string name, Func<T> defaultCallback)
			=> Get(name, defaultCallback, true);

		public T Get<T>(string name, bool allowNull = false)
			=> Get<T>(name, () => default!, allowNull);

		public T Get<T>(string name, Func<T> defaultValue, bool allowNull = false)
		{
			InternalValue setting = new(Configuration, name, allowNull);

			try
			{
				if(setting.Section is null)
					return (T)Convert.ChangeType(setting.Value, typeof(T));

				return setting.Section.Value.FromJsonString<T>()
					?? defaultValue();
			}
			catch (Exception)
			{
				return defaultValue();
			}
		}
	}
}
