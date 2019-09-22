using System;
using System.Collections.Generic;
using AirtableApiClient;

namespace AirtableApiClient.Tests
{
    public class ListAllRecordsTestResponse
    {
        public ListAllRecordsTestResponse(bool success, string errorMessage, List<AirtableRecord> records)
        {
            Success = success;
            ErrorMessage = errorMessage;
            Records = records;
        }

        public bool Success { get; set; }
        public String ErrorMessage { get; set; }
        public List<AirtableRecord> Records { get; set; }
    }
}
