using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GXCSharp
{
    public class CGXCData<T> where T : class
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
