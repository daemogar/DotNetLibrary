using System;

namespace DotNetLibrary.Configuration.Exceptions
{
	public class SqlConnectionStringException : Exception
	{
		public SqlConnectionStringException(string connectionString)
			: base("Connection to a database could not be established.")
		{
			ConnectionString = connectionString;
		}

		public string ConnectionString { get; }
	}
	//*/
}

//[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
//public class ApiKeyAttribute : Attribute, IAsyncActionFilter
//{
//	public string Apikey { get; }
//
//	public ApiKeyAttribute(string apikey) => Apikey = apikey;
//
//	public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//	{
//		
//	}
//}
