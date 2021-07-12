using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

using Serilog;

using DotNetLibrary.Exceptions;

namespace DotNetLibrary.Cookies
{
	public class CookieManager : ICookieManager
	{
		private ChunkingCookieManager Base { get; }

		protected ILogger Logger { get; }

		private IHttpContextAccessor ContextAccessor { get; }
		protected HttpContext GetContext() => ContextAccessor.HttpContext!;

		public CookieOptions DefaultOptions { get; } = new();

		public CookieManager(ILogger logger, IHttpContextAccessor context)
		{
			Logger = logger;
			ContextAccessor = context;
			Base = new();
		}

		public void AppendResponseCookie(string key, string? value)
			=> AppendResponseCookie(GetContext(), key, value, DefaultOptions);
		public void AppendResponseCookie(string key, string? value, CookieOptions options)
			=> AppendResponseCookie(GetContext(), key, value, options);
		public void AppendResponseCookie(HttpContext context, string key, string? value)
			=> AppendResponseCookie(context, key, value, DefaultOptions);
		public void AppendResponseCookie(HttpContext context, string key, string? value, CookieOptions options)
		{
			if (context == null)
				return;

			Logger.StartLogging(nameof(AppendResponseCookie));

			if (value == null)
				Base.DeleteCookie(context, key, options);
			else 
				Base.AppendResponseCookie(context, key, value, options);

			Logger.FinishLogging(nameof(AppendResponseCookie));
		}

		public void DeleteCookie(string key)
			=> DeleteCookie(GetContext(), key, DefaultOptions);
		public void DeleteCookie(string key, CookieOptions options)
			=> DeleteCookie(GetContext(), key, options);
		public void DeleteCookie(HttpContext context, string key)
			=> DeleteCookie(context, key, DefaultOptions);
		public void DeleteCookie(HttpContext context, string key, CookieOptions options)
		{
			if (context == null)
				return;

			Logger.StartLogging(nameof(DeleteCookie));
			Base.DeleteCookie(context, key, options);
			Logger.FinishLogging(nameof(DeleteCookie));
		}

		public string GetRequestCookie(string key, bool allowNull = true)
			=> GetRequestCookie(GetContext(), key, allowNull)!;
		public string? GetRequestCookie(HttpContext context, string key)
			=> GetRequestCookie(context, key, true);
		public string? GetRequestCookie(HttpContext context, string key, bool allowNull)
		{
			if (context == null)
				return GetValue(key, null, allowNull);

			try
			{
				Logger.StartLogging(nameof(GetRequestCookie));
				Logger.Verbose("Looking Up Cookie: {Key}", key);
				string? cookie = GetValue(key, Base.GetRequestCookie(context, key), allowNull);
				Logger.Debug("Retrieve Cookie: {Key} = {Value}", key, cookie);
				Logger.FinishLogging(nameof(GetRequestCookie));
				return cookie;
			}
			catch (System.Exception e)
			{
				Logger.Error(e, "What is going on? Where is this getting eatin?");
				throw;
			}

			static string? GetValue(string key, string? value, bool allowNull)
				=> value ?? (allowNull ? null : throw new NullCookieValueException(key));
		}
	}
}