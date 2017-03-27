using Newtonsoft.Json;

namespace AirtableApiClient
{
    public class AirtableAttachment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("size")]
        public long? Size { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("thumbnails")]
        public Thumbnails Thumbnails { get; set; }
    }


    //--------------------------------------------------------

    public class Thumbnails
    {
        [JsonProperty("small")]
        public Thumbnail Small { get; internal set; }

        [JsonProperty("large")]
        public Thumbnail Large { get; internal set; }
    }


    //--------------------------------------------------------

    public class Thumbnail
    {
        [JsonProperty("url")]
        public string Url { get; internal set; }

        [JsonProperty("width")]
        public long Width { get; internal set; }

        [JsonProperty("height")]
        public long Height { get; internal set; }
    }

}
