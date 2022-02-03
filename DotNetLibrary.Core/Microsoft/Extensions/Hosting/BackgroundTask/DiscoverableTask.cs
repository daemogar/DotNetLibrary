using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <inheritdoc /> This task is for configuring and setting scheduled 
/// events.
/// </summary>
/// <typeparam name="T"><inheritdoc /></typeparam>
public abstract class DiscoverableTask<T> : DiscoverableBackgroundService<T>
	where T : DiscoverableTask<T>
{
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
	/// A discoverable task that can be scheduled.
	/// </summary>
	/// <param name="lastRun">The date and time of the last run. If this task has never run this this should be set to <seealso cref="DateTime.MinValue"/>.</param>
	/// <param name="schedule">A schedule describing how often this task should be run.</param>
	public DiscoverableTask(DateTime lastRun, DiscoverableTaskSchedule schedule)
	{
		LastRun = lastRun;
		TaskSchedule = schedule;
		OnLastRunUpdated += async p => { await Task.CompletedTask; };
	}

	/// <summary>
	/// This method is called every time the 
	/// <inheritdoc cref="DiscoverableBackgroundService{T}.ExecuteAsync(CancellationToken)" />
	/// </summary>
	/// <param name="stoppingToken"></param>
	/// <returns></returns>
	protected abstract Task ExecuteTaskAsync(CancellationToken stoppingToken);

	/// <summary>
	/// Notify any process that want to know that the last run date has been updated.
	/// </summary>
	protected event Func<DateTime, Task> OnLastRunUpdated;

	/// <inheritdoc />
	protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (TaskSchedule.Type == DiscoverableTaskScheduleType.Disabled)
		{
			await Task.CompletedTask;
			return;
		}

		while (!stoppingToken.IsCancellationRequested)
		{
			await TaskSchedule.DelayUntilNextEventAsync(LastRun, stoppingToken);
			await ExecuteTaskAsync(stoppingToken);

			LastRun = DateTime.Now;
			if (OnLastRunUpdated is not null)
				await OnLastRunUpdated.Invoke(LastRun);
		}
	}
}
