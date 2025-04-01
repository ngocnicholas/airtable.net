using System;
using System.Collections.Generic;
using AirtableApiClient;

namespace AirtableApiClient.Tests
{
    public class GetBaseSchemaTestResponse
    {
        public GetBaseSchemaTestResponse(bool success, string errorMessage, List<TableModel> tables)
        {
            Success = success;
            ErrorMessage = errorMessage;
            Tables = tables;
        }

        public bool Success { get; set; }
        public String ErrorMessage { get; set; }
        public List<TableModel> Tables { get; set; }
    }
}
