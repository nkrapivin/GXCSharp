using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ILoveGXC
{
    class MyFileImpl
    {
        [JsonPropertyName("SECRET_refresh_token")]
        public string? RefreshToken { get; set; }

        public static async Task<MyFileImpl> Fetch()
        {
            using (var fs = new FileStream("secret_data.json", FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, 4096, true))
            {
                try
                {
                    MyFileImpl? impl = await JsonSerializer.DeserializeAsync<MyFileImpl>(fs);
                    if (impl is null)
                    {
                        return new MyFileImpl();
                    }
                    else
                    {
                        return impl;
                    }
                }
                catch
                {
                    return new MyFileImpl();
                }
            }
        }

        public static async Task Push(MyFileImpl data)
        {
            using (var fs = new FileStream("secret_data.json", FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await JsonSerializer.SerializeAsync(fs, data);
                await fs.FlushAsync();
            }
        }
    }
}
