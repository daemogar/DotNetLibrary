using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DotNetLibrary.Expressions
{
	public class SimpleExpression : ISimpleExpression
	{
		public Expression Expression { get; }
		public ReadOnlyCollection<ParameterExpression> Parameters { get; }

		public Type Type => Expression.Type;
		public string Property => Expression.ToString().Replace(Parameters[0] + ".", "");

		public SimpleExpression(Expression expression, ReadOnlyCollection<ParameterExpression> parameters)
		{
			Expression = expression;
			Parameters = parameters;
		}
	}
}
