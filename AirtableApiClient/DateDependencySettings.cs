using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AirtableApiClient
{
    public  class DateDependencySettings
    {
        [JsonPropertyName("durationFieldId")]
        [JsonInclude]
        public string? DurationFieldId {  get; set; }

        [JsonPropertyName("endDateFieldId")]
        [JsonInclude]
        public string? EndDateFieldId { get; set; }

        [JsonPropertyName("isEnabled")]
        [JsonInclude]

        public bool IsEnabled {  get; set; }


        [JsonPropertyName("predecessorFieldId")]
        [JsonInclude]
        public string? PredecessorFieldId {  get; set; }


        [JsonPropertyName("reschedulingMode")]
        [JsonInclude]
        public string? ReschedulingMode { get; set; }        // "flexible" | "fixed" | "none"


        [JsonPropertyName("shouldSkipWeekendsAndHolidays")]
        [JsonInclude]
        public bool ShouldSkipWeekendsAndHolidays { get; set; }


        [JsonPropertyName("startDateFieldId")]
        [JsonInclude]
        public string? StartDateFieldId { get; set; }


        [JsonPropertyName("holidays")]
        [JsonInclude]
        public string[]? Holidays { get; set;  }
    }
}
