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
            DOpenUrl WINDOWSurlopener = MyOpenUrl.Get();

            MyFileStorage mfs = new();

            // class to auth.
            CGXCAuthenticator myauth = new(WINDOWSurlopener, mfs);

            Console.WriteLine("You will be asked to authenticate through the browser (for the first time).");

            // class to main gxc api.
            if (await myauth.Authenticate() is CGXCApi myapi)
            {
                Console.WriteLine("OAuth/V2 flow OK!");

                var gameslist = await myapi.ListGames();
                if (gameslist.Success && gameslist.Value is not null && gameslist.Value.Data is not null)
                {
                    Console.WriteLine("Your current games:");
                    for (int i = 0; i < gameslist.Value.Data.Length; ++i)
                    {
                        Console.WriteLine($"[{i,3}] | Id = {gameslist.Value.Data[i].Id}, Name = '{gameslist.Value.Data[i].Name}'.");
                    }
                    Console.WriteLine();

                    Console.Write("Enter the index of the game you wish to update (0,1,2,etc): ");
                    if (Console.ReadLine() is string myline && myline.Length > 0)
                    {
                        // awful, I know...
                        if (int.TryParse(myline, out int gameind) && gameind >= 0 && gameind < myline.Length
                            && gameslist.Value.Data[gameind] is CGXCGame mygame && mygame.Id is Guid myguid)
                        {
                            Console.WriteLine($"Preparing upload for game '{mygame.Name}'...");
                            Console.WriteLine("Drag and drop the zip file you wish to upload, or type full path to it:");
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
                        else
                        {
                            Console.WriteLine("Not a valid index... cheers.");
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



