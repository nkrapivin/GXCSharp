using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        private readonly HttpClient MyClient = new HttpClient();

        private string MyServer { get; set; } = CGXCAuthenticator.DEFAULT_API_SERVER;

        internal CGXCApi(CGXCAuthenticatorReply _myreply, string _serverstring)
        {
            MyReply = _myreply;
            MyClient.DefaultRequestHeaders.Add("Authorization", $"{MyReply.TokenType} {MyReply.AccessToken}");
            MyServer = _serverstring;
        }

        private void AddAuth(HttpWebRequest hwr)
        {
            hwr.Headers.Add(HttpRequestHeader.Authorization, $"{MyReply.TokenType} {MyReply.AccessToken}");
        }

        private bool CheckHttpCode(HttpStatusCode hsc) => (int)hsc >= 200 && (int)hsc <= 299;

        public async Task<CGXCResult<CGXCData<CGXCGame>>> UploadGame(Guid gameGuid, Stream zipStream, DGXCProgress? progressDel = null)
        {
            try
            {
                using (var hc = new StreamContent(zipStream))
                using (var mfdc = new MultipartFormDataContent())
                {
                    var myurl = $"{MyServer}gms/games/{gameGuid:D}/bundles/upload";
                    hc.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
                    mfdc.Add(hc, "file", "GameBundle.zip");

                    // initialize with 0% and length.
                    if (progressDel is DGXCProgress)
                        progressDel(0, zipStream.Length);

                    var taskreply = await MyClient.PostAsync(myurl, mfdc);

                    if (taskreply.IsSuccessStatusCode)
                    {
                        using (var replys = await taskreply.Content.ReadAsStreamAsync())
                        {
                            var jsonreply = await JsonSerializer.DeserializeAsync<CGXCData<CGXCGame>>(replys);
                            if (jsonreply is null)
                            {
                                return CGXCResult<CGXCData<CGXCGame>>.Fail(EGXCErrorCode.INTERNAL);
                            }
                            else
                            {
                                return CGXCResult<CGXCData<CGXCGame>>.Ok(jsonreply);
                            }
                        }
                    }
                }

                return CGXCResult<CGXCData<CGXCGame>>.Fail(EGXCErrorCode.INTERNAL);
            }
            catch
            {
                // TODO:
                return CGXCResult<CGXCData<CGXCGame>>.Fail(EGXCErrorCode.INTERNAL);
            }
        }

        public async Task<CGXCResult<CGXCData<CGXCGame>>> CreateGame(string lang, string gameName)
        {
            var hwr = WebRequest.CreateHttp($"{MyServer}gms/games/create");
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
                    return CGXCResult<CGXCData<CGXCGame>>.Fail(EGXCErrorCode.INTERNAL);
                }

                using (var st = hwres.GetResponseStream())
                {
                    var ret = await JsonSerializer.DeserializeAsync<CGXCData<CGXCGame>>(st);
                    if (ret is null)
                    {
                        return CGXCResult<CGXCData<CGXCGame>>.Fail(EGXCErrorCode.INTERNAL);
                    }

                    return CGXCResult<CGXCData<CGXCGame>>.Ok(ret);
                }
            }
            catch
            {
                return CGXCResult<CGXCData<CGXCGame>>.Fail(EGXCErrorCode.INTERNAL);
            }
        }

        public async Task<CGXCResult<CGXCData<CGXCGame[]>>> ListGames()
        {
            var hwr = WebRequest.CreateHttp($"{MyServer}gms/games");
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
