using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    internal class AirtableDeletedRecord
    {
        [JsonPropertyName("deleted")]
        [JsonInclude]
        public bool Deleted { get; set; }

        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; set; }
    }
}
