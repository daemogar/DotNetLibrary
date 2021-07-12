using System;

namespace DotNetLibrary.Configuration.Environment
{
	public interface IEnvironmentModel
	{
		Type? AuthStateType { get; }
		bool IsAuthenticationEnabled { get; }
		EnvironmentType Type { get; }
		string Rendered { get; }
		void SetType(string type);
		void SetType(EnvironmentType type);
		void SetRendered(string type);
		public bool IsProduction { get; }
		public bool IsDevelopment { get; }
		public bool IsDebug { get; }
		public bool IsBrowser { get; }
		public bool IsServer { get; }
	}
}