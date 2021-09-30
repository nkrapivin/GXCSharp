using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GXCSharp
{
    class CGXCHttpListenerDummy : IGXCHttpListener
    {
        public async Task<string?> ListenOn(string urlToUse)
        {
            try
            {
                using (var hl = new HttpListener())
                {
                    hl.Prefixes.Add(urlToUse);
                    hl.Start();

                    var ctx = await hl.GetContextAsync();
                    var ctx_req = ctx.Request.RawUrl;

                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                    ctx.Response.StatusDescription = "OK";
                    ctx.Response.ContentEncoding = Encoding.UTF8;

                    // a simple html...
                    var bytes_send = Encoding.UTF8.GetBytes("<!DOCTYPE html><html lang=\"en\"><body align=\"center\"><h1>Done!</h1>Please return to your app.<br/>GXCSharp by nkrapivindev</body></html>");

                    ctx.Response.ContentType = "text/html";
                    ctx.Response.ContentLength64 = bytes_send.Length;
                    await ctx.Response.OutputStream.WriteAsync(bytes_send, 0, bytes_send.Length);
                    ctx.Response.Close();

                    return ctx_req;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
