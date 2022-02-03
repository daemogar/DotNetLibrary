namespace Microsoft.Extensions.Hosting;

/// <summary>
/// How frequently should a task be run.
/// </summary>
public enum DiscoverableTaskScheduleType
{
	/// <summary>
	/// Schedule is ignored.
	/// </summary>
	Disabled = 0,

	/// <summary>
	/// As soon as the process is complete,
	/// trigger the process again.
	/// </summary>
	Continuously = 1,

	/// <summary>
	/// As soon as the process is complete,
	/// wait for frequency to be met before
	/// triggering the process again.
	/// </summary>
	[UseTime(false)]
	Frequency = 2,

	/// <summary>
	/// Run every day on an hourly bases 
	/// at a specific time during the hour.
	/// </summary>
	[UseTime(false)]
	Hourly = 24,
	 
	/// <summary>
	/// Run every day at a specific time 
	/// of day.
	/// </summary>
	[UseTime(true)]
	Daily = 31,

	/// <summary>
	/// Run every day at a specific time
	/// on specific days.
	/// </summary>
	[UseTime(true)]
	[UseDay(true)]
	Weekly = 7,

	/// <summary>
	/// Run once a month at a specific
	/// day and time.
	/// </summary>
	[UseTime(true)]
	[UseDay(false)]
	Monthly = 12
}

