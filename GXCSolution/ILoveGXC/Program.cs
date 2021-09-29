using GXCSharp;

namespace ILoveGXC
{
    public static class Program
    {
        public static async Task<int> Main()
        {
            Console.WriteLine("GXCSharp/Uploader Test:");
            Console.WriteLine("(gxcsharp by nkrapivindev, gxc by opera lol)");

            // delegate to open a url.
            DGXCOpenUrl WINDOWSurlopener = MyOpenUrl.Get();

            MyFileStorage mfs = new();

            // class to auth.
            CGXCAuthenticator myauth = new(WINDOWSurlopener, mfs);

            Console.WriteLine("You will be asked to authenticate through the browser (for the first time).");

            CGXCResult<CGXCData<CGXCGame[]>>? gameslist = null;

            // class to main gxc api.
            if (await myauth.Authenticate() is CGXCApi myapi)
            {
                Console.WriteLine("OAuth/V2 flow OK!");

                // I am WAY too lazy to split this up into functions....
LDoList:
                gameslist = await myapi.ListGames();
                if (gameslist.Success && gameslist.Value is not null && gameslist.Value.Data is not null)
                {
                    Console.WriteLine("Your current games:");
                    for (int i = 0; i < gameslist.Value.Data.Length; ++i)
                    {
                        Console.WriteLine($"[{i,3}] | Id = {gameslist.Value.Data[i].Id}, Name = '{gameslist.Value.Data[i].Name}'.");
                    }
                    Console.WriteLine();

                    Console.Write("Enter the index of the game you wish to update OR -1 to make a new (0,1,etc): ");
                    if (Console.ReadLine() is string myline && myline.Length > 0)
                    {
                        bool gok = int.TryParse(myline, out int gameind);
                        // awful, I know...
                        if (gok && gameind >= 0 && gameind < gameslist.Value.Data.Length
                            && gameslist.Value.Data[gameind] is CGXCGame mygame && mygame.Id is Guid myguid)
                        {
                            Console.WriteLine($"Preparing upload for game '{mygame.Name}'...");
                            Console.WriteLine("Drag and drop the ZIP file you wish to upload, or type full path to it:");
                            if (Console.ReadLine() is string mypath && mypath.Length > 0)
                            {
                                Console.WriteLine($"Going to upload {mypath}, close the program if you don't want that.");
                                Console.Write("Press any key to continue . . . ");
                                Console.ReadKey(true);

                                using (var fs = new FileStream(mypath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true))
                                {
                                    Console.WriteLine("UPLOADING, DO NOT CLOSE THE PROGRAM...");
                                    await myapi.UploadGame(myguid, fs);
                                }

                                Console.WriteLine("Upload done, please see GXC.");
                            }
                            else
                            {
                                Console.WriteLine("Upload cancelled, good!");
                            }
                        }
                        else if (gameind != -1)
                        {
                            Console.WriteLine("Not a valid index... cheers.");
                        }
                        else
                        {
                            Console.WriteLine("Type the name of your new game (or close me to cancel):");
                            if (Console.ReadLine() is string gamename && gamename.Length > 1)
                            {
                                Console.WriteLine($"Creating game '{gamename}'...");
                                var ok = (await myapi.CreateGame("en", gamename)).Success;
                                if (ok)
                                {
                                    Console.WriteLine("Game created, refreshing list...");
                                    // I know... I know...
                                    goto LDoList;
                                }
                                else
                                {
                                    Console.WriteLine("Game creation fail!");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No.. input given? Okay, see ya!");
                    }
                }
                else
                {
                    Console.WriteLine($"Request failed, errcode={gameslist.Error}");
                }
            }
            else
            {
                Console.WriteLine("TASK HAS FAILED ..?");
            }

            Console.Write("Press any key to exit . . . ");
            Console.ReadKey(true);

            return 0;
        }
    }
}



