using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace DotNetLibrary.Expressions
{
	public static class PropertyExtension
	{
		private static NullReferenceException CompileLambdaWithFailedPropertyExpression(string property, string expression)
			=> new($"Expression `{expression}` is not a valid property expression `{property}`.");

		private static NullReferenceException CompileLambdaWithInvoker(string expression)
			=> new($"Expression `{expression}` cannot be compiled into a lambda function.");

		private static NotImplementedException InvalidExpression(string propertyExpression)
			=> new($"The property expression `{propertyExpression}` is invalid or not supported.");

		public static ISimpleExpression DecompileLambda<TIn, TOut>(this Expression<Func<TIn, TOut>> propertyExpression)
			=> new SimpleExpression(propertyExpression.Body is UnaryExpression expression
				? expression.Operand
				: propertyExpression.Body, propertyExpression.Parameters);

		/// <summary>
		/// Use this method to convert a string expression into a lambda function that 
		/// takes a typeof(T) object. The method then invokes that property on the object
		/// returning the value of that property.
		/// </summary>
		/// <typeparam name="T">The object type to get the value from.</typeparam>
		/// <param name="_">Target model to build lambda model from.</param>
		/// <param name="propertyExpression">
		///		A string representation of the property to get the value of.
		///	</param>
		/// <returns>The string value of the property retrieved.</returns>
		public static Func<T, string> CompileLambda<T>(this T _, string propertyExpression)
			=> CompileLambda<T>(propertyExpression);

		/// <summary>
		/// Use this method to convert a string expression into a lambda function that 
		/// takes a typeof(T) object. The method then invokes that property on the object
		/// returning the value of that property.
		/// </summary>
		/// <typeparam name="T">The object to get the value from.</typeparam>
		/// <param name="propertyExpression">
		///		A string representation of the property to get the value of.
		///	</param>
		/// <returns>The string value of the property retrieved.</returns>
		public static Func<T, string> CompileLambda<T>(string propertyExpression)
		{
			var callback = DynamicExpressionParser.ParseLambda(
				typeof(T),
				GetType(typeof(T), propertyExpression.Split('.')),
				propertyExpression
			).Compile();

			return p => Convert.ToString(callback.DynamicInvoke(p))
				?? throw CompileLambdaWithInvoker(propertyExpression);

			Type GetType(Type type, IEnumerable<string> properties)
			{
				if (properties == null)
					throw CompileLambdaWithFailedPropertyExpression("Null", propertyExpression);

				var parts = properties.First().Split('[', ']').Take(2).ToArray();
				if (parts.Length > 1 && (int.TryParse(parts[1], out var index) || string.IsNullOrWhiteSpace(parts[1])))
					throw InvalidExpression(propertyExpression);

				var property = parts[0];
				var propertyInfo = type.GetProperty(property)
					?? throw CompileLambdaWithFailedPropertyExpression(property, propertyExpression);

				type = propertyInfo.PropertyType;

				return properties.Count() > 1
					? GetType(type, properties.Skip(1))
					: type;
			}
		}
	}
}
