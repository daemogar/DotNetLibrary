using Serilog;

namespace DotNetLibrary.Configuration
{
	public delegate void ConfigureCallbackDelegate(ILogger logger);
	public delegate void ConfigureCallbackDelegate<in TOption>(ILogger logger, TOption options);
	public delegate TResult ConfigureCallbackDelegate<in TOption, out TResult>(ILogger logger, TOption options);

	public class ConfigureCallback : ConfigureCallback<bool>
	{
		public ConfigureCallback(string name) : base(name) { }

		public new void Invoke(ILogger logger)
			=> Invoke(logger, default);

		public new ConfigureCallbackDelegate Value
		{
			set
			{
				base.Value = (logger, _) => value(logger);
			}
		}
	}

	public class ConfigureCallback<TOption> : ConfigureCallback<TOption, bool>
	{
		public ConfigureCallback(string name) : base(name) { }

		public new void Invoke(ILogger logger, TOption option)
			=> base.Invoke(logger, option);

		public new ConfigureCallbackDelegate<TOption> Value
		{
			set
			{
				base.Value = (logger, options) =>
				{
					value(logger, options);
					return true;
				};
			}
		}
	}

	public class ConfigureCallback<TOption, TResult>
	{
		public string Name { get; }
		public bool IsSet { get; private set; }

		public ConfigureCallback(string name)
		{
			Name = name;
			Invoke = NotConfigured;

			TResult NotConfigured(ILogger logger, TOption options)
			{
				logger.Verbose($"No {Name}", "Option");
				return default!;
			}
		}

		public ConfigureCallbackDelegate<TOption, TResult> Invoke;
		public ConfigureCallbackDelegate<TOption, TResult> Value
		{
			set
			{
				Invoke = ConfigureLogging;
				IsSet = true;

				TResult ConfigureLogging(ILogger logger, TOption options)
				{
					logger.StartLogging(Name, "Option");
					var result = value.Invoke(logger, options);
					logger.FinishLogging(Name, "Option");
					return result;
				}
			}
		}
	}
}