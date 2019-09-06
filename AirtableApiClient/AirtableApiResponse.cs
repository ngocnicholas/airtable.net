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


    public class AirtableListRecordsResponse<T> : AirtableApiResponse
    {
        public AirtableListRecordsResponse(AirtableApiException error) : base(error)
        {
            Offset = null;
            Records = null;
        }

        public AirtableListRecordsResponse(AirtableRecordList<T> recordList) : this(recordList.Offset)
        {
            Records = recordList.Records;
        }

        protected AirtableListRecordsResponse(string offset) : base()
        {
            Offset = offset;
        }

        public readonly IEnumerable<AirtableRecord<T>> Records;
        public readonly string Offset;
    }


    public class AirtableListRecordsResponse : AirtableListRecordsResponse<Dictionary<string, object>>
    {
        public AirtableListRecordsResponse(AirtableApiException error) : base(error)
        {
            Records = null;
        }

        public AirtableListRecordsResponse(AirtableRecordList recordList) : base(recordList.Offset)
        {
            Records = recordList.Records;
        }

        public new readonly IEnumerable<AirtableRecord> Records;
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
}
