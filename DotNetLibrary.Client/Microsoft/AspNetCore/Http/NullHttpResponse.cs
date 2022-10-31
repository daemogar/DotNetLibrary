using Microsoft.AspNetCore.Http;

namespace DotNetLibrary.Microsoft.AspNetCore.Http;

internal class NullHttpResponse : HttpResponse
{
    public override IResponseCookies Cookies { get; }

    #region Ignored Properties and Methods

    public override HttpContext HttpContext { get; }
    public override int StatusCode { get; set; }
    public override IHeaderDictionary Headers { get; }
    public override Stream Body { get; set; }
    public override long ContentLength { get; set; }
    public override string ContentType { get; set; }
    public override bool HasStarted { get; }
    public override void OnCompleted(Func<object, Task> callback, object state) { }
    public override void OnStarting(Func<object, Task> callback, object state) { }
    public override void Redirect(string location, bool permanent) { }

    #endregion

    internal NullHttpResponse(IResponseCookies cookies)
    {
        Cookies = cookies;
    }
}
