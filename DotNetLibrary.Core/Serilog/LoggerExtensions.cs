using System;
using System.Threading.Tasks;

namespace Serilog
{
	[Obsolete("Find another way to do this stuff.")]
	public static class LoggerExtensions
	{
		public static TReturn TryCatchClass<TReturn, TClass>(
			this ILogger logger, Func<ILogger, TReturn> callback,
			Func<Exception, TReturn> defaultValue = null!)
			where TClass : class
			=> logger.TryCatch(callback,
				log => log.StartLogging<TClass>(),
				(log, exception) => log.Fatal(exception, ""),
				log => log.FinishLogging<TClass>(),
				defaultValue);

		public static TReturn TryCatchMethod<TReturn>(
			this ILogger logger, string methodName,
			Func<ILogger, TReturn> callback, Func<Exception, TReturn> defaultValue = null!)
			=> logger.TryCatch(callback,
				log => log.StartLogging(methodName),
				(log, exception) => log.Fatal(exception, ""),
				log => log.FinishLogging(methodName),
				defaultValue);

		public static void TryCatch(
			this ILogger logger, Action<ILogger> callback,
			Action<ILogger> @try = null!, Action<ILogger, Exception> @catch = null!, Action<ILogger> @finally = null!)
			=> logger.TryCatch<Exception>(callback, @try, @catch, @finally);

		public static TReturn TryCatch<TReturn>(
			this ILogger logger, Func<ILogger, TReturn> callback,
			Action<ILogger> @try = null!, Action<ILogger, Exception> @catch = null!, Action<ILogger> @finally = null!,
			Func<Exception, TReturn> defaultValue = null!)
			=> logger.TryCatch<TReturn, Exception>(callback, @try, @catch, @finally, defaultValue);

		public static void TryCatch<TException>(
			this ILogger logger, Action<ILogger> callback,
			Action<ILogger> @try = null!, Action<ILogger, TException> @catch = null!, Action<ILogger> @finally = null!)
			where TException : Exception
			=> logger.TryCatch(_ =>
			{
				callback(logger);
				return "Intentionally Not Used";
			}, @try, @catch, @finally);

		public static TReturn TryCatch<TReturn, TException>(
			this ILogger logger, Func<ILogger, TReturn> callback,
			Action<ILogger> @try = null!, Action<ILogger, TException> @catch = null!, Action<ILogger> @finally = null!,
			Func<Exception, TReturn> defaultValue = null!)
			where TException : Exception
		{
			TException exception;

			try
			{
				@try?.Invoke(logger);
				return callback(logger);
			}
			catch (TException e)
			{
				if (@catch == null)
					logger.Error(e, nameof(TryCatch));
				else
					@catch(logger, e);

				exception = e;
			}
			finally
			{
				@finally?.Invoke(logger);
			}

			return defaultValue == null ? default! : defaultValue(exception);
		}

		// change the Ilogger in the action to use Trace
		public static Task<TReturn> TryCatchAsync<TReturn, TException>(
			this ILogger logger, Func<Task<TReturn>> callback,
			Action<ILogger> @try = null!, Action<ILogger, TException> @catch = null!, Action<ILogger> @finally = null!,
			Func<TException, TReturn> defaultValue = null!)
			where TException : Exception
		{
			TException exception = null!;
			return logger.TryCatch(_ => callback(), @try, (l, e) =>
			{
				if (e is TException ex)
				{
					@catch(l, ex);
					exception = ex;
				}
				else
				{
					throw e;
				}
			}, @finally)
				?? Task.FromResult(defaultValue == null ? default! : defaultValue(exception));
		}

		public static TReturn WrapClassUnsafe<TReturn, TClass>(
			this ILogger log, Func<TReturn> callback)
			where TClass : class
		{
			log.StartLogging<TClass>();
			var result = callback();
			log.FinishLogging<TClass>();
			return result;
		}

		public static void WrapMethodUnsafe(this ILogger log, string methodName, Action callback)
		{
			log.StartLogging(methodName);
			callback();
			log.FinishLogging(methodName);
		}

		public static Func<TInput, TReturn> WrapMethodUnsafe<TInput, TReturn>(
			this ILogger log, string methodName, Func<TInput, TReturn> callback)
			=> options =>
			{
				log.StartLogging(methodName);
				var result = callback(options);
				log.FinishLogging(methodName);
				return result;
			};

		public static TReturn WrapMethodUnsafe<TReturn>(
			this ILogger log, string methodName, Func<TReturn> callback)
		{
			log.StartLogging(methodName);
			var result = callback();
			log.FinishLogging(methodName);
			return result;
		}

		public static async Task<TReturn> WrapMethodUnsafe<TReturn>(
			this ILogger log, string methodName, Func<Task<TReturn>> callback)
		{
			log.StartLogging(methodName);
			var result = await callback();
			log.FinishLogging(methodName);
			return result;
		}

		#region Start and Finish Logging for use at the beginning and ending of a process.

		public static void StartLogging<T>(this ILogger log)
			where T : class
			=> Logging(log, "Start", "Constructor", typeof(T).Name);

		public static void FinishLogging<T>(this ILogger log)
			where T : class
			=> Logging(log, "Finish", "Constructor", typeof(T).Name);

		public static void StartLogging(this ILogger log, string methodName, string tag = "Method")
			=> Logging(log, "Start", tag, methodName);

		public static void FinishLogging(this ILogger log, string methodName, string tag = "Method")
			=> Logging(log, "Finish", tag, methodName);

		private static void Logging(ILogger log, string action, string type, string name)
			=> log.Verbose($"{action} {{{type}}}", name);

		#endregion
	}
}
