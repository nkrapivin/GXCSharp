using GXCSharp;
using System.Diagnostics;

namespace ILoveGXC
{
    public static class Program
    {
        public static void PrintGXCGameArr(CGXCGame[] vv)
        {
            Console.WriteLine("Games:");
            foreach (var gg in vv)
            {
                Console.WriteLine($"ID = {gg.Id}, Name = {gg.Name}");
            }
            Console.WriteLine("Listing end.");
        }

        public static async Task<int> Main()
        {
            Console.WriteLine("GXCSharp test:");

            // delegate to open a url.
            DOpenUrl WINDOWSurlopener = MyOpenUrl.Get();

            MyFileStorage mfs = new();

            // class to auth.
            CGXCAuthenticator myauth = new(WINDOWSurlopener, mfs);

            // class to main gxc api.
            if (await myauth.Authenticate() is CGXCApi myapi)
            {
                Console.WriteLine("auth ok!");

                var gameslist = await myapi.ListGames();
                if (gameslist.Success && gameslist.Value is not null && gameslist.Value.Data is not null)
                {
                    PrintGXCGameArr(gameslist.Value.Data);
                }
                else
                {
                    Console.WriteLine($"Request failed, errcode={gameslist.Error}");
                }

                Console.Write("Do you wish to make a new game? If so, type it's name: ");
                var line = Console.ReadLine();

                if (line is string && line.Length > 1)
                {
                    Console.WriteLine("okay...");


                    /*
                     * STRESSTEST, DO NOT EXECUTE.
                    for (int i = 0; i < 10000000; ++i)
                    {
                        await myapi.CreateGame("en", $"bruh moment number {i}");
                    }
                     */

                    var res = await myapi.CreateGame("en", line);

                    if (res.Success && res.Value is not null && res.Value.Data is not null)
                    {
                        Console.WriteLine($"Game Name - {res.Value.Data.Name}");
                        Console.WriteLine($"Edit URL  - {res.Value.Data.EditUrl}");
                        Console.WriteLine($"Game Lang - {res.Value.Data.Lang}");
                    }
                    else
                    {
                        Console.WriteLine("An error has occurred..?");
                    }

                    Console.WriteLine("Test complete!");
                }
            }
            else
            {
                Console.WriteLine("TASK HAS FAILED ..?");
            }

            Console.ReadKey(true);


            return 0;
        }
    }
}



