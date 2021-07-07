namespace DotNetLibrary.Configuration.Settings
{
	public class EmailerSettings : IAppSettings.IEmailer
	{
		internal EmailerSettings(AppSettingsElement settings)
		{
			OverrideEmailAddress = settings.Get("Emailer:OverrideEmailAddress", "developer@southern.edu");
			DeveloperEmailAddress = settings.Get("Emailer:DeveloperEmailAddress", OverrideEmailAddress);
			SendMessageEmailAddress = settings.Get("Emailer:SendMessageEmailAddress", OverrideEmailAddress);
		}

		public string DeveloperEmailAddress { get; }

		public string SendMessageEmailAddress { get; }

		public string OverrideEmailAddress { get; }
	}
}
