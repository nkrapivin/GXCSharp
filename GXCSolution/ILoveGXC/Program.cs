using GXCSharp;
using System.Diagnostics;

namespace ILoveGXC
{
    public static class Program
    {
        public static async Task<int> Main()
        {
            Console.WriteLine("gxc test:");

            // delegate to open a url.
            DOpenUrl WINDOWSurlopener =
                 (string urlToOpen) => { Process.Start(new ProcessStartInfo(urlToOpen) { UseShellExecute = true })?.Dispose(); };

            MyFileStorage mfs = new();

            // class to auth.
            CGXCAuthenticator myauth = new CGXCAuthenticator(WINDOWSurlopener, mfs);

            // class to main gxc api.
            if (await myauth.Authenticate() is CGXCApi myapi)
            {
                Console.WriteLine("auth ok!");

            }
            else
            {
                Console.WriteLine("TASK HAS FAILED ..?");
            }


            return 0;
        }
    }
}



