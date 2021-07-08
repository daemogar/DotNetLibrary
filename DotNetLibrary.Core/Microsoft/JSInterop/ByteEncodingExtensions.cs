using Microsoft.AspNetCore.Components;

using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.JSInterop
{
	public static class ByteEncodingExtensions
	{
		private const string MethodPrefix = "window.";
		private static readonly Regex DoubleSpaces = new("\\s+", RegexOptions.None);

		public static MarkupString ToBase64String<T>(
			this JavascriptMethodNames method, T data)
		{
			var tag = $"{MethodPrefix}{method:G}";

			return (MarkupString)DoubleSpaces.Replace($@"
					{tag} = () => {{
						delete {tag};
						return ""{data?.ToBase64ByteEncodedString()}"";
					}};
				", " ");
		}
	}
}
