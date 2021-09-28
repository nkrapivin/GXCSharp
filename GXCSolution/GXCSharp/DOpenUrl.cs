using System;
using System.Collections.Generic;
using System.Text;

namespace GXCSharp
{
    /// <summary>
    /// A delegate whose sole purpose is to open a URL so the httplistener can catch up.
    /// </summary>
    /// <param name="urlToOpen">URL to open, is never null.</param>
    public delegate void DOpenUrl(string urlToOpen);
}
