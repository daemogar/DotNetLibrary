using Microsoft.Extensions.Hosting;

using Serilog;

using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <inheritdoc /> This task is for configuring and setting scheduled 
/// events.
/// </summary>
/// <typeparam name="T"><inheritdoc /></typeparam>
public abstract class DiscoverableTask<T>
	: DiscoverableBackgroundService<T>, IDisposable
	where T : DiscoverableTask<T>
{
	/// <summary>
	/// Used for debuging and tagging the error messages.
	/// </summary>
	protected string Tag { get; }

	private class ThrottleState : IDisposable
	{
		private static TimeSpan MaxTimeSpan { get; set; }

		private TimeSpan Time { get; set; }

		private PeriodicTimer? Timer { get; set; }

		public ThrottleState(int maxTimeSpan)
		{
			MaxTimeSpan = TimeSpan.FromMinutes(maxTimeSpan);
			Time = TimeSpan.FromSeconds(1);
			Timer = new(Time);
		}

		public async Task WaitForNextTickAsync(CancellationToken cancellationToken)
		{
			if (Timer is not null)
				await Timer.WaitForNextTickAsync(cancellationToken);
		}

		public void Reset()
		{
			Time = TimeSpan.FromSeconds(1);
			Dispose();
		}

		public void AddDelay()
		{
			if (Time < MaxTimeSpan)
			{
				Time = Time.Add(Time);
				if (Time > MaxTimeSpan)
					Time = MaxTimeSpan;

				Dispose();
				Timer = new(Time);
			}
		}

		public void Dispose()
		{
			if (Timer is null)
				return;

			Timer.Dispose();
			Timer = null;
		}
	}

	/// <summary>
	/// The date and time of the last run.
	/// </summary>
	public DateTime LastRun { get; private set; }

	/// <summary>
	/// The schedule and frequency the <seealso cref="ExecuteTaskAsync(CancellationToken)"/>
	/// method should be triggered.
	/// </summary>
	protected DiscoverableTaskSchedule TaskSchedule { get; }

	/// <summary>
	/// The default value of the last run or initial value.
	/// </summary>
	/// <returns>The last run datetime.</returns>
	protected abstract DateTime InitializeLastRun();

	/// <summary>
	/// The default or initial task schedule.
	/// </summary>
	/// <returns>The initial task schedule used for setting up the task.</returns>
	protected abstract DiscoverableTaskSchedule InitializeTaskSchedule();

	private ThrottleState Throttle { get; }

	private Stopwatch Stopwatch { get; }

	/// <summary>
	/// A discoverable task that can be scheduled.
	/// </summary>
	/// <param name="maxThrottleInMinutes">The when a task is failing, what is the maximum time to wait between trys.</param>
	public DiscoverableTask(int maxThrottleInMinutes = 60)
	{
		Tag = $"{GetType().Name}: ";
		Stopwatch = Stopwatch.StartNew();
		Throttle = new(maxThrottleInMinutes);
		LastRun = InitializeLastRun();
		TaskSchedule = InitializeTaskSchedule();
		OnLastRunUpdated += p => Task.CompletedTask;
	}

	/// <summary>
	/// This method is called every time the 
	/// <inheritdoc cref="DiscoverableBackgroundService{T}.ExecuteAsync(CancellationToken)" />
	/// </summary>
	/// <param name="stoppingToken"></param>
	/// <returns>If the execution was successful or not. If false, then the throttle is increased and the last run is not updated. If true, then the throttle is reset and the last run is updated with the current datetime.</returns>
	protected abstract Task<bool> ExecuteTaskAsync(CancellationToken stoppingToken);

	/// <summary>
	/// Notify any process that want to know that the last run date has been updated.
	/// </summary>
	protected event Func<DateTime, Task> OnLastRunUpdated;

	/// <inheritdoc />
	protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (TaskSchedule.Type == DiscoverableTaskScheduleType.Disabled)
			return;

		while (!stoppingToken.IsCancellationRequested)
		{
			await TaskSchedule.DelayUntilNextEventAsync(LastRun, stoppingToken);
			await Throttle.WaitForNextTickAsync(stoppingToken);

			if (!await ExecuteTaskAsync(stoppingToken))
			{
				Throttle.AddDelay();
				continue;
			}
			Throttle.Reset();

			LastRun = DateTime.Now;
			await OnLastRunUpdated.Invoke(LastRun);
		}
	}

	/// <summary>
	/// Get the elapsed millisconds from the last time the stopwatch was stopped.
	/// </summary>
	/// <returns>The total time the stopwatch ran for the last time it was stopped.</returns>
	protected long ElapsedMillisecondsLastRun()
		=> Stopwatch.ElapsedMilliseconds;

	/// <summary>
	/// Stop the task timer so the ellapsed time can be retrieved for how long in seconds the task took to complete.
	/// </summary>
	/// <returns>The total time the stopwatch ran for, either by stopping the clock or what the value was the last time it was stopped.</returns>
	protected long StopTiming()
	{
		if (Stopwatch.IsRunning)
			Stopwatch.Stop();

		return Stopwatch.ElapsedMilliseconds;
	}

	/// <summary>
	/// Start the task timer so the time ellapsed to run the process can be recorded.
	/// </summary>
	protected void StartTiming() => Stopwatch.Restart();

	/// <inheritdoc/>
	public override void Dispose()
	{
		Throttle.Dispose();
		base.Dispose();
	}
}
