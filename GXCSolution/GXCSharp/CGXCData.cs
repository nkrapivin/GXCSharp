using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GXCSharp
{
    /// <summary>
    /// This class wraps API responses from GXC. The actual data is in the .Data member.
    /// </summary>
    /// <typeparam name="T">Type of API reply</typeparam>
    public class CGXCData<T> where T : class
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("errors")]
        public string[]? Errors { get; set; }
    }
}
