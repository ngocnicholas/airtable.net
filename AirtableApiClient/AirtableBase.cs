using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;                  // for JsonConvert


namespace AirtableApiClient
{
    public class AirtableBase : IDisposable
    {
        private enum OperationType { CREATE, UPDATE, REPLACE };

        private const int MAX_PAGE_IZE = 100;

        private const string AIRTABLE_API_URL = "https://api.airtable.com/v0/";

        private readonly string BaseId;
        private readonly HttpClient Client;

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

            if (delegatingHandler == null)
            {
                Client = new HttpClient();                      // for communicating with Airtable
            }
            else
            {
                Client = new HttpClient(delegatingHandler);     // for communicating with the specified handler
            }           

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
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
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table Name cannot be null", "tableName");
            }

            AirtableRecordList recordList = null;
            var uri = BuildUriForListRecords(tableName, offset, fields, filterByFormula, maxRecords, pageSize, sort, view);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await Client.SendAsync(request);
            AirtableApiException error = CheckForAirtableException(response);
            if (error != null)
            {
                return new AirtableListRecordsResponse(error);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            recordList = JsonConvert.DeserializeObject<AirtableRecordList>(responseBody);

            return new AirtableListRecordsResponse(recordList);
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

            AirtableRecord airtableRecord = null;
            string uriStr = AIRTABLE_API_URL + BaseId + "/" + HttpUtility.UrlEncode(tableName) + "/" + id;
            var request = new HttpRequestMessage(HttpMethod.Get, uriStr);
            var response = await Client.SendAsync(request);
            AirtableApiException error = CheckForAirtableException(response);
            if (error != null)
            {
                return new AirtableRetrieveRecordResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            airtableRecord = JsonConvert.DeserializeObject<AirtableRecord>(responseBody);

            return new AirtableRetrieveRecordResponse(airtableRecord);
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
        // Called to update a record with the specified ID in the specified table using jsoncnotent .
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
            var response = await task;
            return response;
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

            string uriStr = AIRTABLE_API_URL + BaseId + "/" + HttpUtility.UrlEncode(tableName) + "/" + id;
            var request = new HttpRequestMessage(HttpMethod.Delete, uriStr);
            var response = await Client.SendAsync(request);
            AirtableApiException error = CheckForAirtableException(response);
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
        // AirtableBase.Dispose
        // 
        //----------------------------------------------------------------------------

        public void Dispose()
        {
            Client.Dispose();
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
            var uriBuilder = new UriBuilder(AIRTABLE_API_URL + BaseId + "/" + HttpUtility.UrlEncode(tableName));

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (!string.IsNullOrEmpty(offset))
            {
                query["offset"] = offset;
            }

            if (fields != null)
            {
                QueryParamHelper.FlattenFieldsParam(fields, query);
            }

            if (!string.IsNullOrEmpty(filterByFormula))
            {
                query["filterByFormula"] = filterByFormula;
            }

            if (sort != null)
            {
                QueryParamHelper.FlattenSortParam(sort, query);
            }

            if (!string.IsNullOrEmpty(view))
            {
                query["view"] = view;
            }

            if (query.Count > 0)
            {
                if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
                {
                    uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + query.ToString();
                }
                else
                {
                    uriBuilder.Query = query.ToString();
                }
            }

            if (maxRecords != null)
            {
                if (maxRecords <= 0)
                {
                    throw new ArgumentException("Maximum Number of Records must be > 0", "maxRecords");
                }

                if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
                {
                    uriBuilder.Query = uriBuilder.Query.Substring(1) + "&maxRecords=" + maxRecords;
                }
                else
                {
                    uriBuilder.Query = $"maxRecords={maxRecords}";
                }
            }

            if (pageSize != null)
            {
                if (pageSize <= 0 || pageSize > MAX_PAGE_IZE)
                {
                    throw new ArgumentException("Page Size must be > 0 and <= 100", "pageSize");
                }

                if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
                {
                    uriBuilder.Query = uriBuilder.Query.Substring(1) + "&pageSize=" + pageSize;
                }
                else
                {
                    uriBuilder.Query = $"pageSize={pageSize}";
                }
            }

            return uriBuilder.Uri;
        }


        //----------------------------------------------------------------------------
        // 
        // AirtableBase.AirtableCreateUpdateReplaceRecordResponse
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

            string uriStr = AIRTABLE_API_URL + BaseId + "/" + HttpUtility.UrlEncode(tableName) + "/";
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
                    throw new ArgumentException("Operation Type must be one of { OperationType.UPDATE, .REPLACE, OperationType.CREATE }", "operationType");
            }

            var fieldsAndTypecast = new { fields = fields.FieldsCollection, typecast = typecast };
            var json = JsonConvert.SerializeObject(fieldsAndTypecast, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var request = new HttpRequestMessage(httpMethod, uriStr);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.SendAsync(request);

            AirtableApiException error = CheckForAirtableException(response);
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
        // AirtableBase.CheckForAirtableException
        // 
        // construct and return the appropriate exception based on the specified message response
        // 
        //----------------------------------------------------------------------------

        private AirtableApiException CheckForAirtableException(HttpResponseMessage response)
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

                case (System.Net.HttpStatusCode)422:        // There is no HttpStatusCode.InvalidRequest defined in HttpStatusCode Enumeration.
                    return (new AirtableInvalidRequestException());

                default:
                    throw new AirtableUnrecognizedException(response.StatusCode);
            }
        }

    }   // end class

}
