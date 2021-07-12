using System;

namespace DotNetLibrary.Expressions
{
	public interface ISimpleExpression
	{
		Type Type { get; }
		string Property { get; }
	}
}
