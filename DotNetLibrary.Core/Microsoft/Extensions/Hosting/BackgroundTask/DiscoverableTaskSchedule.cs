using Serilog;

using System.ComponentModel;
using System.Text;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// A schedule for when a task should be trigger.
/// </summary>
public class DiscoverableTaskSchedule
{
	/// <summary>
	/// Task ID for use 
	/// </summary>
	public DiscoverableTaskScheduleID Identification { get; } = new();

	/// <summary>
	/// The schedule type for this task schedule.
	/// </summary>
	public DiscoverableTaskScheduleType Type { get; }

	/// <summary>
	/// The days of the week this task should be run 
	/// on if applicable.
	/// </summary>
	public List<DayOfWeek> DaysOfTheWeek() => DaysOfTheWeekArray?.Select(p => p.DayOfWeek).ToList()!;
	private List<(DayOfWeek DayOfWeek, int Days)> DaysOfTheWeekArray { get; set; } = default!;

	/// <summary>
	/// The time of day or frequency a task should run.
	/// </summary>
	public TimeSpan Time { get; }

	internal EnumExtensions.UseDayTime DayTime { get; }

	/// <summary>
	/// Create a task schedule. Not all parameters are used or needed or
	/// interpreted the same, use with caution.
	/// </summary>
	/// <param name="type">The type of schedule.</param>
	/// <param name="time">The time for frequency of triggers.</param>
	protected DiscoverableTaskSchedule(
		DiscoverableTaskScheduleType type = DiscoverableTaskScheduleType.Disabled,
		TimeSpan time = default)
	{
		Type = type;
		Time = time;

		DayTime = Type.GetDayTime();
	}

	/// <summary>
	/// Delay the processing until the next time a task should be run.
	/// </summary>
	/// <param name="lastRun">The date and time of the last run.</param>
	/// <param name="stoppingToken">A token handling cancelling the task if it should be stopped early.</param>
	/// <returns>An awaitable delay task.</returns>
	public async Task DelayUntilNextEventAsync(DateTime lastRun, CancellationToken stoppingToken)
	{
		var next = NextScheduledTime(lastRun);
		await Task.Delay(next < DateTime.Now
			? TimeSpan.Zero
			: next - DateTime.Now, stoppingToken);
	}

	/// <summary>
	/// Get the next date and time for when the scheduled task should be run given a previous run time.
	/// </summary>
	/// <param name="lastRun">The date and time of the last run.</param>
	/// <returns>The date and time of the next run.</returns>
	public DateTime NextScheduledTime(DateTime lastRun)
		=> Type switch
		{
			DiscoverableTaskScheduleType.Continuously => DateTime.Now,

			DiscoverableTaskScheduleType.Frequency
				=> lastRun.Add(Time),

			DiscoverableTaskScheduleType.Hourly
				=> lastRun.Date.AddHours(lastRun.Hour + 1).Add(Time),

			DiscoverableTaskScheduleType.Daily
				=> lastRun.Date.AddDays(1).Add(Time),

			DiscoverableTaskScheduleType.Weekly
				=> lastRun.Date.AddDays(DaysOfTheWeekArray[
						DaysOfTheWeek().IndexOf(lastRun.DayOfWeek) + 1
					].Days).Add(Time),

			DiscoverableTaskScheduleType.Monthly
				=> new DateTime(lastRun.Year, lastRun.Month, 1)
					.AddMonths(1).Add(Time),

			_ => DateTime.MaxValue
		};
	/*
	private TimeSpan NextDay(DateTime lastRun, int days)
	{
		var next = lastRun.Date.AddDays(days).Add(Time);

		return DateTime.Now >= next
			? TimeSpan.Zero
			: DateTime.Now - next;
	}

	private TimeSpan NextMonth(DateTime lastRun)
	{
		var next = new DateTime(lastRun.Year, lastRun.Month, 1)
			.AddMonths(1).Add(Time);

		return next - DateTime.Now;
	}

	private TimeSpan NextWeekDay(DateTime lastRun)
	{
		var index = DaysOfTheWeek().IndexOf(lastRun.DayOfWeek) + 1;
		var days = DaysOfTheWeekArray[index].Days;

		return NextDay(lastRun, days);
	}

	private TimeSpan Add(DateTime lastRun)
		=> new TimeSpan(lastRun.Ticks - DateTime.Now.Ticks).Add(Time);
	*/
	/// <summary>
	/// Create a task schedule that is disabled.
	/// </summary>
	/// <returns>A disabled task schedule.</returns>
	public static DiscoverableTaskSchedule CreateDisabledTask() => new();

	/// <summary>
	/// Create a task that runs again immediately after previous run finishes.
	/// </summary>
	/// <returns>A task schedule configured to run continuously.</returns>
	public static DiscoverableTaskSchedule CreateContinuousTask()
		=> new(DiscoverableTaskScheduleType.Continuously);

	/// <summary>
	/// Create a task that runs every x number of <paramref name="minutes"/>.
	/// </summary>
	/// <param name="minutes">The time between each run of the task.</param>
	/// <returns>A task schedule configured for a frequency type.</returns>
	public static DiscoverableTaskSchedule CreateFrequencyTask(int minutes)
		=> new(DiscoverableTaskScheduleType.Frequency, new(0, minutes, 0));

	/// <summary>
	/// Create a task that runs every hour on the <paramref name="minute"/> mark.
	/// </summary>
	/// <param name="minute">The minute mark of the hour the task should run.</param>
	/// <returns>A task schedule configured for a hourly task type.</returns>
	public static DiscoverableTaskSchedule CreateHourlyTask(int minute)
	{
		if (minute < 0 || minute > 59)
			Log.Warning(
				"Scheduled hourly task was adjusted to run on the hour from " +
				"{Original Time} to {Scheduled Time}.",
				minute, minute % 60);

		return new(DiscoverableTaskScheduleType.Hourly, new(0, minute % 60, 0));
	}

	/// <summary>
	/// Create a task that runs every day on the <paramref name="hour"/> and
	/// <paramref name="minute"/>.
	/// </summary>
	/// <param name="hour">The hour the task should run each day.</param>
	/// <param name="minute">The minute of the hour that the task should run each day.</param>
	/// <returns>A task schedule configured for a daily running task.</returns>
	public static DiscoverableTaskSchedule CreateDaily(int hour, int minute)
		=> new(DiscoverableTaskScheduleType.Daily, HourMinute(hour, minute,
			$"daily task was adjusted to run every day"));

	/// <summary>
	/// Create a task that runs for each <paramref name="daysOfTheWeek"/> at
	/// the specific <paramref name="hour"/> and <paramref name="minute"/>.
	/// </summary>
	/// <param name="hour">The hour the task should run each <paramref name="daysOfTheWeek"/>.</param>
	/// <param name="minute">The minute of the hour that the task should run each <paramref name="daysOfTheWeek"/>.</param>
	/// <param name="daysOfTheWeek">The days of the week the task should run on.</param>
	/// <returns>A task schedule configured for a weekly running task.</returns>
	public static DiscoverableTaskSchedule CreateWeekly(int hour, int minute, params DayOfWeek[] daysOfTheWeek)
	{
		if (!(daysOfTheWeek?.Length > 0))
		{
			Log.Warning(
				$"Scheduled weekly task did not have any days, so returning a disabled task.");
			return new();
		}

		DiscoverableTaskSchedule service = new(DiscoverableTaskScheduleType.Weekly,
			HourMinute(hour, minute, $"weekly task was adjusted to run on the scheduled days"));
		service.DaysOfTheWeekArray = Expand(0).Union(Expand(7)).ToList();
		return service;

		IEnumerable<(DayOfWeek p, int)> Expand(int index)
			=> daysOfTheWeek?.Select(p => (p, index + (int)p))!;
	}

	/// <summary>
	/// Create a task that runs each month on the <paramref name="day"/> of the
	/// month at the specific <paramref name="hour"/> and <paramref name="minute"/>.
	/// </summary>
	/// <param name="day">The day of the month to run the task.</param>
	/// <param name="hour">The hour of the day to run the task.</param>
	/// <param name="minute">The minute of the hour to run the task.</param>
	/// <returns>A task schedule configured for a monthly running task.</returns>
	public static DiscoverableTaskSchedule CreateMonthly(int day, int hour, int minute)
			=> new(DiscoverableTaskScheduleType.Monthly, HourMinute(hour, minute,
				$"montyly task was adjusted to run on the {day.WithSuffix()} of the month")
				.Add(new(day - 1, 0, 0, 0)));

	private static TimeSpan HourMinute(int hour, int minute, string message)
	{
		TimeSpan original = new(hour, minute, 0);
		TimeSpan scheduled = new(original.Hours, original.Minutes, 0);

		if (original.Days > 0)
			Log.Warning(
				$"Scheduled {message} at {{Scheduled Time}} instead of {{Original Time}}.",
				scheduled, original);

		return scheduled;
	}
}
