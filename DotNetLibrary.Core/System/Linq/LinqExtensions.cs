using System.Collections.Generic;

namespace System.Linq;

/// <summary>
/// Linq extension methods.
/// </summary>
public static class LinqExtensions
{
	/// <summary>
	/// Linq method to iterate over all items in a <paramref name="list"/> and
	/// calling <paramref name="predicate"/> on each item.
	/// </summary>
	/// <typeparam name="TModel">Generic type list of objects to add a single item to.</typeparam>
	/// <typeparam name="TResult">The type to return from the <paramref name="predicate"/>.</typeparam>
	/// <param name="list">List of items to have single item added to.</param>
	/// <param name="predicate">A method to convert an each item in list to a string.</param>
	public static TResult[] ForEach<TModel, TResult>(
		this IEnumerable<TModel> list, Func<TModel, TResult> predicate)
		=> list.ToArray().ForEach(predicate);

	/// <summary>
	/// Linq method to iterate over all items in a <paramref name="list"/> and
	/// calling <paramref name="predicate"/> on each item.
	/// </summary>
	/// <typeparam name="T">Generic type list of objects to add a single item to.</typeparam>
	/// <param name="list">List of items to have single item added to.</param>
	/// <param name="predicate">A method to convert an each item in list to a string.</param>
	public static void ForEach<T>(
		this IEnumerable<T> list, Action<T> predicate)
		=> list.ToArray().ForEach(predicate);

	/// <summary>
	/// Linq method to iterate over all items in a <paramref name="list"/> and
	/// calling <paramref name="predicate"/> on each item.
	/// </summary>
	/// <typeparam name="TModel">Generic type list of objects to add a single item to.</typeparam>
	/// <typeparam name="TResult">The type to return from the <paramref name="predicate"/>.</typeparam>
	/// <param name="list">List of items to have single item added to.</param>
	/// <param name="predicate">A method to convert an each item in list to a string.</param>
	public static TResult[] ForEach<TModel, TResult>(
		this TModel[] list, Func<TModel, TResult> predicate)
		=> list.Select(p => predicate(p)).ToArray();

	/// <summary>
	/// Linq method to iterate over all items in a <paramref name="list"/> and
	/// calling <paramref name="predicate"/> on each item.
	/// </summary>
	/// <typeparam name="T">Generic type list of objects to add a single item to.</typeparam>
	/// <param name="list">List of items to have single item added to.</param>
	/// <param name="predicate">A method to convert an each item in list to a string.</param>
	public static void ForEach<T>(
		this T[] list, Action<T> predicate)
		=> Array.ForEach(list, p => predicate(p));

	/// <summary>
	/// General method to flatten a <paramref name="list"/> into a string of 
	/// each string in list and seperated by <paramref name="seperator"/>.
	/// </summary>
	/// <param name="list">List of items to have single item added to.</param>
	/// <param name="seperator">The string seperater to place between each item.</param>
	/// <returns>String converted of <paramref name="list"/> and joined by <paramref name="seperator"/>.</returns>
	public static string Join(this IEnumerable<string> list, string seperator)
		=> string.Join(seperator, list);

	/// <summary>
	/// General method to flatten a <paramref name="list"/> into a string of 
	/// each item converted and seperated by <paramref name="seperator"/>.
	/// </summary>
	/// <typeparam name="T">Generic type list of objects to add a single item to.</typeparam>
	/// <param name="list">List of items to have single item added to.</param>
	/// <param name="seperator">The string seperater to place between each item.</param>
	/// <param name="predicate">A method to convert an each item in list to a string.</param>
	/// <returns>String converted of <paramref name="list"/> and joined by <paramref name="seperator"/>.</returns>
	public static string Flatten<T>(
		this IEnumerable<T> list,
		string seperator,
		Func<T, string> predicate)
		=> string.Join(seperator, list.Select(p => predicate(p)));

	/// <summary>
	/// Linq extension to conditionally join a object to a list of similar 
	/// objects.
	/// </summary>
	/// <typeparam name="T">Generic type list of objects to add a single item to.</typeparam>
	/// <param name="list">List of items to have single item added to.</param>
	/// <param name="conditional">Callback to determine if <paramref name="item"/> should be added to the list or not.</param>
	/// <param name="item">The item to add to the list.</param>
	/// <returns></returns>
	public static IEnumerable<T> UnionIf<T>(
		this IEnumerable<T> list,
		Func<bool> conditional,
		T item)
		=> list.UnionIf(conditional, new[] { item });

	/// <summary>
	/// Linq extension to conditionally join a list of objects to another list 
	/// of similar objects.
	/// </summary>
	/// <typeparam name="T">Generic type list of objects to add a single item to.</typeparam>
	/// <param name="list">List of items to have single item added to.</param>
	/// <param name="conditional">Callback to determine if <paramref name="items"/> should be added to the list or not.</param>
	/// <param name="items">The list of items to add to the list.</param>
	/// <returns></returns>
	public static IEnumerable<T> UnionIf<T>(
		this IEnumerable<T> list,
		Func<bool> conditional,
		IEnumerable<T> items)
		=> conditional() ? list.Union(items) : list;

	/// <summary>
	/// Linq extension to union/join an object to a list.
	/// </summary>
	/// <typeparam name="T">Generic type list of objects to add a single item to.</typeparam>
	/// <param name="list">List of items to have single item added to.</param>
	/// <param name="item">The item to add to the list.</param>
	/// <returns>The list with the item added to it.</returns>
	public static IEnumerable<T> Union<T>(this IEnumerable<T> list, T item)
		=> Enumerable.Union(list, new[] { item });
}
