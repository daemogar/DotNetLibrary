#if !NETSTANDARD2_0_OR_GREATER
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Microsoft.Extensions.DependencyInjection;

public static class StringInputFormatterExtensions
{
    public static FormatterCollection<IInputFormatter> AddStringInputFormatter(
        this FormatterCollection<IInputFormatter> collection)
    {
        collection.Add(new StringInputFormatter());
        return collection;
    }
}
#endif