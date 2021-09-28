using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GXCSharp
{
    public class CGXCGame
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
    }
}
