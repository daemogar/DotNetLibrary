using DotNetLibrary.Configuration.Settings;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class AppSettingsExtensions
	{
		public static void AddSettings(this IServiceCollection services, BaseAppSettings settings)
		{			
			services.AddSingleton<IAppSettings>(settings);
			services.AddSingleton(p => p.GetRequiredService<IAppSettings>().Environment);
		}
	}
}
