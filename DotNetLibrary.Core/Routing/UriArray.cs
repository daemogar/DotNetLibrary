using System.Collections;
using System.Collections.Generic;

namespace DotNetLibrary.Routing
{
	/// <summary>
	/// Object for collection Style/Javascript pathes for use on the _Host.cshtml file.
	/// </summary>
	public class UriArray : IEnumerable<string>
	{
		private List<string> List { get; } = new();

		/// <summary>
		/// Add a Style/Javascript to the end of the section.
		/// </summary>
		/// <param name="uri">The absolute or relative path for the resource.</param>
		public void Add(string uri) => List.Add(uri);

		/// <summary>
		/// Add a Style/Javascript to the beginning of the section.
		/// </summary>
		/// <param name="uri">The absolute or relative path for the resource.</param>
		public void Insert(string uri) => List.Insert(0, uri);

		/// <summary>
		/// Add a range of Styles/Javascript to the beginning of the section.
		/// </summary>
		/// <param name="uris">List of absolute or relative pathes for the resource.</param>
		public void InsertRange(params string[] uris) => List.InsertRange(0, uris);

		/// <summary>
		/// Add a range of Styles/Javascript to the end of the section.
		/// </summary>
		/// <param name="uris">List of absolute or relative pathes for the resource.</param>
		public void AddRange(params string[] uris) => List.AddRange(uris);

		public UriArray() { }

		public IEnumerator<string> GetEnumerator()
			=> List.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> List.GetEnumerator();
	}
}