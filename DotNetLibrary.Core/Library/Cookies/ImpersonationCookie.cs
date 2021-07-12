namespace DotNetLibrary.Cookies
{
	public record ImpersonationCookie : BaseCookie
	{
		public ImpersonationCookie(CookieManager manager)
			: base(manager, "ImpersonatedPerson")
		{
		}
	}
}