using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{ 
    public class UserIdAndScopes
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; set; }

        [JsonPropertyName("scopes")]
        [JsonInclude]
        public ICollection<string> Scopes { get; set; }
    }
}
