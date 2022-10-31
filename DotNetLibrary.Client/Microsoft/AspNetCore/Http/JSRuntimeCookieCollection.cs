using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DotNetLibrary.Microsoft.AspNetCore.Http;

internal class JSRuntimeCookieCollection : IResponseCookies, IRequestCookieCollection
{
    private IJSRuntime Runtime { get; }

    public string this[string key] { get; }

    public int Count { get; }
    public ICollection<string> Keys { get; }

    public JSRuntimeCookieCollection(IJSRuntime runtime)
    {
        Runtime = runtime;
    }

    public void Append(string key, string value)
    {
        throw new NotImplementedException();
    }

    public void Append(string key, string value, CookieOptions options)
    {
        throw new NotImplementedException();
    }

    public bool ContainsKey(string key)
    {
        throw new NotImplementedException();
    }

    public void Delete(string key)
    {
        throw new NotImplementedException();
    }

    public void Delete(string key, CookieOptions options)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
    {
        try
        {
            Runtime.InvokeAsync<string>("document.", new { })

                }
        catch (Exception e)
        { }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}