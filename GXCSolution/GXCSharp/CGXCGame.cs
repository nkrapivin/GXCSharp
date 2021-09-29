using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GXCSharp
{
    /// <summary>
    /// A descriptor for a GXC game.
    /// </summary>
    public class CGXCGame
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("publicShareUrl")]
        public string? PublicShareUrl { get; set; }

        [JsonPropertyName("internalShareUrl")]
        public string? InternalShareUrl { get; set; }

        [JsonPropertyName("editUrl")]
        public string? EditUrl { get; set; }

        [JsonPropertyName("lang")]
        public string? Lang { get; set; }

        [JsonPropertyName("group")]
        public string? Group { get; set; }
    }
}
