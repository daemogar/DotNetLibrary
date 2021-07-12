using Microsoft.AspNetCore.Components;

using System;

namespace DotNetLibrary.Configuration.Menu
{
	public interface IHtmlMenuService
	{
		bool HasMenu { get; }
		MarkupString Markup { get; }
		event Action StateChanged;
	}
}
