using DotNetLibrary.Configuration.Environment;

using System;

namespace DotNetLibrary.Configuration.Settings
{
	public interface IAppSettings
	{
		public interface IEmailer
		{
			string DeveloperEmailAddress { get; }
			string SendMessageEmailAddress { get; }
			string OverrideEmailAddress { get; }
		}

		IEnvironmentModel Environment { get; }
		string ApplicationName { get; }
		string? DefaultUser { get; }
		bool IsServerSideBlazor { get; }
		bool IsMockUser { get; }

		IEmailer Email { get; }

		string Get(string name, bool allowNulls);
		string? Get(string name);
		T Get<T>(string name, bool allowNulls = false);
		T Get<T>(string name, T defaultValue);
		T Get<T>(string name, Func<T> defaultCallback);
		T Get<T>(Type type, string name, Func<T> defaultCallback, bool allowNulls = false);
	}
}
