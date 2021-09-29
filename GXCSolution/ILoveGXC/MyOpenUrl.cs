using GXCSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILoveGXC
{
    public static class MyOpenUrl
    {
        private static void Implementation(string urlToOpen)
        {
            var proc = Process.Start(new ProcessStartInfo(urlToOpen) { UseShellExecute = true });
            if (proc is not null)
            {
                proc.Dispose();
            }
        }

        public static DGXCOpenUrl Get() => Implementation;
    }
}
