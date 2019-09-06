using System;
using System.Collections.Generic;
using AirtableApiClient;

namespace AirtableApiClient.Tests
{
    public class ListAllRecordsTestResponse<T>
    {
        public ListAllRecordsTestResponse(bool success, string errorMessage, List<AirtableRecord<T>> records)
        {
            Success = success;
            ErrorMessage = errorMessage;
            Records = records;
        }

        protected ListAllRecordsTestResponse(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }


        public bool Success { get; set; }
        public String ErrorMessage { get; set; }
        public List<AirtableRecord<T>> Records { get; set; }
    }


    public class ListAllRecordsTestResponse : ListAllRecordsTestResponse<Dictionary<string, object>>
    {
        public ListAllRecordsTestResponse(bool success, string errorMessage, List<AirtableRecord> records) : base(success, errorMessage)
        {
            Records = records;
        }

        public new List<AirtableRecord> Records { get; set; }
    }
}
