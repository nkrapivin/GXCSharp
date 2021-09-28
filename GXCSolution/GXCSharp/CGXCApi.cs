using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GXCSharp
{
    /// <summary>
    /// This is the class that operates with the GXC API, once authenticated.
    /// It cannot be constructed from the public API, an authenticator approach must be used.
    /// </summary>
    public class CGXCApi
    {
        private CGXCAuthenticatorReply MyReply { get; set; }

        internal CGXCApi(CGXCAuthenticatorReply _myreply)
        {
            MyReply = _myreply;
        }

        private void AddAuth(HttpWebRequest hwr)
        {
            hwr.Headers.Add(HttpRequestHeader.Authorization, $"{MyReply.TokenType} {MyReply.AccessToken}");
        }

        private bool CheckHttpCode(HttpStatusCode hsc) => (int)hsc >= 200 && (int)hsc <= 299;

        public async Task<CGXCResult<CGXCData<CGXCGameCreateResult>>> UploadGame(Guid gameid, Stream zipstream)
        {
            string myurl = $"{CGXCAuthenticator.DEFAULT_API_SERVER}gms/games/{gameid:D}/bundles/upload";
            // TODO:
            return CGXCResult<CGXCData<CGXCGameCreateResult>>.Fail(EGXCErrorCode.INTERNAL);
        }

        public async Task<CGXCResult<CGXCData<CGXCGameCreateResult>>> CreateGame(string lang, string gameName)
        {
            var hwr = WebRequest.CreateHttp($"{CGXCAuthenticator.DEFAULT_API_SERVER}gms/games/create");
            AddAuth(hwr);

            hwr.Method = "POST";
            hwr.ContentType = "application/json";
            hwr.Accept = "*/*";

            var obj = new Dictionary<string, string>
            {
                ["lang"] = lang,
                ["name"] = gameName
            };

            await JsonSerializer.SerializeAsync(await hwr.GetRequestStreamAsync(), obj);

            try
            {
                var hwres = (HttpWebResponse)await hwr.GetResponseAsync();
                if (!CheckHttpCode(hwres.StatusCode))
                {
                    return CGXCResult<CGXCData<CGXCGameCreateResult>>.Fail(EGXCErrorCode.INTERNAL);
                }

                using (var st = hwres.GetResponseStream())
                {
                    var ret = await JsonSerializer.DeserializeAsync<CGXCData<CGXCGameCreateResult>>(st);
                    if (ret is null)
                    {
                        return CGXCResult<CGXCData<CGXCGameCreateResult>>.Fail(EGXCErrorCode.INTERNAL);
                    }

                    return CGXCResult<CGXCData<CGXCGameCreateResult>>.Ok(ret);
                }
            }
            catch
            {
                return CGXCResult<CGXCData<CGXCGameCreateResult>>.Fail(EGXCErrorCode.INTERNAL);
            }
        }

        public async Task<CGXCResult<CGXCData<CGXCGame[]>>> ListGames()
        {
            var hwr = WebRequest.CreateHttp($"{CGXCAuthenticator.DEFAULT_API_SERVER}gms/games");
            AddAuth(hwr);

            hwr.Method = "GET";

            try
            {
                var hwres = (HttpWebResponse)await hwr.GetResponseAsync();

                if (!CheckHttpCode(hwres.StatusCode))
                {
                    return CGXCResult<CGXCData<CGXCGame[]>>.Fail(EGXCErrorCode.INTERNAL);
                }

                using (var st = hwres.GetResponseStream())
                {
                    var ret = await JsonSerializer.DeserializeAsync<CGXCData<CGXCGame[]>>(st);
                    if (ret is null)
                    {
                        return CGXCResult<CGXCData<CGXCGame[]>>.Fail(EGXCErrorCode.INTERNAL);
                    }

                    return CGXCResult<CGXCData<CGXCGame[]>>.Ok(ret);
                }
            }
            catch
            {
                return CGXCResult<CGXCData<CGXCGame[]>>.Fail(EGXCErrorCode.INTERNAL);
            }
        }
    }
}
