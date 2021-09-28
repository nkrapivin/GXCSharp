using System;
using System.Collections.Generic;
using System.Text;

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


    }
}
