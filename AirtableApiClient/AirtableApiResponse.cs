using System;
using System.Collections.Generic;

namespace AirtableApiClient
{
    public abstract class AirtableApiResponse
    {
        protected AirtableApiResponse()
        {
            Success = true;
            AirtableApiError = null;
        }

        protected AirtableApiResponse(AirtableApiException error)
        {
            Success = false;
            AirtableApiError = error;
        }

        public readonly bool Success;
        public readonly AirtableApiException AirtableApiError;
    }


    public class AirtableGetUserIdAndScopesResponse : AirtableApiResponse
    {
        public AirtableGetUserIdAndScopesResponse(AirtableApiException error) : base(error)
        {
            UserId = null;
            Scopes = null;
        }

        public AirtableGetUserIdAndScopesResponse(UserIdAndScopes userIdAndScopes) : base()
        {
            UserId = userIdAndScopes.Id;
            Scopes = userIdAndScopes.Scopes;
        }
        public readonly string UserId;
        public readonly ICollection<string> Scopes;
    }


    public class AirtableListRecordsResponse : AirtableApiResponse
    {
        public AirtableListRecordsResponse(AirtableApiException error) : base(error)
        {
            Offset = null;
            Records = null;
        }


        public AirtableListRecordsResponse(AirtableRecordList recordList) : base()
        {
            Offset = recordList.Offset;
            Records = recordList.Records;
        }

        public readonly IEnumerable<AirtableRecord> Records;
        public readonly string Offset;
    }


    public class AirtableListRecordsResponse<T> : AirtableApiResponse
    {
        public AirtableListRecordsResponse(AirtableApiException error) : base(error)
        {
            Offset = null;
            Records = null;
        }


        public AirtableListRecordsResponse(AirtableRecordList<T> recordList) : base()
        {
            Offset = recordList.Offset;
            Records = recordList.Records;
        }

        public readonly IEnumerable<AirtableRecord<T>> Records;
        public readonly string Offset;
    }


    public class AirtableRetrieveRecordResponse : AirtableApiResponse
    {
        public AirtableRetrieveRecordResponse(AirtableApiException error) : base(error)
        {
            Record = null;
        }

        public AirtableRetrieveRecordResponse(AirtableRecord record) : base()
        {
            Record = record;
        }

        public readonly AirtableRecord Record;
    }


    public class AirtableRetrieveRecordResponse<T> : AirtableApiResponse
    {
        public AirtableRetrieveRecordResponse(AirtableApiException error) : base(error)
        {
            Record = null;
        }

        public AirtableRetrieveRecordResponse(AirtableRecord<T> record) : base()
        {
            Record = record;
        }

        public readonly AirtableRecord<T> Record;
    }


    public class AirtableCreateUpdateReplaceRecordResponse : AirtableApiResponse
    {
        public AirtableCreateUpdateReplaceRecordResponse(AirtableApiException error) : base(error)
        {
            Record = null;
        }

        public AirtableCreateUpdateReplaceRecordResponse(AirtableRecord record) : base()
        {
            Record = record;
        }

        public readonly AirtableRecord Record;
    }


    public class AirtableCreateUpdateReplaceMultipleRecordsResponse : AirtableApiResponse
    {
        public AirtableCreateUpdateReplaceMultipleRecordsResponse(AirtableApiException error) : base(error)
        {
            Records = null;
            CreatedRecords = null;
            UpdatedRecords = null;
        }

        public AirtableCreateUpdateReplaceMultipleRecordsResponse(AirtableUpSertRecordList upsertRecordList) : base()
        {
            Records = upsertRecordList.Records;
            CreatedRecords = upsertRecordList.CreatedRecords;
            UpdatedRecords = upsertRecordList.UpdatedRecords;
        }

        public readonly AirtableRecord[] Records;
        public readonly string[] CreatedRecords;
        public readonly string[] UpdatedRecords;
    }


    public class AirtableDeleteRecordResponse : AirtableApiResponse
    {
        public AirtableDeleteRecordResponse(AirtableApiException error) : base(error)
        {
            Deleted = false;
            Id = null;
        }

        public AirtableDeleteRecordResponse(bool deleted, string id) : base()
        {
            Deleted = deleted;
            Id = id;
        }

        public readonly bool Deleted;
        public readonly string Id;
    }


    public class AirtableDeleteCommentResponse : AirtableApiResponse
    {
        public AirtableDeleteCommentResponse(AirtableApiException error) : base(error)
        {
            Deleted = false;
            Id = null;
        }

        public AirtableDeleteCommentResponse(bool deleted, string id) : base()
        {
            Deleted = deleted;
            Id = id;
        }

        public readonly bool Deleted;
        public readonly string Id;
    }


    public class AirtableCreateUpdateCommentResponse : AirtableApiResponse
    {
        public AirtableCreateUpdateCommentResponse(AirtableApiException error) : base(error)
        {
            Comment = null;
        }

        public AirtableCreateUpdateCommentResponse(Comment comment) : base()
        {
            Comment = comment;
        }

        public readonly Comment Comment;
    }


    public class AirtableListCommentsResponse : AirtableApiResponse
    {
        public AirtableListCommentsResponse(AirtableApiException error) : base(error)
        {
            Comments = null;
            Offset = null;
        }

        public AirtableListCommentsResponse(CommentList commentList) : base()
        {
            Comments = commentList.Comments;
            Offset = commentList.Offset;
        }

        public readonly Comment[] Comments;
        public readonly string Offset;
    }

    public class AirtableCreateWebhookResponse : AirtableApiResponse
    {
        public AirtableCreateWebhookResponse(AirtableApiException error) : base(error)
        {
            createWebhookResponse = null;
        }

        public AirtableCreateWebhookResponse(CreateWebhookResponse createResponse) : base()
        {
            createWebhookResponse = createResponse;
        }

        public readonly CreateWebhookResponse createWebhookResponse;
    }

    public class AirtableListWebhooksResponse : AirtableApiResponse
    {
        public AirtableListWebhooksResponse(AirtableApiException error) : base(error)
        {
            Webhooks = null;
        }

        public AirtableListWebhooksResponse(Webhooks webhooks) : base()
        {
            Webhooks = webhooks;
        }

        public readonly Webhooks Webhooks;
    }

    public class AirtableListPayloadsResponse : AirtableApiResponse
    {
        public AirtableListPayloadsResponse(AirtableApiException error) : base(error)
        {
            Payloads = null;
        }


        public AirtableListPayloadsResponse(PayloadList payloadList) : base()
        {
            Payloads = payloadList.Payloads;
            Cursor = payloadList.Cursor;
            MighHaveMore = payloadList.MightHaveMore;
        }

        public readonly WebhooksPayload[] Payloads;
        public readonly int Cursor;
        public readonly bool MighHaveMore;
    }


    public class AirtableDeleteWebhookResponse : AirtableApiResponse
    {
        public AirtableDeleteWebhookResponse(AirtableApiException error) : base(error)
        {
        }

        public AirtableDeleteWebhookResponse() : base()
        {
            // The base class's ctor is protected so we need this public method here
            // so that we can construct an tmpty AirtabeDeleteWebhookResponse in a Successful operation.
        }
    }
    public class AirtabeEnableOrDisableWebhookNotificationsResponse : AirtableApiResponse
    {
        public AirtabeEnableOrDisableWebhookNotificationsResponse(AirtableApiException error) : base(error)
        {
        }

        public AirtabeEnableOrDisableWebhookNotificationsResponse() : base()
        {
            // The base class's ctor is protected so we need this public method here
            // so that we can construct an tmpty AirtabeEnableWebhookNotificationsResponse in a Successful operation.
        }
    }

    public class AirtabeRefreshWebhookResponse : AirtableApiResponse
    {
        public AirtabeRefreshWebhookResponse(AirtableApiException error) : base(error)
        {
            ExpirationTime = null;
        }

        public AirtabeRefreshWebhookResponse(DateTime? expirationTime) : base()
        {
            ExpirationTime = expirationTime;    
        }

        public readonly DateTime? ExpirationTime;
    }
}
