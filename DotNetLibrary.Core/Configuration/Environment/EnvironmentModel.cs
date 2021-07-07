using System;

namespace DotNetLibrary.Configuration.Environment
{
	public class EnvironmentModel : IEnvironmentModel
	{
		public bool IsAuthenticationEnabled { get; private set; } = true;

		public const string NotSetupMessage = "Not Setup";
		public const string BrowserTypeName = "BROWSER";

		private EnvironmentType _type = EnvironmentType.INVALID;
		private string _rendered = NotSetupMessage;

		public EnvironmentType Type { get => _type; set => SetType(value); }

		public string Rendered { get => _rendered; set => SetRendered(value); }

		public void DisableAuthentication() => IsAuthenticationEnabled = false;

		public void SetType(string type)
			=> SetType(Enum.TryParse<EnvironmentType>(type, true, out var typed) ? typed :
				throw new InvalidCastException(
					$"'Application Environment' type cannot be set to an unknown type of {type ?? "Missing AppSetting Config"}."));
		public void SetType(EnvironmentType type)
		{
			if (type == EnvironmentType.INVALID)
				throw new ArgumentOutOfRangeException(nameof(type),
					$"Environment type cannot be set to {nameof(EnvironmentType.INVALID)}.");

			_type = type;

			IsProduction = Type.Equals(EnvironmentType.PRODUCTION);
			IsDevelopment = !IsProduction;
			IsDebug = Type.Equals(EnvironmentType.DEBUG);
		}

		public void SetRendered(string type)
		{
			_rendered = type.ToUpper();

			IsBrowser = Rendered.Equals(BrowserTypeName);
			IsServer = !IsBrowser;
		}

		public bool IsProduction { get; private set; }
		public bool IsDevelopment { get; private set; }
		public bool IsDebug { get; private set; }
		public bool IsBrowser { get; private set; }
		public bool IsServer { get; private set; }
	}
}