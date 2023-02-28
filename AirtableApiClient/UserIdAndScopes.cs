using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{ 
    public class UserIdAndScopes
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("scopes")]
        public ICollection<string> Scopes { get; set; }
    }
}
