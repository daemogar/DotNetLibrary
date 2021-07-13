using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using System;







namespace DotNetLibrary.Tests.Utilities
{
	public class AutoMoqDataAttribute : AutoDataAttribute
	{
		public AutoMoqDataAttribute()
			: base(new Func<IFixture>(() =>
				new Fixture().Customize(new AutoMoqCustomization())))
			{
			}
	}
}
