﻿using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class AirtableAttachment
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("size")]
        public long? Size { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("thumbnails")]
        public Thumbnails? Thumbnails { get; set; }
    }


    //--------------------------------------------------------

    public class Thumbnails
    {
        [JsonPropertyName("small")]
        [JsonInclude]
        public Thumbnail? Small { get; set; }

        [JsonPropertyName("large")]
        [JsonInclude]
        public Thumbnail? Large { get; set; }
    }


    //--------------------------------------------------------

    public class Thumbnail
    {
        [JsonPropertyName("url")]
        [JsonInclude]
        public string? Url { get; set; }

        [JsonPropertyName("width")]
        [JsonInclude]
        public long Width { get; set; }

        [JsonPropertyName("height")]
        [JsonInclude]
        public long Height { get; set; }
    }

}
