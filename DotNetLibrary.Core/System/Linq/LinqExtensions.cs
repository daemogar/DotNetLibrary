using System.Collections.Generic;

namespace System.Linq
{
	public delegate string FlattenToStringDelegate<T>(T item);

	public static class LinqExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> list, Action<T> predicate)
			=> list.ToArray().ForEach(predicate);

		public static void ForEach<T>(this T[] list, Action<T> predicate)
			=> Array.ForEach(list, p => predicate(p));

		public static string Join(this IEnumerable<string> list, string seperator)
			=> string.Join(seperator, list);

		public static string Flatten<T>(
			this IEnumerable<T> list,
			string seperator,
			FlattenToStringDelegate<T> callback)
			=> string.Join(seperator, list.Select(p => callback(p)));

		public static IEnumerable<T> Union<T>(this IEnumerable<T> list, T item)
			=> Enumerable.Union(list, new [] { item });
	}
}
