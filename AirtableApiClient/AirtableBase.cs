using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace AirtableApiClient
{
    public class AirtableBase : IDisposable
    {
        private enum OperationType { CREATE, UPDATE, REPLACE };

        private const int MAX_PAGE_SIZE = 100;

        private const string AIRTABLE_API_URL = "https://api.airtable.com/v0/";

        private readonly string BaseId;
        private readonly HttpClientWithRetries httpClientWithRetries;

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
        //    constructor -- for end users
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
        //    constructor -- for Unit tests only
        // 
        //----------------------------------------------------------------------------

        internal AirtableBase(
            string apiKey,
            string baseId,
            DelegatingHandler delegatingHandler)    // specific handler for unit test purpose
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
            string view = null)
        {
            HttpResponseMessage response = await ListRecordsInternal(tableName, offset, fields, filterByFormula,
                maxRecords, pageSize, sort, view);
            AirtableApiException error = await CheckForAirtableException(response);
            if (error != null)
            {
                return new AirtableListRecordsResponse(error);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            AirtableRecordList recordList = JsonConvert.DeserializeObject<AirtableRecordList>(responseBody);
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
            string view = null)
        {
            HttpResponseMessage response = await ListRecordsInternal(tableName, offset, fields, filterByFormula,
                maxRecords, pageSize, sort, view);
            AirtableApiException error = await CheckForAirtableException(response);
            if (error != null)
            {
                return new AirtableListRecordsResponse<T>(error);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            AirtableRecordList<T> recordList = JsonConvert.DeserializeObject<AirtableRecordList<T>>(responseBody);
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
            var response = await httpClientWithRetries.SendAsync(request);
            AirtableApiException error = await CheckForAirtableException(response);
            if (error != null)
            {
                return new AirtableRetrieveRecordResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var airtableRecord = JsonConvert.DeserializeObject<AirtableRecord>(responseBody);

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
            var response = await httpClientWithRetries.SendAsync(request);
            AirtableApiException error = await CheckForAirtableException(response);
            if (error != null)
            {
                return new AirtableRetrieveRecordResponse<T>(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            AirtableRecord<T> airtableRecord = JsonConvert.DeserializeObject<AirtableRecord<T>>(responseBody);

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
            Task<AirtableCreateUpdateReplaceRecordResponse> task = CreateUpdateReplaceRecord(tableName, fields, OperationType.CREATE, typecast: typecast);
            var response = await task;
            return response;
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
            Task<AirtableCreateUpdateReplaceRecordResponse> task = CreateUpdateReplaceRecord(tableName, fields, OperationType.UPDATE, id, typeCast);
            var response = await task;
            return response;
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
            Task<AirtableCreateUpdateReplaceRecordResponse> task = CreateUpdateReplaceRecord(tableName, fields, OperationType.REPLACE, id, typeCast);
            return (await task);
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
            var response = await httpClientWithRetries.SendAsync(request);
            AirtableApiException error = await CheckForAirtableException(response);
            if (error != null)
            {
                return new AirtableDeleteRecordResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var deletedRecord = JsonConvert.DeserializeObject<AirtableDeletedRecord>(responseBody);
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
            var json = JsonConvert.SerializeObject(new { records = fields, typecast = typecast }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = CreateUpdateReplaceMultipleRecords(tableName, HttpMethod.Post, json);
            var response = await task;
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
            var json = JsonConvert.SerializeObject(new { records = idFields, typecast = typecast }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = CreateUpdateReplaceMultipleRecords(tableName, new HttpMethod("PATCH"), json);
            return (await task);
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
            var json = JsonConvert.SerializeObject(new { records = idFields, typecast = typecast }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = CreateUpdateReplaceMultipleRecords(tableName, HttpMethod.Put, json);
            return (await task);
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
            string view)
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
            OperationType operationType,
            string recordId = null,
            bool typecast = false)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table Name cannot be null", "tableName");
            }

            string uriStr = AIRTABLE_API_URL + BaseId + "/" + Uri.EscapeDataString(tableName) + "/";
            HttpMethod httpMethod;
            switch (operationType)
            {
                case OperationType.UPDATE:
                    uriStr += recordId + "/";
                    httpMethod = new HttpMethod("PATCH");
                    break;
                case OperationType.REPLACE:
                    uriStr += recordId + "/";
                    httpMethod = HttpMethod.Put;
                    break;
                case OperationType.CREATE:
                    httpMethod = HttpMethod.Post;
                    break;
                default:
                    throw new ArgumentException("Operation Type must be one of { OperationType.UPDATE, OperationType.REPLACE, OperationType.CREATE }", "operationType");
            }

            var fieldsAndTypecast = new { fields = fields.FieldsCollection, typecast = typecast };
            var json = JsonConvert.SerializeObject(fieldsAndTypecast, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var request = new HttpRequestMessage(httpMethod, uriStr);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClientWithRetries.SendAsync(request);

            AirtableApiException error = await CheckForAirtableException(response);
            if (error != null)
            {
                return new AirtableCreateUpdateReplaceRecordResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var airtableRecord = JsonConvert.DeserializeObject<AirtableRecord>(responseBody);

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
            var response = await httpClientWithRetries.SendAsync(request);

            AirtableApiException error = await CheckForAirtableException(response);
            if (error != null)
            {
                return new AirtableCreateUpdateReplaceMultipleRecordsResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var recordList = JsonConvert.DeserializeObject<AirtableRecordList>(responseBody);

            return new AirtableCreateUpdateReplaceMultipleRecordsResponse(recordList.Records);
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
                    var error = await ReadResponseErrorMessage(response);
                    return (new AirtableInvalidRequestException(error));

                case (System.Net.HttpStatusCode)429:    // There is no HttpStatusCode.TooManyRequests defined in HttpStatusCode Enumeration.
                    return (new AirtableTooManyRequestsException());

                default:
                    throw new AirtableUnrecognizedException(response.StatusCode);
            }
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
            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            var json = JObject.Parse(content);
            var errorMessage = json["error"]?["message"]?.Value<string>();

            return errorMessage;
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
            string view)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table Name cannot be null", "tableName");
            }
            var uri = BuildUriForListRecords(tableName, offset, fields, filterByFormula, maxRecords, pageSize, sort, view);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return (await httpClientWithRetries.SendAsync(request));
        }

    }   // end class

}
