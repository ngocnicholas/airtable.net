using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{ 
    public class UserIdAndScopes
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("scopes")]
        [JsonInclude]
        public ICollection<string> Scopes { get; internal set; }
    }
}
