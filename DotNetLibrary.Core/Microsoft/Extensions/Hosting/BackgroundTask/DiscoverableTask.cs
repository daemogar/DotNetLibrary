﻿#if !NETSTANDARD2_0_OR_GREATER
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <inheritdoc /> This task is for configuring and setting scheduled 
/// events.
/// </summary>
/// <typeparam name="TTask">A type that implements <seealso cref="DiscoverableTask{TTask, TSchedule}" />.</typeparam>
/// <typeparam name="TSchedule">A type that implements <seealso cref="DiscoverableTaskSchedule"/>.</typeparam>
public abstract class DiscoverableTask<TTask, TSchedule>
	: DiscoverableBackgroundService<TTask>, IDisposable
	where TTask : DiscoverableTask<TTask, TSchedule>
	where TSchedule : DiscoverableTaskSchedule
{
	/// <summary>
	/// Response from the <seealso cref="RepeatTaskAsync(CancellationToken)"/>.
	/// Use the static helper methods <seealso cref="Success(string)"/>,
	/// <seealso cref="BasicHealthCheck.FailureState(Exception, IReadOnlyDictionary{string, object})"/>, 
	/// and <seealso cref="Critical(Exception)"/> for ideal return values for this class record.<br />
	/// <br/>
	/// <seealso cref="Success(string)"/> -- Returns 
	/// </summary>
	/// <param name="IsSuccessful">Did the <seealso cref="RepeatTaskAsync(CancellationToken)"/> complete successfully. Note the task could have failed, but still completed. Use <seealso cref="Success(string)"/></param>
	/// <param name="ShouldUpdate"></param>
	/// <param name="Message"></param>
	/// <param name="Exception"></param>
	public record Response(
		bool IsSuccessful,
		bool ShouldUpdate,
		string Message,
		Exception Exception)
	{
		/// <summary>
		/// Create and return a success response.
		/// </summary>
		/// <param name="message">Optional message to be returned.</param>
		/// <returns>Successful response object.</returns>
		public static Response Success(string message = null!)
			=> new(true, true, string.IsNullOrWhiteSpace(message)
				? message
				: "Task Completed Successfully!", default!);

		/// <summary>
		/// Create and return a failure response. Used to handle basic
		/// errors. If an exception is thrown, then 
		/// <seealso cref="Critical(Exception)"/> should be called.
		/// </summary>
		/// <param name="message">The failure message.</param>
		/// <returns>Failure response with message.</returns>
		public static Response Failure(string message)
			=> new(false, true, message, default!);

		/// <summary>
		/// Create and return a critical response. Used to signify that
		/// the failure was catastrophic. If the issue is a recoverable or 
		/// able to be ignored, then consider returning a 
		/// <seealso cref="Failure(string)"/>.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns>Critical reseponse with associated exception.</returns>
		public static Response Critical(Exception exception)
			=> new(false, false, exception.Message, exception);
	}

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
	public DateTime LastRun { get; private set; } = default!;

	/// <summary>
	/// The schedule and frequency the <seealso cref="RepeatTaskAsync(CancellationToken)" />
	/// method should be triggered.
	/// </summary>
	protected TSchedule TaskSchedule { get; private set; } = default!;

	/// <summary>
	/// The default value of the last run or initial value.
	/// </summary>
	/// <param name="stoppingToken"><inheritdoc cref="CancellationToken" /></param>
	/// <returns>The last run datetime.</returns>
	protected abstract Task<DateTime> InitializeLastRunAsync(CancellationToken stoppingToken);

	/// <summary>
	/// The default or initial task schedule.
	/// </summary>
	/// <param name="stoppingToken"><inheritdoc cref="CancellationToken" /></param>
	/// <returns>The initial task schedule used for setting up the task.</returns>
	protected abstract Task<TSchedule> InitializeTaskScheduleAsync(CancellationToken stoppingToken);

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
	}

	/// <summary>
	/// If <seealso cref="DiscoverableBackgroundService{T}.InitializeAsync(CancellationToken)" /> 
	/// returns true, then this method is called repeated by 
	/// <inheritdoc cref="DiscoverableBackgroundService{T}.ExecuteAsync(CancellationToken)" />
	/// </summary>
	/// <param name="stoppingToken"><inheritdoc cref="CancellationToken" /></param>
	/// <returns>If the execution was successful or not. If false, then the throttle is increased and the last run is not updated. If true, then the throttle is reset and the last run is updated with the current datetime.</returns>
	protected abstract Task<Response> RepeatTaskAsync(CancellationToken stoppingToken);

	/// <inheritdoc />
	public sealed override async Task StartAsync(CancellationToken stoppingToken)
	{
		try
		{
			TaskSchedule = await InitializeTaskScheduleAsync(stoppingToken);
			LastRun = await InitializeLastRunAsync(stoppingToken);

			if (TaskSchedule.Type == DiscoverableTaskScheduleType.Disabled
				|| !await InitializeAsync(stoppingToken))
				return;

			await ExecuteAsync(stoppingToken);
		}
		catch (Exception e)
		{
			await UpdateTaskStatusAndLogAsync(Response.Critical(e), stoppingToken);
			return;
		}
	}

	/// <inheritdoc />
	protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await TaskSchedule.DelayUntilNextEventAsync(LastRun, stoppingToken);
			await Throttle.WaitForNextTickAsync(stoppingToken);

			DiscoverableTask<TTask, TSchedule>.Response response;

			StartTiming();
			try
			{
				response = await RepeatTaskAsync(stoppingToken);
			}
			catch (Exception e)
			{
				response = Response.Critical(e);
			}
			StopTiming();

			try
			{
				if (response.IsSuccessful)
				{
					await TaskCompletedAsync(response.Message, stoppingToken);
					Throttle.Reset();
				}
				else
				{
					Throttle.AddDelay();
					await TaskFailedAsync(response.Exception, stoppingToken);
				}
			}
			catch (Exception e)
			{
				response = Response.Critical(e);
			}

			await UpdateTaskStatusAndLogAsync(response, stoppingToken);
		}
	}

	private async Task UpdateTaskStatusAndLogAsync(Response response, CancellationToken stoppingToken)
	{
		if (response.ShouldUpdate)
		{
			LastRun = DateTime.Now;
			await UpdateLastRunAsync(stoppingToken);
		}

		if (response.IsSuccessful)
			await TaskCompletedAsync(response.Message, stoppingToken);
		else
			await TaskFailedAsync(response.Exception, stoppingToken);
	}

	/// <inheritdoc />
	public sealed override Task StopAsync(CancellationToken cancellationToken)
		=> base.StopAsync(cancellationToken);

	/// <summary>
	/// Override this method to be notified when the last run date has been 
	/// updated.
	/// </summary>
	/// <param name="stoppingToken"><inheritdoc cref="CancellationToken" /></param>
	/// <returns>An asynchronous task operation.</returns>
	protected virtual Task UpdateLastRunAsync(CancellationToken stoppingToken) => Task.CompletedTask;

	/// <summary>
	/// Override this method to be notified when a task completes successfully.
	/// </summary>
	/// <param name="message">The message returned doing the task execution.</param>
	/// <param name="stoppingToken"><inheritdoc cref="CancellationToken" /></param>
	/// <returns>An asynchronous task operation.</returns>
	protected virtual Task TaskCompletedAsync(string message, CancellationToken stoppingToken) => Task.CompletedTask;

	/// <summary>
	/// Override this method to be notified when a task fails to complete 
	/// successfully.
	/// </summary>
	/// <param name="exception">The exception thrown during task execution.</param>
	/// <param name="stoppingToken"><inheritdoc cref="CancellationToken" /></param>
	/// <returns>An asynchronous task operation.</returns>
	protected virtual Task TaskFailedAsync(Exception exception, CancellationToken stoppingToken) => Task.CompletedTask;

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
#endif