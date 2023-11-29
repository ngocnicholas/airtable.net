﻿using System;
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
        private const int MAX_LIST_RECORDS_URL_SIZE = 16000;

        private readonly string UrlHead = null;
        private readonly string UrlHeadWebhooks = null;
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

        public AirtableBase(string apiKeyOrAccessToken, string baseId) : this(apiKeyOrAccessToken, baseId, null)
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
            string apiKeyOrAccessToken,
            string baseId,
            DelegatingHandler delegatingHandler)
        {
            if (String.IsNullOrEmpty(apiKeyOrAccessToken))
            {
                throw new ArgumentException("api Key or access token cannot be null", "apiKeyOrAccessToken");
            }

            if (String.IsNullOrEmpty(baseId))
            {
                throw new ArgumentException("baseId cannot be null", "baseId");
            }

            UrlHead = "https://api.airtable.com/v0/" + baseId + "/";
            UrlHeadWebhooks = "https://api.airtable.com/v0/" + ("bases/" + baseId + "/webhooks");
            httpClientWithRetries = new HttpClientWithRetries(delegatingHandler, apiKeyOrAccessToken);
        }


        public async Task<AirtableGetUserIdAndScopesResponse> GetUserIdAndScopes()
        {
            string uriStr = "https://api.airtable.com/v0/meta/whoami";
            var request = new HttpRequestMessage(HttpMethod.Get, uriStr);

            var response = (await httpClientWithRetries.SendAsync(request).ConfigureAwait(false));

            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableGetUserIdAndScopesResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            UserIdAndScopes userIdAndScopes = JsonSerializer.Deserialize<UserIdAndScopes>(responseBody, JsonOptionIgnoreNullValues);
            return new AirtableGetUserIdAndScopesResponse(userIdAndScopes);
        }
        
        
        //----------------------------------------------------------------------------
        //
        // AirtableBase.ListRecords
        //
        // Called to get a list of records in the table specified by 'tableIdOrName'
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableListRecordsResponse> ListRecords(
            string tableIdOrName,
            string offset = null,
            IEnumerable<string> fields = null,
            //string[] fields = null,
            string filterByFormula = null,
            int? maxRecords = null,
            int? pageSize = null,
            IEnumerable<Sort> sort = null,
            string view = null,
            string cellFormat = null,
            string timeZone = null,
            string userLocale = null,
            bool returnFieldsByFieldId = false,
            bool? includeCommentCount = null
            )
        {
            HttpResponseMessage response = await ListRecordsInternal(tableIdOrName, offset, fields, filterByFormula,
                maxRecords, pageSize, sort, view,
                cellFormat, timeZone, userLocale,
                returnFieldsByFieldId, includeCommentCount
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
        // Called to get a list of records<T> in the table specified by 'tableIdOrName'
        // The fields of each record are deserialized to type <T>.
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableListRecordsResponse<T>> ListRecords<T>(
            string tableIdOrName,
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
            bool returnFieldsByFieldId = false,
            bool? includeCommentCount = null)
        {
            HttpResponseMessage response = await ListRecordsInternal(tableIdOrName, offset, fields, filterByFormula,
                maxRecords, pageSize, sort, view, cellFormat, timeZone, userLocale, returnFieldsByFieldId, includeCommentCount).ConfigureAwait(false);
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
        string tableIdOrName,
        string id,
        string cellFormat = null,
        string timeZone = null,
        string userLocale = null,
        bool returnFieldsByFieldId = false)
        {
            Uri uri = BuildUriForRetrieveRecord(tableIdOrName, id, cellFormat, timeZone, userLocale, returnFieldsByFieldId);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
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
            string tableIdOrName,
            string id,
            string cellFormat = null,
            string timeZone = null,
            string userLocale = null,
            bool returnFieldsByFieldId = false)
        {
            TableIdOrNameAndRecordIdCheck(tableIdOrName, id);

            Uri uri = BuildUriForRetrieveRecord(tableIdOrName, id, cellFormat, timeZone, userLocale, returnFieldsByFieldId);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
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
            string tableIdOrName,
            Fields fields,
            bool typecast = false)
        {
            return await (CreateUpdateReplaceRecord(tableIdOrName, fields, HttpMethod.Post, typecast: typecast)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.UpdateRecord
        //
        // Called to update a record with the specified ID in the specified table.
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableCreateUpdateReplaceRecordResponse> UpdateRecord(
            string tableIdOrName,
            Fields fields,
            string id,
            bool typeCast = false)
        {
            return await (CreateUpdateReplaceRecord(tableIdOrName, fields, new HttpMethod("PATCH"), id, typeCast)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ReplaceRecord
        //
        // Called to replace a record with the specified ID in the specified table using jsoncnotent .
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableCreateUpdateReplaceRecordResponse> ReplaceRecord(
            string tableIdOrName,
            Fields fields,
            string id,
            bool typeCast = false)
        {
            return await (CreateUpdateReplaceRecord(tableIdOrName, fields, HttpMethod.Put, id, typeCast)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.DeleteRecord
        //
        // Called to delete a record with the specified ID in the specified table
        //
        //----------------------------------------------------------------------------

        public async Task<AirtableDeleteRecordResponse> DeleteRecord(
            string tableIdOrName,
            string id)
        {
            TableIdOrNameAndRecordIdCheck(tableIdOrName, id);

            string uriStr = UrlHead + Uri.EscapeDataString(tableIdOrName) + "/" + id;
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
            string tableIdOrName,
            Fields[] fields,
            bool typecast = false)
        {
            if (fields == null || fields.Length == 0 || fields.Length > MAX_RECORD_OPERATION_SIZE)
            {
                throw new ArgumentException(String.Format("Number of records to be created must be >0 && <= {0}", MAX_RECORD_OPERATION_SIZE));
            }
            var json = JsonSerializer.Serialize(new { records = fields, typecast = typecast }, JsonOptionIgnoreNullValues);
            return await (CreateUpdateReplaceMultipleRecords(tableIdOrName, HttpMethod.Post, json)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.CreateMultipleRecords (overload)
        //
        // Called to create all records specified in the provided record list.
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> CreateMultipleRecords(
            string tableIdOrName,
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
                    response = await (CreateUpdateReplaceMultipleRecords(tableIdOrName, HttpMethod.Post, json)).ConfigureAwait(false);
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
            string tableIdOrName,
            IdFields[] idFields,
            bool typecast = false,
            bool returnFieldsByFieldId = false,
            PerformUpsert performUpsert = null)
        {
            var json = ReplaceUpdateMultipleRecordsInternal(idFields, typecast, returnFieldsByFieldId, performUpsert);
            return await (CreateUpdateReplaceMultipleRecords(tableIdOrName, new HttpMethod("PATCH"), json)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.UpdateMultipleRecords (overload)
        //
        // Called to update all records specified in the provided record list.
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> UpdateMultipleRecords(
            string tableIdOrName,
            AirtableRecord[] records,
            bool typecast = false,
            bool returnFieldsByFieldId = false,
            PerformUpsert performUpsert = null)
        {
            IdFields[] idFields = ConvertAirtableRecordsToIdFields(records, performUpsert != null);
            var json = ReplaceUpdateMultipleRecordsInternal(idFields, typecast, returnFieldsByFieldId, performUpsert);
            return await (CreateUpdateReplaceMultipleRecords(tableIdOrName, new HttpMethod("PATCH"), json)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ReplaceMultipleRecords
        //
        // Called to replace multiple records with the specified IDs in the specified table in one single operation.
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> ReplaceMultipleRecords(
            string tableIdOrName,
            IdFields[] idFields,
            bool typecast = false,
            bool returnFieldsByFieldId = false,
            PerformUpsert performUpsert = null)
        {
            var json = ReplaceUpdateMultipleRecordsInternal(idFields, typecast, returnFieldsByFieldId, performUpsert);
            return await (CreateUpdateReplaceMultipleRecords(tableIdOrName, HttpMethod.Put, json)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ReplaceMultipleRecords (overload)
        //
        // Called to replace all records specified in the provided record list.
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> ReplaceMultipleRecords(
            string tableIdOrName,
            AirtableRecord[] records,
            bool typecast = false,
            bool returnFieldsByFieldId = false,
            PerformUpsert performUpsert = null)
        {
            IdFields[] idFields = ConvertAirtableRecordsToIdFields(records, performUpsert != null);
            var json = ReplaceUpdateMultipleRecordsInternal(idFields, typecast, returnFieldsByFieldId, performUpsert);
            return await (CreateUpdateReplaceMultipleRecords(tableIdOrName, HttpMethod.Put, json)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ListComments
        //
        // Called to list all comments for the record with the provided record ID
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableListCommentsResponse> ListComments(
            string tableIdOrName,
            string recordId,                  // ID of record that we want to list the comments for
            string offset = null,
            int? pageSize = null)
        {
            TableIdOrNameAndRecordIdCheck(tableIdOrName, recordId);
            var uriBuilder = new UriBuilder(UrlHead + Uri.EscapeDataString(tableIdOrName) + "/" + recordId + "/comments");
            if (!string.IsNullOrEmpty(offset))
            {
                AddParametersToQuery(ref uriBuilder, $"offset={HttpUtility.UrlEncode(offset)}");
            }
            if (pageSize != null)
            {
                if (pageSize <= 0 || pageSize > MAX_PAGE_SIZE)
                {
                    throw new ArgumentException("Page Size must be > 0 and <= 100", "pageSize");
                }
                AddParametersToQuery(ref uriBuilder, $"pageSize={pageSize}");
            }
            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableListCommentsResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var commentList = JsonSerializer.Deserialize<CommentList>(responseBody, JsonOptionIgnoreNullValues);
            return new AirtableListCommentsResponse(commentList);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.CreateComment
        //
        // Called to create a comment with info stored in commenText for the record with the provided record ID
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateUpdateCommentResponse> CreateComment(
            string tableIdOrName,
            string recordId,
            string commentText)
        {
            return await (CreateUpdateComment(tableIdOrName, recordId, commentText)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.UpdateComment
        //
        // Called to Update a comment row with info stored in commenData for the row comments specified by rowCommentId of the record
        // with the provided record ID
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateUpdateCommentResponse> UpdateComment(
        string tableIdOrName,
           string recordId,
           string commentText,
           string rowCommentId)
        {
            if (string.IsNullOrEmpty(rowCommentId))
            {
                throw new ArgumentException("Comment ID cannot be null.", "rowCommentId");
            }

            return await (CreateUpdateComment(tableIdOrName, recordId, commentText, rowCommentId)).ConfigureAwait(false);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.DeleteComment
        //
        // Called to Delte a comment row with the provided record ID and the rowCommentId.
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableDeleteCommentResponse> DeleteComment(
        string tableIdOrName,
            string recordId,
            string rowCommentId)
        {
            TableIdOrNameAndRecordIdCheck(tableIdOrName, recordId);
            if (string.IsNullOrEmpty(rowCommentId))
            {
                throw new ArgumentException("Comment ID cannot be null", "recordId, rowCommentId");
            }

            string uriStr = UrlHead + Uri.EscapeDataString(tableIdOrName) + "/" + recordId + "/comments" + "/" + rowCommentId;
            var request = new HttpRequestMessage(HttpMethod.Delete, uriStr);
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);

            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableDeleteCommentResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var deletedComment = JsonSerializer.Deserialize<AirtableDeletedRecord>(responseBody);
            return new AirtableDeleteCommentResponse(deletedComment.Deleted, deletedComment.Id);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ListWebhooks
        //
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableListWebhooksResponse> ListWebhooks()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, UrlHeadWebhooks);
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableListWebhooksResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var webhooks = JsonSerializer.Deserialize<Webhooks>(responseBody, JsonOptionIgnoreNullValues);

            return new AirtableListWebhooksResponse(webhooks);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ListPayloads
        //
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableListPayloadsResponse> ListPayloads(
            string webhookId,
            int? cursor = null,
            int? limit = null)
        {
            var uriBuilder = new UriBuilder(UrlHeadWebhooks + "/" + webhookId + "/payloads");
            if(cursor != null)
            {
                AddParametersToQuery(ref uriBuilder, $"cursor={HttpUtility.UrlEncode(cursor.ToString())}");
            }

            if (limit != null)
            {
                AddParametersToQuery(ref uriBuilder, $"limit={HttpUtility.UrlEncode(limit.ToString())}");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableListPayloadsResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var payloadList = JsonSerializer.Deserialize<PayloadList>(responseBody, JsonOptionIgnoreNullValues);

            return new AirtableListPayloadsResponse(payloadList);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.CreateWebhook
        //
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableCreateWebhookResponse> CreateWebhook(
            WebhooksSpecification spec,
            string notificationUrl = null)              // optional

        {
            if (spec == null)
            {
                throw new ArgumentException("specification cannot be null");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, UrlHeadWebhooks);
            var urlAndSpec = new { specification = spec, notificationUrl = notificationUrl };
            var json = JsonSerializer.Serialize(urlAndSpec, JsonOptionIgnoreNullValues);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableCreateWebhookResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var createWebhookResponse = JsonSerializer.Deserialize<CreateWebhookResponse>(responseBody, JsonOptionIgnoreNullValues);

            return new AirtableCreateWebhookResponse(createWebhookResponse);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.EnableOrDisableWebhookNotifications
        //
        //
        //----------------------------------------------------------------------------
        public async Task<AirtabeEnableOrDisableWebhookNotificationsResponse> EnableOrDisableWebhookNotifications(
            string webhookId,
            bool enable)
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("Webhook ID cannot be null.");
            }
            string path = UrlHeadWebhooks + "/" + webhookId + "/enableNotifications";
            var request = new HttpRequestMessage(HttpMethod.Post, path);
            var json = JsonSerializer.Serialize(new { enable = enable }, JsonOptionIgnoreNullValues);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtabeEnableOrDisableWebhookNotificationsResponse(error);
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new AirtabeEnableOrDisableWebhookNotificationsResponse();
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.RefreshWebhook
        //
        //----------------------------------------------------------------------------
        public async Task<AirtabeRefreshWebhookResponse> RefreshWebhook(
            string webhookId)
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("Webhook ID cannot be null.");
            }
            string path = UrlHeadWebhooks + "/" + webhookId + "/refresh";
            var request = new HttpRequestMessage(HttpMethod.Post, path);
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtabeRefreshWebhookResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            NotificationExpirationTime exp = JsonSerializer.Deserialize<NotificationExpirationTime>(responseBody, JsonOptionIgnoreNullValues);
            return new AirtabeRefreshWebhookResponse(exp.ExpirationTime);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.DeleteWebhook
        //
        //
        //----------------------------------------------------------------------------
        public async Task<AirtableDeleteWebhookResponse> DeleteWebhook(
            string webhookId)
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("Webhook ID cannot be null.");
            }
            var request = new HttpRequestMessage(HttpMethod.Delete, UrlHeadWebhooks + "/" + webhookId);
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableDeleteWebhookResponse(error);
            }

            return new AirtableDeleteWebhookResponse();
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
        // AirtableBase.BuildParametersForListRecords
        //
        // Build Parameters for the request body of the List Records operation
        //
        //----------------------------------------------------------------------------
        private string BuildParametersForListRecords(
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
            bool returnFieldsByFieldId,
            bool? includeCommentCount           
            )
        {
            if (pageSize != null)
            {
                if (pageSize <= 0 || pageSize > MAX_PAGE_SIZE)
                {
                    throw new ArgumentException("Page Size must be > 0 and <= 100", "pageSize");
                }
            }

            if (maxRecords != null)
            {
                if (maxRecords <= 0)
                {
                    throw new ArgumentException("Maximum Number of Records must be > 0", "maxRecords");
                }
            }

            var listRecordsParameters = new ListRecordsParameters
            {
                Offset = offset,
                FilterByFormula = filterByFormula,
                MaxRecords = maxRecords,
                PageSize = pageSize,
                Sort = sort,
                View = view,
                CellFormat = cellFormat,
                TimeZone = timeZone,
                UserLocale = userLocale,
                ReturnFieldsByFieldId = returnFieldsByFieldId,
                RecordMetadata = null
            };

            if (fields != null)
            {
                listRecordsParameters.Fields = new List<string>(fields).ToArray();
            }

            if (includeCommentCount.HasValue && includeCommentCount.Value)
            {
                listRecordsParameters.RecordMetadata = (includeCommentCount.Value ? "commentCount" : null);
            }

            // Need to set the Converters to convert the SortDirection Enum to string to be used in the request boday of ListRecords.
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return JsonSerializer.Serialize(listRecordsParameters, options);
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
            string tableIdOrName,
            Fields fields,
            HttpMethod httpMethod,
            string recordId = null,
            bool typecast = false)
        {
            if (string.IsNullOrEmpty(tableIdOrName))
            {
                throw new ArgumentException("Table ID or Name cannot be null", "tableIdOrName");
            }

            string uriStr = UrlHead + Uri.EscapeDataString(tableIdOrName) + "/";
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
        private async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> CreateUpdateReplaceMultipleRecords(string tableIdOrName, HttpMethod method, string json)
        {
            if (string.IsNullOrEmpty(tableIdOrName))
            {
                throw new ArgumentException("Table ID or Name cannot be null", "tableIdOrName");
            }

            string uriStr = UrlHead + Uri.EscapeDataString(tableIdOrName) + "/";
            var request = new HttpRequestMessage(method, uriStr);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);

            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableCreateUpdateReplaceMultipleRecordsResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var upsertRecordList = JsonSerializer.Deserialize<AirtableUpSertRecordList>(responseBody);

            return new AirtableCreateUpdateReplaceMultipleRecordsResponse(upsertRecordList);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ConvertAirtableRecordsToIdFields
        //
        // Convert an AirtableRecord array to an IdFields array 
        //
        //----------------------------------------------------------------------------
        private IdFields[] ConvertAirtableRecordsToIdFields(AirtableRecord[] records, bool upSert=false)
        {
            if (records == null || records.Length == 0 || records.Length > MAX_RECORD_OPERATION_SIZE)
            {
                throw new ArgumentException("Record list cannot be null or empty or cannot have more than 10 records");
            }
            IdFields[] idFieldsArray = new IdFields[records.Length];
            for (int index = 0; index < records.Length; index++)
            {
                AirtableRecord record = records[index];
                if (string.IsNullOrEmpty(record.Id) && !upSert)                // Record ID is only optional if we are performing upSert.
                {
                    throw new ArgumentException("All records in record list must contain an ID", "records");
                }
                idFieldsArray[index] = new IdFields(record.Id);
                idFieldsArray[index].FieldsCollection = record.Fields;
            }
            return idFieldsArray;
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.ReplaceUpdateMultipleRecordsInternal
        //
        //----------------------------------------------------------------------------
        private string ReplaceUpdateMultipleRecordsInternal(
            IdFields[] idFields,
            bool typecast = false,
            bool returnFieldsByFieldId = false,
            PerformUpsert performUpsert = null)
        {
            if (idFields == null || idFields.Length == 0 || idFields.Length > MAX_RECORD_OPERATION_SIZE)
            {
                throw new ArgumentException(String.Format("Number of records to be replaced/updated must be >0 && <= {0}", MAX_RECORD_OPERATION_SIZE));
            }

            UpsertRecordsParameters upsertRecordsParameters = new UpsertRecordsParameters
            {
                PerformUpsert = performUpsert,
                ReturnFieldsByFieldId = returnFieldsByFieldId,
                Typecast = typecast,
                Records = idFields
            };

            if (performUpsert != null && (performUpsert.FieldsToMergeOn != null && performUpsert.FieldsToMergeOn.Length < 1 || performUpsert.FieldsToMergeOn.Length > 3))
            {
                throw new ArgumentException("FieldsToMergeOn must be >0 && <= 3");
            }

            return JsonSerializer.Serialize<UpsertRecordsParameters>(upsertRecordsParameters, JsonOptionIgnoreNullValues);
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
        // AirtableBase.BuildUriForRetrieveRecord
        //
        // construct and return the appropriate exception based on the specified message response
        //
        //----------------------------------------------------------------------------
        private Uri BuildUriForRetrieveRecord(
            string tableIdOrName,
            string id,
            string cellFormat,
            string timeZone,
            string userLocale,
            bool returnFieldsByFieldId)
        {
            TableIdOrNameAndRecordIdCheck(tableIdOrName, id);
            if (!string.IsNullOrEmpty(cellFormat) && (cellFormat == "string"))
            {
                if (string.IsNullOrEmpty(timeZone) || string.IsNullOrEmpty(userLocale))
                {
                    throw new ArgumentException("Both \'timeZone\' and \'userLocal\' parameters are required when using \'string\' as \'cellFormat\'.");
                }
            }
            var uriBuilder = new UriBuilder(UrlHead + Uri.EscapeDataString(tableIdOrName) + "/" + id);
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
            string tableIdOrName,
            string offset,
            IEnumerable<string> fields,
            //string[] fields,
            string filterByFormula,
            int? maxRecords,
            int? pageSize,
            IEnumerable<Sort> sort,
            string view,
            string cellFormat,
            string timeZone,
            string userLocale,
            bool returnFieldsByFieldId,
            bool? includeCommentCount
            )
        {
            if (string.IsNullOrEmpty(tableIdOrName))
            {
                throw new ArgumentException("Table ID or Name cannot be null", "tableIdOrName");
            }
            if (!string.IsNullOrEmpty(cellFormat) && (cellFormat == "string"))
            {
                if (string.IsNullOrEmpty(timeZone) || string.IsNullOrEmpty(userLocale))
                {
                    throw new ArgumentException("Both \'timeZone\' and \'userLocal\' parameters are required when using \'string\' as \'cellFormat\'.");
                }
            }

            // New method: has all the parameters in the request body.
            string uriStr = UrlHead + Uri.EscapeDataString(tableIdOrName) + "/listRecords";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uriStr);
            string jsonParameters = BuildParametersForListRecords(offset, fields, filterByFormula, maxRecords, pageSize, sort, view, 
                cellFormat, timeZone, userLocale, returnFieldsByFieldId, includeCommentCount);

            // Pass the parameters within the body of the request instead of the query parameters.
            request.Content = new StringContent(jsonParameters, Encoding.UTF8, "application/json");

            return (await httpClientWithRetries.SendAsync(request).ConfigureAwait(false));
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.CreateUpdateComment
        //
        // This is the worker for both CreateComment and UpdateComment
        //
        //----------------------------------------------------------------------------
        private async Task<AirtableCreateUpdateCommentResponse> CreateUpdateComment(
        string tableIdOrName,
           string id,
           string commentText,
           string rowCommentId = null)      // == null if we are doing Create Comment Otherwise we are doing Update Comment
        {
            TableIdOrNameAndRecordIdCheck(tableIdOrName, id);

            if (string.IsNullOrEmpty(commentText))
            {
                throw new ArgumentException("Comment text cannot be null");
            }

            HttpMethod httpMethod = HttpMethod.Post;
            string uriStr = UrlHead + Uri.EscapeDataString(tableIdOrName) + "/" + id + "/comments";
            if (rowCommentId != null)   // Updating a commet? If so, tell us the ID of the comment row to be updated
            {
                uriStr += "/" + rowCommentId;
                httpMethod = new HttpMethod("PATCH");
            }
            var request = new HttpRequestMessage(httpMethod, uriStr);
            var json = JsonSerializer.Serialize(new { text = commentText }, JsonOptionIgnoreNullValues);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json"); ;
            var response = await httpClientWithRetries.SendAsync(request).ConfigureAwait(false);

            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableCreateUpdateCommentResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var comment = JsonSerializer.Deserialize<Comment>(responseBody, JsonOptionIgnoreNullValues);

            return new AirtableCreateUpdateCommentResponse(comment);
        }


        //----------------------------------------------------------------------------
        //
        // AirtableBase.TableIdOrNameAndRecordIdCheck
        //
        // Arguments checking helper
        //
        //----------------------------------------------------------------------------
        private void TableIdOrNameAndRecordIdCheck(string tableIdOrName, string id)
        { 
            if (string.IsNullOrEmpty(tableIdOrName))
            {
                throw new ArgumentException("Table ID or name cannot be null", "tableIdOrName");
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Record ID cannot be null", "id");
            }
        }

    }   // end class

}
