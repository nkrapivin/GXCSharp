using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GXCSharp
{
    /// <summary>
    /// An interface that implements a one-time Http Listener serving for GXCSharp.
    /// One might use Nginx with that, dunno.
    /// </summary>
    public interface IGXCHttpListener
    {
        /// <summary>
        /// Listens for a query string on a specified URL.
        /// </summary>
        /// <param name="urlToUse">URL to use</param>
        /// <returns>A query string or null</returns>
        Task<string?> ListenOn(string urlToUse);
    }
}
