using Microsoft.AspNetCore.Http;

using Moq;

using System.Threading.Tasks;

using Xunit;

namespace DotNetLibrary.Tests;

public class BasicCookieTests
{
	private Mock<BasicCookieManager> Manager { get; }

	public BasicCookieTests()
	{
		Manager = new Mock<BasicCookieManager>();
	}

	[Theory]
	[InlineData("test")]
	public void CreateReturnsCookieKey(string key)
	{
		BasicCookie<HttpContext> cookie = new(Manager.Object, key);
		Assert.Equal(key, cookie.CookieKey);
	}

	[Theory]
	[InlineData("test", "value")]
	public async Task CreateThrowsCookieKeyError(string key, string expected)
	{		
		Manager.Setup(p => p.GetRequestCookieAsync(key, false, default!))
			.Returns(Task.FromResult(expected));

		BasicCookie<string> cookie = new(Manager.Object, key);
		var actual = await cookie.GetAsync();
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("test", "value")]
	public async Task GetCookieIs(string key, string expected)
	{
		Manager.Setup(p => p.GetRequestCookieAsync(key, false, default!))
			.Returns(Task.FromResult(expected));

		BasicCookie<string> cookie = new(Manager.Object, key);
		var actual = await cookie.GetAsync();
		Assert.Equal(expected, actual);
	}
}