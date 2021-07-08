using Serilog.Core;
using Serilog.Events;

namespace System.Diagnostics
{
	/// <summary>
	/// This converts a Microsoft Exception into something that is readable and 
	/// understandable to a human. This uses Ben Adams Demystifier project to 
	/// clean the stack trace up.
	/// <see cref="https://github.com/benaadams/Ben.Demystifier"/>
	/// </summary>
	public class BenDemystifyExceptionsSerilogEnricher
		: ILogEventEnricher
	{
		/// <summary>
		/// <inheritdoc cref="Enrich(LogEvent, ILogEventPropertyFactory)"/>
		/// </summary>
		/// <param name="logEvent"><inheritdoc cref="LogEvent"/></param>
		/// <param name="propertyFactory"><inheritdoc cref="ILogEventPropertyFactory"/></param>
		public void Enrich(
			LogEvent logEvent,
			ILogEventPropertyFactory propertyFactory)
			=> logEvent.Exception.Demystify();
	}
}
