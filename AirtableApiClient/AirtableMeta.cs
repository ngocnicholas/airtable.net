using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;

namespace AirtableApiClient
{
    public class AirtableMetaBasesList
    {
        [JsonPropertyName("bases")]
        [JsonInclude]
        public AirtableMetaBases[] Bases { get; internal set; }
    }

    public class AirtableMetaTablesList
    {
        [JsonPropertyName("tables")]
        [JsonInclude]
        public AirtableMetaTables[] Tables { get; internal set; }
    }

    public class AirtableMetaBases
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("permissionLevel")]
        [JsonInclude]
        public string Permission { get; internal set; }
    }

    public class AirtableMetaTables
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("primaryFieldId")]
        [JsonInclude]
        public string PrimaryField { get; internal set; }

        [JsonPropertyName("fields")]
        [JsonInclude]
        public AirtableMetaTableFields[] Fields { get; internal set; }

        [JsonPropertyName("views")]
        [JsonInclude]
        public AirtableMetaTableViews[] Views { get; internal set; }
    }

    public class AirtableMetaTableFields
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; internal set; }
    }

public class AirtableMetaTableViews
{
    [JsonPropertyName("id")]
    [JsonInclude]
    public string Id { get; internal set; }

    [JsonPropertyName("name")]
    [JsonInclude]
    public string Name { get; internal set; }

    [JsonPropertyName("type")]
    [JsonInclude]
    public string Type { get; internal set; }
}

public class AirtableMetaBasesResponse : AirtableApiResponse
    {
        public AirtableMetaBasesResponse(AirtableApiException error) : base(error)
        {
            Bases = null;
        }

        public AirtableMetaBasesResponse(AirtableMetaBasesList meta)
        {
            Bases = meta.Bases;
        }

        public readonly IEnumerable<AirtableMetaBases> Bases;
    }

    public class AirtableMetaTablesResponse : AirtableApiResponse
    {
        public AirtableMetaTablesResponse(AirtableApiException error) : base(error)
        {
            Tables = null;
        }

        public AirtableMetaTablesResponse(AirtableMetaTablesList meta)
        {
            Tables = meta.Tables;
        }

        public readonly IEnumerable<AirtableMetaTables> Tables;
    }

    public class AirtableMeta : IDisposable
    {
        private const string AIRTABLE_META_API_URL = "https://api.airtable.com";
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
        // AirtableMeta.AirtableMeta
        //    constructor -- for end users
        // 
        //----------------------------------------------------------------------------

        public AirtableMeta(string apiKey) : this(apiKey, null)
        {
            // No delegating handler is given; a normal HttpClient will be constructed
            // to communicate with Airtable.
        }

        //----------------------------------------------------------------------------
        // 
        // AirtableMeta.AirtableMeta
        //    constructor -- for Unit tests only
        // 
        //----------------------------------------------------------------------------

        internal AirtableMeta(
            string apiKey,
            DelegatingHandler delegatingHandler)    // specific handler for unit test purpose
        {
            httpClientWithRetries = new HttpClientWithRetries(delegatingHandler, apiKey);
        }

        //----------------------------------------------------------------------------
        // 
        // AirtableMeta.ListMetadata
        // 
        // If baseId is excluded returns a list of bases.  If baseId is included returns a list of tables.
        // 
        //----------------------------------------------------------------------------

        public async Task<AirtableMetaBasesResponse> ListBases(
            string apiKey)
        {
            HttpResponseMessage response = await ListMetadataInternal(apiKey, null).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableMetaBasesResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            AirtableMetaBasesList meta = JsonSerializer.Deserialize< AirtableMetaBasesList>(responseBody, JsonOptionIgnoreNullValues);
            return new AirtableMetaBasesResponse(meta);
        }

        public async Task<AirtableMetaTablesResponse> ListTables(
            string apiKey,
            string baseId)
        {
            HttpResponseMessage response = await ListMetadataInternal(apiKey, baseId).ConfigureAwait(false);
            AirtableApiException error = await CheckForAirtableException(response).ConfigureAwait(false);
            if (error != null)
            {
                return new AirtableMetaTablesResponse(error);
            }
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            AirtableMetaTablesList meta = JsonSerializer.Deserialize<AirtableMetaTablesList>(responseBody, JsonOptionIgnoreNullValues);
            return new AirtableMetaTablesResponse(meta);
        }

        //----------------------------------------------------------------------------
        // 
        // AirtableMeta.Dispose
        // 
        //----------------------------------------------------------------------------

        public void Dispose()
        {
            httpClientWithRetries.Dispose();
        }

        //----------------------------------------------------------------------------
        // 
        // AirtableMeta.BuildUriForListRecords
        // 
        // Build URI for the List Records operation
        // 
        //----------------------------------------------------------------------------

        private Uri BuildUriForMetadataApi(
            string apiKey,
            string baseId)
        {
            var uriBuilder = new UriBuilder(AIRTABLE_META_API_URL);
            if (!string.IsNullOrEmpty(baseId))
            {
                uriBuilder.Path = "v0/meta/bases/" + baseId + "/tables";
            }
            else
            {
                uriBuilder.Path = "v0/meta/bases";
            }
            uriBuilder.Query = "?api_key=" + apiKey;
            return uriBuilder.Uri;
        }

        //----------------------------------------------------------------------------
        // 
        // AirtableMeta.ListMetadataInternal
        // 
        //----------------------------------------------------------------------------

        private async Task<HttpResponseMessage> ListMetadataInternal(
            string apiKey,
            string baseId)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("Api key cannot be null", "apiKey");
            }
            var uri = BuildUriForMetadataApi(apiKey, baseId);
            Debug.WriteLine(uri);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return (await httpClientWithRetries.SendAsync(request).ConfigureAwait(false));
        }

        //----------------------------------------------------------------------------
        // 
        // AirtableMeta.CheckForAirtableException
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
                    var error = await ReadResponseErrorMessage(response).ConfigureAwait(false);
                    return (new AirtableInvalidRequestException(error));

                case (System.Net.HttpStatusCode)429:    // There is no HttpStatusCode.TooManyRequests defined in HttpStatusCode Enumeration.
                    return (new AirtableTooManyRequestsException());

                default:
                    throw new AirtableUnrecognizedException(response.StatusCode);
            }
        }


        class MessagePart
        {
            public string Error { get; set; }
            public string Message { get; set; }
        }

        //----------------------------------------------------------------------------
        // 
        // AirtableMeta.ReadResponseErrorMessage
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

            MessagePart json = JsonSerializer.Deserialize<MessagePart>(content);
            if (json.Error != null)
            {
                return json.Error;
            }
            return json.Message;
        }
    }
}
