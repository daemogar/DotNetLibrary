using Microsoft.AspNetCore.Http;

using Moq;

using Xunit;

namespace DotNetLibrary.Tests;

public class BasicCookieTests
{
	private Mock<IBasicCookieManager> Manager { get; }

	public BasicCookieTests()
	{
		Manager = new Mock<IBasicCookieManager>();
	}

	[Theory]
	[InlineData("test")]
	public void CreateReturnsCookieKey(string key)
	{
		BasicCookie cookie = new(Manager.Object, key);
		Assert.Equal(key, cookie.CookieKey);
	}

	[Theory]
	[InlineData("test", "value")]
	public void CreateThrowsCookieKeyError(string key, string expected)
	{
		Manager.Setup(p => p.GetRequestCookie(key)).Returns(expected);
		BasicCookie cookie = new(Manager.Object, key);
		var actual = cookie.Get();
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("test", "value")]
	public void GetCookieIs(string key, string expected)
	{
		Manager.Setup(p => p.GetRequestCookie(key)).Returns(expected);
		BasicCookie cookie = new(Manager.Object, key);
		var actual = cookie.Get();
		Assert.Equal(expected, actual);
	}
}