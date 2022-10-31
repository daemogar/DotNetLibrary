using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.JSInterop;

using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;

namespace DotNetLibrary.Microsoft.AspNetCore.Http;

internal class NullHttpContext : HttpContext
{
    private JSRuntimeCookieCollection Cookies { get; }

    public override HttpRequest Request { get; }

    public override HttpResponse Response { get; }

    #region Ignored Properties and Methods

    public override IFeatureCollection Features { get; }
    public override ConnectionInfo Connection { get; }
    public override WebSocketManager WebSockets { get; }
    public override AuthenticationManager Authentication { get; }
    public override ClaimsPrincipal User { get; set; }
    public override IDictionary<object, object> Items { get; set; }
    public override IServiceProvider RequestServices { get; set; }
    public override CancellationToken RequestAborted { get; set; }
    public override string TraceIdentifier { get; set; }
    public override ISession Session { get; set; }

    public override void Abort() { }

    #endregion

    internal NullHttpContext(IJSRuntime runtime)
    {
        Cookies = new JSRuntimeCookieCollection(runtime);

        Request = new NullHttpRequest(Cookies);
        Response = new NullHttpResponse(Cookies);
    }
}