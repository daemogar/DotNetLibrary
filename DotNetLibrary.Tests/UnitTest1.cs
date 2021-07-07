using System;

using Xunit;

namespace DotNetLibrary.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			Example e = new();
			Microsoft.Extensions.DependencyInjection.
				AppSettingsExtensions.AddSettings(null, null);
		}
	}
}
