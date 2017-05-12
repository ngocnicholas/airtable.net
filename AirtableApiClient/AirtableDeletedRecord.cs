using Newtonsoft.Json;

namespace AirtableApiClient
{
    internal class AirtableDeletedRecord
    {
        [JsonProperty("deleted")]
        internal bool Deleted { get; set; }

        [JsonProperty("id")]
        internal string Id { get; set; }
    }
}
