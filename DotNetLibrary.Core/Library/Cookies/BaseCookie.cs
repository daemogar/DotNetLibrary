using Microsoft.AspNetCore.Http;

namespace DotNetLibrary.Cookies
{
	public abstract record BaseCookie
	{
		private CookieManager Manager { get; }

		public string Token { get; }

		public BaseCookie(CookieManager manager, string token)
		{
			Manager = manager;
			Token = token;
		}

		public string? Get() => Manager.GetRequestCookie(Token);
		
		public void Set(string identifier) => Set(identifier, new());
		public void Set(string identifier, CookieOptions options)
			=> Manager.AppendResponseCookie(Token, identifier, options);

		public void Delete() => Delete(new());
		public void Delete(CookieOptions options)
			=> Manager.DeleteCookie(Token, options);
	}
}