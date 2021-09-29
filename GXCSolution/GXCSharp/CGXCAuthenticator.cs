using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GXCSharp
{
    /// <summary>
    /// This class is responsible for GXC authentication.
    /// You must use it before using any of the other GXC Api classes!
    /// </summary>
    public class CGXCAuthenticator
    {
        /// <summary>
        /// Staging authentication server.
        /// </summary>
        public static readonly string DEFAULT_TEST_AUTH_SERVER = "https://oauth2.op-test.net/oauth2/v1/";

        /// <summary>
        /// Production authentication server.
        /// </summary>
        public static readonly string DEFAULT_AUTH_SERVER = "https://oauth2.opera-api.com/oauth2/v1/";

        /// <summary>
        /// API Server, same defined in OAuth scopes.
        /// </summary>
        public static readonly string DEFAULT_API_SERVER = "https://api.gmx.dev/";

        /// <summary>
        /// Default redirect URL, the library is responsible for the OAuth flow.
        /// </summary>
        private static readonly string DEFAULT_REDIRECT_URL = "http://localhost:8889/";


        /// <summary>
        /// Default OAuth scopes.
        /// </summary>
        private static readonly string DEFAULT_SCOPES = "user+https://api.gmx.dev/gms:read+https://api.gmx.dev/gms:write";

        /// <summary>
        /// Default client id used for authentication flow, must be set to game-maker.
        /// </summary>
        private static readonly string DEFAULT_CLIENT_TYPE = "game-maker";

        /// <summary>
        /// Message showed to the client after successful OAuth flow.
        /// </summary>
        private static readonly string DEFAULT_REPLY_MESSAGE = "GXCSharp: OAuth OK. Please return to your application for further instructions.";

        private string AuthServer { get; set; } = DEFAULT_AUTH_SERVER;

        private string ReplyMessage { get; set; } = DEFAULT_REPLY_MESSAGE;

        private static readonly Random RANDOM = new Random();

        private string MyNonce { get; set; } = "";

        private DOpenUrl OpenUrlDelegate { get; set; } = (string _) => { };

        private IGXCFileStorage MyFileStorage { get; set; } = new CGXCFileStorageDummy();

        private static string GenerateRandomNonce()
        {
            const string VALID_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const int NONCE_LENGTH = 32;

            char[] nonce = new char[NONCE_LENGTH];
            for (int i = 0; i < nonce.Length; ++i)
                nonce[i] = VALID_CHARS[RANDOM.Next(0, VALID_CHARS.Length)];

            return new string(nonce);
        }

        public CGXCAuthenticator(DOpenUrl openUrlDelegate, IGXCFileStorage? fileStorage = null, string? authServerString = null, string? replyMessage = null)
        {
            if (authServerString is string _as)
            {
                AuthServer = _as;
            }

            if (replyMessage is string _rm)
            {
                ReplyMessage = _rm;
            }

            if (fileStorage is IGXCFileStorage _fs)
            {
                MyFileStorage = _fs;
            }

            OpenUrlDelegate = openUrlDelegate;
            MyNonce = GenerateRandomNonce();
        }

        private async Task<CGXCApi?> RefreshAuthenticate(string reftoken)
        {
            try
            {
                //string text = $"grant_type=refresh_token&refresh_token={m_OAUTH2_refresh_token}&client_id={clientID}&client_secret{clientSecret}";
                string tokenurl = $"grant_type=refresh_token&refresh_token={reftoken}&client_id={DEFAULT_CLIENT_TYPE}&client_secret=";
                HttpWebRequest webreq = WebRequest.CreateHttp($"{AuthServer}token/");
                webreq.Method = "POST";
                webreq.ContentType = "application/x-www-form-urlencoded";
                await (await webreq.GetRequestStreamAsync()).WriteAsync(Encoding.UTF8.GetBytes(tokenurl));
                var webreply = await webreq.GetResponseAsync();
                var jsonreply = await JsonSerializer.DeserializeAsync<CGXCAuthenticatorReply>(webreply.GetResponseStream());
                webreply.Dispose();

                if (jsonreply is null)
                {
                    return null;
                }
                else
                {
                    return new CGXCApi(jsonreply);
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<CGXCApi?> Authenticate()
        {
            var myrefreshtoken = await MyFileStorage.GetProperty(EGXCFileProperty.REFRESH_TOKEN);
            if (myrefreshtoken is null)
            {
                // will try to reoauth from the very beginning (+httplistener)
                return await NewAuthenticate();
            }
            else
            {
                if (await RefreshAuthenticate(myrefreshtoken) is CGXCApi _cgxc)
                {
                    return _cgxc;
                }
                else
                {
                    // refresh token had expired..?
                    //await MyFileStorage.SetProperty(EGXCFileProperty.REFRESH_TOKEN, null);
                    return null;
                }
            }
        }

        private async Task<CGXCApi?> NewAuthenticate()
        {
            try
            {
                if (!HttpListener.IsSupported) return null;

                var rawreply = Encoding.UTF8.GetBytes(ReplyMessage);
                HttpListener hl = new HttpListener();

                try
                {
                    hl.Prefixes.Add(DEFAULT_REDIRECT_URL);
                    hl.Start();
                }
                catch
                {
                    return null;
                }

                string theurl = $"{AuthServer}authorize?response_type=code&client_id={DEFAULT_CLIENT_TYPE}&redirect_uri={DEFAULT_REDIRECT_URL}&state={MyNonce}&scope={DEFAULT_SCOPES}";
                OpenUrlDelegate(theurl);

                var hlctx = await hl.GetContextAsync();
                var hlreply = hlctx.Response;
                var hlmsg = hlctx.Request;
                var gxccode = hlmsg.QueryString["code"];

                hlreply.ContentLength64 = rawreply.Length;
                await hlreply.OutputStream.WriteAsync(rawreply);
                hlreply.OutputStream.Close();
                hlreply.Close();

                string tokenurl = $"grant_type=authorization_code&code={gxccode}&redirect_uri={DEFAULT_REDIRECT_URL}&scope={DEFAULT_SCOPES}&client_id={DEFAULT_CLIENT_TYPE}&client_secret=";
                HttpWebRequest webreq = WebRequest.CreateHttp($"{AuthServer}token/");
                webreq.Method = "POST";
                webreq.ContentType = "application/x-www-form-urlencoded";
                await (await webreq.GetRequestStreamAsync()).WriteAsync(Encoding.UTF8.GetBytes(tokenurl));
                var webresponse = await webreq.GetResponseAsync();
                var jsonreply = await JsonSerializer.DeserializeAsync<CGXCAuthenticatorReply>(webresponse.GetResponseStream());
                webresponse.Dispose();
                hl.Stop();

                if (jsonreply is null)
                {
                    return null;
                }
                else
                {
                    await MyFileStorage.SetProperty(EGXCFileProperty.REFRESH_TOKEN, jsonreply.RefreshToken);
                    return new CGXCApi(jsonreply);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
