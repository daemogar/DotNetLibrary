using Microsoft.AspNetCore.Http;

namespace DotNetLibrary.Microsoft.AspNetCore.Http;

internal class NullHttpRequest : HttpRequest
{
    public override IRequestCookieCollection Cookies { get; set; }

    #region Ignored Properties and Methods

    public override HttpContext HttpContext { get; }
    public override string Method { get; set; }
    public override string Scheme { get; set; }
    public override bool IsHttps { get; set; }
    public override HostString Host { get; set; }
    public override PathString PathBase { get; set; }
    public override PathString Path { get; set; }
    public override QueryString QueryString { get; set; }
    public override IQueryCollection Query { get; set; }
    public override string Protocol { get; set; }
    public override IHeaderDictionary Headers { get; }
    public override long ContentLength { get; set; }
    public override string ContentType { get; set; }
    public override Stream Body { get; set; }
    public override bool HasFormContentType { get; }
    public override IFormCollection Form { get; set; }
    public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
        => ReadFormAsync(cancellationToken);

    #endregion

    internal NullHttpRequest(IRequestCookieCollection cookies)
    {
        Cookies = cookies;
    }
}
