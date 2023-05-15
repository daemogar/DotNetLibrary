using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Used to simplify setting an appsettings.json file options object.
/// </summary>
/// <typeparam name="T">The options object that should be used when called from dependent code.</typeparam>
public abstract class DiscoverableAppSettings<T> : DiscoverableService
		where T : class
{
	internal IConfiguration Configuration { get; private set; } = default!;

	/// <summary>
	/// The reference in the configuration for the appsettings.<br/>
	/// ie: if the value was "AppSettings"
	/// <code>
	///		appsettings.json = {
	///			"AppSettings": { ... }
	///		}
	/// </code>
	/// or if the value was "AppSettings:Data"
	/// <code>
	///		appsettings.json = {
	///			"AppSettings":
	///			{
	///				"Data": { ... }
	///			}
	///		}
	/// </code>
	/// Note: leaving this blank or null would result in accessing 
	/// the whole appsettings.json file.
	/// </summary>
	protected abstract string AppSettingSection { get; }

	/// <summary>
	/// Options configuration method for when the settings object 
	/// is generated.
	/// </summary>
	/// <param name="settings"></param>
	protected abstract void ConfigureAppSettings(T settings);

	/// <inheritdoc cref="DiscoverableService.ConfigureAsService(IServiceCollection, IConfiguration)"/>
	protected internal override void ConfigureAsService(
		IServiceCollection services, IConfiguration configuration)
	{
		Configuration = configuration;
		services.Configure<T>(ConfigureAppSettings);
	}

	/// <summary>
	/// Get the value of the data from the appsettings.json 
	/// configuration. To get the value for the 
	/// <paramref name="key"/> = "ApplicationName" with an 
	/// <seealso cref="AppSettingSection"/> of "AppSettings":
	/// <code>
	///		appsettings.json = {
	///			"AppSettings": {
	///				"ApplicationName": "Dotnet Library",
	///				...
	///			}
	///		}
	/// </code>
	/// or if the <paramref name="key"/> = "Data:ApplicationName"
	/// for the same <seealso cref="AppSettingSection"/> would return
	/// the same value for or if the <paramref name="key"/> = "ApplicationName"
	/// but the <seealso cref="AppSettingSection"/> was 
	/// "AppSettings:Data" it would would return the same value for:
	/// <code>
	///		appsettings.json = {
	///			"AppSettings": {
	///				"Data":
	///				{
	///					"ApplicationName": "Dotnet Library",
	///					...
	///				}
	///			}
	///		}
	/// </code>
	/// </summary>
	/// <param name="key">The key to the appsettings.json value to retrieve prefixed by the <seealso cref="AppSettingSection"/> value.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	protected string GetSetting(string key)
		=> Configuration.GetValue<string>(
			$"{AppSettingSection}:{key}".TrimStart(':'))
			?? throw new ArgumentNullException(key);

	/// <summary>
	/// Get configuration section.
	/// </summary>
	/// <param name="key">Appsetting section will be prepended to the key.</param>
	/// <returns>The configuration section from the appsettings.</returns>
	/// <exception cref="ArgumentNullException">If the section returns null.</exception>
	protected IConfiguration GetSection(string key)
		=> Configuration.GetSection(
			$"{AppSettingSection}:{key}".TrimStart(':'))
			?? throw new ArgumentNullException(key);
}
