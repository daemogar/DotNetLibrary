using System;

namespace DotNetLibrary.Exceptions
{
	public class AppSettingNotConfiguredException : Exception
	{
		public string SettingName { get; }

		public AppSettingNotConfiguredException(string name)
			: base($"{name} is not configured.")
		{
			SettingName = name;
		}
	}
}
