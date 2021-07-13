using DotNetLibrary.Configuration.Settings;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class AppSettingsConnectionStringsExtensions
	{
		public static void AddSettings<TAppSettings>(this IServiceCollection services, TAppSettings settings)
			where TAppSettings : BasicAppSettings
		{			
			services.AddSingleton<IAppSettings>(settings);
			services.AddSingleton(p => p.GetRequiredService<IAppSettings>().Environment);
		}
	}
}
