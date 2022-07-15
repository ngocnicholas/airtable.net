using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;


namespace AirtableApiClient
{
    public class AirtableBase : IDisposable
    {
        private const int MAX_PAGE_SIZE = 100;
        private const int MAX_RECORD_OPERATION_SIZE = 10;

        private const string AIRTABLE_API_URL = "https://api.airtable.com/v0/";

        private readonly string BaseId;
        private readonly HttpClientWithRetries httpClientWithRetries;

        private readonly JsonSerializerOptions JsonOptionIgnoreNullValues = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, };

        public bool ShouldNotRetryIfRateLimited
        {
            get { return httpClientWithRetries.ShouldNotRetryIfRateLimited; }
            set { httpClientWithRetries.ShouldNotRetryIfRateLimited = value; }
        }

        public int RetryDelayMillisecondsIfRateLimited
        {
            get { return httpClientWithRetries.RetryDelayMillisecondsIfRateLimited; }
            set { httpClientWithRetries.RetryDelayMillisecondsIfRateLimited = value; }

        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.AirtableBase
        //    constructor -- for creating an instance of AirtableBase using the default
        //                   delegating handler.
        //
        //----------------------------------------------------------------------------

        public AirtableBase(string apiKey, string baseId) : this(apiKey, baseId, null)
        {
            // No delegating handler is given; a normal HttpClient will be constructed
            // to communicate with Airtable.
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.AirtableBase
        //    constructor -- for Unit tests and for users who want to pass in their own handler.
        //
        //----------------------------------------------------------------------------

        public AirtableBase(
            string apiKey,
            string baseId,
            DelegatingHandler delegatingHandler)
        {
            if (String.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("apiKey cannot be null", "apiKey");
            }

            if (String.IsNullOrEmpty(baseId))
            {
                throw new ArgumentException("baseId cannot be null", "baseId");
            }

            BaseId = baseId;
            httpClientWithRetries = new HttpClientWithRetries(delegatingHandler, apiKey);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ListRecords
        //
        // Called to get a list of records in the table specified by 'tableName'
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableListRecordsResponse> ListRecords(
            string tableName,
            string offset = null,
            IEnumerable<string> fields = null,
            string filterByFormula = null,
            int? maxRecords = null,
            int? pageSize = null,
            IEnumerable<Sort> sort = null,
            string view = null,
            string cellFormat = null,
            string timeZone = null,
            string userLocale = null,
            bool returnFieldsByFieldId = false
            )
        {
            HttpResponseMessage response = await ListRecordsInternal(tableName, offset, fields, filterByFormula,
                maxRecords, pageSize, sort, view,
                cellFormat, timeZone, userLocale, 
                returnFieldsByFieldId
                ).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableListRecordsResponse(error);
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            AirtableRecordList recordList = JsonSerializer.Deserialize<AirtableRecordList>(responseBody, JsonOptionIgnoreNullValues);
            return new AirtableListRecordsResponse(recordList);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ListRecords<T>
        //
        // Called to get a list of records<T> in the table specified by 'tableName'
        // The fields of each record are deserialized to type <T>.
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableListRecordsResponse<T>> ListRecords<T>(
            string tableName,
            string offset = null,
            IEnumerable<string> fields = null,
            string filterByFormula = null,
            int? maxRecords = null,
            int? pageSize = null,
            IEnumerable<Sort> sort = null,
            string view = null,
            string cellFormat = null,
            string timeZone = null,
            string userLocale = null,
            bool returnFieldsByFieldId = false
            )
        {
            HttpResponseMessage response = await ListRecordsInternal(tableName, offset, fields, filterByFormula,
                maxRecords, pageSize, sort, view, cellFormat, timeZone, userLocale, returnFieldsByFieldId).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableListRecordsResponse<T>(error);
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            AirtableRecordList<T> recordList = JsonSerializer.Deserialize<AirtableRecordList<T>>(responseBody);
            return new AirtableListRecordsResponse<T>(recordList);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.RetrieveRecord
        //
        // Called to retrieve a record with the specified id from the specified table.
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableRetrieveRecordResponse> RetrieveRecord(
            string tableName,
            string id)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table Name cannot be null", "tableName");
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Record ID cannot be null", "id");
            }

            string uriStr = AIRTABLE_API_URL + BaseId + "/" + Uri.EscapeDataString(tableName) + "/" + id;
            var request = new HttpRequestMessage(HttpMethod.Get, uriStr);
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableRetrieveRecordResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var airtableRecord = JsonSerializer.Deserialize<AirtableRecord>(responseBody, JsonOptionIgnoreNullValues);

            return new AirtableRetrieveRecordResponse(airtableRecord);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.RetrieveRecord<T>
        //
        // Called to retrieve a record with the specified id from the specified table.
        // The fields of the retrieved record are deserialized to type <T>.
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableRetrieveRecordResponse<T>> RetrieveRecord<T>(
            string tableName,
            string id)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table Name cannot be null", "tableName");
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Record ID cannot be null", "id");
            }

            string uriStr = AIRTABLE_API_URL + BaseId + "/" + Uri.EscapeDataString(tableName) + "/" + id;
            var request = new HttpRequestMessage(HttpMethod.Get, uriStr);
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableRetrieveRecordResponse<T>(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            AirtableRecord<T> airtableRecord = JsonSerializer.Deserialize<AirtableRecord<T>>(responseBody);

            return new AirtableRetrieveRecordResponse<T>(airtableRecord);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.CreateRecord
        //
        // Called to create a record in the specified table.
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableCreateUpdateReplaceRecordResponse> CreateRecord(
            string tableName,
            Fields fields,
            bool typecast = false)
        {
            return await (CreateUpdateReplaceRecord(tableName, fields, HttpMethod.Post, typecast: typecast)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.UpdateRecord
        //
        // Called to update a record with the specified ID in the specified table.
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableCreateUpdateReplaceRecordResponse> UpdateRecord(
            string tableName,
            Fields fields,
            string id,
            bool typeCast = false)
        {
            return await (CreateUpdateReplaceRecord(tableName, fields, new HttpMethod("PATCH"), id, typeCast)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ReplaceRecord
        //
        // Called to replace a record with the specified ID in the specified table using jsoncnotent .
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableCreateUpdateReplaceRecordResponse> ReplaceRecord(
            string tableName,
            Fields fields,
            string id,
            bool typeCast = false)
        {
            return await (CreateUpdateReplaceRecord(tableName, fields, HttpMethod.Put, id, typeCast)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.DeleteRecord
        //
        // Called to delete a record with the specified ID in the specified table
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableDeleteRecordResponse> DeleteRecord(
            string tableName,
            string id)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table name cannot be null", "tableName");
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Record ID cannot be null", "id");
            }

            string uriStr = AIRTABLE_API_URL + BaseId + "/" + Uri.EscapeDataString(tableName) + "/" + id;
            var request = new HttpRequestMessage(HttpMethod.Delete, uriStr);
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableDeleteRecordResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var deletedRecord = JsonSerializer.Deserialize<AirtableDeletedRecord>(responseBody);
            return new AirtableDeleteRecordResponse(deletedRecord.Deleted, deletedRecord.Id);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.CreateMultipleRecords
        //
        // Called to create multiple records in the specified table in one single operation.
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> CreateMultipleRecords(
            string tableName,
            Fields[] fields,
            bool typecast = false)
        {
            if (fields == null || fields.Length == 0 || fields.Length > MAX_RECORD_OPERATION_SIZE)
            {
                throw new ArgumentException(String.Format("Number of records to be created must be >0 && <= {0}", MAX_RECORD_OPERATION_SIZE));
            }
            var json = JsonSerializer.Serialize(new { records = fields, typecast = typecast }, JsonOptionIgnoreNullValues);
            return await (CreateUpdateReplaceMultipleRecords(tableName, HttpMethod.Post, json)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.CreateMultipleRecords (overload)
        //
        // Called to create all records specified in the provided record list.
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> CreateMultipleRecords(
            string tableName,
            AirtableRecord[] records,
            bool typecast = false)
        {
            if (records == null || records.Length == 0 || records.Length > MAX_RECORD_OPERATION_SIZE)
            {
                throw new ArgumentException("Record list cannot be null or empty or cannot have more than 10 records");
            }

            AirtableCreateUpdateReplaceMultipleRecordsResponse response = null;
            string json = null;
            List<Fields> fieldsList = new List<Fields>();       // Must use List<> so that we can expand it later.
            for (int index = 0; index < records.Length; index++)
            {
                AirtableRecord record = records[index];
                fieldsList.Add(new Fields());
                fieldsList[index].FieldsCollection = record.Fields;

                if (index == (records.Length - 1))              // Done filling fieldsList?
                {
                    json = JsonSerializer.Serialize(new { records = fieldsList.ToArray(), typecast = typecast }, JsonOptionIgnoreNullValues);
                    response = await (CreateUpdateReplaceMultipleRecords(tableName, HttpMethod.Post, json)).ConfigureAwait(false);
                    break;
                }
            }
            return response;
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.UpdateMultipleRecords
        //
        // Called to update multiple records with the specified IDs in the specified table in one single operation.
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> UpdateMultipleRecords(
            string tableName,
            IdFields[] idFields,
            bool typecast = false)
        {
            if (idFields == null || idFields.Length == 0 || idFields.Length > MAX_RECORD_OPERATION_SIZE)
            {
                throw new ArgumentException(String.Format("Number of records to be updated must be >0 && <= {0}", MAX_RECORD_OPERATION_SIZE));
            }

            var json = JsonSerializer.Serialize(new { records = idFields, typecast = typecast }, JsonOptionIgnoreNullValues);
            return await (CreateUpdateReplaceMultipleRecords(tableName, new HttpMethod("PATCH"), json)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.UpdateMultipleRecords (overload)
        //
        // Called to update all records specified in the provided record list.
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> UpdateMultipleRecords(
            string tableName,
            AirtableRecord[] records,
            bool typecast = false)
        {
            return await (UpdateReplaceRecordsFromList(tableName, records, new HttpMethod("PATCH"), typecast)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ReplaceMultipleRecords
        //
        // Called to update multiple records with the specified IDs in the specified table in one single operation.
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> ReplaceMultipleRecords(
            string tableName,
            IdFields[] idFields,
            bool typecast = false)
        {
            if (idFields == null || idFields.Length == 0 || idFields.Length > MAX_RECORD_OPERATION_SIZE)
            {
                throw new ArgumentException(String.Format("Number of records to be replaced must be >0 && <= {0}", MAX_RECORD_OPERATION_SIZE));
            }

            var json = JsonSerializer.Serialize(new { records = idFields, typecast = typecast }, JsonOptionIgnoreNullValues);
            return await (CreateUpdateReplaceMultipleRecords(tableName, HttpMethod.Put, json)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ReplaceMultipleRecords (overload)
        //
        // Called to replace all records specified in the provided record list.
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> ReplaceMultipleRecords(
            string tableName,
            AirtableRecord[] records,
            bool typecast = false)
        {
            return await (UpdateReplaceRecordsFromList(tableName, records, HttpMethod.Put, typecast)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.Dispose
        //
        //----------------------------------------------------------------------------
        public void Dispose()
        {
            httpClientWithRetries.Dispose();
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.BuildUriForListRecords
        //
        // Build URI for the List Records operation
        //
        //----------------------------------------------------------------------------
        private Uri BuildUriForListRecords(
            string tableName,
            string offset,
            IEnumerable<string> fields,
            string filterByFormula,
            int? maxRecords,
            int? pageSize,
            IEnumerable<Sort> sort,
            string view,
            string cellFormat,
            string timeZone,
            string userLocale,
            bool returnFieldsByFieldId)
        {
            var uriBuilder = new UriBuilder(AIRTABLE_API_URL + BaseId + "/" + Uri.EscapeDataString(tableName));

            if (!string.IsNullOrEmpty(offset))
            {
                AddParametersToQuery(ref uriBuilder, $"offset={HttpUtility.UrlEncode(offset)}");
            }

            if (fields != null)
            {
                string flattenFieldsParam = QueryParamHelper.FlattenFieldsParam(fields);
                AddParametersToQuery(ref uriBuilder, flattenFieldsParam);
            }

            if (!string.IsNullOrEmpty(filterByFormula))
            {
                AddParametersToQuery(ref uriBuilder, $"filterByFormula={HttpUtility.UrlEncode(filterByFormula)}");
            }

            if (sort != null)
            {
                string flattenSortParam = QueryParamHelper.FlattenSortParam(sort);
                AddParametersToQuery(ref uriBuilder, flattenSortParam);
            }

            if (!string.IsNullOrEmpty(view))
            {
                AddParametersToQuery(ref uriBuilder, $"view={HttpUtility.UrlEncode(view)}");
            }

            if (maxRecords != null)
            {
                if (maxRecords <= 0)
                {
                    throw new ArgumentException("Maximum Number of Records must be > 0", "maxRecords");
                }
                AddParametersToQuery(ref uriBuilder, $"maxRecords={maxRecords}");
            }

            if (pageSize != null)
            {
                if (pageSize <= 0 || pageSize > MAX_PAGE_SIZE)
                {
                    throw new ArgumentException("Page Size must be > 0 and <= 100", "pageSize");
                }
                AddParametersToQuery(ref uriBuilder, $"pageSize={pageSize}");
            }

            if (!string.IsNullOrEmpty(timeZone))
            {
                AddParametersToQuery(ref uriBuilder, $"timeZone={HttpUtility.UrlEncode(timeZone)}");
            }

            if (!string.IsNullOrEmpty(userLocale))
            {
                AddParametersToQuery(ref uriBuilder, $"userLocale={HttpUtility.UrlEncode(userLocale)}");
            }

            if (!string.IsNullOrEmpty(cellFormat) && !string.IsNullOrEmpty(timeZone) && !string.IsNullOrEmpty(userLocale))
            {
                AddParametersToQuery(ref uriBuilder, $"cellFormat={HttpUtility.UrlEncode(cellFormat)}");
            }

            if (returnFieldsByFieldId != false)
            {
                AddParametersToQuery(ref uriBuilder, $"returnFieldsByFieldId={returnFieldsByFieldId}");
            }
            return uriBuilder.Uri;
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.AddParametersToQuery
        //
        // Helper function for URI parameters
        //
        //----------------------------------------------------------------------------
        private void AddParametersToQuery(ref UriBuilder uriBuilder, string queryToAppend)
        {
            if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
            {
                uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + queryToAppend;
            }
            else
            {
                uriBuilder.Query = queryToAppend;
            }
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.CreateUpdateReplaceRecord
        //
        // worker function which does the real work for creating, updating, or replacing a record
        //
        //----------------------------------------------------------------------------
        private async Task<AirtableCreateUpdateReplaceRecordResponse> CreateUpdateReplaceRecord(
            string tableName,
            Fields fields,
            HttpMethod httpMethod,
            string recordId = null,
            bool typecast = false)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table Name cannot be null", "tableName");
            }

            string uriStr = AIRTABLE_API_URL + BaseId + "/" + Uri.EscapeDataString(tableName) + "/";
            if (httpMethod != HttpMethod.Post)
            {
                uriStr += recordId + "/";
            }

            var fieldsAndTypecast = new { fields = fields.FieldsCollection, typecast = typecast };
            var json = JsonSerializer.Serialize(fieldsAndTypecast, JsonOptionIgnoreNullValues);
            var request = new HttpRequestMessage(httpMethod, uriStr);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);

            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableCreateUpdateReplaceRecordResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var airtableRecord = JsonSerializer.Deserialize<AirtableRecord>(responseBody);

            return new AirtableCreateUpdateReplaceRecordResponse(airtableRecord);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.CreateUpdateReplaceMultipleRecords
        //
        // worker function which does the real work for creating, updating or replacing multiple records
        // in one operation
        //
        //----------------------------------------------------------------------------
        private async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> CreateUpdateReplaceMultipleRecords(string tableName, HttpMethod method, string json)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table Name cannot be null", "tableName");
            }

            string uriStr = AIRTABLE_API_URL + BaseId + "/" + Uri.EscapeDataString(tableName) + "/";
            var request = new HttpRequestMessage(method, uriStr);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);

            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableCreateUpdateReplaceMultipleRecordsResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var recordList = JsonSerializer.Deserialize<AirtableRecordList>(responseBody);

            return new AirtableCreateUpdateReplaceMultipleRecordsResponse(recordList.Records);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.UpdateReplaceRecordsFromList
        //
        // Called to replace or update all records specified in the providied record list.
        //
        //----------------------------------------------------------------------------
        private async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> UpdateReplaceRecordsFromList(
            string tableName,
            AirtableRecord[] records,
            HttpMethod httpMethod,
            bool typecast = false)
        {
            if (records == null || records.Length == 0 || records.Length > MAX_RECORD_OPERATION_SIZE)
            {
                throw new ArgumentException("Record list cannot be null or empty or cannot have more than 10 records");
            }

            AirtableCreateUpdateReplaceMultipleRecordsResponse response = null;
            string json = null;

            List<IdFields> idFieldsList = new List<IdFields>();     // Use List<> instead of array because we don't know how many fields that we are going to have.
            for (int index = 0; index < records.Length; index++)
            {
                AirtableRecord record = records[index];
                if (string.IsNullOrEmpty(record.Id))                // This record exists already
                {
                    throw new ArgumentException("All records in record list must contain an ID", "records");
                }

                idFieldsList.Add(new IdFields(record.Id));
                idFieldsList[index].FieldsCollection = record.Fields;

                if (index == (records.Length - 1))                  // Done filling idFieldList?
                {
                    json = JsonSerializer.Serialize(new { records = idFieldsList.ToArray(), typecast = typecast }, JsonOptionIgnoreNullValues);
                    response = await (CreateUpdateReplaceMultipleRecords(tableName, httpMethod, json)).ConfigureAwait(false);
                    break;
                }
            }
            return response;
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.CheckForAirtableException
        //
        // construct and return the appropriate exception based on the specified message response
        //
        //----------------------------------------------------------------------------
        private async Task<AirtableApiException> CheckForAirtableException(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    return null;

                case System.Net.HttpStatusCode.BadRequest:
                    return (new AirtableBadRequestException());

                case System.Net.HttpStatusCode.Forbidden:
                    return (new AirtableForbiddenException());

                case System.Net.HttpStatusCode.NotFound:
                    return (new AirtableNotFoundException());

                case System.Net.HttpStatusCode.PaymentRequired:
                    return (new AirtablePaymentRequiredException());

                case System.Net.HttpStatusCode.Unauthorized:
                    return (new AirtableUnauthorizedException());

                case System.Net.HttpStatusCode.RequestEntityTooLarge:
                    return (new AirtableRequestEntityTooLargeException());

                case (System.Net.HttpStatusCode)422:    // There is no HttpStatusCode.InvalidRequest defined in HttpStatusCode Enumeration.
                    var detailedErrorMessage = await ReadResponseErrorMessage(response).ConfigureAwait(false);
                    return (new AirtableInvalidRequestException(detailedErrorMessage));

                case (System.Net.HttpStatusCode)429:    // There is no HttpStatusCode.TooManyRequests defined in HttpStatusCode Enumeration.
                    return (new AirtableTooManyRequestsException());

                default:
                    throw new AirtableUnrecognizedException(response.StatusCode);
            }
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.InvalidRequestError
        //
        // Wrapper class of the HTTP Invalid Request error
        //
        //----------------------------------------------------------------------------
        private class InvalidRequestError
        {
            [JsonPropertyName("error")]
            [JsonInclude]
            public MessagePart MessagePart { get; set; }
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.MessagePart
        //
        // The error message of the HTTP Invalid Request error
        //
        //----------------------------------------------------------------------------
        private class MessagePart
        {
            [JsonPropertyName("type")]
            [JsonInclude]
            public string ErrorType { get; set; }

            [JsonPropertyName("message")]
            [JsonInclude]
            public string Message { get; set; }
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ReadResponseErrorMessage
        //
        // attempts to read the error message in the response body.
        //
        //----------------------------------------------------------------------------
        private static async Task<string> ReadResponseErrorMessage(HttpResponseMessage response)
        {

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            InvalidRequestError json = JsonSerializer.Deserialize<InvalidRequestError>(content);
            if (json.MessagePart != null && json.MessagePart.ErrorType != null)
            {
                return json.MessagePart.Message;
            }
            return null;
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ListRecordsInternal
        //
        //----------------------------------------------------------------------------
        private async Task<HttpResponseMessage> ListRecordsInternal(
            string tableName,
            string offset,
            IEnumerable<string> fields,
            string filterByFormula,
            int? maxRecords,
            int? pageSize,
            IEnumerable<Sort> sort,
            string view,
            string cellFormat,
            string timeZone,
            string userLocale,
            bool returnFieldsByFieldId)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table Name cannot be null", "tableName");
            }
            if (!string.IsNullOrEmpty(cellFormat) && (cellFormat == "string"))
            {
                if (string.IsNullOrEmpty(timeZone) || string.IsNullOrEmpty(userLocale))
                {
                    throw new ArgumentException("Both \'timeZone\' and \'userLocal\' parameters are required when using \'string\' as \'cellFormat\'.");
                }
            }
            var uri = BuildUriForListRecords(tableName, offset, fields, filterByFormula, maxRecords, pageSize, sort, view, cellFormat, timeZone, userLocale, returnFieldsByFieldId);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return (await httpClientWithRetries.SendAsync(request).ConfigureAwait(false));
        }

    }   // end class

}
